using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.Components;
using PressPlay.FFWD.Interfaces;
using System.Text;

namespace PressPlay.FFWD
{
    public class Application : DrawableGameComponent
    {
        public Application(Game game)
            : base(game)
        {
            UpdateOrder = 1;
            DrawOrder = 1;
        }

        private SpriteBatch spriteBatch;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        private static string sceneToLoad = "";

#if DEBUG
        private ComponentProfiler componentProfiler = new ComponentProfiler();

        private Stopwatch frameTime = new Stopwatch();
        private Stopwatch timeUpdateEndUpdateStart = new Stopwatch();
        private Stopwatch updateTime = new Stopwatch();
        private Stopwatch fixedUpdateTime = new Stopwatch();
        private Stopwatch lateUpdateTime = new Stopwatch();
        //private Stopwatch awakeTime = new Stopwatch();
        //private Stopwatch startTime = new Stopwatch();
        private Stopwatch physics = new Stopwatch();
        private Stopwatch graphics = new Stopwatch();
        public static Stopwatch iTweenUpdateTime = new Stopwatch();
        public static Stopwatch raycastTimer = new Stopwatch();
        public static Stopwatch lemmyStuffTimer = new Stopwatch();
        public static Stopwatch turnOffTimer = new Stopwatch();
        public static Stopwatch particleAnimTimer = new Stopwatch();
        public static Stopwatch particleEmitTimer = new Stopwatch();
        public static Stopwatch particleDrawTimer = new Stopwatch();
        public static int particleDraws = 0;
#endif

        public static ScreenManager.ScreenManager screenManager;

        private static Dictionary<int, UnityObject> objects = new Dictionary<int, UnityObject>();
        internal static List<Component> newComponents = new List<Component>();
        internal static List<Asset> newAssets = new List<Asset>();

        private static List<Component> activeComponents = new List<Component>();
        internal static List<UnityObject> markedForDestruction = new List<UnityObject>();
        internal static List<GameObject> dontDestroyOnLoad = new List<GameObject>();
        private static List<Interfaces.IUpdateable> lateUpdates = new List<Interfaces.IUpdateable>();

        internal static bool loadingScene = false;

        // Lists and variables used for loading a scene
        internal static bool isLoadingAssetBeforeSceneInitialize = false;
        internal static bool loadIsComplete = false;
        internal static bool hasDrawBeenCalled = false;
        private static int totalNumberOfAssetsToLoad = 0;
        private static int numberOfAssetsLoaded = 0;
        internal static StringBuilder progressString = new StringBuilder();
        internal static float _loadingProgess = 0;
        public static float loadingProgress
        {
            get
            {
                return _loadingProgess;
            }
        }
        private static Scene scene;
        private static Stopwatch stopWatch = new Stopwatch();
        internal static List<Component> tempComponents = new List<Component>();
        internal static List<Asset> tempAssets = new List<Asset>();

        private static AssetHelper assetHelper = new AssetHelper();

        public override void Initialize()
        {
            base.Initialize();
            ContentHelper.Services = Game.Services;
            ContentHelper.StaticContent = new ContentManager(Game.Services, Game.Content.RootDirectory);
            ContentHelper.Content = new ContentManager(Game.Services, Game.Content.RootDirectory);
            ContentHelper.IgnoreMissingAssets = true;
            Camera.FullScreen = Game.GraphicsDevice.Viewport;
            Resources.AssetHelper = assetHelper;
            Physics.Initialize();
            Time.Reset();
            Input.Initialize();
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            assetHelper.CreateContentManager = CreateContentManager;
        }

        private ContentManager CreateContentManager()
        {
            return new ContentManager(Game.Services, Game.Content.RootDirectory);
        }

