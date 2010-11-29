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
        }
    }
}
