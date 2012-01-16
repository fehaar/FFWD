using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    internal class QuaternionSampler : Sampler
    {
        internal AnimationCurve x;
        internal AnimationCurve y;
        internal AnimationCurve z;
        internal AnimationCurve w;

        public QuaternionSampler(object t, string memberName)
            : base(t, memberName)
        {

        }

        protected override object GetSampleValue(float time)
        {
            Quaternion q = (Quaternion)GetOriginalValue();
            if (x != null)
            {
                q.x = x.Evaluate(time);
            }
            if (y != null)
            {
                q.y = y.Evaluate(time);
            }
            if (z != null)
            {
                q.z = z.Evaluate(time);
            }
            if (w != null)
            {
                q.w = w.Evaluate(time);
            }
            return q;
        }

        internal void GetSampleValue(float time, ref Quaternion q)
        {
            if (x != null)
            {
                q.x = x.Evaluate(time);
            }
            if (y != null)
            {
                q.y = y.Evaluate(time);
            }
            if (z != null)
            {
                q.z = z.Evaluate(time);
            }
            if (w != null)
            {
                q.w = w.Evaluate(time);
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
            if (curveData.propertyName.EndsWith(".w"))
            {
                w = curveData.curve;
            }
        } 
    }
}