        public override void Update(GameTime gameTime)
        {
#if DEBUG
            timeUpdateEndUpdateStart.Stop(); //measure time since last draw ended to try and measure graphics performance
#endif
            if (Application.quitNextUpdate)
            {
                base.Game.Exit();
                return;
            }
            
            base.Update(gameTime);
            Time.Update((float)gameTime.ElapsedGameTime.TotalSeconds, (float)gameTime.TotalGameTime.TotalSeconds);
            UpdateFPS(gameTime);

            if (isLoadingAssetBeforeSceneInitialize)
            {
                if (loadIsComplete)
                {
                    OnSceneLoadComplete();
                }
                else
                {
                    if (hasDrawBeenCalled)
                    {
                        LoadSceneAssets();
                    }

                    CalculateLoadingProgress();
                }
            }

            if (!String.IsNullOrEmpty(sceneToLoad))
            {
                CleanUp();
                GC.Collect();
                DoSceneLoad();
            }
            LoadNewAssets();

#if DEBUG
            fixedUpdateTime.Start();
#endif
            AwakeNewComponents();
            for (int i = 0; i < activeComponents.Count; i++)
            {
                if (!activeComponents[i].isStarted)
                {
                    activeComponents[i].Start();
                    activeComponents[i].isStarted = true;
                }
                if (activeComponents[i].gameObject == null || !activeComponents[i].gameObject.active)
                {
                    continue;
                }
                if (activeComponents[i] is IFixedUpdateable)
                {
#if DEBUG && COMPONENT_PROFILE
                    componentProfiler.StartFixedUpdateCall(activeComponents[i]);
#endif
                    (activeComponents[i] as IFixedUpdateable).FixedUpdate();
#if DEBUG && COMPONENT_PROFILE
                    componentProfiler.EndFixedUpdateCall();
#endif
                }
            }
#if DEBUG
            fixedUpdateTime.Stop();
            physics.Start();
#endif
            //Physics.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            Physics.Update(Time.deltaTime);
#if DEBUG
            physics.Stop();
#endif

            hasDrawBeenCalled = false;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Time.Draw();

            hasDrawBeenCalled = true;

            frameCounter++;
#if DEBUG
            updateTime.Start();
#endif
            
            for (int i = 0; i < activeComponents.Count; i++)
            {
                if (!activeComponents[i].isStarted)
                {
                    activeComponents[i].Start();
                    activeComponents[i].isStarted = true;
                }
                if (activeComponents[i].gameObject == null || !activeComponents[i].gameObject.active)
                {
                    continue;
                }
                if (activeComponents[i] is PressPlay.FFWD.Interfaces.IUpdateable)
                {
#if DEBUG && COMPONENT_PROFILE
                    componentProfiler.StartUpdateCall(activeComponents[i]);
#endif
                    (activeComponents[i] as PressPlay.FFWD.Interfaces.IUpdateable).Update();

#if DEBUG && COMPONENT_PROFILE
                    componentProfiler.EndUpdateCall();
#endif
                    lateUpdates.Add((activeComponents[i] as PressPlay.FFWD.Interfaces.IUpdateable));
                }
                if ((activeComponents[i] is MonoBehaviour))
                {
                    (activeComponents[i] as MonoBehaviour).UpdateInvokeCalls();
                }
            }
#if DEBUG
            updateTime.Stop();
            lateUpdateTime.Start();
#endif
            for (int i = 0; i < lateUpdates.Count; i++)
            {
                    lateUpdates[i].LateUpdate();
            }

            CleanUp();
#if DEBUG
            lateUpdateTime.Stop();
            graphics.Start();
#endif
            Camera.DoRender(GraphicsDevice);
#if DEBUG
            graphics.Stop();
            double total = fixedUpdateTime.Elapsed.TotalSeconds + lateUpdateTime.Elapsed.TotalSeconds + updateTime.Elapsed.TotalSeconds + graphics.Elapsed.TotalSeconds + physics.Elapsed.TotalSeconds;
            if (ApplicationSettings.ShowDebugLines)
            {
                //Camera lineCam = (String.IsNullOrEmpty(ApplicationSettings.DebugLineCamera)) ? Camera.main : Camera.FindByName(ApplicationSettings.DebugLineCamera);
                Camera lineCam = ApplicationSettings.DebugCamera;
                
                Debug.DrawLines(GraphicsDevice, lineCam);
                if (lineCam != null)
               //Camera lineCam = ApplicationSettings.DebugCamera;

                /*if (ApplicationSettings.DebugCamera == null)
                {
                    ApplicationSettings.DebugCamera = LevelHandler.Instance.cam.GUICamera;
                }*/

                Debug.DrawLines(GraphicsDevice, ApplicationSettings.DebugCamera);
                /*if (lineCam != null)
                {
                    Debug.Display(lineCam.name, lineCam.transform.position);
                }*/
            }

#if COMPONENT_PROFILE
            componentProfiler.Sort();
            Debug.Display("GetWorst()", componentProfiler.GetWorst());
            componentProfiler.FlushData();
#endif
            if (ApplicationSettings.ShowiTweenUpdateTime)
            {
                Debug.Display("iTweenUpdateTime", iTweenUpdateTime.ElapsedMilliseconds);
                iTweenUpdateTime.Reset();
            }


            if (ApplicationSettings.ShowRaycastTime)
            {
                Debug.Display("Raycasts ms", Application.raycastTimer.ElapsedMilliseconds);
                raycastTimer.Reset();
            }
            if (ApplicationSettings.ShowRaycastTime)
            {
                Debug.Display("Lemmystuff ms", Application.lemmyStuffTimer.ElapsedMilliseconds);
                lemmyStuffTimer.Reset();
            }
            if (ApplicationSettings.ShowTurnOffTime)
            {
                Debug.Display("TurnOffTime ms", Application.turnOffTimer.ElapsedMilliseconds);
                turnOffTimer.Reset();
            }
            if (ApplicationSettings.ShowParticleAnimTime)
            {
                Debug.Display("Particle Anim ms", Application.particleAnimTimer.ElapsedMilliseconds);
                particleAnimTimer.Reset();
                Debug.Display("Particle Emit ms", Application.particleEmitTimer.ElapsedMilliseconds);
                particleEmitTimer.Reset();
                Debug.Display("Particle Draw ms", Application.particleDrawTimer.ElapsedMilliseconds);
                particleDrawTimer.Reset();
                Debug.Display("Particle Draw calls", Application.particleDraws);
                Application.particleDraws = 0;
            }
            if (ApplicationSettings.ShowTimeBetweenUpdates)
            {
                Debug.Display("TimeBetweenUpdates", timeUpdateEndUpdateStart.ElapsedMilliseconds);
                timeUpdateEndUpdateStart.Reset();
            }
            
            if (ApplicationSettings.ShowFPSCounter)
            {
                Debug.Display("FPS", String.Format("{0} ms {1}", frameRate, frameTime.ElapsedMilliseconds));
                //Debug.Display("frame time", frameTime.ElapsedMilliseconds);
                frameTime.Reset();
                frameTime.Start();
            }
            if (ApplicationSettings.ShowPerformanceBreakdown)
            {
                //Debug.Display("% S | P | G", String.Format("{0:P1} | {1:P1} | {2:P1}", scripts.Elapsed.TotalSeconds / total, physics.Elapsed.TotalSeconds / total, graphics.Elapsed.TotalSeconds / total));
                Debug.Display("ms U | P | G", String.Format("{0}ms | {1}ms | {2}ms", updateTime.Elapsed.Milliseconds + fixedUpdateTime.Elapsed.Milliseconds + lateUpdateTime.Elapsed.Milliseconds, physics.Elapsed.Milliseconds, graphics.Elapsed.Milliseconds));
            }
            if (ApplicationSettings.ShowDebugDisplays)
	        {
		        spriteBatch.Begin();

                KeyValuePair<string, string>[] displayStrings = Debug.DisplayStrings.ToArray();
                Microsoft.Xna.Framework.Vector2 Position = new Microsoft.Xna.Framework.Vector2(32, 32);
                Microsoft.Xna.Framework.Vector2 offset = Microsoft.Xna.Framework.Vector2.Zero;
                for (int i = 0; i < displayStrings.Length; i++)
                {
                    string text = displayStrings[i].Key + ": " + displayStrings[i].Value;
                    spriteBatch.DrawString(ApplicationSettings.DebugFont, text, Position + Microsoft.Xna.Framework.Vector2.One + offset, Microsoft.Xna.Framework.Color.Black);
                    spriteBatch.DrawString(ApplicationSettings.DebugFont, text, Position + offset, Microsoft.Xna.Framework.Color.White);
                    offset.Y += ApplicationSettings.DebugFont.MeasureString(text).Y * 0.75f;
                }

                spriteBatch.End();
            }
            updateTime.Reset();
            lateUpdateTime.Reset();
            fixedUpdateTime.Reset();
            physics.Reset();
            graphics.Reset();

            timeUpdateEndUpdateStart.Start(); //measure time from draw ended to beginning of Update, to try and measure graphics performance
#endif
        }

