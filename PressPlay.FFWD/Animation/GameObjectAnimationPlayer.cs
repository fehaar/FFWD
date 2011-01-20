using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD
{
    public class GameObjectAnimationPlayer : BaseAnimationPlayer
    {
        private Keyframe currentKeyframe;

        protected override void SetKeyframe(Keyframe keyframe)
        {
            currentKeyframe = keyframe;
        }

        public void GetTransform(out Matrix m)
        {
            m = currentKeyframe.Transform;
        }
    }
}
