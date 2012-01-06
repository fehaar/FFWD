using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    public class AnimationState
    {
        public AnimationState(Animation anim, AnimationClip clip)
        {
            animation = anim;
            this.clip = clip;
        }

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

        internal bool wasUpdated = false;
        internal Animation animation;

        private AnimationClip _clip;
        public AnimationClip clip 
        {
            get
            {
                return _clip;
            }
            set
            {
                _clip = value;
                name = clip.name;
                length = clip.length;
                wrapMode = clip.wrapMode;
            }
        }

        internal void Update(float deltaTime)
        {
            wasUpdated = enabled;
            if (enabled)
            {
                time += deltaTime * speed;
                if (time >= length || time < 0)
                {
                    WrapMode wm = wrapMode;
                    if (wm == WrapMode.Default)
                    {
                        if (animation.wrapMode == WrapMode.Default)
                        {
                            wm = clip.wrapMode;
                        }
                        else
                        {
                            wm = animation.wrapMode;
                        }
                    }
                    switch (wm)
                    {
                        case WrapMode.Default:
                        case WrapMode.Once:
                            time = (speed > 0) ? length : 0;
                            enabled = false;
                            break;
                        case WrapMode.Loop:
                            if (speed > 0)
                            {
                                time -= length;
                            }
                            else
                            {
                                time = length - time;
                            }
                            break;
                        case WrapMode.PingPong:
                            if (speed > 0)
                            {
                                time = length + (length - time);
                            }
                            else
                            {
                                time = -time;
                            }
                            speed *= -1;
                            break;
                        case WrapMode.Clamp:
                            time = (speed > 0) ? length : 0;
                            break;
                    }
                }
            }
        }
    }
}
