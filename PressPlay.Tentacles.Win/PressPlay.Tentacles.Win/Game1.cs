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
using SkinnedModel;

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
        bool rotate = false;
        AnimationPlayer animationPlayer;

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
            //model = Content.Load<Model>("Levels/Models/levelEntrance_edited_noMaterial");
            //model = Content.Load<Model>("xna_hierarchy_test_02");
            model = Content.Load<Model>("super_lemmy_parts_anim_cooked");
            boneTransforms = new Matrix[model.Bones.Count];

            font = Content.Load<SpriteFont>("TestFont");

            tex = Content.Load<Texture2D>("Levels/Maps/block_brown_desat");
            camPos = new Vector3(0, 0, 50);

            // Look up our custom skinning information.
            SkinningData skinningData = model.Tag as SkinningData;
            if (skinningData != null)
            {
                // Create an animation player, and start decoding an animation clip.
                animationPlayer = new AnimationPlayer(skinningData);
                AnimationClip clip = skinningData.AnimationClips["Take 001"];
                animationPlayer.StartClip(clip);
            }
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
            camPos += dir;

            if (oldState.IsKeyUp(Keys.R) && key.IsKeyDown(Keys.R))
            {
                rotate = !rotate;
            }
            oldState = key;

            animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //// TODO: Add your drawing code here
            // Set the world matrix as the root transform of the model.
            //model.Root.Transform = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

            // Look up combined bone matrices for the entire model.
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            Matrix world;
            if (rotate)
            {
                float time = (float)gameTime.TotalGameTime.TotalSeconds;
                float yaw = time * 0.4f;
                float pitch = time * 0.7f;
                float roll = time * 1.1f;
                world = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            }
            else
            {
                world = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
            }

            float aspect = GraphicsDevice.Viewport.AspectRatio;
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), aspect, 1, 20000);

            Matrix[] bones = new Matrix[0];
            if (animationPlayer != null)
            {
                bones = animationPlayer.GetSkinTransforms();
            }

            //GraphicsDevice.RasterizerState = new RasterizerState() { FillMode = FillMode.WireFrame };

            // Draw the model.
            for (int i = 0; i < model.Meshes.Count; i++)
            {
                ModelMesh mesh = model.Meshes[i];
                for (int e = 0; e < mesh.Effects.Count; e++)
                {
                    if (mesh.Effects[e] is BasicEffect)
                    {
                        BasicEffect effect = mesh.Effects[e] as BasicEffect;
                        effect.World = world;
                        effect.View = GetViewMatrix();
                        effect.Projection = projection;
                        effect.LightingEnabled = false;
                        effect.Texture = tex;
                        effect.TextureEnabled = true;
                    }
                    if (mesh.Effects[e] is SkinnedEffect)
                    {
                        SkinnedEffect sEffect = mesh.Effects[e] as SkinnedEffect;
                        sEffect.SetBoneTransforms(bones);
                        sEffect.World = world;
                        sEffect.View = GetViewMatrix();
                        sEffect.Projection = projection;
                        sEffect.EnableDefaultLighting();
                        sEffect.SpecularColor = new Vector3(0.25f);
                        sEffect.SpecularPower = 16;
                        sEffect.Texture = tex;
                        //sEffect.DiffuseColor = new Vector3(255, 0, 0);
                    }
                    mesh.Draw();
                }
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Camera: " + camPos, new Vector2(10, 10), Color.White);
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
