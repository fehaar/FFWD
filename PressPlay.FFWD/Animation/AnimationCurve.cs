using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
    public class AnimationCurve : Curve
    {
        /// <summary>
        /// In Unity the tangents of the curves are normalized so they fit the time intervals of the curve. 
        /// XNA does not handle tangents that way so we need to denormalize them.
        /// </summary>
        public void Denormalize()
        {
            for (int i = 1; i < Keys.Count; i++)
            {
                CurveKey k = Keys[i];
                CurveKey j = Keys[i - 1];
                j.TangentOut = DenormalizeTangent(j.Position, k.Position, j.TangentOut);
                k.TangentIn = DenormalizeTangent(j.Position, k.Position, k.TangentIn);
            }
        }

        private float DenormalizeTangent(float time, float otherTime, float tangent)
        {
            if (tangent == 0)
            {
                return 0;
            }
            return tangent * (otherTime - time);
        }
    }
}
