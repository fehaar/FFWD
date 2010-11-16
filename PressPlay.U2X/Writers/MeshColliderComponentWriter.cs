using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.U2X.Interfaces;
using UnityEngine;

namespace PressPlay.U2X.Writers
{
    public class MeshColliderComponentWriter : IComponentWriter
    {
        #region IComponentWriter Members
        public void Write(SceneWriter scene, object component)
        {
            MeshCollider collider = component as MeshCollider;
            if (collider == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }
            scene.WriteElement("Material", collider.material.name.Replace(" (Instance)", ""));
            scene.WriteElement("IsTrigger", collider.isTrigger);
            scene.WriteElement("Triangles", collider.sharedMesh.triangles);
            scene.WriteElement("Vertices", collider.sharedMesh.vertices);
            scene.WriteElement("Normals", collider.sharedMesh.normals);
        }
        #endregion
    }
}
