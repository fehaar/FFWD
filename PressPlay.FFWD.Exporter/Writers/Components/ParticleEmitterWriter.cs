using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;
using System.Reflection;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    internal class ParticleEmitterWriter : IComponentWriter
    {
        #region IComponentWriter Members
        public void Write(SceneWriter writer, object component)
        {
            ParticleEmitter pr = component as ParticleEmitter;
            if (pr == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }
            writer.WriteElement("minEnergy", pr.minEnergy);
            writer.WriteElement("maxEnergy", pr.maxEnergy);
            writer.WriteElement("minEmission", pr.minEmission);
            writer.WriteElement("maxEmission", pr.maxEmission);

            writer.WriteElement("emit", pr.emit);
            writer.WriteElement("minSize", pr.minSize);
            writer.WriteElement("maxSize", pr.maxSize);
            
            writer.WriteElement("emitterVelocityScale", pr.emitterVelocityScale);
            writer.WriteElement("worldVelocity", pr.worldVelocity);
            writer.WriteElement("localVelocity", pr.localVelocity);
            writer.WriteElement("rndVelocity", pr.rndVelocity);
            writer.WriteElement("useWorldSpace", pr.useWorldSpace);
            writer.WriteElement("enabled", pr.enabled);

            Component c = pr.GetComponent("XNAEllipsoidParticleEmitter");
            if (c != null)
            {
                writeValue<Vector3>(writer, c, "ellipsoid");
                writeValue<Boolean>(writer, c, "oneShot");
                writeValue<Vector3>(writer, c, "tangentVelocity");
                writeValue<float>(writer, c, "minEmitterRange");
            }
        }
        #endregion

        private void writeValue<T>(SceneWriter writer, Component c, string fieldName)
        {
            FieldInfo f = c.GetType().GetField(fieldName);
            object value = f.GetValue(c);
            if (!((T)value).Equals(default(T)))
            {
                writer.WriteElement(fieldName, value);
            }
        }
    }
}
