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

       

#if DEBUG
        private Stopwatch scripts = new Stopwatch();
        private Stopwatch physics = new Stopwatch();
        private Stopwatch graphics = new Stopwatch();
#endif

        private static Dictionary<int, UnityObject> objects = new Dictionary<int, UnityObject>();
        private static List<Component> activeComponents = new List<Component>();
        internal static List<UnityObject> markedForDestruction = new List<UnityObject>();
        internal static List<GameObject> dontDestroyOnLoad = new List<GameObject>();
        private static List<Interfaces.IUpdateable> lateUpdates = new List<Interfaces.IUpdateable>();
        internal static bool loadingScene = false;

        public override void Initialize()
        {
            base.Initialize();
            ContentHelper.Services = Game.Services;
            ContentHelper.StaticContent = new ContentManager(Game.Services, Game.Content.RootDirectory);
            ContentHelper.Content = new ContentManager(Game.Services, Game.Content.RootDirectory);
            ContentHelper.IgnoreMissingAssets = true;
            Camera.FullScreen = Game.GraphicsDevice.Viewport;
            Physics.Initialize();
            Time.Reset();
            Input.Initialize();
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Time.Update((float)gameTime.ElapsedGameTime.TotalSeconds, (float)gameTime.TotalGameTime.TotalSeconds);
            UpdateFPS(gameTime);

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
            Physics.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
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
                if ((activeComponents[i] is Renderer))
                {
                    Camera.AddRenderer(activeComponents[i] as Renderer);
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
            if (ApplicationSettings.ShowFPSCounter)
            {
                Debug.Display("FPS", frameRate);
            }
            if (ApplicationSettings.ShowPerformanceBreakdown)
            {
                Debug.Display("S | P | G", String.Format("{0:P1} | {1:P1} | {2:P1}", scripts.Elapsed.TotalSeconds / total, physics.Elapsed.TotalSeconds / total, graphics.Elapsed.TotalSeconds / total));
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

        public static void LoadLevel(string name)
        {
            loadingScene = true;
            Scene scene = ContentHelper.Content.Load<Scene>(name);
            loadingScene = false;
            LoadLevel(scene);
            loadedLevelName = name.Contains('/') ? name.Substring(name.LastIndexOf('/') + 1) : name;
        }

        public static void LoadLevel(Scene scene)
        {
            foreach (UnityObject obj in objects.Values)
            {
                if (obj is GameObject)
                {
                    GameObject gObj = (GameObject)obj;

                    if (!dontDestroyOnLoad.Contains(gObj))
                    {
                        markedForDestruction.Add(obj);
                    }
                }
            }
            CleanUp();

            scene.AfterLoad();
            AwakeNewComponents();
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
                if (obj.GetType() == type)
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
                if (obj.GetType() == type)
                {
                    return obj;
                }
            }
            return null;
        }

        internal static List<Component> NewComponents = new List<Component>();

        internal static void AwakeNewComponents()
        {
            for (int i = 0; i < NewComponents.Count; i++)
            {
                Component cmp = NewComponents[i];
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
            // Remove placeholder references and replace them with live ones
            for (int i = 0; i < NewComponents.Count; i++)
            {
                NewComponents[i].FixReferences(objects);
            }
            // All components will exist to be found before awaking - otherwise we can get issues with instantiating on awake.
            Component[] componentsToAwake = NewComponents.ToArray();
            NewComponents.Clear();
            for (int i = 0; i < componentsToAwake.Length; i++)
            {
                if (!componentsToAwake[i].isPrefab)
                {
                    componentsToAwake[i].Awake();
                }
            }
        }

        internal static void AddNewComponent(Component component)
        {
            NewComponents.Add(component);
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
    }
}
