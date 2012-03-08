using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Components;
using System.Reflection;

namespace PressPlay.FFWD
{
    internal class FloatSampler : Sampler
    {
        private AnimationCurve curve;

        public FloatSampler(object t, string memberName, AnimationCurve curve)
            : base(t, memberName)
        {
            this.curve = curve;
            if (curve == null)
            {
                throw new ArgumentNullException("curve", "The animation curve cannot be null");
            }
        }

        protected override object GetSampleValue(float time)
        {
            return curve.Evaluate(time);
        }
    }
}
