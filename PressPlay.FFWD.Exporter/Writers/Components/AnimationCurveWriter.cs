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
            foreach (Keyframe item in curve.keys)
            {
                sb.AppendFormat("{0} {1} {2} {3} {4} ", item.time, item.value, item.inTangent, item.outTangent, "Smooth");
            }
            writer.WriteElement("Keys", sb.ToString().TrimEnd());
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