        private void UpdateFPS(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        private void DoSceneLoad()
        {
            Debug.Log("DoSceneLoad: " + sceneToLoad);
            
            if (!String.IsNullOrEmpty(loadedLevelName))
            {
                UnloadCurrentLevel();
                CleanUp();
                assetHelper.Unload(loadedLevelName);
            }

            loadingScene = true;
            isLoadingAssetBeforeSceneInitialize = true;
            loadIsComplete = false;

            loadedLevelName = sceneToLoad.Contains('/') ? sceneToLoad.Substring(sceneToLoad.LastIndexOf('/') + 1) : sceneToLoad;
            scene = assetHelper.Load<Scene>(sceneToLoad);
            sceneToLoad = "";

            totalNumberOfAssetsToLoad = tempAssets.Count;
            numberOfAssetsLoaded = 0;
            //Debug.Log("TempAssets.Count: "+tempAssets.Count);

            if (scene == null)
            {
                OnSceneLoadComplete();
            }
        }

        private void LoadSceneAssets()
        {
            //Debug.Log("Application > LoadSceneAssets. Assets left to load: "+tempAssets.Count);

            stopWatch.Start();

            int count = 0;

            for (int i = tempAssets.Count - 1; i >= 0; i--)
            {
                //Debug.Log("Assets left: "+tempAssets.Count+" Elapsed time: " + stopWatch.ElapsedMilliseconds);

                if (stopWatch.ElapsedTicks > ApplicationSettings.AssetLoadInterval)
                {
                    //Debug.Log("Application > Chewing asset loading. Assets left to load: " + tempAssets.Count);
                    stopWatch.Stop();
                    stopWatch.Reset();
                    return;
                }

                tempAssets[i].LoadAsset(assetHelper);
                tempAssets.RemoveAt(i);
                numberOfAssetsLoaded++;
                count++;
            }

            //Debug.Log("Finished asset loading. Elapsed time: " + stopWatch.ElapsedTicks + " count: " + count);

            //OnSceneLoadComplete();
            loadIsComplete = true;
        }

        private void CalculateLoadingProgress()
        {
            if (totalNumberOfAssetsToLoad == 0)
            {
                _loadingProgess = 1;
            }
            else
            {
                _loadingProgess = Mathf.Clamp01(((float)numberOfAssetsLoaded / (float)totalNumberOfAssetsToLoad));
            }

            //Debug.Log("Application.loadingProgress: " + loadingProgress);
        }

        private void OnSceneLoadComplete()
        {
            
            Debug.Log("OnSceneLoadComplete");

            stopWatch.Stop();
            stopWatch.Reset();

            newComponents.AddRange(tempComponents);
            tempComponents.Clear();

            loadingScene = false;
            isLoadingAssetBeforeSceneInitialize = false;
            loadIsComplete = false;

            if (scene != null)
            {
                scene.Initialize();
            }

            _loadingProgess = 0;

            GC.Collect();
        }

        internal static void LoadNewAssets()
        {
            for (int i = newAssets.Count - 1; i >= 0; i--)
            {
                newAssets[i].LoadAsset(assetHelper);
                newAssets.RemoveAt(i);
            }
        }

        public static void LoadLevel(string name)
        {
            sceneToLoad = name;
            UnloadCurrentLevel();
        }

        public static void UnloadCurrentLevel()
        {
            foreach (UnityObject obj in objects.Values)
            {
                if (obj is GameObject)
                {
                    GameObject gObj = (GameObject)obj;

                    if (!dontDestroyOnLoad.Contains(gObj))
                    {
                        UnityObject.Destroy(gObj);
                    }
                }
            }
        }

        public static UnityObject Find(int id)
        {
            if (objects.ContainsKey(id))
            {
                return objects[id];
            }
            return null;
        }

        internal static T[] FindObjectsOfType<T>() where T : UnityObject
        {
            List<T> list = new List<T>();
            foreach (UnityObject obj in objects.Values)
            {
                T myObj = obj as T;
                if (myObj != null)
                {
                    list.Add(myObj);
                }
            }
            return list.ToArray();
        }

        internal static UnityObject[] FindObjectsOfType(Type type)
        {
            List<UnityObject> list = new List<UnityObject>();
            foreach (UnityObject obj in objects.Values)
            {
                if (obj.GetType() == type && !obj.isPrefab)
                {
                    list.Add(obj);
                }
            }
            return list.ToArray();
        }

        internal static UnityObject FindObjectOfType(Type type)
        {
            foreach (UnityObject obj in objects.Values)
            {
                if (obj.GetType() == type && !obj.isPrefab)
                {
                    return obj;
                }
            }
            return null;
        }

        internal static void AwakeNewComponents()
        {
            for (int i = 0; i < newComponents.Count; i++)
            {
                Component cmp = newComponents[i];
                if (cmp.gameObject != null)
                {
                    // TODO: Fix this with a content processor!
                    // Purge superfluous Transforms that is created when GameObjects are imported from the scene
                    if ((cmp is Transform) && (cmp.gameObject.transform != cmp))
                    {
                        cmp.gameObject = null;
                        continue;
                    }
                    objects.Add(cmp.GetInstanceID(), cmp);

                    if (!cmp.isPrefab)
                    {
                        activeComponents.Add(cmp);
                        if (cmp is Renderer)
                        {
                            Camera.AddRenderer(cmp as Renderer);
                        }
                    }
                    if (!objects.ContainsKey(cmp.gameObject.GetInstanceID()))
                    {
                        objects.Add(cmp.gameObject.GetInstanceID(), cmp.gameObject);
                    }
                }
            }
            // All components will exist to be found before awaking - otherwise we can get issues with instantiating on awake.
            Component[] componentsToAwake = newComponents.ToArray();
            newComponents.Clear();
            for (int i = 0; i < componentsToAwake.Length; i++)
            {
                if (!componentsToAwake[i].isPrefab)
                {
                    componentsToAwake[i].Awake();
                }
            }
            // Do a recursive awake to awake components instantiated in the previous awake.
            // In this way we will make sure that everything is instantiated before the first run.
            if (newComponents.Count > 0)
            {
                AwakeNewComponents();
            }
        }

        internal static void AddNewComponent(Component component)
        {

            if (isLoadingAssetBeforeSceneInitialize)
            {
                tempComponents.Add(component);
            }
            else
            {
                newComponents.Add(component);
            }
        }

        internal static void AddNewAsset(Asset asset)
        {
            newAssets.Add(asset);

            if (isLoadingAssetBeforeSceneInitialize)
            {
                tempAssets.Add(asset);
            }
            else
            {
                newAssets.Add(asset);
            }
        }

        internal static void Reset()
        {
            objects.Clear();
            activeComponents.Clear();
            markedForDestruction.Clear();
            lateUpdates.Clear();
        }

        internal static void CleanUp()
        {
            for (int i = 0; i < markedForDestruction.Count; i++)
            {
                objects.Remove(markedForDestruction[i].GetInstanceID());

                if (markedForDestruction[i] is Component)
	            {
                    Component cmp = (markedForDestruction[i] as Component);
                    if (cmp is Renderer)
                    {
                        Camera.RemoveRenderer(cmp as Renderer);
                    }

                    if (cmp is Camera)
                    {
                        Camera.RemoveCamera(cmp as Camera);
                    }

                    if (cmp.gameObject != null)
                    {
                        cmp.gameObject.RemoveComponent(cmp);
                    }

                    if (newComponents.Contains(cmp))
                    {
                        newComponents.Remove(cmp);
                    }

                    activeComponents.Remove(cmp);

	            }
            }
            markedForDestruction.Clear();
            lateUpdates.Clear();
        }

        public static string loadedLevelName { get; private set; }

        internal static void DontDestroyOnLoad(UnityObject target)
        {
            if (target is Component)
            {
                if(!dontDestroyOnLoad.Contains(((Component)target).gameObject))
                {
                    dontDestroyOnLoad.Add(((Component)target).gameObject);
                }
            }

            if (target is GameObject)
            {
                if (!dontDestroyOnLoad.Contains((GameObject)target))
                {
                    dontDestroyOnLoad.Add((GameObject)target);
                }
            }
        }


        private static bool quitNextUpdate = false;
        
        /// <summary>
        /// Quits the application using game.Exit in the begin of the next Update 
        /// </summary>
        public static void Quit()
        {
            quitNextUpdate = true;
        }

        public static T Load<T>(string name)
        {
            return assetHelper.Load<T>(name);
        }

        public static void Preload<T>(string name)
        {
            assetHelper.Preload<T>(name);
        }

        public static void PreloadInstant<T>(string name)
        {
            assetHelper.PreloadInstant<T>(name);
        }
    }
}
