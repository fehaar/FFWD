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
using System.Reflection;

namespace PressPlay.FFWD
{
    public class Application : DrawableGameComponent
    {
        public Application(Game game)
            : base(game)
        {
            UpdateOrder = 1;
            DrawOrder = 0;

            isUpdateable.Add(typeof(iTween));
            isLateUpdateable.Add(typeof(iTween));
            isFixedUpdateable.Add(typeof(iTween));
            isUpdateable.Add(typeof(PressPlay.FFWD.UI.Controls.ScrollingPanelControl));
        }

#if DEBUG
        private SpriteBatch spriteBatch;
#endif

        public static bool isDeactivated = false;
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

        private static readonly Dictionary<int, UnityObject> objects = new Dictionary<int, UnityObject>(5000);
        internal static readonly List<Asset> newAssets = new List<Asset>(100);

        internal static readonly List<Component> newComponents = new List<Component>();
        private static readonly Queue<Component> componentsToAwake = new Queue<Component>(2500);
        private static readonly Queue<Component> instantiatedComponentsToAwake = new Queue<Component>(50);
        private static readonly List<Component> componentsToStart = new List<Component>();
        private static readonly List<PressPlay.FFWD.Interfaces.IUpdateable> updateComponents = new List<PressPlay.FFWD.Interfaces.IUpdateable>(500);
        private static readonly List<PressPlay.FFWD.Interfaces.IFixedUpdateable> fixedUpdateComponents = new List<PressPlay.FFWD.Interfaces.IFixedUpdateable>(100);
        private static readonly List<PressPlay.FFWD.Interfaces.IUpdateable> lateUpdateComponents = new List<PressPlay.FFWD.Interfaces.IUpdateable>(100);
        private static readonly List<Component> componentsChangingActivity = new List<Component>(50);

        private static readonly TypeSet isUpdateable = new TypeSet(100);
        private static readonly TypeSet isFixedUpdateable = new TypeSet(25);
        private static readonly TypeSet isLateUpdateable = new TypeSet(25);
        private static readonly TypeSet hasAwake = new TypeSet(50);
        internal static readonly TypeSet fixReferences = new TypeSet(5);

        private static readonly List<InvokeCall> invokeCalls = new List<InvokeCall>(10);

        internal static readonly List<UnityObject> markedForDestruction = new List<UnityObject>();
        internal static readonly List<GameObject> dontDestroyOnLoad = new List<GameObject>(50);

        internal static bool loadingScene = false;

        // Lists and variables used for loading a scene
        public static bool isLoadingAssetBeforeSceneInitialize = false;
        private static bool doGarbageCollectAfterAwake = false;
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
        internal static readonly List<Component> tempComponents = new List<Component>();
        internal static readonly List<Asset> tempAssets = new List<Asset>();

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
            assetHelper.CreateContentManager = CreateContentManager;
            Camera.spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            Camera.basicEffect = new BasicEffect(Game.GraphicsDevice);
            // Note we cannot share this as it is used in between cameras as it is done now
            TextRenderer3D.basicEffect = new BasicEffect(Game.GraphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true,
                World = TextRenderer3D.invertY,
                View = Matrix.Identity
            };
            TextRenderer3D.batch = new SpriteBatch(Game.GraphicsDevice);

#if DEBUG
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
#endif
        }

        private ContentManager CreateContentManager()
        {
            return new ContentManager(Game.Services, Game.Content.RootDirectory);
        }

