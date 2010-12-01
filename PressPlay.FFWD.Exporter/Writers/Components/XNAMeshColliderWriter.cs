using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    public class XNAMeshColliderWriter : IComponentWriter
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
            ColliderWriter writer = new ColliderWriter();
            writer.Write(scene, collider);
            scene.WriteElement("triangles", collider.sharedMesh.triangles);
            scene.WriteElement("vertices", collider.sharedMesh.vertices);
        }
        #endregion
    }
}
