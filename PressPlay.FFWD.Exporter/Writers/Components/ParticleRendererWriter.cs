using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;

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
            writer.WriteElement("materials", pr.sharedMaterials);
            writer.WriteElement("lengthScale", pr.lengthScale);
            writer.WriteElement("velocityScale", pr.velocityScale);
            //writer.WriteElement("cameraVelocityScale", pr.cameraVelocityScale);
            writer.WriteElement("maxParticleSize", pr.maxParticleSize);
            writer.WriteElement("uvAnimation", new Vector3(pr.uvAnimationXTile, pr.uvAnimationYTile, pr.uvAnimationCycles));
        }

        #endregion
    }
}
