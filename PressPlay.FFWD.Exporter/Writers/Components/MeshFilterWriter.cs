using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    public class MeshFilterWriter : IComponentWriter
    {
        #region IComponentWriter Members
        public void Write(SceneWriter scene, object component)
        {
            MeshFilter filter = (component as Component).GetComponent<MeshFilter>();
            if (filter == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }
            if (filter.sharedMesh != null)
            {
                scene.WriteMesh(filter.sharedMesh, "mesh", filter.gameObject.isStatic);
            }
            scene.WriteElement("isStatic", filter.gameObject.isStatic);
        }
        #endregion
    }
}
