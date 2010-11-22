using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
#if !WINDOWS
using Microsoft.Xna.Framework.Input.Touch;
#endif

namespace PressPlay.Tentacles
{
    public class TouchHandler : GameComponent
    {
        public TouchHandler(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
#if !WINDOWS
            samples.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                samples.Add(TouchPanel.ReadGesture());
            }
#endif
        }

#if !WINDOWS
        private static List<GestureSample> samples = new List<GestureSample>();
        public static IEnumerable<GestureSample> GetSample(GestureType type)
        {
            for (int i = 0; i < samples.Count; i++)
            {
                if ((samples[i].GestureType & type) != 0)
                {
                    yield return samples[i];
                }
            }
            yield break;
        }
#endif
    }
}
