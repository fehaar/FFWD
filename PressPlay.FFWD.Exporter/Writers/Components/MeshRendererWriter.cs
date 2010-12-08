using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;
using System.Xml;
using UnityEditor;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    internal class MeshRendererWriter : IComponentWriter
    {
        #region IComponentWriter Members
        public void Write(SceneWriter writer, object component)
        {
            Renderer mr = component as Renderer;
            if (mr == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }
            if (mr.sharedMaterial != null && mr.sharedMaterial.mainTexture != null)
            {
                writer.WriteTexture(mr.sharedMaterial.mainTexture);
                writer.WriteElement("shader", mr.sharedMaterial.shader.name);
            }

            if (component is SkinnedMeshRenderer)
            {
                writer.WriteMesh((component as SkinnedMeshRenderer).sharedMesh);
            }
            else
            {
                MeshFilter filter = (component as Component).GetComponent<MeshFilter>();
                if (filter != null && filter.sharedMesh != null)
                {
                    writer.WriteMesh(filter.sharedMesh);
                }
            }
        }
        #endregion
    }
}
