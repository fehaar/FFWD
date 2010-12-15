using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using PressPlay.FFWD.Components;
using PressPlay.FFWD;
using PressPlay.Tentacles.Debugging;
using PressPlay.Tentacles.Scripts;

namespace PressPlay.Tentacles
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        DebugRenderer debug;
        Vector3 camStart;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
#if WINDOWS
            this.IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 30.0);
#else
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 30.0);
#endif
            Components.Add(new Application(this));
            debug = new DebugRenderer(this);
            //debug.Wireframe = debug.PhysicsDebug = true;
            Components.Add(debug);

#if DEBUG
            FrameRateCounter counter = new FrameRateCounter(this, Content.RootDirectory + "/TestFont");
            counter.Position = new Vector2(32, 18);
            counter.DrawOrder = 2;
            Components.Add(counter);

            //PanCamera cam = new PanCamera(this);
            //Components.Add(cam);
#endif
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
#if WINDOWS_PHONE
            // HACK: Force Phone to include Scripts DLL
            SimpleRotate rotate = new SimpleRotate();
#endif
            Application.LoadScene("Scenes/DesatGreen_intro");

            Camera.main.transform.localPosition = camStart = new Vector3(-7, -7, -17);
            Camera.main.transform.localRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(-110));
            Camera.main.up = Vector3.Backward;            
            Camera.main.forward = Vector3.UnitY;
            Camera.main.viewPort = GraphicsDevice.Viewport;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        KeyboardState oldState;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState key = Keyboard.GetState();
            if (oldState.IsKeyUp(Keys.M) && key.IsKeyDown(Keys.M))
            {
                debug.NextMode();
            }
            if (oldState.IsKeyUp(Keys.S) && key.IsKeyDown(Keys.S))
            {
                Camera.main.transform.localPosition = camStart;
            }
            oldState = key;

#if WINDOWS
            foreach (String asset in ContentHelper.MissingAssets)
            {
                Debug.Log("Missing " + asset);
            }
            ContentHelper.MissingAssets.Clear();
#endif
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Debug.Display("Cam", Camera.main.transform.position);
            //Debug.Display("Mouse screen", InputHandler.Instance.GetInputScreenPosition());
            //Ray ray = Camera.main.ScreenPointToRay(InputHandler.Instance.GetInputScreenPosition());
            //float? dist = ray.Intersects(new Plane() { Normal = Vector3.Up });
            //if (dist.HasValue)
            //{
            //    Debug.Display("Mouse", ray.Position + ray.Direction * dist);
            //}

            if (debug.Wireframe)
            {
                GraphicsDevice.RasterizerState = new RasterizerState() { FillMode = FillMode.WireFrame };
            }
            base.Draw(gameTime);
        }

        /// <summary>
        /// Gets spaceship view matrix
        /// </summary>
        private Matrix GetViewMatrix()
        {
            return Matrix.CreateLookAt(
                Camera.main.transform.localPosition,
                Camera.main.transform.localPosition + new Vector3(0, 0, -1000),
                Vector3.Up);
        }
    }
}
