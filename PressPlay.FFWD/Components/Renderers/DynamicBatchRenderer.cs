using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.SkinnedModel;

namespace PressPlay.FFWD.Components
{
    public class DynamicBatchRenderer
    {
        public DynamicBatchRenderer(GraphicsDevice device)
        {
            this.device = device;
        }

        public static int InitialBatchSize = 20;
        public static int BatchExpansion = 5;

        private Material currentMaterial = Material.Default;

        private GraphicsDevice device;
        private BasicEffect effect;

        private struct BatchData
        {
            internal Matrix world;
            internal Mesh mesh;
            internal CpuSkinnedModelPart model;
        }

        private int currentBatchIndex = 0;
        private BatchData[] data = new BatchData[InitialBatchSize];
        private int batchVertexSize = 0;
        private int batchIndexSize = 0;
        private int currentVertexIndex = 0;
        private int currentIndexIndex = 0;

        private VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[0];
        private Microsoft.Xna.Framework.Vector3[] positionData = new Microsoft.Xna.Framework.Vector3[0];
        private short[] indexData = new short[0];

        /// <summary>
        /// Batch draw the current mesh filter.
        /// We will only do the actual draw call if the material has changed.
        /// Until that we will collect the mesh filters into a list and draw them all together.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        internal int Draw<T>(Camera cam, Material material, T verts, Transform transform)
        {
            int drawCalls = 0;

            if (currentMaterial.name != material.name)
            {
                drawCalls = DoDraw(device, cam);
                currentMaterial = material;
            }            
            Add(verts, transform);
            return drawCalls;
        }

        private void EndBatch()
        {
            currentMaterial = Material.Default;
            currentBatchIndex = 0;
            batchVertexSize = 0;
            batchIndexSize = 0;
        }

        private void Add<T>(T model, Transform transform)
        {
            if (currentBatchIndex == data.Length)
            {
                ExpandData();
            }

            data[currentBatchIndex].world = (transform != null) ? transform.world : Matrix.Identity;
            MeshFilter filter = model as MeshFilter;
            if (filter != null)
            {
                data[currentBatchIndex].mesh = filter.sharedMesh;
                data[currentBatchIndex].model = null;
                batchVertexSize += filter.sharedMesh.vertices.Length;
                batchIndexSize += filter.sharedMesh.triangles.Length;
            }

            CpuSkinnedModelPart part = model as CpuSkinnedModelPart;
            if (part != null)
            {
                data[currentBatchIndex].mesh = null;
                data[currentBatchIndex].model = part;
                batchVertexSize += part.gpuVertices.Length;
                batchIndexSize += part.indices.Length;
            }

            currentBatchIndex++;
        }

        private void ExpandData()
        {
            BatchData[] newData = new BatchData[data.Length + BatchExpansion];
            data.CopyTo(newData, 0);
            data = newData;
        }

        internal int DoDraw(GraphicsDevice device, Camera cam)
        {
            if (currentBatchIndex == 0)
            {
                return 0;
            }

            PrepareData();

            if (currentIndexIndex == 0)
            {
                return 0;
            }

            if (effect == null)
            {
                effect = new BasicEffect(device);
                effect.VertexColorEnabled = false;
                effect.World = Matrix.Identity;
                effect.LightingEnabled = false;
            }

            effect.View = cam.view;
            effect.Projection = cam.projectionMatrix;
            currentMaterial.SetBlendState(device);

#if DEBUG
            if (Camera.logRenderCalls)
            {
                Debug.LogFormat("Dyn batch draw: {0} on {1} batched {2}, verts {3}, indices {4}", currentMaterial.mainTexture, cam.gameObject, currentBatchIndex, currentVertexIndex, currentIndexIndex);
            }
#endif

            if (currentMaterial.texture != null)
            {
                effect.TextureEnabled = true;
                effect.Texture = currentMaterial.texture;
                effect.DiffuseColor = Color.white;
            }
            else
            {
                effect.DiffuseColor = currentMaterial.color;
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    vertexData,
                    0,
                    currentVertexIndex,
                    indexData,
                    0,
                    currentIndexIndex / 3
                );
            }

            EndBatch();
            return 1;
        }

        private void PrepareData()
        {
            if (vertexData.Length < batchVertexSize)
            {
                vertexData = new VertexPositionNormalTexture[batchVertexSize];
            }
            if (indexData.Length < batchIndexSize)
            {
                indexData = new short[batchIndexSize];
            }

            currentVertexIndex = 0;
            currentIndexIndex = 0;
            for (int i = 0; i < currentBatchIndex; i++)
            {
                if (data[i].mesh != null)
                {
                    PrepareMesh(data[i].mesh, ref data[i].world);
                }
                if (data[i].model != null)
                {
                    PrepareVerts(data[i].model, ref data[i].world);
                }
            }
        }

        private void PrepareVerts(CpuSkinnedModelPart model, ref Matrix transform)
        {
            model.gpuVertices.CopyTo(vertexData, currentVertexIndex);
            for (int t = 0; t < model.indices.Length; t++)
            {
                indexData[currentIndexIndex + t] = (short)(model.indices[t] + currentVertexIndex);
            }

            currentVertexIndex += model.gpuVertices.Length;
            currentIndexIndex += model.indices.Length;
        }

        private void PrepareMesh(Mesh mesh, ref Matrix transform)
        {
            if (positionData.Length < mesh.vertices.Length)
            {
                positionData = new Microsoft.Xna.Framework.Vector3[mesh.vertices.Length];
            }

            if (transform != Matrix.Identity)
            {
                Microsoft.Xna.Framework.Vector3.Transform(mesh.vertices, ref transform, positionData);
            }
            else
            {
                mesh.vertices.CopyTo(positionData, 0);
            }

            for (int v = 0; v < mesh.vertices.Length; v++)
            {
                vertexData[currentVertexIndex + v].Position = positionData[v];
                vertexData[currentVertexIndex + v].TextureCoordinate = mesh.uv[v];
                if (mesh.normals != null)
                {
                    vertexData[currentVertexIndex + v].Normal = mesh.normals[v];
                }
            }

            for (int t = 0; t < mesh.triangles.Length; t++)
            {
                indexData[currentIndexIndex + t] = (short)(mesh.triangles[t] + currentVertexIndex);
            }

            currentVertexIndex += mesh.vertices.Length;
            currentIndexIndex += mesh.triangles.Length;
        }
    }
}
