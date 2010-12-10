using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts
{
    public class TentacleTip : ClawBehaviour
    {


        private float idleMovementRandomizer1 = 1;
        private float idleMovementRandomizer2 = 1;

        //public Transform animationObjectTransform;
        //private bool tightlyLinkedToAnimationObject = true;

        /*private Collider recentHitCollider;
        private Vector3 recentHitPosition;
        private Vector3 recentHitNormal;
        private float recentHitTime;*/

        //private RaycastHit rh;
        private RaycastHit rh_1;
        private RaycastHit rh_2;
        //private Ray ray = new Ray(Vector3.Zero, Vector3.Zero);

        //private Vector3 lastPosition;
        private GameObject body;
        private Vector3 bodyNormal;
        private TentacleStats stats;

        private bool isInitialized = false;

        //private Vector3 connectionNormal;
        private Collider connectedTo;
        //private float connectionTime = 0;
        //private Vector3 connectionPosition;

        //private Joint joint;

        private float shootTime = 0;
        private Vector3 shootDir = Vector3.Zero;

        public bool isAttacking
        {
            get { return (state == TentacleTip.States.usingClawState && clawState == ClawStates.attacking); }
        }

        public bool isIdle
        {
            get { return (state == TentacleTip.States.usingClawState && clawState == ClawStates.idle); }
        }

        public bool isConnected
        {
            get { return (state == TentacleTip.States.usingClawState && clawState == ClawStates.connected); }
        }

        public bool isSearchingForConnection
        {
            get { return (state == States.searchingForConnection); }
        }

        /*public bool isControlled{
            get{return state == States.controlled;}
        }*/

        public bool isDormant
        {
            get { return (state == TentacleTip.States.usingClawState && clawState == ClawStates.dormant); }
        }


        public enum States
        {
            //idle,
            usingClawState,
            searchingForConnection,
            //connected,
            //controlled,
            //attacking,
            objectGrabbed
        }

        private States _state = States.usingClawState;

        private States state
        {
            get
            {
                return _state;
            }
            /*set{
                _state = value;
            }*/
        }

        // Use this for initialization
        public void Initialize(GameObject _body, Vector3 _bodyNormal, TentacleStats _stats, Lemmy lemmy)
        {
            if (isInitialized) { return; }

            Initialize(lemmy);
            body = _body;
            bodyNormal = _bodyNormal;
            stats = _stats;

            idleMovementRandomizer1 = PressPlay.FFWD.Random.Range(0.7f, 1.3f);
            idleMovementRandomizer2 = PressPlay.FFWD.Random.Range(0.7f, 1.3f);

            isInitialized = true;

            //Physics.IgnoreCollision(collider, lemmy.collider);
        }

        public override void Update()
        {
            if (isConnected)
            {

                // We check if the collider is still connectable
                //if (connectedCollider != null && connectedCollider.gameObject.layer != GlobalSettings.Instance.tentacleColliderLayerInt)
                //{
                //    BreakConnection(50);
                //}
                //else
                //{
                //    HandleConnection();
                //}
            }
        }

        // Update is called once per frame
        public override void FixedUpdate()
        {
            if (isDormant)
            {
                transform.position = lemmy.transform.position;
            }

            if (isSearchingForConnection)
            {
                CheckSearchForConnectionTime();
            }

            //do checks to break connection
            if (isConnected)
            {
                CheckConnectionDistance();
                CheckConnectionTime();

                // We check if the collider is still connectable
                //if (connectedCollider != null && connectedCollider.gameObject.layer != GlobalSettings.Instance.tentacleColliderLayerInt)
                //{
                //    BreakConnection(50);
                //}

            }

            if (isConnected)
            {
                HandleConnection();
            }

            /*if (isConnected)
            {
			
                // We check if the collider is still connectable
                if (connectedCollider != null && connectedCollider.gameObject.layer != GlobalSettings.Instance.tentacleColliderLayerInt)
                {
                    BreakConnection(50);
                }
                else
                {
                    HandleConnection();
                }
            }*/

            if (isSearchingForConnection || isIdle)
            {
                HandleOverextensionElasticity();
            }

            if (isIdle)
            {
                IdleMovement();
            }

            if (isAttacking)
            {
                RaycastForEnemies();
            }

            if (isSearchingForConnection)
            {
                RaycastForConnection();
                WallSeekingHelp();
            }
        }

        /*void Update()
        {
            base.DoUpdate();
        }*/


        override protected void ChangeClawState(ClawStates _clawState)
        {
            _state = TentacleTip.States.usingClawState;
            base.ChangeClawState(_clawState);

        }

        void ChangeTentacleState(States newState)
        {
            //Debug.Log("Change Tentacle State to : "+newState);
            _state = newState;
        }


        public override void ExitDormant()
        {
            if (state == TentacleTip.States.usingClawState && clawState == ClawBehaviour.ClawStates.dormant)
            {
                ChangeClawState(ClawStates.idle);
            }
        }

        /*void DoConnection()
        {
            if (isConnected)
            {
                rigidbody.velocity = Vector3.Zero;
                transform.position = connectionPosition;
			
                transform.LookAt(transform.position - connectionNormal);
            }
        }*/

        void IdleMovement()
        {
            Vector3 vecToIdlePosition = transform.position - (body.transform.position + bodyNormal * 2) + new Vector3(Mathf.Cos(Time.time * 2.5f * idleMovementRandomizer1) * 0.8f, 0, Mathf.Sin(Time.time * 1.75f * idleMovementRandomizer2) * 0.8f); ;

            float distToIdlePosition = vecToIdlePosition.Length();

            rigidbody.velocity *= 0.92f;
            Vector3 elasticityForce = distToIdlePosition * (-vecToIdlePosition) * stats.overMaxLengthElasticity;
            rigidbody.AddForce(elasticityForce);

            transform.LookAt(transform.position + (transform.position - body.transform.position));
        }

        void CheckSearchForConnectionTime()
        {
            if (isSearchingForConnection && Time.time - shootTime > stats.searchForConnectionTimeout)
            {
                ChangeClawState(ClawStates.idle);
            }
        }

        void CheckConnectionDistance()
        {
            if (isConnected)
            {
                Vector3 vecToBody = body.transform.position - transform.position;

                //if (vecToBody.sqrMagnitude > stats.connectionMaxLength * stats.connectionMaxLength)
                //{
                //    BreakConnection();
                //}
            }
        }

        void CheckConnectionTime()
        {
            //if (!InputHandler.Instance.GetKeepConnection())
            //{
            //    connectionTime += Time.deltaTime;
            //}

            //if (isConnected && connectionTime > stats.connectionTimeout)
            //{


            //    //only break connection if not holding hold connection button
            //    if (!InputHandler.Instance.GetKeepConnection())
            //    {
            //        BreakConnection();
            //    }
            //}
        }

        void WallSeekingHelp()
        {

            ray.Position = transform.position;
            //ray.Direction = transform.right;
            //Debug.DrawRay(ray.origin, ray.direction * stats.wallSeekHelpDistance, Color.red);
            //if (Physics.Raycast(ray, out rh, stats.wallSeekHelpDistance, GlobalSettings.Instance.tentacleColliderLayer))
            //{
            //    SuckTowardRayHit(rh, ray);
            //}

            //ray.direction = -ray.direction;
            //Debug.DrawRay(ray.origin, ray.direction * stats.wallSeekHelpDistance, Color.red);
            //if (Physics.Raycast(ray, out rh, stats.wallSeekHelpDistance, GlobalSettings.Instance.tentacleColliderLayer))
            //{
            //    SuckTowardRayHit(rh, ray);
            //}
        }

        void SuckTowardRayHit(RaycastHit _rh, Ray _ray)
        {
            //rigidbody.AddForce((stats.wallSeekHelpDistance - _rh.distance) * ray.direction * stats.wallSeekHelpPower);
        }

        void RaycastForConnection()
        {
            //raycast from last position. Makes sure that the tip doesn't penetrate walls

            traversedVector = transform.position - lastPosition;
            //Vector3 futurePathVector = rigidbody.velocity * Time.deltaTime * 2; //times two

            //ray.origin = lastPosition;
            //ray.direction = traversedVector;

            ray.Position = lastPosition;
            //ray.Direction = futurePathVector + traversedVector;

            lastPosition = transform.position;

            //if (ray.direction.sqrMagnitude == 0)
            //{
            //    return;
            //}

            //float rayLength = (futurePathVector + traversedVector).magnitude;

            //Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.green);

            ////do bounce raycast	
            //bool bounceHit = Physics.Raycast(ray, out rh_1, rayLength, GlobalSettings.Instance.tentacleBounceColliderLayers);
            ////do connection raycast
            //bool connectHit = Physics.Raycast(ray, out rh_2, rayLength, GlobalSettings.Instance.tentacleColliderLayer);
            //compare distances

            /*if (connectHit)
            {
                recentHitCollider = rh_2.collider;
            }*/

            //if (isSearchingForConnection && bounceHit && (!connectHit || rh_1.distance < rh_2.distance))
            //{
                //sndBounce.PlaySound();
                //transform.position = rh_1.point;
                //rigidbody.velocity = -rigidbody.velocity * 0.2f;
                //lastPosition = transform.position;
                //ChangeClawState(ClawStates.idle);

                //// Add bounce feedback here!
                //if (createAtSlipperyConnect != null)
                //{
                //    ObjectPool.Instance.Draw(createAtSlipperyConnect, transform.position + (transform.forward / 2), transform.rotation);
                //}

                //if (visualSoundAtSlipperyConnect != null)
                //{
                //    ObjectPool.Instance.Draw(visualSoundAtSlipperyConnect, transform.position + (transform.forward / 2), transform.rotation);
                //}

            //}
            //else if (connectHit && (!bounceHit || rh_1.distance > rh_2.distance))
            //{
                //ConnectToAtPosition(rh_2.point + rh_2.normal * 0.3f, rh_2.normal, rh_2.collider.gameObject);
            //}
        }

        void HandleOverextensionElasticity()
        {
            //handle elasticity of arm. Slow down movement if distance to lemmy is greater than arm length
            //This is what limits the arm reach (appart from the physics engine. Dampening, gravity, etc. works as well)
            Vector3 vecToBody = transform.position - body.transform.position;
            //float distToBody = vecToBody.magnitude;
            //if (distToBody > stats.tentacleLength)
            //{
            //    rigidbody.velocity *= 0.91f; //HACK WARNING!!! this is framerate dependent if run i Update instead of FixedUpdate
            //    Vector3 elasticityForce = (stats.tentacleLength - distToBody) * vecToBody * stats.overMaxLengthElasticity;
            //    rigidbody.AddForce(elasticityForce);
            //}
        }

        public void ShootInDirection(Vector3 _direction)
        {
            if (isConnected)
            {
                BreakConnection();
            }

            transform.position = body.transform.position;
            lastPosition = body.transform.position;

            //float speed = Mathf.Min(_direction.magnitude, stats.maxShootSpeed);
            //speed = Mathf.Max(speed, stats.minShootSpeed);

            //rigidbody.velocity = _direction.normalized * stats.tentacleTipMoveSpeed * speed;

            //ChangeTentacleState(TentacleTip.States.searchingForConnection);

            //shootTime = Time.time;

            //shootDir = _direction.normalized;

            //transform.LookAt(transform.position + _direction);
            //PPMetrics.AddFloatIncrement("shoot_tentacle", 1);
        }

        public Vector3 GetElasticityForce()
        {
            Vector3 force = Vector3.Zero;

            //Vector3 bodyDir = (body.transform.position - transform.position).normalized;

            //Vector3 vecToBody = body.transform.position - (transform.position + bodyDir * stats.optimalConnectionDistance);

            //Debug.DrawRay(transform.position, bodyDir * stats.optimalConnectionDistance, Color.gray);

            //float distToBody = vecToBody.magnitude;
            //if (distToBody > stats.dragDistMin)
            //{
            //    force += -vecToBody.normalized * (stats.dragBodyForce * Mathf.Pow(distToBody - stats.dragDistMin, stats.dragCurvePow) + Mathf.Cos(Time.time * (1.75f * idleMovementRandomizer2) + idleMovementRandomizer1) * 2.2f);
            //}

            return force;
        }

        public override void DoOnReset()
        {
            if (isConnected)
            {
                BreakConnection();
            }

            //rigidbody.velocity = Vector3.Zero;
            transform.position = body.transform.position;
            ChangeClawState(ClawStates.idle);

            lastPosition = body.transform.position;
        }

        public override void DoOnGrab(GameObject _obj)
        {
            base.DoOnGrab(_obj);
            ChangeTentacleState(TentacleTip.States.objectGrabbed);
        }

        public override void DoOnReleaseGrabbedObject(GameObject _grabbedObject)
        {
            base.DoOnReleaseGrabbedObject(_grabbedObject);

            ChangeClawState(ClawStates.idle);
        }
    }
}