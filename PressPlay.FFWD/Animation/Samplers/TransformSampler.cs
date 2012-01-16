using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    internal class TransformSampler : Sampler
    {
        internal Vector3Sampler position;
        internal QuaternionSampler rotation;
        internal Vector3Sampler scale;
        internal Transform transform;

        public TransformSampler(Transform t)
            : base(t, null)
        {
            this.transform = t;
        }

        protected override object GetSampleValue(float time)
        {
            if (position != null)
            {
                position.GetSampleValue(time, ref transform._localPosition);
            }
            if (rotation != null)
            {
                rotation.GetSampleValue(time, ref transform._localRotation);
            }
            if (scale != null)
            {
                scale.GetSampleValue(time, ref transform._localScale);
            }
            transform.hasDirtyWorld = true;
            return null;
        }

        internal override void AddCurveData(AnimationClipCurveData curveData)
        {
            base.AddCurveData(curveData);
            if (curveData.propertyName.StartsWith("m_LocalPosition"))
            {
                if (position == null)
                {
                    position = new Vector3Sampler(null, null);
                }
                position.AddCurveData(curveData);
                return;
            }
            if (curveData.propertyName.StartsWith("m_LocalRotation"))
            {
                if (rotation == null)
                {
                    rotation = new QuaternionSampler(null, null);
                }
                rotation.AddCurveData(curveData);
                return;
            }
            if (curveData.propertyName.StartsWith("m_LocalScale"))
            {
                if (scale == null)
                {
                    scale = new Vector3Sampler(null, null);
                }
                scale.AddCurveData(curveData);
            }
        } 
    }
}
