using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;

namespace PressPlay.FFWD.Exporter.Writers
{
    public class XNAMeshColliderComponentWriter : IComponentWriter
    {
        #region IComponentWriter Members
        public void Write(SceneWriter scene, object component)
        {
            MonoBehaviour script = component as MonoBehaviour;
            if (script == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }
            MeshCollider collider = (component as MonoBehaviour).collider as MeshCollider;
            if (collider == null)
            {
                throw new Exception("XNAMeshCollider needs a MeshCollider on the same object");
            }
            scene.WriteElement("Material", collider.material.name.Replace(" (Instance)", ""));
            scene.WriteElement("IsTrigger", collider.isTrigger);
            scene.WriteElement("Triangles", collider.sharedMesh.triangles);
            scene.WriteElement("Vertices", collider.sharedMesh.vertices);
        }
        #endregion
    }
}
