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
            if (Instance != null)
            {
                throw new InvalidOperationException("You cannot have two FFWD applications running at a time");
            }
            Instance = this;
            Game.Components.Add(new Time(game));
        }

        public static Application Instance { get; private set; }
        private SpriteBatch spriteBatch;

        private Dictionary<int, GameObject> gameObjects = new Dictionary<int, GameObject>();
        private Dictionary<int, GameObject> prefabs = new Dictionary<int, GameObject>();

        public Scene currentScene
        {
            get;
            private set;
        }

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
            Component.AwakeNewComponents();
            if (currentScene != null)
            {
                currentScene.FixedUpdate();
            }
            Physics.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (currentScene != null)
            {
                Color bg = new Color(78, 115, 74);
                GraphicsDevice.Clear(bg);
                currentScene.Update();
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
                currentScene.Draw(spriteBatch);
            }
        }

        public static void LoadScene(string name)
        {
            Instance.currentScene = ContentHelper.Content.Load<Scene>(name);
            Instance.currentScene.AfterLoad();
            for (int i = 0; i < Instance.currentScene.gameObjects.Count; i++)
            {
                Instance.gameObjects.Add(Instance.currentScene.gameObjects[i].id, Instance.currentScene.gameObjects[i]);
            }
            for (int i = 0; i < Instance.currentScene.prefabs.Count; i++)
            {
                Instance.prefabs.Add(Instance.currentScene.prefabs[i].id, Instance.currentScene.prefabs[i]);
            }
        }

        public GameObject Find(int id)
        {
            if (gameObjects.ContainsKey(id))
            {
                return gameObjects[id];
            }
            if (prefabs.ContainsKey(id))
            {
                return prefabs[id];
            }
            return null;
        }
    }
}
