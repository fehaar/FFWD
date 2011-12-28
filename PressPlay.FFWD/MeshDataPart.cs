using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD
{
    internal class MeshDataPart
    {
        [ContentSerializer]
        internal Microsoft.Xna.Framework.Vector3[] vertices;
        [ContentSerializer]
        internal Microsoft.Xna.Framework.Vector3[] normals;
        [ContentSerializer]
        internal Microsoft.Xna.Framework.Vector2[] uv;
        [ContentSerializer]
        internal short[][] triangles;
        [ContentSerializer]
        internal byte[] blendIndices;
        [ContentSerializer]
        internal Microsoft.Xna.Framework.Vector4[] blendWeights;

        [ContentSerializer]
        internal Microsoft.Xna.Framework.BoundingBox boundingBox;

        internal void AddSubMesh(MeshDataPart mesh)
        {
            short[][] newTris = new short[triangles.Length + mesh.triangles.Length][];
            for (int i = 0; i < triangles.Length; i++)
            {
                newTris[i] = triangles[i];
            }
            for (int i = 0; i < mesh.triangles.Length; i++)
            {
                short[] triPart = new short[mesh.triangles[i].Length];
                for (int x = 0; x < triPart.Length; x++)
                {
                    triPart[x] = (short)(mesh.triangles[i][x] + vertices.Length);
                }
                newTris[triangles.Length + i] = triPart;
            }
            triangles = newTris;

            Microsoft.Xna.Framework.Vector3[] newVerts = new Microsoft.Xna.Framework.Vector3[vertices.Length + mesh.vertices.Length];
            vertices.CopyTo(newVerts, 0);
            mesh.vertices.CopyTo(newVerts, vertices.Length);
            vertices = newVerts;

            Microsoft.Xna.Framework.Vector3[] newNormals = new Microsoft.Xna.Framework.Vector3[normals.Length + mesh.normals.Length];
            normals.CopyTo(newNormals, 0);
            mesh.normals.CopyTo(newNormals, normals.Length);
            normals = newNormals;

            Microsoft.Xna.Framework.Vector2[] newUV = new Microsoft.Xna.Framework.Vector2[uv.Length + mesh.uv.Length];
            uv.CopyTo(newUV, 0);
            mesh.uv.CopyTo(newUV, uv.Length);
            uv = newUV;
        }
    }
}
