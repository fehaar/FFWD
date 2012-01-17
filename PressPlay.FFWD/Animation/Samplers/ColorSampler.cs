using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    internal class ColorSampler : Sampler
    {
        AnimationCurve r;
        AnimationCurve g;
        AnimationCurve b;
        AnimationCurve a;

        public ColorSampler(object t, string memberName, AnimationCurve r, AnimationCurve g, AnimationCurve b, AnimationCurve a)
            :base(t, memberName)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        protected override object GetSampleValue(float time)
        {
            Color c = (Color)GetOriginalValue();
            if (r != null)
            {
                c.r = r.Evaluate(time);
            }
            if (g != null)
            {
                c.g = g.Evaluate(time);
            }
            if (b != null)
            {
                c.b = b.Evaluate(time);
            }
            if (a != null)
            {
                c.a = a.Evaluate(time);
            }
            return c;
        }
    }
}