        private void StartComponents()
        {
            for (int i = 0; i < componentsToStart.Count; i++)
            {
                Component cmp = componentsToStart[i];
                componentsChangingActivity.Add(cmp);
                cmp.Start();
            }
            componentsToStart.Clear();
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
            Time.FixedUpdate((float)gameTime.ElapsedGameTime.TotalSeconds, (float)gameTime.TotalGameTime.TotalSeconds);
            UpdateFPS(gameTime);

            if (isLoadingAssetBeforeSceneInitialize)
            {
                if (loadIsComplete)
                {
                    OnSceneLoadComplete();
                    return;
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
                DoSceneLoad();
            }
            LoadNewAssets();

#if DEBUG
            fixedUpdateTime.Start();
#endif
            AwakeNewComponents(false);
            StartComponents();
            ChangeComponentActivity();
            if (Time.timeScale > 0)
            {
                int count = fixedUpdateComponents.Count;
                for (int i = 0; i < count; i++)
                {
                    IFixedUpdateable cmp = fixedUpdateComponents[i];
                    if (!cmp.gameObject.active)
                    {
                        continue;
                    }
#if DEBUG && COMPONENT_PROFILE
                componentProfiler.StartFixedUpdateCall(fixedUpdateComponents[i] as Component);
#endif
                    cmp.FixedUpdate();
#if DEBUG && COMPONENT_PROFILE
                componentProfiler.EndFixedUpdateCall();
#endif
                }
            }
            ChangeComponentActivity();
#if DEBUG
            fixedUpdateTime.Stop();
            physics.Start();
#endif
            Physics.Update(Time.deltaTime);
#if DEBUG
            physics.Stop();
#endif

            hasDrawBeenCalled = false;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Time.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            hasDrawBeenCalled = true;

            frameCounter++;
#if DEBUG
            updateTime.Start();
#endif

            StartComponents();
            ChangeComponentActivity();
            int count = updateComponents.Count;
            for (int i = 0; i < count; i++)
            {
                PressPlay.FFWD.Interfaces.IUpdateable cmp = updateComponents[i];
                if (!cmp.gameObject.active)
                {
                    continue;
                }
#if DEBUG && COMPONENT_PROFILE
                componentProfiler.StartUpdateCall(updateComponents[i] as Component);
#endif
                cmp.Update();

#if DEBUG && COMPONENT_PROFILE
                componentProfiler.EndUpdateCall();
#endif
            }
            ChangeComponentActivity();
            UpdateInvokeCalls();
#if DEBUG
            updateTime.Stop();
            lateUpdateTime.Start();
#endif
            count = lateUpdateComponents.Count;
            for (int i = 0; i < count; i++)
            {
                lateUpdateComponents[i].LateUpdate();
            }
            ChangeComponentActivity();

            CleanUp();
#if DEBUG
            lateUpdateTime.Stop();
            graphics.Start();
#endif
            Camera.DoRender(GraphicsDevice);
#if DEBUG
            graphics.Stop();
            double total = fixedUpdateTime.Elapsed.TotalSeconds + lateUpdateTime.Elapsed.TotalSeconds + updateTime.Elapsed.TotalSeconds + graphics.Elapsed.TotalSeconds + physics.Elapsed.TotalSeconds;
            if (ApplicationSettings.LogActivatedComponents)
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
                Debug.Display("Active comps U | F | L", String.Format("{0} | {1} | {2}", updateComponents.Count, fixedUpdateComponents.Count, lateUpdateComponents.Count));
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

            _loadingProgess = 0;

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

            if (scene != null)
            {
                isUpdateable.AddRange(scene.isUpdateable);
                isFixedUpdateable.AddRange(scene.isFixedUpdateable);
                isLateUpdateable.AddRange(scene.isLateUpdateable);
                hasAwake.AddRange(scene.hasAwake);
                fixReferences.AddRange(scene.fixReferences);
            }

            if (scene == null)
            {
                Debug.Log("Scene is NULL. Completing load!");
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

            //_loadingProgess = 0;

            doGarbageCollectAfterAwake = true;
            //GC.Collect();
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
            AwakeNewComponents(false);
        }

        internal static void AwakeNewComponents(bool onInstantiate)
        {
            int componentCount = newComponents.Count;
            for (int i = 0; i < componentCount; i++)
            {
                Component cmp = newComponents[i];
                if (cmp.gameObject != null)
                {
                    // TODO: Fix this with a content processor!
                    // Purge superfluous Transforms that is created when GameObjects are imported from the scene
                    //if ((cmp is Transform) && (cmp.gameObject.transform != cmp))
                    //{
                    //    cmp.gameObject = null;
                    //    continue;
                    //}
                    objects.Add(cmp.GetInstanceID(), cmp);

                    if (!cmp.isPrefab)
                    {
                        componentsToStart.Add(cmp);
                    }
                    if (!objects.ContainsKey(cmp.gameObject.GetInstanceID()))
                    {
                        objects.Add(cmp.gameObject.GetInstanceID(), cmp.gameObject);
                    }
                    if (!cmp.isPrefab && hasAwake.Contains(cmp.GetType()))
                    {
                        if (onInstantiate)
                        {
                            instantiatedComponentsToAwake.Enqueue(cmp);
                        }
                        else
                        {
                            componentsToAwake.Enqueue(cmp);
                        }
                    }
                }
            }
            newComponents.Clear();

            if (onInstantiate)
            {
                while (instantiatedComponentsToAwake.Count > 0)
                {
                    Component cmp = instantiatedComponentsToAwake.Dequeue();
                    cmp.Awake();
                }
            }
            else
            {
                while (componentsToAwake.Count > 0)
                {
                    Component cmp = componentsToAwake.Dequeue();
                    cmp.Awake();
                }
            }

            // Do a recursive awake to awake components instantiated in the previous awake.
            // In this way we will make sure that everything is instantiated before the first run.
            if (newComponents.Count > 0)
            {
                AwakeNewComponents(onInstantiate);
            }

            if (!onInstantiate && doGarbageCollectAfterAwake)
            {
                GC.Collect();
                doGarbageCollectAfterAwake = false;
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
            //newAssets.Add(asset);

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
            updateComponents.Clear();
            fixedUpdateComponents.Clear();
            lateUpdateComponents.Clear();
            markedForDestruction.Clear();
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

                    if (componentsToStart.Contains(cmp))
                    {
                        componentsToStart.Remove(cmp);
                    }

                    if (cmp is PressPlay.FFWD.Interfaces.IUpdateable)
                    {
                        PressPlay.FFWD.Interfaces.IUpdateable upd = cmp as PressPlay.FFWD.Interfaces.IUpdateable;
                        if (updateComponents.Contains(upd))
                        {
                            updateComponents.Remove(upd);
                        }
                        if (lateUpdateComponents.Contains(upd))
                        {
                            lateUpdateComponents.Remove(upd);
                        }
                    }

                    if (cmp is IFixedUpdateable)
                    {
                        if (fixedUpdateComponents.Contains(cmp as IFixedUpdateable))
                        {
                            fixedUpdateComponents.Remove(cmp as IFixedUpdateable);
                        }
                    }

                    for (int j = invokeCalls.Count - 1; j >= 0; j--)
                    {
                        if (invokeCalls[j].behaviour == cmp)
                        {
                            invokeCalls.RemoveAt(j);
                        }
                    }
	            }
            }
            markedForDestruction.Clear();
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

        public static void AddStaticAsset(string name)
        {
            assetHelper.AddStaticAsset(name);
        }

        public static void Preload<T>(string name)
        {
            assetHelper.Preload<T>(name);
        }

        public static T PreloadInstant<T>(string name)
        {
            return assetHelper.PreloadInstant<T>(name);
        }

        internal static void UpdateGameObjectActive(List<Component> components)
        {
            for (int i = 0; i < components.Count; i++)
            {
                componentsChangingActivity.Add(components[i]);
            }
        }

        private static void ChangeComponentActivity()
        {
            for (int i = 0; i < componentsChangingActivity.Count; i++)
            {
                Component cmp = componentsChangingActivity[i];
                Type tp = cmp.GetType();
                if (cmp.gameObject.active)
                {
                    if (isUpdateable.Contains(tp))
                    {
                        if (!updateComponents.Contains(cmp as PressPlay.FFWD.Interfaces.IUpdateable))
                        {
                            updateComponents.Add(cmp as PressPlay.FFWD.Interfaces.IUpdateable);
#if DEBUG
                            if (ApplicationSettings.LogActivatedComponents)
                            {
                                Debug.Log("Added to update: " + cmp);
                            }
#endif
                        }
                    }
                    if (isLateUpdateable.Contains(tp))
                    {
                        if (!lateUpdateComponents.Contains(cmp as PressPlay.FFWD.Interfaces.IUpdateable))
                        {
                            lateUpdateComponents.Add(cmp as PressPlay.FFWD.Interfaces.IUpdateable);
#if DEBUG
                            if (ApplicationSettings.LogActivatedComponents)
                            {
                                Debug.Log("Added to lateupdate: " + cmp);
                            }
#endif
                        }
                    }
                    if (isFixedUpdateable.Contains(tp))
                    {
                        if (!fixedUpdateComponents.Contains(cmp as PressPlay.FFWD.Interfaces.IFixedUpdateable))
                        {
                            fixedUpdateComponents.Add(cmp as PressPlay.FFWD.Interfaces.IFixedUpdateable);
#if DEBUG
                            if (ApplicationSettings.LogActivatedComponents)
                            {
                                Debug.Log("Added to fixedupdate: " + cmp);
                            }
#endif
                        }
                    }
                    if (cmp is Renderer)
                    {
                        Camera.AddRenderer(cmp as Renderer);
                    }
                }
                else
                {
                    if (isUpdateable.Contains(tp))
                    {
                        if (updateComponents.Contains(cmp as PressPlay.FFWD.Interfaces.IUpdateable))
                        {
                            updateComponents.Remove(cmp as PressPlay.FFWD.Interfaces.IUpdateable);
                        }
                    }
                    if (isLateUpdateable.Contains(tp))
                    {
                        if (lateUpdateComponents.Contains(cmp as PressPlay.FFWD.Interfaces.IUpdateable))
                        {
                            lateUpdateComponents.Remove(cmp as PressPlay.FFWD.Interfaces.IUpdateable);
                        }
                    }
                    if (isFixedUpdateable.Contains(tp))
                    {
                        if (fixedUpdateComponents.Contains(cmp as PressPlay.FFWD.Interfaces.IFixedUpdateable))
                        {
                            fixedUpdateComponents.Remove(cmp as PressPlay.FFWD.Interfaces.IFixedUpdateable);
                        }
                    }
                    if (cmp is Renderer)
                    {
                        Camera.RemoveRenderer(cmp as Renderer);
                    }
                }
            }
            componentsChangingActivity.Clear();
        }

        internal static void AddInvokeCall(MonoBehaviour behaviour, string methodName, float time, float repeatRate)
        {
            invokeCalls.Add(new InvokeCall() { behaviour = behaviour, methodName = methodName, time = time, repeatRate = repeatRate });
        }

        internal static bool IsInvoking(MonoBehaviour behaviour, string methodName)
        {
            for (int i = 0; i < invokeCalls.Count; i++)
            {
                if (invokeCalls[i].behaviour == behaviour && invokeCalls[i].methodName == methodName)
                {
                    return true;
                }
            }
            return false;
        }

        internal static void UpdateInvokeCalls()
        {
            for (int i = invokeCalls.Count - 1; i >= 0; i--)
            {
                InvokeCall call = invokeCalls[i];
                if (call.Update(Time.deltaTime))
                {
                    call.behaviour.SendMessage(invokeCalls[i].methodName, null);
                    invokeCalls.RemoveAt(i);
                }
                else
                {
                    invokeCalls[i] = call;
                }
            }
        }

    }
}
