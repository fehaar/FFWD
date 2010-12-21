using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Components;
using PressPlay.FFWD;
using System.Collections;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.Tentacles.Scripts
{
    public class LevelHandler : TurnOffAtDistanceHandler
    {
        public enum LevelState
        {
            preloading,
            intro,
            playing,
            respawning,
            gameover,
            paused,
            outro,
            endscreen,
            finalizeEndScreen
        }

        private LevelState _state = LevelState.preloading;
        [ContentSerializerIgnore]
        public LevelState state
        {
            get
            {
                return _state;
            }
            set
            {
                ChangeState(value);
            }
        }

        private float _lastStateChange;
        public float lastStateChange
        {
            get
            {
                return _lastStateChange;
            }
        }


        public Level.LevelType debugLevelType;
        //private LevelTypeSettings levelTypeSettings;

        //public AudioWrapper endSound;

        [ContentSerializer(SharedResource = true)]
        public Lemmy lemmyPrefab;
        //public PathFollowCam cameraPrefab;
        //public Camera feedbackCameraPrefab;
        //public GUICamera GUICameraPrefab;
        //public UIManager uIManagerPrefab;
        //public IngameGUI ingameGUIPrefab;

        //public FeedbackHandler feedback;

        //public BaseMenuScreen endLevelScreen;
        //public BaseMenuScreen deathScreen;
        //private BaseMenuScreen currentScreen;

        //private GameObjectPreloader objectPreloader;

        //public UIManager uIManager;

        //public GUICamera GUICamera;

        //public PPAnimationHandler lemmyPainAnimation;

        //public DamageVisualizer lemmyPainVisualizer;

        [ContentSerializerIgnore]
        public Lemmy lemmy;

        [ContentSerializerIgnore]
        public PathFollowCam cam
        {
            get
            {
                return PathFollowCam.Instance;
            }
        }

        //public Camera feedbackCam;

        //public IngameGUI ingameGUI;


        //private ResetOnLemmyDeath[] resetOnDeathObjects;

        //public GameObject inGameMenuPrefab;

        //private CinematicSequence currentCinematicSequence;
        private bool _isPlayingCinematicSequence = false;
        public bool isPlayingCinematicSequence
        {
            get { return _isPlayingCinematicSequence; }
        }

        //private bool doStandardSpawnInsteadOfIntro = false;
        private CheckPoint startingCheckPoint;

        [ContentSerializerIgnore]
        public LevelStartCheckPoint levelStartCheckPoint;

        [ContentSerializerIgnore]
        public CheckPoint[] checkpointOrder = new CheckPoint[0];

        private List<CheckPoint> activatedCheckPoints = new List<CheckPoint>();
        private CheckPoint[] checkPoints;
        private CheckPoint lastActivatedCheckpoint;

        //private LevelExit levelExit;

        public static bool isLoaded = false;
        private static LevelHandler instance;

        //public PathFollowObject lemmyHunter;
        //private GameObject followObject;

        //public GameLibrary library;

        //public InputRayPlaneHandler inputRayPlaneHandlerPrefab;
        //[HideInInspector]
        //public InputRayPlaneHandler inputRayPlaneHandler;

        //private WWW www;
        //private bool isCommunicatingWithServer = false;

        //#region framerate variables
        //private float frameRateCalcCnt = 0;
        //private float averageFrameRate;

        //private float averageFramerate10Frames;
        //private float[] lastFrames = new float[10];
        //private int frameIndex = 0;

        //private float currentFramerate;
        public float criticalFrameRateDropLimit = 20;
        public bool doCriticalFrameRateDropCheck = false;
        //private int criticalFrameRateDropCounter = 0;
        public int framesBetweenCriticalFrameChecks = 10;
        //#endregion

        private Level _currentLevel;
        [ContentSerializerIgnore]
        public Level currentLevel
        {
            get
            {
                if (_currentLevel == null)
                {
                    Debug.LogError("LevelHandler> CurrentLevel is NULL");
                }

                return _currentLevel;
            }
            set
            {

                if (value != null)
                {
                    Debug.Log("************************ Setting Current Level " + value.sceneName);
                }
                else
                {
                    Debug.Log("************************ Setting Current Level NULL");
                }
                _currentLevel = value;
            }
        }


        //public ChallengeHandler challengeHandler;


        private float _globalTimeSinceLemmySpawn = 0;
        public float globalTimeSinceLemmySpawn
        {
            get
            {
                return _globalTimeSinceLemmySpawn;
            }
        }

        private float _globalLevelTime = 0;
        public float globalLevelTime
        {
            get
            {
                return _globalLevelTime;
            }
        }
        private float _globalLevelDeltaTime = 0;
        public float globalLevelDeltaTime
        {
            get
            {
                return _globalLevelDeltaTime;
            }
        }


        //private float lastExtraLifeCashInTime = 0;

        //public MusicController musicController;

        public static LevelHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    Debug.LogError("Attempt to access instance of LevelHandler singleton earlier than Start or without it being attached to a GameObject.");
                }
                return instance;
            }
        }

        public override void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("Cannot have two instances of LevelHandler. Self destruction in 3...");
                Destroy(this);
                return;
            }

            isLoaded = true;
            instance = this;

            Initialize();
        }


        public bool CheckHitUIElements()
        {
            return false;
            //if (GUICamera == null || GUICamera.uiManager == null || !InputHandler.isLoaded) return false;
            //return Physics.Raycast(GUICamera.uiManager.rayCamera.ScreenPointToRay(InputHandler.Instance.GetInputScreenPosition()), GUICamera.uiManager.rayDepth, GUICamera.uiManager.rayMask);
        }

        private void Initialize()
        {
            Debug.Log("STARTING INITIALIZING LEVEL HANDLER");

            Debug.Log("STARTING LEVEL!!!!!!");

            levelStartCheckPoint = (LevelStartCheckPoint)GameObject.FindObjectOfType(typeof(LevelStartCheckPoint));

            CreateArrayOfCheckpoints();

            // We initialize the needed game objects
            lemmy = (Lemmy)Instantiate(lemmyPrefab, startingCheckPoint.GetSpawnPosition(), lemmyPrefab.transform.rotation);
            //cam = (PathFollowCam)Instantiate(cameraPrefab);

            //HACK!! we need the game object preloader, and for now it is attached to the main camera, so here we go...
            //objectPreloader = cam.GetComponentInChildren<GameObjectPreloader>();

            // This is not really the GUICamera. This is used for visualFX. Stupid naming!
            //feedbackCam = cam.GUICamera;

            //levelExit = (LevelExit)FindObjectOfType(typeof(LevelExit));

            //if (GUICameraPrefab != null)
            //{
            //    GUICamera = (GUICamera)Instantiate(GUICameraPrefab);
            //    lemmyPainAnimation = GUICamera.GetComponentInChildren<PPAnimationHandler>();
            //    lemmyPainVisualizer = GUICamera.GetComponentInChildren<DamageVisualizer>();
            //}

            //if (GlobalManager.isLoaded && GlobalManager.Instance.includeFramerateCounter && GlobalManager.Instance.framerateCounterPrefab != null)
            //{
            //    Instantiate(GlobalManager.Instance.framerateCounterPrefab);
            //}

            //ingameGUI = (IngameGUI)Instantiate(ingameGUIPrefab);
            //if (GlobalManager.isLoaded)
            //{
            //    ingameGUI.InitGUIBottomBar();
            //}

            //inputRayPlaneHandler = (InputRayPlaneHandler)Instantiate(inputRayPlaneHandlerPrefab);
            //inputRayPlaneHandler.Initialize(cam.raycastCamera);

            // Here we set different variables. It seems kind of confusing :-)
            //lemmy.lemmyFollowCamera = cam.raycastCamera;
            //lemmy.pathFollowCam = cam;
            //cam.followObject = lemmy.gameObject;
            //followObject = cam.gameObject;


            //resetOnDeathObjects = (ResetOnLemmyDeath[])(FindObjectsOfType(typeof(ResetOnLemmyDeath)));
            //InitializeDistanceHandling(followObject);

            //if (inGameMenuPrefab)
            //{
            //    Instantiate(inGameMenuPrefab);
            //}


            //if (GlobalManager.isLoaded)
            //{
            //    GlobalManager.Instance.fullscreenImageHandler.DoInstantBlackScreen();
            //}

            //if (levelStartCheckPoint != null)
            //{
            //    lemmy.transform.position = levelStartCheckPoint.GetStartTweenPosition(0);
            //}

            lemmy.Initialize();

            ChangeState(LevelState.preloading);

            Debug.Log("FINISHED INITIALIZING LEVEL HANDLER");
        }

        public override void Start()
        {
            //if (GlobalManager.isLoaded)
            //{
            //    musicController = GetComponent<MusicController>();
            //}
        }

        public void ChangeState(LevelState newState)
        {
            Debug.Log("LevelHandler changing state from " + state + " to " + newState);

            _lastStateChange = Time.time;
            _state = newState;

            // We lock the input on lemmy by default
            lemmy.isInputLocked = true;
            //lemmy.levelSession.Pause(true);

               //if (!GlobalManager.isLoaded) return;

            switch (newState)
            {
                case LevelState.preloading:
                    //GlobalManager.Instance.fullscreenImageHandler.DoInstantBlackScreen();



                    break;

                case LevelState.intro:

                    //GlobalManager.Instance.fullscreenImageHandler.FadeFromBlack(0.85f);

                    //create dummy level data if the scene is not in the database. Else load data from database
                    //if (GlobalManager.Instance.database.GetCurrentLevelFromSceneName(Application.loadedLevelName) == null)
                    //{
                    //    currentLevel = Level.CreateDummyLevel(Application.loadedLevelName);
                    //    currentLevel.numberOfLives = GlobalManager.Instance.gameplaySettings.defaultNumberOfLives;
                    //    currentLevel.levelType = debugLevelType;
                    //}
                    //else
                    //{
                    //    currentLevel = GlobalManager.Instance.database.GetCurrentLevelFromSceneName(Application.loadedLevelName);
                    //}

                    //levelTypeSettings = GetLevelTypeSettings(currentLevel.levelType);
                    //cam.SetBackgroundColor(levelTypeSettings.backgroundColor);

                    if (currentLevel == null)
                    {
                        lemmy.SetNumberOfLives(3);
                        //lemmy.SetNumberOfLives(GlobalManager.Instance.gameplaySettings.defaultNumberOfLives);
                    }
                    else
                    {
                        lemmy.SetNumberOfLives(currentLevel.numberOfLives);
                    }

                    lemmy.SpawnAt(startingCheckPoint);

                    //if (levelStartCheckPoint != null)
                    //{
                    //    lemmy.transform.position = levelStartCheckPoint.GetStartTweenPosition(0);
                    //}
                    lemmy.ChangeState(Lemmy.State.dormantBeforeSpawn);

                    //We assume the playing cinematic sequence will take care of all camera handling
                    //if (!isPlayingCinematicSequence)
                    //{
                    //    cam.MoveToStablePosition();
                    //    UpdateAllObjectsImmediatly(cam.gameObject);
                    //}

                    //lemmy.mainBody.LookRight();
                    lemmy.isInputLocked = false;
                    break;

                case LevelState.playing:

                    // We get and set all the pickups in level. This is used for endlevel feedback!
                    //PointPickup[] pickups = (PointPickup[])GameObject.FindObjectsOfType(typeof(PointPickup));
                    //lemmy.levelSession.totalNumberOfPickups = pickups.Length;
                    //Debug.Log("NUMBER OF PICKUPS IN LEVEL: " + lemmy.levelSession.totalNumberOfPickups);

                    lemmy.isInputLocked = false;
                    //lemmy.levelSession.Pause(false);
                    lemmy.ChangeState(Lemmy.State.normalActivity);
                    //lemmy.levelSession.StartTime();
                    //We assume the playing cinematic sequence will take care of all camera handling
                    //if (!isPlayingCinematicSequence)
                    //{
                    //    cam.followObject = lemmy.gameObject;
                    //    cam.FollowPathDefaultStats();
                    //}
                    break;

                case LevelState.paused:
                    break;

                case LevelState.respawning:
                    break;

                case LevelState.gameover:
                    //if (endLevelScreen != null)
                    //{
                    //    lemmy.levelSession.StopTime();
                    //    currentScreen = (BaseMenuScreen)Instantiate(deathScreen);
                    //    currentScreen.Init(DoGameOverCallback);
                    //}
                    break;

                case LevelState.outro:
                    //endSound.PlaySound();
                    //cam.PlaceObjectInViewPort(new Vector3(0, 2.2f, 8));
                    break;

                case LevelState.endscreen:

                    //lemmy.levelSession.StopTime();

                    //if (endLevelScreen != null)
                    //{
                    //    currentScreen = (BaseMenuScreen)Instantiate(endLevelScreen);
                    //    currentScreen.Init(DoEndLevelCallback);
                    //}
                    break;

                case LevelState.finalizeEndScreen:
                    //cam.FollowPathDefaultStats();
                    lemmy.BreakConnections();
                    break;

            }
        }

        //public void StartCinematicSequence(CinematicSequence _cinematicSequence)
        //{
        //    if (_isPlayingCinematicSequence)
        //    {
        //        Debug.LogError("THERE IS ALLREADY A CINEMATIC SEQUENCE RUNNING : " + currentCinematicSequence.name + "   IT MUST BE ENDED BEFORE A NEW ONE CAN BE STARTED. FOR NOW, IT IS AUTOMATICALLY ENDED");
        //        currentCinematicSequence.EndSequence();
        //    }

        //    //Lemmy has connections broken and speed is set to zero. This should take of the worst penetration with walls and other random stuff... hopefully...
        //    lemmy.BreakConnections();
        //    lemmy.rigidbody.velocity = Vector3.zero;

        //    _isPlayingCinematicSequence = true;
        //    currentCinematicSequence = _cinematicSequence;
        //    currentCinematicSequence.StartSequence();
        //}

        //public void EndCinematicSequence(CinematicSequence _cinematicSequence)
        //{
        //    _isPlayingCinematicSequence = false;
        //    _cinematicSequence.EndSequence();

        //    cam.followObject = lemmy.gameObject;
        //    cam.FollowPathDefaultStats();
        //}

        public override void FixedUpdate()
        {
            //UpdateTurnOffAtDistanceObjects(followObject);
        }

        public override void Update()
        {
            //if (!GlobalManager.isLoaded) return;

            //if (isCommunicatingWithServer && www.isDone)
            //{
            //    Debug.Log("Server Response: " + www.data);
            //    isCommunicatingWithServer = false;
            //}

            switch (state)
            {
                case LevelState.preloading:
                    DoStatePreloadingUpdate();
                    break;

                case LevelState.intro:
                    DoStateIntroUpdate();
                    break;

                case LevelState.playing:
                    DoStatePlayingUpdate();
                    break;

                case LevelState.paused:
                    DoStatePausedUpdate();
                    break;

                case LevelState.respawning:
                    DoStateRespawnUpdate();
                    break;

                case LevelState.outro:
                    DoStateOutroUpdate();
                    break;

                case LevelState.endscreen:
                    DoStateEndscreenUpdate();
                    break;

                case LevelState.finalizeEndScreen:
                    DoStateFinalizeEndScreenUpdate();
                    break;
            }

            _globalLevelTime += Time.deltaTime;
            _globalLevelDeltaTime = Time.deltaTime;
            _globalTimeSinceLemmySpawn += Time.deltaTime;

            if (state != LevelHandler.LevelState.preloading)
            {
                DoFramerateCheck();
            }
        }


        public void SkipLevel()
        {
            StartLevelExit();
        }

        public void NextCheckpoint()
        {
            int currentCheckpointIndex = 0;

            for (int i = 0; i < checkPoints.Length; i++)
            {
                if (lastActivatedCheckpoint == checkPoints[i])
                {
                    currentCheckpointIndex = i;
                    break;
                }
            }

            if (currentCheckpointIndex + 1 < checkPoints.Length)
            {
                Debug.Log("NextCheckpoint");
                ActivateCheckpoint(checkPoints[currentCheckpointIndex + 1]);
                RespawnAtLastCheckpoint();
            }
            else
            {
                Debug.Log("Trying to go to next checkpoint, but no more checkpoints");
            }
        }

        private void DoFramerateCheck()
        {
            //currentFramerate = 1 / Time.deltaTime;

            //if (doCriticalFrameRateDropCheck)
            //{
            //    if (criticalFrameRateDropCounter > framesBetweenCriticalFrameChecks)
            //    {
            //        if (currentFramerate < criticalFrameRateDropLimit)
            //        {
            //            PPMetrics.AddPositionString("critical_framerate_drop", LevelHandler.Instance.lemmy.transform.position, currentFramerate.ToString());
            //            criticalFrameRateDropCounter = 0;
            //        }
            //    }
            //    criticalFrameRateDropCounter++;
            //}

            ////calculate avg. framerate
            //averageFrameRate = ((averageFrameRate * frameRateCalcCnt) + currentFramerate) / (frameRateCalcCnt + 1);
            //frameRateCalcCnt++;

            ////calculate average over 10 frames
            //lastFrames[frameIndex] = currentFramerate;
            //averageFramerate10Frames = 0;
            //for (int i = 0; i < lastFrames.Length; i++)
            //{
            //    averageFramerate10Frames += lastFrames[i];
            //}
            //averageFramerate10Frames /= lastFrames.Length;
            //frameIndex = (frameIndex + 1) % lastFrames.Length;
        }

        protected void DoStatePreloadingUpdate()
        {
            //if (objectPreloader.isComplete)
            //{
            //    //this is for debugging purposes. Lemmy is moved to another checkpoint and play is immediate. No intro is played.
            //    if (doStandardSpawnInsteadOfIntro)
            //    {
            //        ChangeState(LevelState.intro);
            //        ChangeState(LevelState.playing);
            //        lemmy.SpawnAt(startingCheckPoint);
            //    }
            //    else
            //    {
            ChangeState(LevelState.intro);
            //    }
            //}
        }

        protected void DoStateIntroUpdate()
        {
            //if (InputHandler.Instance.GetShootTentacle() || Time.time > lastStateChange + GlobalManager.Instance.gameplaySettings.levelIntroDuration)
            //{
            ChangeState(LevelState.playing);
            //    return;
            //}


            //float stateLengthFraction = (Time.time - lastStateChange) / GlobalManager.Instance.gameplaySettings.levelIntroDuration;

            //stateLengthFraction = Mathf.Min(1, stateLengthFraction);

            //if (levelStartCheckPoint != null)
            //{
            //    lemmy.Push((levelStartCheckPoint.GetStartTweenPosition(stateLengthFraction) - lemmy.transform.position) * 350f * Time.deltaTime);
            //}
        }

        protected void DoStatePlayingUpdate()
        {

        }

        protected void DoStatePausedUpdate()
        {

        }

        protected void DoStateOutroUpdate()
        {
            //if (Time.time > lastStateChange + GlobalManager.Instance.gameplaySettings.levelOutroDuration)
            //{
            //    state = LevelState.endscreen;
            //}
        }

        protected void DoStateEndscreenUpdate()
        {

        }


        protected void DoStateFinalizeEndScreenUpdate()
        {
            //if (Time.time > lastStateChange + GlobalManager.Instance.gameplaySettings.finalizeEndScreenDuration)
            //{
            //    if (currentLevel.id == -1)
            //    {
            //        GlobalManager.Instance.OpenMainMenu();
            //        return;
            //    }

            //    GlobalManager.Instance.OpenNextLevel();
            //    return;
            //}

            //if (Time.time > lastStateChange + GlobalManager.Instance.gameplaySettings.finalizeEndScreenDuration * 0.5f)
            //{
            //    if (GlobalManager.Instance.fullscreenImageHandler.fadeToBlackState == FullScreenImageHandler.FadeToBlackState.clearScreen)
            //    {
            //        GlobalManager.Instance.fullscreenImageHandler.FadeToBlack(GlobalManager.Instance.gameplaySettings.finalizeEndScreenDuration * 0.5f);
            //    }


            //}

            //float stateLengthFraction = (Time.time - lastStateChange) / GlobalManager.Instance.gameplaySettings.finalizeEndScreenDuration;

            //stateLengthFraction = Mathf.Min(1, stateLengthFraction);

            //if (levelExit != null)
            //{
            //    lemmy.Push((levelExit.GetMoveOutTweenPosition(stateLengthFraction) - lemmy.transform.position) * 5);
            //}
        }

        protected void DoStateRespawnUpdate()
        {
            //if (GlobalManager.Instance.fullscreenImageHandler.fadeToBlackState == FullScreenImageHandler.FadeToBlackState.clearScreen && Time.time > lastStateChange + GlobalManager.Instance.gameplaySettings.timeToRespawn - 0.35f)
            //{
            //    GlobalManager.Instance.fullscreenImageHandler.FadeToBlack(0.35f);
            //}

            //if (Time.time > lastStateChange + GlobalManager.Instance.gameplaySettings.timeToRespawn)
            //{
            //    DoRespawnAtLastCheckpoint();
            //}
        }

        #region checkpoint code
        private void CreateArrayOfCheckpoints()
        {
            if (levelStartCheckPoint == null)
            {
                Debug.LogError("No levelStartCheckPoint set. This is BAD!!! ALL LEVELS MUST HAVE A START CHECKPOINT!!");
            }

            Object[] tmpCheckpointArray = FindObjectsOfType(typeof(CheckPoint));

            //use manually set checkpoint order
            if (checkpointOrder.Length == tmpCheckpointArray.Length)
            {
                checkPoints = checkpointOrder;
                for (int i = 0; i < checkPoints.Length; i++)
                {
                    //only assign start checkpoint if in the editor. This should always only be used for debug.
                    //if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
                    //{
                    //    if (checkPoints[i].start)
                    //    {
                    //        doStandardSpawnInsteadOfIntro = true;
                    //        startingCheckPoint = checkPoints[i];
                    //    }
                    //}
                }
            }
            else
            {
                checkPoints = new CheckPoint[tmpCheckpointArray.Length];
                for (int i = 0; i < tmpCheckpointArray.Length; i++)
                {

                    checkPoints[i] = (CheckPoint)(tmpCheckpointArray[i]);

                    //only assign start checkpoint if in the editor. This should always only be used for debug.
                    //if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
                    //{
                    //    if (checkPoints[i].start)
                    //    {
                    //        doStandardSpawnInsteadOfIntro = true;
                    //        startingCheckPoint = checkPoints[i];
                    //    }
                    //}
                }
            }


            //if the starting checkpoint is not set, do it now
            if (startingCheckPoint == null)
            {
                startingCheckPoint = levelStartCheckPoint;
            }



        }

        public void DoOnLemmyDeath()
        {
            //reset objects
            //for (int i = 0; i < resetOnDeathObjects.Length; i++)
            //{
            //    if (resetOnDeathObjects[i].resetTiming == ResetOnLemmyDeath.ResetTiming.OnDeath)
            //    {
            //        resetOnDeathObjects[i].Reset();
            //    }
            //}
        }

        public void DoBeforeLemmyRespawn()
        {
            //reset objects
            //for (int i = 0; i < resetOnDeathObjects.Length; i++)
            //{
            //    if (resetOnDeathObjects[i].resetTiming == ResetOnLemmyDeath.ResetTiming.BeforeRespawn)
            //    {
            //        resetOnDeathObjects[i].Reset();
            //    }
            //}
        }

        public void DoAfterLemmyRespawn()
        {
            //reset objects
            //for (int i = 0; i < resetOnDeathObjects.Length; i++)
            //{
            //    if (resetOnDeathObjects[i].resetTiming == ResetOnLemmyDeath.ResetTiming.AfterRespawn)
            //    {
            //        resetOnDeathObjects[i].Reset();
            //    }
            //}
        }

        public void ActivateCheckpoint(CheckPoint _checkpoint)
        {
            if (activatedCheckPoints.Contains(_checkpoint))
            {
                return;
            }

            activatedCheckPoints.Add(_checkpoint);
            if (!_checkpoint.isCheckPointActive)
            {
                _checkpoint.ActivateCheckPoint();
            }

            lastActivatedCheckpoint = _checkpoint;
        }

        public CheckPoint GetStartingCheckPoint()
        {
            if (startingCheckPoint != null)
            {
                return startingCheckPoint;
            }

            if (checkPoints.Length > 0)
            {
                startingCheckPoint = checkPoints[0];
                return startingCheckPoint;
            }

            return null;
        }

        public void RespawnAtLastCheckpoint()
        {
            ChangeState(LevelState.respawning);
        }

        private void DoRespawnAtLastCheckpoint()
        {
            DoBeforeLemmyRespawn();

            //GlobalManager.Instance.fullscreenImageHandler.FadeFromBlack(0.35f);
            RespawnAtCheckpoint(GetLastActivatedCheckPoint());
            ChangeState(LevelState.playing);

            //reset time since spawn
            _globalTimeSinceLemmySpawn = 0;

            DoAfterLemmyRespawn();
        }

        private void RespawnAtCheckpoint(CheckPoint _checkPoint)
        {
            //if (lemmyHunter != null)
            //{
            //    if (GetLastActivatedCheckPoint().connectionedNode != null)
            //    {
            //        lemmyHunter.GotoNode(_checkPoint.connectionedNode);
            //    }
            //    else
            //    {
            //        lemmyHunter.Reset();
            //    }
            //}

            lemmy.SpawnAt(_checkPoint);

            //cam.MoveToStablePosition();
            //UpdateAllObjectsImmediatly(cam.gameObject);
        }

        public CheckPoint GetLastActivatedCheckPoint()
        {
            return lastActivatedCheckpoint;
        }
        #endregion

        public void RestartLevel()
        {
            //GlobalManager.Instance.OpenLevel(GlobalManager.Instance.currentLevel);
        }

        public void StartLevelExit()
        {
            ChangeState(LevelState.outro);
        }

        private void DoEndLevelCallback()
        {
            SubmitStatistics();

            ChangeState(LevelState.finalizeEndScreen);
        }

        private void SubmitStatistics()
        {
            //if (PPMetrics.playsessionId == -1) return;

            //PPMetrics.AddFloat("level_id", currentLevel.id);
            //PPMetrics.AddFloat("score", lemmy.levelSession.score);
            //PPMetrics.AddFloat("playtime", lemmy.levelSession.time);
            //PPMetrics.AddFloat("average_framerate", averageFrameRate);

            //string url = "http://explosivelove.com/tentacles/";

            //WWWForm form = PPMetrics.GetPostData();
            //form.AddField("action", "save_metrics");
            //form.AddField("playsession_id", PPMetrics.playsessionId);
            //form.AddField("level_id", currentLevel.id);
            //www = new WWW(url, form);
            //isCommunicatingWithServer = true;
        }

        private void DoGameOverCallback()
        {
            //GlobalManager.Instance.RestartCurrentLevel();
        }

        public Microsoft.Xna.Framework.Quaternion GetCameraRelativeRotation(Vector3 direction)
        {
            // TODO: This is a missing function in XNA
            return Microsoft.Xna.Framework.Quaternion.Identity;//Quaternion.LookRotation(LevelHandler.Instance.feedbackCam.transform.TransformDirection(direction));
        }


        private LevelTypeSettings GetLevelTypeSettings(Level.LevelType _type)
        {
            switch (_type)
            {
                case Level.LevelType.brain:
                    //return ((GameObject)Resources.Load("LevelSettings/BRAIN_settings")).GetComponent<LevelTypeSettings>();
                    break;

                case Level.LevelType.veins:
                    //return ((GameObject)Resources.Load("LevelSettings/VEINS_settings")).GetComponent<LevelTypeSettings>();
                    break;

                case Level.LevelType.intestines:
                    //return ((GameObject)Resources.Load("LevelSettings/INTESTINES_settings")).GetComponent<LevelTypeSettings>();
                    break;

                case Level.LevelType.petriDish:
                    //return ((GameObject)Resources.Load("LevelSettings/PETRIDISH_settings")).GetComponent<LevelTypeSettings>();
                    break;

                case Level.LevelType.desatGreen:
                    //return ((GameObject)Resources.Load("LevelSettings/DESATGREEN_settings")).GetComponent<LevelTypeSettings>();
                    break;
            }

            return null;
        }
    }
}
