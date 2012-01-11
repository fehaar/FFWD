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
        private AnimationCurve x;
        private AnimationCurve y;
        private AnimationCurve z;

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
            return v;
        }
    }
}
