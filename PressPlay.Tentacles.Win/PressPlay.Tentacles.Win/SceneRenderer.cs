using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PressPlay.U2X.Xna;
#if !WINDOWS
using Microsoft.Xna.Framework.Input.Touch;
#endif
using Box2D.XNA;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.U2X.Xna.Components;
using PressPlay.Tentacles.Debugging;

namespace PressPlay.Tentacles
{
    public class SceneRenderer : DrawableGameComponent
    {
        public SceneRenderer(Game game) : base(game)
        {
        }

        public bool Wireframe = false;
        public bool PhysicsDebug = false;
        private Box2DDebugDraw physicsDebugDraw;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        public Vector2 Position = new Vector2(32, 32);

        public Scene currentScene
        {
            get;
            set;
        }

        public override void Initialize()
        {
            base.Initialize();
#if !WINDOWS            
            TouchPanel.EnabledGestures = TouchPanel.EnabledGestures | 
                                         GestureType.Tap |
                                         GestureType.DoubleTap |
                                         GestureType.Hold;
#endif
            physicsDebugDraw = new Box2DDebugDraw() { Flags = DebugDrawFlags.Shape, worldView = Matrix.CreateRotationX(MathHelper.PiOver2) };
            Physics.DebugDraw = physicsDebugDraw;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            font = Game.Content.Load<SpriteFont>("TestFont");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
#if !WINDOWS
            foreach (GestureSample gesture in TouchHandler.GetSample(GestureType.Tap |
                                         GestureType.DoubleTap |
                                         GestureType.Hold))
            {
                switch (gesture.GestureType)
                {
                    case GestureType.Tap:
                        break;
                    case GestureType.DoubleTap:
                        NextMode();
                        break;
                    case GestureType.Hold:
                        Reset();
                        break;
                }
            }
#endif
            currentScene.FixedUpdate();
            Physics.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public override void Draw(GameTime gameTime)
        {
            Color bg = new Color(78, 115, 74);
            GraphicsDevice.Clear(bg);
            if (Wireframe)
            {
                GraphicsDevice.RasterizerState = new RasterizerState() { FillMode = FillMode.WireFrame };
            }

            currentScene.Update();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            currentScene.Draw(spriteBatch);

            Physics.DoDebugDraw();
            physicsDebugDraw.FinishDrawShapes((PhysicsDebug) ? GraphicsDevice : null);

            spriteBatch.Begin();

            spriteBatch.DrawString(font, ToString(), Position + Vector2.One, Color.Black);
            spriteBatch.DrawString(font, ToString(), Position, Color.White);

            physicsDebugDraw.FinishDrawString(spriteBatch, font);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void Reset()
        {
            Wireframe = false;
            PhysicsDebug = false;
        }

        public void NextMode()
        {
            if (Wireframe)
            {
                if (PhysicsDebug)
                {
                    Wireframe = false;
                }
                else
                {
                    PhysicsDebug = true;
                }
            }
            else
            {
                if (PhysicsDebug)
                {
                    PhysicsDebug = false;
                }
                else
                {
                    Wireframe = true;
                }
            }
        }

        public override string ToString()
        {
            return "Cam :" + Camera.main.transform.position;
        }

    }
}
