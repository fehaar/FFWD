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
            scene.WriteElement("Material", collider.material);
            scene.WriteElement("IsTrigger", collider.isTrigger);
            scene.WriteElement("Mesh", collider.sharedMesh.name);
        }
        #endregion
    }
}
