using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
#if !WINDOWS
using Microsoft.Xna.Framework.Input.Touch;
#endif
using PressPlay.FFWD.Components;
using PressPlay.FFWD;
using Microsoft.Xna.Framework.Input;

namespace PressPlay.Tentacles.Debugging
{
    class PanCamera : GameComponent
    {
        public PanCamera(Game game) : base(game)
        {
        }

        public float TurnSpeed = 30;
        public float DeadZone = 0.1f;

        public override void Initialize()
        {
            base.Initialize();
            Accelerometer.Initialize();
#if WINDOWS_PHONE
            TouchPanel.EnabledGestures = TouchPanel.EnabledGestures | 
                                         GestureType.FreeDrag |
                                         GestureType.Pinch;
#endif
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
#if WINDOWS_PHONE
            if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Device)
            {
                foreach (GestureSample gesture in TouchHandler.GetSample(GestureType.FreeDrag |
                                         GestureType.Pinch))
                {
                    switch (gesture.GestureType)
                    {
                        case GestureType.FreeDrag:
                            Move(gesture.Delta * (float)gameTime.ElapsedGameTime.TotalSeconds);
                            break;
                        case GestureType.Pinch:
                            // get the current and previous locations of the two fingers
                            Vector2 a = gesture.Position;
                            Vector2 aOld = gesture.Position - gesture.Delta;
                            Vector2 b = gesture.Position2;
                            Vector2 bOld = gesture.Position2 - gesture.Delta2;

                            // figure out the distance between the current and previous locations
                            float d = Vector2.Distance(a, b);
                            float dOld = Vector2.Distance(aOld, bOld);

                            // calculate the difference between the two and use that to alter the scale
                            float scaleChange = (d - dOld) * .1f;
                            Camera.main.transform.localPosition += new Vector3(0, scaleChange, 0);
                            break;
                    }
                }
            }
            else
#endif
            {
                KeyboardState key = Keyboard.GetState();
                Vector3 dir = Vector3.Zero;
                float speed = 5;
                // Translate camera
                if (key.IsKeyDown(Keys.A) || key.IsKeyDown(Keys.Q) || key.IsKeyDown(Keys.Z))
                {
                    dir.X += speed;
                }
                if (key.IsKeyDown(Keys.D) || key.IsKeyDown(Keys.E) || key.IsKeyDown(Keys.C))
                {
                    dir.X -= speed;
                }
                if (key.IsKeyDown(Keys.W) || key.IsKeyDown(Keys.Q) || key.IsKeyDown(Keys.E))
                {
                    dir.Z += speed;
                }
                if (key.IsKeyDown(Keys.X) || key.IsKeyDown(Keys.Z) || key.IsKeyDown(Keys.C))
                {
                    dir.Z -= speed;
                }
                if (key.IsKeyDown(Keys.F))
                {
                    dir.Y += speed;
                }
                if (key.IsKeyDown(Keys.R))
                {
                    dir.Y -= speed;
                }
                if (key.IsKeyDown(Keys.RightShift))
                {
                    dir *= 10;
                }
                Move(new Vector2(dir.X, dir.Z) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                Camera.main.transform.localPosition += new Vector3(0, dir.Y, 0);
            }

            AccelerometerState state = Accelerometer.GetState();
            float turn = state.Acceleration.Y;
            if (Math.Abs(turn) > DeadZone)
            {
                Camera.main.transform.Rotate(Vector3.UnitY, turn * TurnSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, Space.Self);
            }
        }

        private void Move(Vector2 amount)
        {
            Vector3 camUp = Vector3.Transform(Camera.main.up, Camera.main.transform.localRotation);
            Vector3 camLeft = new Vector3(-camUp.Z, 0, camUp.X);
            Camera.main.transform.localPosition += (camUp * amount.Y) + (camLeft * amount.X);
        }
    }
}
