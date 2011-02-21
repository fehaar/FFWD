#region File Description
//-----------------------------------------------------------------------------
// CpuSkinnedModelPart.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.SkinnedModel
{
    public class CpuSkinnedModelPart
    {
        public readonly string name;
        private readonly int triangleCount;
        private readonly int vertexCount;
        private readonly CpuVertex[] cpuVertices;
        //internal readonly VertexPositionNormalTexture[] gpuVertices;
        //internal readonly short[] indices;

        internal Mesh mesh;

        public BasicEffect Effect { get; internal set; }
        
        internal CpuSkinnedModelPart(string name, int triangleCount, CpuVertex[] vertices, IndexBuffer indexBuffer)
        {
            this.name = name;
            this.triangleCount = triangleCount;
            this.vertexCount = vertices.Length;
            this.cpuVertices = vertices;

            mesh = new Mesh();
            mesh.vertices = new Microsoft.Xna.Framework.Vector3[cpuVertices.Length];
            mesh.normals = new Microsoft.Xna.Framework.Vector3[cpuVertices.Length];
            mesh.uv = new Microsoft.Xna.Framework.Vector2[cpuVertices.Length];
            mesh.triangles = new short[indexBuffer.IndexCount];
            indexBuffer.GetData<short>(mesh.triangles);

            // copy texture coordinates once since they don't change with skinnning
            for (int i = 0; i < cpuVertices.Length; i++)
            {
                mesh.uv[i] = cpuVertices[i].TextureCoordinate;
            }
        }

        internal void InitializeMesh(Mesh newMesh)
        {
            newMesh.vertices = mesh.vertices;
            newMesh.normals = mesh.normals;
            newMesh.uv = mesh.uv;
            newMesh.triangles = mesh.triangles;
        }

        public void SetBones(Matrix[] bones, ref Matrix world, Mesh mesh)
        {
            // skin all of the vertices
            for (int i = 0; i < vertexCount; i++)
            {
                CpuSkinningHelpers.SkinVertex(
                    bones,
                    ref cpuVertices[i].Position,
                    ref cpuVertices[i].Normal,
                    ref world,
                    ref cpuVertices[i].BlendIndices,
                    ref cpuVertices[i].BlendWeights,
                    out mesh.vertices[i],
                    out mesh.normals[i]);
            }

            // put the vertices into our vertex buffer
            //vertexBuffer.SetData(gpuVertices, 0, vertexCount, SetDataOptions.Discard);
        }
    }
}
