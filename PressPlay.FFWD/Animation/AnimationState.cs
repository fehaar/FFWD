using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.Components
{
    public class AnimationState
    {
        public bool enabled = false;
        public float time = 0.0f;
        public float speed = 1.0f;
        public float length;
        public WrapMode wrapMode;

        internal int firstFrame = -1;
        internal int lastFrame = Int32.MaxValue;
    }
}
