using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD;
using System.Diagnostics;

namespace PressPlay.FFWD
{
    public class Application : DrawableGameComponent
    {
        public Application(Game game)
            : base(game)
        {
            Game.Components.Add(new Time(game));
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

        public override void Initialize()
        {
            base.Initialize();
            ContentHelper.Services = Game.Services;
            ContentHelper.StaticContent = new ContentManager(Game.Services, Game.Content.RootDirectory);
            ContentHelper.Content = new ContentManager(Game.Services, Game.Content.RootDirectory);
            ContentHelper.IgnoreMissingAssets = true;
            Camera.main.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), Game.GraphicsDevice.Viewport.AspectRatio, 0.3f, 1000);
            Physics.Initialize();
            Input.Initialize();
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

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

            frameCounter++;
#if DEBUG
            scripts.Start();
#endif
            Color bg = new Color(78, 115, 74);

            for (int i = 0; i < activeComponents.Count; i++)
            {
                if (!activeComponents[i].isStarted)
                {
                    activeComponents[i].Start();
                    activeComponents[i].isStarted = true;
                }
                if (activeComponents[i] is PressPlay.FFWD.Interfaces.IUpdateable)
                {
                    (activeComponents[i] as PressPlay.FFWD.Interfaces.IUpdateable).Update();
                }
            }
#if DEBUG
            scripts.Stop();
            graphics.Start();
#endif
            // TODO: This is not very cool. Needed to avoid test failures... But cameras should handle this
            if (gameTime.ElapsedGameTime.TotalMilliseconds > 0)
            {
                GraphicsDevice.Clear(bg);
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            }
            List<IRenderable> deferred = new List<IRenderable>();
            List<IRenderable> arms = new List<IRenderable>();
            for (int i = 0; i < activeComponents.Count; i++)
            {
                if (activeComponents[i] is IRenderable)
                {
                    if (activeComponents[i] is MeshRenderer)
                    {
                        MeshRenderer r = (activeComponents[i] as MeshRenderer);
                        if (r.shader == "iPhone/Particles/Additive Culled")
                        {
                            deferred.Add(activeComponents[i] as IRenderable);
                            continue;
                        }
                        if (r.sharedMaterial != null)
                        {
                            arms.Add(activeComponents[i] as IRenderable);
                            continue;
                        }
                    }
                    (activeComponents[i] as IRenderable).Draw(spriteBatch);
                }
            }
            for (int i = 0; i < deferred.Count; i++)
            {
                (deferred[i] as IRenderable).Draw(spriteBatch);
            }
            for (int i = 0; i < arms.Count; i++)
            {
                (arms[i] as IRenderable).Draw(spriteBatch);
            }
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

        public static void LoadScene(string name)
        {
            Scene scene = ContentHelper.Content.Load<Scene>(name);
            LoadLevel(scene);
        }

        public static void LoadLevel(Scene scene)
        {
            Reset();
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

        internal static UnityObject[] FindObjectsOfType(Type type)
        {
            List<UnityObject> list = new List<UnityObject>();
            foreach (UnityObject obj in objects.Values)
            {
                if (obj.GetType().IsAssignableFrom(type))
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
                if (obj.GetType().IsAssignableFrom(type))
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

                objects.Add(cmp.GetInstanceID(), cmp);
                if (cmp.gameObject != null)
                {
                    if (!cmp.gameObject.isPrefab)
                    {
                        activeComponents.Add(cmp);
                    }
                    if (!objects.ContainsKey(cmp.gameObject.GetInstanceID()))
                    {
                        objects.Add(cmp.gameObject.GetInstanceID(), cmp.gameObject);
                    }
                }
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

        internal static bool IsAwake(Component component)
        {
            return !NewComponents.Contains(component);
        }

        internal static void AddNewComponent(Component component)
        {
            NewComponents.Add(component);
        }

        internal static void Reset()
        {
            objects.Clear();
            activeComponents.Clear();
        }
    }
}
