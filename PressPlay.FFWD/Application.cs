#define COMPONENT_PROFILE


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

            Instance = this;

            Screen.height = game.GraphicsDevice.Viewport.Height;
            Screen.width = game.GraphicsDevice.Viewport.Width;
        }

#if DEBUG
        private SpriteBatch spriteBatch;
#endif
        internal static Application Instance { get; private set; }

        public static bool isDeactivated = false;
        int frameRate = 0;
        int frameCounter = 0;
        int updateRate = 0;
        int updateCounter = 0;
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
        public static Stopwatch raycastTimer = new Stopwatch();
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
        internal static readonly List<PressPlay.FFWD.Components.MonoBehaviour> guiComponents = new List<PressPlay.FFWD.Components.MonoBehaviour>(50);
        private static readonly List<Component> componentsChangingActivity = new List<Component>(50);

        internal static readonly TypeSet typeCaps = new TypeSet(100);
        private static readonly TypeSet isUpdateable = new TypeSet(100);
        private static readonly TypeSet isFixedUpdateable = new TypeSet(25);
        private static readonly TypeSet isLateUpdateable = new TypeSet(25);
        private static readonly TypeSet hasGUI = new TypeSet(25);
        internal static readonly TypeSet hasAwake = new TypeSet(50);
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

        public static string dataPath
        {
            get
            {
                return "";
            }
        }

        public static bool isLoadingLevel
        {
            get
            {
                return !String.IsNullOrEmpty(sceneToLoad);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            //ContentHelper.Services = Game.Services;
            //ContentHelper.StaticContent = new ContentManager(Game.Services, Game.Content.RootDirectory);
            //ContentHelper.Content = new ContentManager(Game.Services, Game.Content.RootDirectory);
            //ContentHelper.IgnoreMissingAssets = true;
            Camera.FullScreen = Game.GraphicsDevice.Viewport;
            Resources.AssetHelper = assetHelper;
            Physics.Initialize();
            Time.Reset();
            Input.Initialize();
            assetHelper.CreateContentManager = CreateContentManager;
            GUI.spriteBatch = new SpriteBatch(Game.GraphicsDevice);
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
            LayerMask.LoadLayerNames(assetHelper);

#if DEBUG
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
#endif
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GUI.spriteFont = Game.Content.Load<SpriteFont>("GUIFont");
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

            Input.Update();

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
                DoSceneLoad();
            }
            LoadNewAssets(false);

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
                    if (cmp.gameObject == null || !cmp.gameObject.active)
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
            Input.BeginFixedUpdate();

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
                if (cmp.gameObject == null || !cmp.gameObject.active)
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
            Animation.SampleAnimations();
#if DEBUG
            updateTime.Stop();
            lateUpdateTime.Start();
#endif
            StartComponents();
            ChangeComponentActivity();
            count = lateUpdateComponents.Count;
            for (int i = 0; i < count; i++)
            {
                Component c = lateUpdateComponents[i] as Component;
                if (c.gameObject != null && c.gameObject.active)
                {
                    lateUpdateComponents[i].LateUpdate();
                }
            }
            ChangeComponentActivity();

            CleanUp();
#if DEBUG
            lateUpdateTime.Stop();
            graphics.Start();
#endif
            Camera.DoRender(GraphicsDevice);
            ChangeComponentActivity();
            CleanUp();
#if DEBUG
            graphics.Stop();
            double total = fixedUpdateTime.Elapsed.TotalSeconds + lateUpdateTime.Elapsed.TotalSeconds + updateTime.Elapsed.TotalSeconds + graphics.Elapsed.TotalSeconds + physics.Elapsed.TotalSeconds;

#if COMPONENT_PROFILE
            componentProfiler.Sort();
            Debug.Display("GetWorst()", componentProfiler.GetWorst());
            componentProfiler.FlushData();
#endif
            if (ApplicationSettings.ShowRaycastTime)
            {
                Debug.Display("Raycasts ms", Application.raycastTimer.ElapsedMilliseconds);
                raycastTimer.Reset();
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
                Debug.Display("FPS", String.Format("{0}, ms {1}, UR {2}", frameRate, frameTime.ElapsedMilliseconds, updateRate));
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
                    spriteBatch.DrawString(GUI.spriteFont, text, Position + Microsoft.Xna.Framework.Vector2.One + offset, Microsoft.Xna.Framework.Color.Black);
                    spriteBatch.DrawString(GUI.spriteFont, text, Position + offset, Microsoft.Xna.Framework.Color.White);
                    offset.Y += GUI.spriteFont.MeasureString(text).Y * 0.75f;
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
            //Debug.ClearLines();
            Input.ClearStates();
        }

        private void UpdateFPS(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;
            updateCounter++;
            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                updateRate = updateCounter;
                frameCounter = 0;
                updateCounter = 0;
            }
        }

        private void DoSceneLoad()
        {
            //Debug.Log("DoSceneLoad: " + sceneToLoad);

            _loadingProgess = 0;

            if (!String.IsNullOrEmpty(loadedLevelName))
            {
                CleanUp();
                assetHelper.Unload(loadedLevelName);
                Physics.Reset();
            }

            loadingScene = true;
            isLoadingAssetBeforeSceneInitialize = true;
            loadIsComplete = false;

            loadedLevelName = sceneToLoad;
            scene = assetHelper.Load<Scene>("Scenes/" + sceneToLoad);
            sceneToLoad = "";

            if (scene != null)
            {
                typeCaps.Add(scene.typeCaps);
                tempAssets.AddRange(scene.assets);
            }
            totalNumberOfAssetsToLoad = tempAssets.Count;
            numberOfAssetsLoaded = 0;

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
                if (ApplicationSettings.AssetLoadInterval > 0 && stopWatch.ElapsedMilliseconds > ApplicationSettings.AssetLoadInterval)
                {
                    stopWatch.Stop();
                    stopWatch.Reset();
                    return;
                }

                tempAssets[i].LoadAsset(assetHelper);
                tempAssets.RemoveAt(i);
                numberOfAssetsLoaded++;
                count++;
            }
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
        }

        private void OnSceneLoadComplete()
        {
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
            doGarbageCollectAfterAwake = true;
        }

        internal static void LoadNewAssets(bool loadResources)
        {
            assetHelper.LoadingResources = loadResources;
            for (int i = newAssets.Count - 1; i >= 0; i--)
            {
                newAssets[i].LoadAsset(assetHelper);
                newAssets.RemoveAt(i);
            }
        }

        public static void LoadLevelAdditive(string m_strLevelBase)
        {
            throw new NotImplementedException();
        }

        public static void LoadLevel(string name)
        {
#if DEBUG
            Debug.Log("******************************** Loading Level " + name + " ***********************************");
#endif
            sceneToLoad = name;
            UnloadCurrentLevel();
        }

        internal static void UnloadCurrentLevel()
        {
            foreach (UnityObject obj in objects.Values)
            {
                if (obj is GameObject)
                {
                    GameObject gObj = (GameObject)obj;
                    if (!dontDestroyOnLoad.Contains(gObj))
                    {
                        UnityObject.Destroy(gObj);
                        gObj.active = false;
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

        internal static GameObject FindByName(string name)
        {
            foreach (UnityObject obj in objects.Values)
            {
                GameObject go = obj as GameObject;
                if (go != null)
                {
                    if (go.isPrefab)
                    {
                        continue;
                    }
                    if (name.Contains('/'))
                    {
                        if (go.FullName() != name)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (go.name != name)
                        {
                            continue;
                        }
                    }
                    if (go.active)
                    {
                        return go;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        internal static IEnumerable<GameObject> FindByTag(string tag)
        {
            return objects.Values.Where(o => o is GameObject && !(o as GameObject).isPrefab && (o as GameObject).tag == tag).Cast<GameObject>();
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
                    objects.Add(cmp.GetInstanceID(), cmp);

                    if (!cmp.isPrefab)
                    {
                        componentsToStart.Add(cmp);
                    }
                    if (!objects.ContainsKey(cmp.gameObject.GetInstanceID()))
                    {
                        objects.Add(cmp.gameObject.GetInstanceID(), cmp.gameObject);
                    }
                    if (!cmp.isPrefab && typeCaps.HasCaps(cmp.GetType(), TypeSet.TypeCapabilities.Awake))
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
                    if (cmp is IInitializable)
                    {
                        (cmp as IInitializable).Initialize(assetHelper);
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
            componentsToAwake.Clear();
            instantiatedComponentsToAwake.Clear();
            componentsToStart.Clear();
            newComponents.Clear();
            newAssets.Clear();
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

                    if (cmp is MonoBehaviour)
	                {
                        if (guiComponents.Contains(cmp as MonoBehaviour))
                        {
                            guiComponents.Remove(cmp as MonoBehaviour);
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
                DontDestroyOnLoad((target as Component).gameObject);
            }

            if (target is GameObject)
            {
                GameObject go = target as GameObject;
                if (!dontDestroyOnLoad.Contains(go))
                {
                    dontDestroyOnLoad.Add(go);
                    go.transform.DontDestroyOnLoadOnChildren();
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
                if (cmp.gameObject != null && cmp.gameObject.active)
                {
                    if (typeCaps.HasCaps(tp, TypeSet.TypeCapabilities.Update))
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
                    if (typeCaps.HasCaps(tp, TypeSet.TypeCapabilities.LateUpdate))
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
                    if (typeCaps.HasCaps(tp, TypeSet.TypeCapabilities.FixedUpdate))
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
                    if (typeCaps.HasCaps(tp, TypeSet.TypeCapabilities.GUI))
                    {
                        if (!guiComponents.Contains(cmp as MonoBehaviour))
                        {
                            guiComponents.Add(cmp as MonoBehaviour);
#if DEBUG
                            if (ApplicationSettings.LogActivatedComponents)
                            {
                                Debug.Log("Added to GUI: " + cmp);
                            }
#endif
                        }
                    }
                    if (cmp is Renderer)
                    {
                        Camera.AddRenderer(cmp as Renderer);
                    }
                    if (cmp is MonoBehaviour)
                    {
                        (cmp as MonoBehaviour).OnEnable();
                    }
                }
                else
                {
                    if (cmp is MonoBehaviour && cmp.gameObject != null)
                    {
                        (cmp as MonoBehaviour).OnDisable();
                    }
                    if (typeCaps.HasCaps(tp, TypeSet.TypeCapabilities.Update))
                    {
                        if (updateComponents.Contains(cmp as PressPlay.FFWD.Interfaces.IUpdateable))
                        {
                            updateComponents.Remove(cmp as PressPlay.FFWD.Interfaces.IUpdateable);
                        }
                    }
                    if (typeCaps.HasCaps(tp, TypeSet.TypeCapabilities.LateUpdate))
                    {
                        if (lateUpdateComponents.Contains(cmp as PressPlay.FFWD.Interfaces.IUpdateable))
                        {
                            lateUpdateComponents.Remove(cmp as PressPlay.FFWD.Interfaces.IUpdateable);
                        }
                    }
                    if (typeCaps.HasCaps(tp, TypeSet.TypeCapabilities.FixedUpdate))
                    {
                        if (fixedUpdateComponents.Contains(cmp as PressPlay.FFWD.Interfaces.IFixedUpdateable))
                        {
                            fixedUpdateComponents.Remove(cmp as PressPlay.FFWD.Interfaces.IFixedUpdateable);
                        }
                    }
                    if (typeCaps.HasCaps(tp, TypeSet.TypeCapabilities.GUI))
                    {
                        if (guiComponents.Contains(cmp as MonoBehaviour))
                        {
                            guiComponents.Remove(cmp as MonoBehaviour);
                        }
                    }
                    if (cmp is Renderer)
                    {
                        Camera.RemoveRenderer(cmp as Renderer);
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
