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

        private Sampler[] samplers;

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

        internal void Sample()
        {
            if (!enabled)
            {
                return;
            }
            if (clip != null && samplers != null)
            {
                clip.Sample(samplers, time);
            }
        }

        internal void InitializeSamplers(GameObject g)
        {
            if (clip.curves == null)
            {
                return;
            }
            Dictionary<string, Sampler> samps = new Dictionary<string, Sampler>();
            for (int i = 0; i < clip.curves.Length; i++)
            {
                AnimationClipCurveData curveData = clip.curves[i];
                string sampleKey = GetSampleKey(curveData);

                Sampler sampler = null;
                if (!samps.ContainsKey(sampleKey))
                {
                    GameObject go = g;
                    if (!String.IsNullOrEmpty(curveData.path))
                    {
                        Transform t = g.transform.Find(curveData.path);
                        if (t != null)
	                    {
                            go = t.gameObject;
	                    }
                        else
                        {
                            continue;
                        }
                    }

                    sampler = GetSampler(go, curveData);
                    if (sampler != null)
                    {
                        samps.Add(sampleKey, sampler);
                    }
                }
                else
                {
                    sampler = samps[sampleKey];
                }

                if (sampler != null)
                {
                    sampler.AddCurveData(curveData);
                }
            }
            samplers = new Sampler[samps.Count];
            samps.Values.CopyTo(samplers, 0);
        }

        private string GetSampleKey(AnimationClipCurveData curveData)
        {
            if (curveData.type == typeof(Transform).FullName)
            {
                return curveData.path + "/" + curveData.type;
            }
            return curveData.path + "/" + curveData.type + ":" + ((curveData.propertyName.Contains(".")) ? curveData.propertyName.Substring(0, curveData.propertyName.LastIndexOf('.')) : curveData.propertyName);
        }

        private Sampler GetSampler(GameObject g, AnimationClipCurveData curveData)
        {
            string tp = curveData.type.Replace("c:", "PressPlay.FFWD.");            
            if (tp == typeof(Transform).FullName)
            {
                return new TransformSampler(g.transform);
            }
            return null;
        }
    }
}
