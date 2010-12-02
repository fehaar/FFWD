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
            MeshRenderer mr = component as MeshRenderer;
            if (mr == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }
            if (mr.sharedMaterial != null && mr.sharedMaterial.mainTexture != null)
            {
                writer.WriteTexture(mr.sharedMaterial.mainTexture);
                writer.WriteElement("shader", mr.sharedMaterial.shader.name);
            }
            MeshFilter filter = (component as Component).GetComponent<MeshFilter>();
            if (filter.sharedMesh != null)
            {
                writer.WriteMesh(filter.sharedMesh);
            }
        }
        #endregion
    }
}
