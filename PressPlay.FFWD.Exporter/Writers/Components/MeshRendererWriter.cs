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
            SkinnedMeshRenderer smr = mr as SkinnedMeshRenderer;
            if (smr != null)
            {
                int[] bones = new int[smr.bones.Length];
                for (int i = 0; i < smr.bones.Length; i++)
			    {
                    bones[i] = smr.bones[i].GetInstanceID();
			    }
                writer.WriteElement("bones", bones);
                writer.WriteMesh(smr.sharedMesh, "sharedMesh");
            }
            if (mr is LineRenderer)
            {
                writer.WriteElement("useWorldSpace", (mr as LineRenderer).useWorldSpace);
            }
        }
        #endregion
    }
}
