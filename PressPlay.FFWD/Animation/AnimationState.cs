using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class AnimationState
    {
        public string name;
        public float normalizedTime {
            get 
            {
                if (length == 0) { return 0; }
                return time / length; 
            }
            set 
            { 
                time = Mathf.Clamp01(value) * length;
            }
        }

        public bool enabled = false;
        public float time = 0.0f;
        
        public float speed = 1.0f;
        public float length;
        public WrapMode wrapMode;
    }
}
