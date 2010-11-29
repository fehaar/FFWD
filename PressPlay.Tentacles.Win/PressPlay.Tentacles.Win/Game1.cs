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
using Box2D.XNA;
using PressPlay.Tentacles.Debugging;

namespace PressPlay.Tentacles
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        DebugRenderer debug;

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
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
#else
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
#endif
            Components.Add(new Application(this));
            debug = new DebugRenderer(this);
            Components.Add(debug);

#if DEBUG
            FrameRateCounter counter = new FrameRateCounter(this, Content.RootDirectory + "/TestFont");
            counter.Position = new Vector2(32, 46);
            counter.DrawOrder = 2;
            Components.Add(counter);

            Components.Add(new TouchHandler(this));

#if !WINDOWS
            Accelerometer.Initialize();
            PanCamera cam = new PanCamera(this);
            Components.Add(cam);
#endif
#endif
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Application.LoadScene("Scenes/DesatGreen_intro");

            Camera.main.transform.localPosition = new Vector3(-13, -9, 7);
            Camera.main.transform.localRotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(MathHelper.ToRadians(90)));
            Camera.main.up = Vector3.Backward;
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
            Vector3 dir = Vector3.Zero;
            if (key.IsKeyDown(Keys.RightAlt))            
            {
                // Rotate camera
                if (key.IsKeyDown(Keys.A))
                {
                    dir.X += 1;
                }
                if (key.IsKeyDown(Keys.D))
                {
                    dir.X -= 1;
                }
                if (key.IsKeyDown(Keys.W))
                {
                    dir.Y += 1;
                }
                if (key.IsKeyDown(Keys.X))
                {
                    dir.Y -= 1;
                }
                if (key.IsKeyDown(Keys.E))
                {
                    dir.Z += 1;
                }
                if (key.IsKeyDown(Keys.Z))
                {
                    dir.Z -= 1;
                }
                if (key.IsKeyDown(Keys.RightShift))
                {
                    dir *= 10;
                }
                if (dir.LengthSquared() > 0)
                {
                    Quaternion rotate = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(dir.X), MathHelper.ToRadians(dir.Y), MathHelper.ToRadians(dir.Z));
                    Camera.main.transform.localRotation *= rotate;
                }
            }
            else
            {
                // Translate camera
                if (key.IsKeyDown(Keys.A))
                {
                    dir.X -= 1;
                }
                if (key.IsKeyDown(Keys.D))
                {
                    dir.X += 1;
                }
                if (key.IsKeyDown(Keys.W))
                {
                    dir.Z += 1;
                }
                if (key.IsKeyDown(Keys.X))
                {
                    dir.Z -= 1;
                }
                if (key.IsKeyDown(Keys.E))
                {
                    dir.Y += 1;
                }
                if (key.IsKeyDown(Keys.Z))
                {
                    dir.Y -= 1;
                }
                if (key.IsKeyDown(Keys.RightShift))
                {
                    dir *= 10;
                }
                Camera.main.transform.localPosition += dir;
            }

            if (oldState.IsKeyUp(Keys.M) && key.IsKeyDown(Keys.M))
            {
                debug.NextMode();
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
#if WINDOWS_PHONE
            AccelerometerState state = Accelerometer.GetState();
            Vector2 pos = new Vector2(Camera.main.transform.position.X, Camera.main.transform.position.Z);
#endif
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
