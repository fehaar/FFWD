using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Components;
using System.Reflection;

namespace PressPlay.FFWD
{
    internal class Vector3Sampler : Sampler
    {
        internal AnimationCurve x;
        internal AnimationCurve y;
        internal AnimationCurve z;

        public Vector3Sampler(object t, string memberName)
            : base(t, memberName)
        {
        }

        public Vector3Sampler(object t, string memberName, AnimationCurve x, AnimationCurve y, AnimationCurve z)
            : base(t, memberName)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        protected override object GetSampleValue(float time)
        {
            Vector3 v = (Vector3)GetOriginalValue();
            GetSampleValue(time, ref v);
            return v;
        }

        internal void GetSampleValue(float time, ref Vector3 v)
        {
            if (x != null)
            {
                v.x = x.Evaluate(time);
            }
            if (y != null)
            {
                v.y = y.Evaluate(time);
            }
            if (z != null)
            {
                v.z = z.Evaluate(time);
            }
        }

        internal override void AddCurveData(AnimationClipCurveData curveData)
        {
            base.AddCurveData(curveData);
            if (curveData.propertyName.EndsWith(".x"))
            {
                x = curveData.curve;
            }
            if (curveData.propertyName.EndsWith(".y"))
            {
                y = curveData.curve;
            }
            if (curveData.propertyName.EndsWith(".z"))
            {
                z = curveData.curve;
            }
        } 
    }
}
