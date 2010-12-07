using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Graphics;

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

        private static Dictionary<int, UnityObject> objects = new Dictionary<int, UnityObject>();
        private static Dictionary<int, UnityObject> prefabObjects = new Dictionary<int, UnityObject>();

        public override void Initialize()
        {
            base.Initialize();
            ContentHelper.Services = Game.Services;
            ContentHelper.StaticContent = new ContentManager(Game.Services, Game.Content.RootDirectory);
            ContentHelper.Content = new ContentManager(Game.Services, Game.Content.RootDirectory);
            ContentHelper.IgnoreMissingAssets = true;
            Camera.main.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), Game.GraphicsDevice.Viewport.AspectRatio, 0.3f, 1000);
            Physics.Initialize();
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            AwakeNewComponents();
            // TODO: Drop Update on GOs
            foreach (UnityObject obj in objects.Values)
            {
                if (obj is GameObject)
                {
                    (obj as GameObject).FixedUpdate();
                }
            }
            Physics.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Color bg = new Color(78, 115, 74);
            // TODO: Drop Update on GOs
            foreach (UnityObject obj in objects.Values)
            {
                if (obj is GameObject)
                {
                    (obj as GameObject).Update();
                }
            }
            // TODO: This is not very cool. Needed to avoid test failures... But cameras should handle this
            if (gameTime.ElapsedGameTime.TotalMilliseconds > 0)
            {
                GraphicsDevice.Clear(bg);
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            }
            // TODO: Drop Draw on GOs
            foreach (UnityObject obj in objects.Values)
            {
                if (obj is GameObject)
                {
                    (obj as GameObject).Draw(spriteBatch);
                }
            }
        }

        public static void LoadScene(string name)
        {
            Scene scene = ContentHelper.Content.Load<Scene>(name);
            LoadLevel(scene);
        }

        public static void LoadLevel(Scene scene)
        {
            objects.Clear();
            prefabObjects.Clear();

            scene.AfterLoad();
            for (int i = 0; i < scene.gameObjects.Count; i++)
            {
                objects.Add(scene.gameObjects[i].GetInstanceID(), scene.gameObjects[i]);
            }
            for (int i = 0; i < scene.prefabs.Count; i++)
            {
                prefabObjects.Add(scene.prefabs[i].GetInstanceID(), scene.prefabs[i]);
            }
            AwakeNewComponents();
        }

        public static UnityObject Find(int id)
        {
            if (objects.ContainsKey(id))
            {
                return objects[id];
            }
            if (prefabObjects.ContainsKey(id))
            {
                return prefabObjects[id];
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
                objects.Add(NewComponents[i].GetInstanceID(), NewComponents[i]);
                NewComponents[i].Awake();
            }
            NewComponents.Clear();
        }

        internal static bool IsAwake(Component component)
        {
            return !NewComponents.Contains(component);
        }

        internal static void AddNewComponent(Component component)
        {
            NewComponents.Add(component);
        }
    }
}
