using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.Components;
using PressPlay.FFWD.Interfaces;

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
        private Stopwatch frameTime = new Stopwatch();
        private Stopwatch scripts = new Stopwatch();
        private Stopwatch physics = new Stopwatch();
        private Stopwatch graphics = new Stopwatch(); 
        public static Stopwatch raycastTimer = new Stopwatch();
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
            if (Application.quitNextUpdate)
            {
                base.Game.Exit();
                return;
            }

            base.Update(gameTime);
            Time.Update((float)gameTime.ElapsedGameTime.TotalSeconds, (float)gameTime.TotalGameTime.TotalSeconds);
            UpdateFPS(gameTime);

            if (!String.IsNullOrEmpty(sceneToLoad))
            {
                CleanUp();
                GC.Collect();
                DoSceneLoad();
            }
            LoadNewAssets();

#if DEBUG
            scripts.Start();
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
                    (activeComponents[i] as IFixedUpdateable).FixedUpdate();
                }
            }
#if DEBUG
            scripts.Stop();
            physics.Start();
#endif
            //Physics.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            Physics.Update(Time.deltaTime);
#if DEBUG
            physics.Stop();
#endif
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Time.Draw();

            frameCounter++;
#if DEBUG
            scripts.Start();
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
                    (activeComponents[i] as PressPlay.FFWD.Interfaces.IUpdateable).Update();
                    lateUpdates.Add((activeComponents[i] as PressPlay.FFWD.Interfaces.IUpdateable));
                }
                if ((activeComponents[i] is MonoBehaviour))
                {
                    (activeComponents[i] as MonoBehaviour).UpdateInvokeCalls();
                }
            }

            for (int i = 0; i < lateUpdates.Count; i++)
            {
                lateUpdates[i].LateUpdate();
            }

            CleanUp();
#if DEBUG
            scripts.Stop();
            graphics.Start();
#endif
            Camera.DoRender(GraphicsDevice);
#if DEBUG
            graphics.Stop();
            double total = scripts.Elapsed.TotalSeconds + graphics.Elapsed.TotalSeconds + physics.Elapsed.TotalSeconds;
            if (ApplicationSettings.ShowDebugLines)
            {
                Camera lineCam = (String.IsNullOrEmpty(ApplicationSettings.DebugLineCamera)) ? Camera.main : Camera.FindByName(ApplicationSettings.DebugLineCamera);
                Debug.DrawLines(GraphicsDevice, lineCam);
                if (lineCam != null)
                {
                    Debug.Display(lineCam.name, lineCam.transform.position);
                }
            }
            if (ApplicationSettings.ShowRaycastTime)
            {
                Debug.Display("Raycasts ms", Application.raycastTimer.ElapsedMilliseconds);
                raycastTimer.Reset();
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
                Debug.Display("% S | P | G", String.Format("{0:P1} | {1:P1} | {2:P1}", scripts.Elapsed.TotalSeconds / total, physics.Elapsed.TotalSeconds / total, graphics.Elapsed.TotalSeconds / total));
                Debug.Display("ms S | P | G", String.Format("{0}ms | {1}ms | {2}ms", scripts.Elapsed.Milliseconds, physics.Elapsed.Milliseconds, graphics.Elapsed.Milliseconds));
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
            scripts.Reset();
            physics.Reset();
            graphics.Reset();
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
            if (!String.IsNullOrEmpty(loadedLevelName))
            {
                assetHelper.Unload(loadedLevelName);
            }

            loadingScene = true;
            loadedLevelName = sceneToLoad.Contains('/') ? sceneToLoad.Substring(sceneToLoad.LastIndexOf('/') + 1) : sceneToLoad;
            Scene scene = assetHelper.Load<Scene>(sceneToLoad);
            sceneToLoad = "";
            loadingScene = false;

            if (scene != null)
            {
                scene.Initialize();
            }
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
                if (obj is Component)
                {
                    GameObject gObj = ((Component)obj).gameObject;

                    if (!dontDestroyOnLoad.Contains(gObj))
                    {
                        markedForDestruction.Add(obj);
                    }
                }

                if (obj is GameObject)
                {
                    GameObject gObj = (GameObject)obj;

                    if (!dontDestroyOnLoad.Contains(gObj))
                    {
                        markedForDestruction.Add(obj);
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
            newComponents.Add(component);
        }

        internal static void AddNewAsset(Asset asset)
        {
            newAssets.Add(asset);
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
    }
}
