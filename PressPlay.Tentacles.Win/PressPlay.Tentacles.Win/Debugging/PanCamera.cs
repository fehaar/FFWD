using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
#if !WINDOWS
using Microsoft.Xna.Framework.Input.Touch;
#endif
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Debugging
{
    class PanCamera : GameComponent
    {
        public PanCamera(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
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
            foreach (GestureSample gesture in TouchHandler.GetSample(GestureType.FreeDrag |
                                         GestureType.Pinch))
            {
                switch (gesture.GestureType)
                {
                    case GestureType.FreeDrag:
                        Camera.main.transform.localPosition += new Vector3(-gesture.Delta.X, 0, gesture.Delta.Y) * (float)gameTime.ElapsedGameTime.TotalSeconds;
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
#endif
        }
    }
}
