using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;
using System.Reflection;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    internal class ParticleRendererWriter : IComponentWriter
    {
        #region IComponentWriter Members

        public void Write(SceneWriter writer, object component)
        {
            ParticleRenderer pr = component as ParticleRenderer;
            if (pr == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }
            writer.WriteElement("enabled", pr.enabled);
            writer.WriteElement("sharedMaterials", pr.sharedMaterials);
            //writer.WriteElement("stretchParticles", pr.);
            writer.WriteElement("lengthScale", pr.lengthScale);
            writer.WriteElement("velocityScale", pr.velocityScale);
            //writer.WriteElement("cameraVelocityScale", pr.cameraVelocityScale);
            writer.WriteElement("maxParticleSize", pr.maxParticleSize);
            writer.WriteElement("uvAnimation", new Vector3(pr.uvAnimationXTile, pr.uvAnimationYTile, pr.uvAnimationCycles));

            Component c = pr.gameObject.GetComponent("XNAEllipsoidParticleEmitter");
            if (c != null)
            {
                writeValue<object>(writer, c, "stretchParticles");
            }
        }
        #endregion

        private void writeValue<T>(SceneWriter writer, Component c, string fieldName)
        {
            Type t = c.GetType();
            FieldInfo f = t.GetField(fieldName);
            object value = f.GetValue(c);
            
            //if (!((T)value).Equals(default(T)))
            //{
                writer.WriteElement(fieldName, value);
            //}
        }
    }
}
