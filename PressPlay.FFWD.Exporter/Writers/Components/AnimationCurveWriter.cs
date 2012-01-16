using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;
 
namespace PressPlay.FFWD.Exporter.Writers.Components
{
    /// <summary>
    /// NOTE: This is a strange abomination since AnimationCurve is not really a component.
    /// </summary>
    public class AnimationCurveWriter : IComponentWriter
    {
        public void Write(SceneWriter writer, object component)
        {
            AnimationCurve curve = component as AnimationCurve;
            if (curve == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }
            writer.WriteElement("PreLoop", ConvertLoopType(curve.preWrapMode));
            writer.WriteElement("PostLoop", ConvertLoopType(curve.postWrapMode));
            StringBuilder sb = new StringBuilder();

            // NOTE: Keyframe.tangentMode might say something about if the tangetn is smooth or broken?

            Keyframe[] normalizedKeys = new Keyframe[curve.keys.Length];
            for (int i = 0; i < curve.keys.Length; i++)
            {
                Keyframe k = curve.keys[i];
                normalizedKeys[i] = new Keyframe(k.time, k.value, k.inTangent, k.outTangent);
                if (i > 0)
                {
                    Keyframe j = curve.keys[i - 1];
                    normalizedKeys[i - 1].outTangent = DenormalizeTangent(j.time, k.time, j.outTangent);
                    normalizedKeys[i].inTangent = DenormalizeTangent(j.time, k.time, k.inTangent);
                }
            }

            foreach (Keyframe item in normalizedKeys)
            {
                sb.AppendFormat("{0} {1} {2} {3} {4} ", item.time, item.value, item.inTangent, item.outTangent, "Smooth");
            }
            writer.WriteElement("Keys", sb.ToString().TrimEnd());
        }

        private float DenormalizeTangent(float time, float otherTime, float tangent)
        {
            if (tangent == 0)
            {
                return 0;
            }
            return tangent * (otherTime - time);
        }

        private string ConvertLoopType(WrapMode wrapMode)
        {
            switch (wrapMode)
            {
                case WrapMode.Clamp:
                case WrapMode.ClampForever:
                case WrapMode.Default:
                    return "Constant";
                case WrapMode.Loop:
                    return "Cycle";
                case WrapMode.PingPong:
                    return "Oscillate";
                default:
                    return "Constant";
            }
        }
    }
}
