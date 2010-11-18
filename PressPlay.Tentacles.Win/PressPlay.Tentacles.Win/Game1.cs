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
using PressPlay.U2X.Xna.Components;
using PressPlay.U2X.Xna;
using PressPlay.U2X;
using Box2D.XNA;

namespace PressPlay.Tentacles.Win
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;

        Box2DDebugDraw physicsDebugDraw;
        Body ball;

        bool renderWireframe = true;

        Scene scene = null; 

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
            ContentHelper.Services = Services;
            ContentHelper.StaticContent = new ContentManager(Services, Content.RootDirectory);
            ContentHelper.Content = new ContentManager(Services, Content.RootDirectory);
            ContentHelper.IgnoreMissingAssets = true;

            Camera.main.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), graphics.GraphicsDevice.Viewport.AspectRatio, 0.3f, 1000);

            FrameRateCounter counter = new FrameRateCounter(this, Content.RootDirectory + "/TestFont");
            counter.Position = new Vector2(10, 46);
            Components.Add(counter);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            scene = Content.Load<Scene>("Scenes/Level1");
            scene.AfterLoad();

            Physics.Initialize();

            font = Content.Load<SpriteFont>("TestFont");

            Camera.main.transform.localPosition = new Vector3(-50, -100, 0);
            Camera.main.transform.localRotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(MathHelper.ToRadians(90)));
            Camera.main.up = Vector3.Backward; // Vector3.Left;

            physicsDebugDraw = new Box2DDebugDraw() { Flags = DebugDrawFlags.Shape, worldView = Matrix.CreateRotationX(MathHelper.PiOver2) };
            Physics.AddDebugDraw(physicsDebugDraw);

            ball = Physics.AddCircle(0.5f, new Vector2(-10, -15.2f), 0, 1);
            ball.SetType(BodyType.Dynamic);
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


            if (oldState.IsKeyUp(Keys.R) && key.IsKeyDown(Keys.R))
            {
                Camera.main.transform.localPosition = new Vector3(-50, 100, 0);
                Camera.main.transform.localRotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(MathHelper.ToRadians(-90)));
            }
            if (oldState.IsKeyUp(Keys.M) && key.IsKeyDown(Keys.M))
            {
                renderWireframe = !renderWireframe;
            }

            if (oldState.IsKeyUp(Keys.Up) && key.IsKeyDown(Keys.Up))
            {
                Vector2 vel = ball.GetLinearVelocity();
                if (vel == Vector2.Zero)
                {
                    vel = new Vector2(0, 500);
                }
                else
                {
                    vel.Normalize();
                    vel = vel * 500;
                }
                ball.ApplyForce(vel, Vector2.Zero);
            }
            oldState = key;

            Component.AwakeNewComponents();

#if WINDOWS
            foreach (String asset in ContentHelper.MissingAssets)
            {
                Debug.Log("Missing " + asset);
            }
            ContentHelper.MissingAssets.Clear();
#endif

            scene.FixedUpdate();
            Physics.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Color bg = Color.Black;
            //Color bg = new Color(78, 115, 74);
            GraphicsDevice.Clear(bg);
            if (renderWireframe)
            {
                GraphicsDevice.RasterizerState = new RasterizerState() { FillMode = FillMode.WireFrame };
            }

            scene.Update();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            scene.Draw(spriteBatch);

            physicsDebugDraw.FinishDrawShapes(GraphicsDevice);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Camera pos: " + Camera.main.transform.localPosition, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(font, "Camera rot: " + Camera.main.transform.localRotation, new Vector2(10, 22), Color.White);
            spriteBatch.DrawString(font, "Camera fwd: " + Camera.main.Forward, new Vector2(10, 34), Color.White);
            physicsDebugDraw.FinishDrawString(spriteBatch, font);
            spriteBatch.End();

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
