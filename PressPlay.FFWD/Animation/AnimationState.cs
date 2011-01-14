using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class AnimationState
    {
        public float normalizedTime {
            get 
            {
                if (length == 0) { return 0; }
                return time / length; 
            }
            set 
            { 
                
                time = value * length;
                Debug.Display("normalized time", value);
            }
        }

        public bool enabled = false;
        public float time = 0.0f;
        internal float startTime = 0.0f;
        
        public float speed = 1.0f;
        public float length;
        public WrapMode wrapMode;

        internal int firstFrame = -1;
        internal int lastFrame = Int32.MaxValue;
    }
}
