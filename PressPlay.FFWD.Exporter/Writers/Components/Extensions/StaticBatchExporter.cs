using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;

namespace PressPlay.FFWD.Exporter.Writers.Components.Extensions
{
    public class StaticBatchExporter : IComponentWriter
    {
        #region IComponentWriter Members
        public void Write(SceneWriter writer, object component)
        {
            Component script = component as Component;
            if (script == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }
            MeshFilter[] meshFilters = script.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            for ( int i = 0; i < meshFilters.Length; i++) {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            }
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combine);
            MeshRenderer mr = script.GetComponentInChildren<MeshRenderer>();
            writer.WriteElement("materials", mr.sharedMaterials);
            writer.WriteElement("triangles", mesh.triangles);
            writer.WriteElement("vertices", mesh.vertices);
            //writer.WriteElement("normals", mesh.normals);
            writer.WriteElement("uv", mesh.uv);
            Debug.Log(script.name + " batched " + combine.Length + " objects into a mesh of " + (mesh.triangles.Length / 3) + " triangles and " + mesh.vertices.Length + " vertices.");
        }
        #endregion
    }
}
