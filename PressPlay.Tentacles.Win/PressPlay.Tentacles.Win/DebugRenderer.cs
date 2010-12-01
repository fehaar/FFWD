using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PressPlay.FFWD;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
using Box2D.XNA;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.Components;
using PressPlay.Tentacles.Debugging;

namespace PressPlay.Tentacles
{
    public class DebugRenderer : DrawableGameComponent
    {
        public DebugRenderer(Game game)
            : base(game)
        {
        }

        public bool Wireframe = false;
        public bool PhysicsDebug = false;
        private Box2DDebugDraw physicsDebugDraw;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        public Vector2 Position = new Vector2(32, 32);

        public override void Initialize()
        {
            base.Initialize();
#if WINDOWS_PHONE            
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
#if WINDOWS_PHONE
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
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            if (PhysicsDebug)
            {
                Physics.DoDebugDraw();
            }
            physicsDebugDraw.FinishDrawShapes(GraphicsDevice);

            spriteBatch.Begin();

            KeyValuePair<string, string>[] displayStrings = Debug.DisplayStrings.ToArray();
            Vector2 offset = Vector2.Zero;
            for (int i = 0; i < displayStrings.Length; i++)
            {
                string text = displayStrings[i].Key + ": " + displayStrings[i].Value;
                spriteBatch.DrawString(font, text, Position + Vector2.One + offset, Color.Black);
                spriteBatch.DrawString(font, text, Position + offset, Color.White);
                offset.Y += font.MeasureString(text).Y;
            }

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

    }
}
