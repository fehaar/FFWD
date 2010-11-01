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

namespace PressPlay.Tentacles.Win
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model model;
        Matrix[] boneTransforms;
        Texture2D tex;
        Vector3 camPos;
        SpriteFont font;

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
            // TODO: Add your initialization logic here

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

            // TODO: use this.Content to load your game content here
            model = Content.Load<Model>("Levels/Models/levelEntrance_edited_noMaterial");
            boneTransforms = new Matrix[model.Bones.Count];

            font = Content.Load<SpriteFont>("TestFont");

            tex = Content.Load<Texture2D>("Levels/Maps/block_brown_desat");
            camPos = new Vector3(0, 0, 2000);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

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
            camPos += dir * 10;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //// TODO: Add your drawing code here
            // Set the world matrix as the root transform of the model.
            //model.Root.Transform = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

            // Look up combined bone matrices for the entire model.
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            float yaw = time * 0.4f;
            float pitch = time * 0.7f;
            float roll = time * 1.1f;

            float aspect = GraphicsDevice.Viewport.AspectRatio;

            Matrix world = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);

            // Draw the model.
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up) * Matrix.CreateRotationZ(yaw);
                    //effect.World = world;
                    effect.View = GetViewMatrix();
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), GraphicsDevice.Viewport.AspectRatio, 10, 20000);
                    effect.EnableDefaultLighting();
                    effect.Texture = tex;
                    effect.TextureEnabled = true;
                }
                mesh.Draw();
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, camPos.ToString(), new Vector2(10, 10), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Gets spaceship view matrix
        /// </summary>
        private Matrix GetViewMatrix()
        {
            return Matrix.CreateLookAt(
                camPos,
                camPos + new Vector3(0, 0, -1000),
                Vector3.Up);
        }
    }
}
