using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;

namespace PressPlay.FFWD.Exporter.Writers.Components.Extensions
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

            Quaternion yNeutralRotation = Quaternion.Euler(collider.transform.rotation.eulerAngles.x, 0, collider.transform.rotation.eulerAngles.z);

            Transform meshOrigin = new GameObject().transform;
            Transform verticePosition = new GameObject().transform;
            verticePosition.parent = meshOrigin;
            Vector3[] rotatedVertices = new Vector3[collider.sharedMesh.vertices.Length];

            meshOrigin.transform.rotation = yNeutralRotation;

            for (int i = 0; i < rotatedVertices.Length; i++)
            {
                verticePosition.localPosition = collider.sharedMesh.vertices[i];
                rotatedVertices[i] = verticePosition.position;
            }

            writer.Write(scene, collider);

            scene.WriteElement("triangles", collider.sharedMesh.triangles);
            scene.WriteElement("vertices", rotatedVertices);
        }
        #endregion
    }
}
