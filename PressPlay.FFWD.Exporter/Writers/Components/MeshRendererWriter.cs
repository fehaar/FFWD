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
            writer.WriteElement("enabled", mr.enabled);
            writer.WriteElement("materials", mr.sharedMaterials);
            if (mr is SkinnedMeshRenderer)
            {
                writer.WriteMesh((mr as SkinnedMeshRenderer).sharedMesh, "sharedMesh");
            }
        }
        #endregion
    }
}
