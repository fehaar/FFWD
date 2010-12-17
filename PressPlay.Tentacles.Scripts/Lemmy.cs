using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Components;
using PressPlay.FFWD;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.Tentacles.Scripts
{
    public class Lemmy : MonoBehaviour
    {
        /*private bool _isInputOn = false;
        public bool isInputOn{
            get{return _isInputOn;}
        }*/

        public bool aimAtFingerPosition = false;
        public bool useControllerInput = false;

        //private RigidbodyAffectedByCurrents currentEffectScript;

        //private EggSack eggSack;

        //public AudioWrapper sndTentacle;
        //public AudioWrapper sndDeath;
        //public AudioWrapper sndDamageLow;
        //public AudioWrapper sndDamageMedium;
        //public AudioWrapper sndDamageHigh;

        //public LemmySquishedTester squishedTester;

        //public ScoreHandler scoreHandler = new ScoreHandler();

        //private RaycastHit rh;
        private Microsoft.Xna.Framework.Ray ray;

        private Vector3 lastPosition;

        //public PoolableObject createOnLowDamage;
        //public PoolableObject createOnMediumDamage;
        //public PoolableObject createOnHighDamage;
        //public PoolableObject createOnPushButNoDamage;
        //public PoolableObject createOnDeath;

        public ObjectReference tentaclePrefab;
        public ObjectReference tentacleTipPrefab;
        public ObjectReference mainBodyPrefab;
        //public Claw clawPrefab;

        //public ParticleEmitter bubbleTrailPrefab;
        //public ParticleEmitter bleedBubbleTrailPrefab;

        [ContentSerializerIgnore]
        public LemmyStats stats { get { return statsRef.Get<LemmyStats>(); } }
        [ContentSerializer(ElementName = "stats")]
        private ObjectReference statsRef = null;

        [ContentSerializerIgnore]
        public TentacleStats tentacleStats { get { return tentacleStatsRef.Get<TentacleStats>(); } }
        [ContentSerializer(ElementName = "tentacleStats")]
        private ObjectReference tentacleStatsRef = null;
        //public TentacleStats clawStats;

        private int currentTentacleIndex = 0;

        private TentacleJoint[] tentacleRoots;
        private Tentacle[] tentacles;
        private TentacleTip[] tentacleTips;
        public MainBody mainBody;
        //private Tentacle clawTentacle;
        //private Claw _claw;
        //public Claw claw
        //{
        //    get
        //    {
        //        return _claw;
        //    }
        //}

        //private ParticleEmitter bubbleTrail;
        //private ParticleAnimator bubbleTrailAnimator;
        //private ParticleEmitter bleedBubbleTrail;
        //private ParticleAnimator bleedBubbleTrailAnimator;

        //public InputHandler inputHandlerPrefab;
        [ContentSerializerIgnore]
        public Camera lemmyFollowCamera;
        //public PathFollowCam pathFollowCam;

        private Vector3 forceFromTentacles;
        private Vector3 lastInputPosition = Vector3.zero;
        private float lastInputTime;

        private float health;

        private bool lemmyHasJustDied = false;

        //public LevelSession levelSession;


        private bool _isInputLocked = false;
        [ContentSerializerIgnore]
        public bool isInputLocked
        {
            get
            {
                return (_isInputLocked || LevelHandler.Instance.isPlayingCinematicSequence);
            }
            set
            {
                _isInputLocked = value;
            }
        }

        private bool _isGrabbed = false;
        [ContentSerializerIgnore]
        public bool isGrabbed
        {
            get
            {
                return _isGrabbed;
            }
            set
            {
                _isGrabbed = value;
            }
        }




        // Have we set the number of lives for Lemmy
        private bool isBroughtToLife = true;
        //public bool hasExtraLives
        //{
        //    get
        //    {
        //        return eggSack.numberOfLivesLeft > 0;
        //    }
        //}

        //public int maxNumberOfLives
        //{
        //    get
        //    {
        //        return eggSack.numberOfLivesFromStart;
        //    }
        //}

        //public int numberOfLives
        //{
        //    get
        //    {
        //        return eggSack.numberOfLivesLeft;
        //    }
        //}

        public enum State
        {
            dormantBeforeSpawn,
            normalActivity
        }
        private State _state = State.dormantBeforeSpawn;
        private float stateChangeTime;


        // Use this for initialization
        public void Initialize()
        {

            //currentEffectScript = GetComponent<RigidbodyAffectedByCurrents>();

            //create main body
            mainBody = (MainBody)Instantiate(mainBodyPrefab);
            mainBody.transform.position = transform.position;
            mainBody.transform.parent = transform;

            TentacleJoint bodyJoint = (TentacleJoint)GetComponent(typeof(TentacleJoint));

            //create claw
            //_claw = (Claw)Instantiate(clawPrefab);
            //claw.transform.position = transform.position;
            //claw.Initialize(this, Vector3.back, clawStats);
            //Physics.IgnoreCollision(collider, claw.collider);
            //clawTentacle = (Tentacle)Instantiate(tentaclePrefab);
            //clawTentacle.Initialize(clawStats, bodyJoint, (TentacleJoint)claw.GetComponent(typeof(TentacleJoint)));

            //create bubble trail
            //bubbleTrail = (ParticleEmitter)Instantiate(bubbleTrailPrefab, transform.position, transform.rotation);
            //bubbleTrail.transform.parent = transform;
            //bubbleTrailAnimator = bubbleTrail.GetComponent<ParticleAnimator>();
            //bleedBubbleTrail = (ParticleEmitter)Instantiate(bleedBubbleTrailPrefab, transform.position, transform.rotation);
            //bleedBubbleTrail.transform.parent = transform;
            //bleedBubbleTrailAnimator = bleedBubbleTrail.GetComponent<ParticleAnimator>();

            //create tentacles		
            tentacleRoots = new TentacleJoint[stats.tentacles];
            tentacles = new Tentacle[stats.tentacles];
            tentacleTips = new TentacleTip[stats.tentacles];
            for (int i = 0; i < stats.tentacles; i++)
            {
                GameObject tmpGameObject = new GameObject();
                tmpGameObject.name = "tentacle root " + i;
                // TODO: This should not be nessecary - the framework is broken!
                tmpGameObject.AddComponent(new Transform());
                tmpGameObject.AddComponent(typeof(TentacleJoint));

                tentacleRoots[i] = tmpGameObject.GetComponent<TentacleJoint>();

                tentacleTips[i] = (TentacleTip)Instantiate(tentacleTipPrefab);
                tentacleTips[i].transform.position = transform.position;

                Vector3 normal = Vector3.zero;
                normal.x = Mathf.Cos(((i + 0.5f) * Mathf.PI * 2) / stats.tentacles);
                normal.z = Mathf.Sin(((i + 0.5f) * Mathf.PI * 2) / stats.tentacles);

                tentacleRoots[i].transform.position = transform.position;
                tentacleRoots[i].transform.parent = transform;

                tentacleTips[i].Initialize(gameObject, normal, tentacleStats, this);

                Physics.IgnoreCollision(tentacleTips[i].collider, collider);
                //Physics.IgnoreCollision(tentacleTips[i].collider, claw.collider);
                for (int j = 0; j < i; j++)
                {
                    Physics.IgnoreCollision(tentacleTips[i].collider, tentacleTips[j].collider);
                }

                tentacles[i] = (Tentacle)Instantiate(tentaclePrefab);
                tentacles[i].Initialize(tentacleStats, tentacleRoots[i], bodyJoint, (TentacleJoint)tentacleTips[i].GetComponent(typeof(TentacleJoint)), true);
                tentacles[i].SetBodyNormal(normal);
            }

            //squishedTester.Initialize();

            //eggSack = GetComponent<EggSack>();

            health = stats.health;

            rigidbody.drag = stats.rigidbodyDrag;

            lastInputTime = Time.time;

            lastPosition = transform.position;

            // TODO: Remove this - it is a hack
            lemmyFollowCamera = Camera.main;
        }

        public void ChangeState(State _newState)
        {
            _state = _newState;
            stateChangeTime = Time.time;

            switch (_state)
            {
                case State.dormantBeforeSpawn:
                    //claw.GoDormant();
                    //claw.Reset();
                    mainBody.LookUp();
                    for (int i = 0; i < tentacleTips.Length; i++)
                    {
                        tentacleTips[i].GoDormant();
                        tentacles[i].Reset();
                    }
                    //eggSack.Reset();

                    break;

                case State.normalActivity:

                    //claw.ExitDormant();
                    mainBody.LookRight();
                    for (int i = 0; i < tentacleTips.Length; i++)
                    {
                        tentacleTips[i].ExitDormant();
                    }
                    break;
            }
        }


        /*public void ToggleInput(bool turnOn)
        {
            _isInputOn = turnOn;
        }*/

        public void SetNumberOfLives(int numberOfLives)
        {
            //if (eggSack == null)
            //{
            //    eggSack = GetComponent<EggSack>();
            //}
            //eggSack.Initialize(this, numberOfLives);
            isBroughtToLife = true;
        }

        public override void Update()
        {
            if (!isBroughtToLife) return;

            //eggSack.DoUpdate();




            //bubble trail SLIGHTLY HACKY! HACK WARNING!
            //Debug.DrawRay(transform.position,bubbleTrail.worldVelocity, Color.blue);
            //float speedSqrt = rigidbody.velocity.sqrMagnitude + currentEffectScript.force.magnitude * 0.95f;
            //Vector3 trailSpeed = currentEffectScript.force * 0.125f;
            //Vector3 trailForce = currentEffectScript.force * 0.05f;
            //float minEmission = speedSqrt * 0.05f + 0.04f;
            //float maxEmission = speedSqrt * 0.1f + 0.05f;

            //bubbleTrail.worldVelocity = trailSpeed;
            //bubbleTrailAnimator.force = trailForce;
            //bubbleTrail.maxEmission = maxEmission;
            //bubbleTrail.minEmission = minEmission;

            //float bleed = 1 - (health / stats.health);
            //bleedBubbleTrail.worldVelocity = trailSpeed * bleed;
            //bleedBubbleTrailAnimator.force = trailForce * bleed;
            //bleedBubbleTrail.maxEmission = maxEmission * bleed;
            //bleedBubbleTrail.minEmission = minEmission * bleed;


            //check if we should use xbox controller input, or do touchscreen/mouse checks
            if (useControllerInput)
            {
                HandleShootTentacleInput_Controller();
            }
            else if (!LevelHandler.Instance.CheckHitUIElements())
            {
                HandleShootTentacleInput();
            }
            ShowNextAvailableTentacle();
        }

        public override void FixedUpdate()
        {
            if (!isBroughtToLife) return;

            DoWallCollisionRaycast();

            HandleConnectedTentacleTips();


            //slightly hacky way to keep claw in front of lemmy
            //if (claw.isIdle)
            //{
            //    claw.rigidbody.AddForce(rigidbody.velocity * 150 * Time.deltaTime);
            //}

            HandleHealth();

            Camera.main.transform.position = mainBody.transform.position + new Vector3(0, -7, 0);
        }

        public void AddHealth(float _health)
        {
            health += _health;

            health = Mathf.Min(health, stats.health);

            //LevelHandler.Instance.lemmyPainVisualizer.SetHealth(health);
        }

        private void HandleHealth()
        {
            if (health < stats.health)
            {

                health += stats.regenerateDamagePerSecond * Time.deltaTime;
                health = Mathf.Min(health, stats.health);

                //Debug.Log("Regenerating: " + health);

                //LevelHandler.Instance.lemmyPainVisualizer.SetHealth(health);
            }

            mainBody.SetHealthFraction(health / stats.health);
        }

        private void HandleConnectedTentacleTips()
        {

            //force from tentacles and claw
            forceFromTentacles = Vector3.zero;

            //this moves tentacle roots (origins) to edge of lemmy in the connected direction, and back to center of lemmy if not connected
            for (int i = 0; i < tentacleTips.Length; i++)
            {
                if (tentacleTips[i].isConnected)
                {
                    forceFromTentacles += tentacleTips[i].GetElasticityForce();

                    //JUST A TEST!! HACK
                    tentacleRoots[i].transform.localPosition = Vector3.Lerp(tentacleRoots[i].transform.localPosition, (tentacleTips[i].transform.position - transform.position).normalized * 0.45f, Time.deltaTime * 1.45f);
                }
                else
                {
                    //JUST A TEST!! HACK
                    tentacleRoots[i].transform.localPosition = Vector3.Lerp(tentacleRoots[i].transform.localPosition, Vector3.zero, Time.deltaTime * 1.5f);
                }
                tentacleRoots[i].transform.localPosition -= new Vector3(0, tentacleRoots[i].transform.localPosition.y, 0);
            }

            //if (claw.isConnected)
            //{
            //    forceFromTentacles += claw.GetElasticityForce();
            //}

            //add connection forces to lemmy rigidbody
            rigidbody.AddForce(forceFromTentacles);
        }

        private void ShowNextAvailableTentacle()
        {
            for (int i = 0; i < tentacles.Length; i++)
            {
                tentacles[i].ShowAsUnavailable();
            }
            int nextTentacleIndex = GetNextAvailableTentacleIndex();

            tentacles[nextTentacleIndex].ShowAsAvailable();
        }

        private void HandleShootTentacleInput_Controller()
        {
            if (isInputLocked)
            {
                return;
            }

            if (InputHandler.Instance.GetShootClaw())
            {
                ShootClawInDirection(InputHandler.InputVecToWorldVec(LevelHandler.Instance.cam.raycastCamera, InputHandler.Instance.GetClawDirection()));
            }

            if (InputHandler.Instance.GetShootTentacle())
            {
                ShootTentacleInDirection(InputHandler.InputVecToWorldVec(LevelHandler.Instance.cam.raycastCamera, InputHandler.Instance.GetTentacleDirection()));
            }
        }


        private void HandleShootTentacleInput()
        {
            if (isInputLocked)
            {
                return;
            }

            ray = lemmyFollowCamera.ScreenPointToRay(InputHandler.Instance.GetInputScreenPosition());

            if (InputHandler.Instance.GetShootTentacle())
            {
                //    //Debug.Log(InputHandler.Instance.ScreenRayCheck(ray,GlobalSettings.Instance.enemyInputLayer).ToString());
                ScreenRayCheckHit hit = InputHandler.Instance.ScreenRayCheck(ray, GlobalSettings.Instance.enemyInputLayer);

                if (hit.hitObjectInLayer)
                {
                    if (aimAtFingerPosition)
                    {
                        //aim at finger position
                        ShootClawInDirection(new Vector3(hit.position.x, 0, hit.position.z) - transform.position);
                    }
                    else
                    {
                        //aim at enemy center
                        ShootClawAtEnemy(hit.obj);
                    }

                }
                else
                {
                    lastInputPosition = hit.position;
                    lastInputTime = Time.time;
                    ShootTentacleInDirection(hit.position - transform.position);
                }
            }
        }

        private void ShootClawInDirection(Vector3 _direction)
        {
            //claw.ShootInDirection(_direction);
            //sndTentacle.PlaySound();
        }

        private void ShootClawAtEnemy(GameObject enemy)
        {
            //claw.ShootInDirection(enemy.transform.position - transform.position);
            //sndTentacle.PlaySound();
        }

        private void ShootTentacleInDirection(Vector3 _direction)
        {

            currentTentacleIndex = GetNextAvailableTentacleIndex();
            tentacleTips[currentTentacleIndex].ShootInDirection(_direction);

            //sndTentacle.PlaySound();
        }

        private int GetNextAvailableTentacleIndex()
        {
            for (int i = currentTentacleIndex; i < tentacleTips.Length + currentTentacleIndex; i++)
            {
                if (tentacleTips[i % stats.tentacles].isIdle)
                {
                    return i % stats.tentacles;
                }
            }

            return (currentTentacleIndex + 1) % stats.tentacles;
        }

        private void DoWallCollisionRaycast()
        {
            ray.Position = lastPosition;
            ray.Direction = transform.position - lastPosition;

            //if (Physics.Raycast(ray, out rh, (transform.position - lastPosition).magnitude, GlobalSettings.Instance.allWallsAndShields))
            //{
            //    transform.position = rh.point + rh.normal * 0.5f;
            //    rigidbody.velocity = -rigidbody.velocity * 0.1f; //this move only happens if velocity is very very high, so we use some hard coded bounce
            //}

            lastPosition = transform.position;
        }

        //void OnCollisionEnter(Collision collision)
        //{
        //    Vector3 contactAverage = Vector3.zero;
        //    for (int i = 0; i < collision.contacts.Length; i++)
        //    {

        //        contactAverage += collision.contacts[i].point;
        //    }
        //    contactAverage /= collision.contacts.Length;

        //    Vector3 normalAverage = Vector3.zero;
        //    for (int i = 0; i < collision.contacts.Length; i++)
        //    {

        //        normalAverage += collision.contacts[i].normal;
        //    }
        //    normalAverage /= collision.contacts.Length;

        //    if (collision.gameObject.tag == GlobalSettings.Instance.pickupTag)
        //    {
        //        GetPickupCollision(collision.gameObject);
        //    }
        //    else if (collision.gameObject.tag == GlobalSettings.Instance.triggeredByLemmyTag)
        //    {
        //        collision.gameObject.GetComponent<TriggeredByLemmy>().Trigger();
        //    }
        //}

        //void OnTriggerEnter(Collider collider)
        //{
        //    if (collider.gameObject.tag == GlobalSettings.Instance.pickupTag)
        //    {
        //        GetPickupCollision(collider.gameObject);
        //    }
        //    else if (collider.gameObject.tag == GlobalSettings.Instance.triggeredByLemmyTag)
        //    {
        //        collider.gameObject.GetComponent<TriggeredByLemmy>().Trigger();
        //    }
        //}

        public void GetPickupCollision(GameObject pickupObj)
        {
            //IPickup pickup = (IPickup)pickupObj.GetComponent(typeof(IPickup));
            //pickup.DoOnCollision(this);
        }
        public void GetPickupGrab(GameObject pickupObj)
        {
            //IPickup pickup = (IPickup)pickupObj.GetComponent(typeof(IPickup));
            //pickup.DoGrabPickUp(this);
        }

        public void Damage(float _damage, Vector3 _direction)
        {
            if (_damage == 0)
            {
                return;
            }

            //PathFollowCam.Instance.ShakeCamera(0.4f);
            //if (!DamageVisualizer.isInitiated)
            //{
            //    LevelHandler.Instance.lemmyPainVisualizer.Init(health);
            //}

            //subtract damage from health
            health -= _damage;

            //register damage in multiplier
            //levelSession.RegisterMultiplierPenalty(_damage * GlobalManager.Instance.gameplaySettings.multiplyDamagePenalty);
            //levelSession.RegisterDamage(_damage);


            //visualize damage
            mainBody.StartTakeDamageAnimation();
            //LevelHandler.Instance.lemmyPainVisualizer.DoDamage(health);


            //create damage particles
            //Quaternion particleRotation = Quaternion.LookRotation(_direction);
            //if (health < 0)
            //{
            //    Kill();

            //    return;
            //}
            //if (_damage == 0 && createOnPushButNoDamage)
            //{
            //    ObjectPool.Instance.Draw(createOnPushButNoDamage, transform.position, particleRotation);
            //}
            //else if (_damage <= 10 && createOnLowDamage != null)
            //{
            //    ObjectPool.Instance.Draw(createOnLowDamage, transform.position, particleRotation);
            //    sndDamageLow.PlaySound();
            //}
            //else if (_damage <= 40 && createOnMediumDamage != null)
            //{
            //    ObjectPool.Instance.Draw(createOnMediumDamage, transform.position, particleRotation);
            //    sndDamageMedium.PlaySound();
            //}
            //else if (createOnHighDamage != null)
            //{
            //    ObjectPool.Instance.Draw(createOnHighDamage, transform.position, particleRotation);
            //    sndDamageHigh.PlaySound();
            //}
        }

        public void Push(Vector3 _push)
        {
            rigidbody.AddForce(_push);
        }

        public void BreakConnections()
        {
            for (int i = 0; i < tentacleTips.Length; i++)
            {
                if (tentacleTips[i].isConnected)
                {
                    tentacleTips[i].BreakConnection();
                }
            }
        }

        public void SetActivationStatus(bool _status)
        {
            for (int i = 0; i < tentacles.Length; i++)
            {
                tentacles[i].gameObject.SetActiveRecursively(_status);
            }

            for (int i = 0; i < tentacleTips.Length; i++)
            {
                tentacleTips[i].gameObject.SetActiveRecursively(_status);
            }

            //claw.gameObject.SetActiveRecursively(_status);
            //clawTentacle.gameObject.SetActiveRecursively(_status);
            mainBody.gameObject.SetActiveRecursively(_status);

            //if (eggSack != null)
            //{
            //    eggSack.SetActive(_status);
            //}

            gameObject.SetActiveRecursively(_status);
        }

        public void SpawnAt(CheckPoint _checkpoint)
        {
            Debug.Log("---------------SPAWNING LEMMY AT : " + _checkpoint.name + "   coordinates : " + _checkpoint.transform.position + "   time : " + Time.realtimeSinceStartup);

            SetActivationStatus(true);

            rigidbody.velocity = Vector3.zero;

            transform.position = _checkpoint.transform.position;

            lastPosition = _checkpoint.transform.position;
            _checkpoint.ActivateCheckPoint();
            health = stats.health;

            //LevelHandler.Instance.lemmyPainVisualizer.SetHealth(stats.health, true);

            for (int i = 0; i < tentacleTips.Length; i++)
            {
                tentacleTips[i].Reset();
            }

            for (int i = 0; i < tentacles.Length; i++)
            {
                tentacles[i].Reset();
            }

            //claw.Reset();
            //clawTentacle.Reset();

            //PathFollowCam.Instance.ActivateClosestConnection(transform.position);

            //if (lemmyHasJustDied && eggSack.numberOfLivesLeft > 0)
            //{
            //    eggSack.RemoveNextEgg();
            //}

            _checkpoint.DoOnSpawnLemmy();
        }

        public void Kill()
        {
            //if (createOnDeath != null)
            //{
            //    ObjectPool.Instance.Draw(createOnDeath, transform.position, transform.rotation);
            //}

            LevelHandler.Instance.DoOnLemmyDeath();

            SetActivationStatus(false);

            //levelSession.RegisterDeath();
            //sndDeath.PlaySound();

            //if (eggSack.numberOfLivesLeft > 0)
            //{
            //    LevelHandler.Instance.RespawnAtLastCheckpoint();
            //}
            //else
            //{
            //    LevelHandler.Instance.state = LevelHandler.LevelState.gameover;
            //}

            //PPMetrics.AddPositionString("lemmy_died", transform.position, LevelHandler.Instance.cam.getCurrentNodeName);
            //PPMetrics.AddFloatIncrement("num_of_deaths", 1);

            lemmyHasJustDied = true;
        }

        public void StartExitAnimation()
        {

        }

        public void CashInExtraLife()
        {
            //EggSackElement egg = eggSack.RemoveNextEgg();
            //levelSession.RegisterPoint(GlobalManager.Instance.gameplaySettings.pointsForExtraLife, 0, egg.transform.position, false);
        }

        public void CashInExtraLives()
        {
            //EggSackElement egg;

            //while (eggSack.numberOfLivesLeft > 0)
            //{
            //    egg = eggSack.RemoveNextEgg();
            //    //Don't know if we should show anything here?
            //}
        }
    }
}
