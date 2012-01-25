using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
    public class DynamicBatchRenderer
    {
        public DynamicBatchRenderer(GraphicsDevice device)
        {
            this.device = device;
        }

        private Material currentMaterial = Material.Default;

        private GraphicsDevice device;

        private int batchVertexSize = 0;
        private int batchIndexSize = 0;
        private int currentVertexIndex = 0;
        private int currentIndexIndex = 0;
        private bool hasColors = false;

        private VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[20000];
        private VertexPositionColorTexture[] vertexColorData = new VertexPositionColorTexture[1000];
        private Microsoft.Xna.Framework.Vector3[] positionData = new Microsoft.Xna.Framework.Vector3[600];
        private short[] indexData = new short[60000];

        /// <summary>
        /// Batch draw the current mesh filter.
        /// We will only do the actual draw call if the material has changed.
        /// Until that we will collect the mesh filters into a list and draw them all together.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        internal int Draw(Camera cam, Material material, Mesh mesh, Transform transform)
        {
            return Draw(cam, material, mesh, transform, 0);
        }

        /// <summary>
        /// Batch draw the current mesh filter.
        /// We will only do the actual draw call if the material has changed.
        /// Until that we will collect the mesh filters into a list and draw them all together.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        internal int Draw(Camera cam, Material material, Mesh mesh, Transform transform, int subMeshIndex)
        {
            int drawCalls = 0;

            if (currentMaterial.name != material.name)
            {
                drawCalls = DoDraw(device, cam);
                currentMaterial = material;
                hasColors = (mesh.colors != null) && (mesh.colors.Length > 0);
            }
            Matrix world = (transform != null) ? transform.world : Matrix.Identity;
            PrepareMesh(mesh, subMeshIndex, ref world);
            return drawCalls;
        }

        private void EndBatch()
        {
            currentMaterial = Material.Default;
            batchVertexSize = 0;
            batchIndexSize = 0;
            currentVertexIndex = 0;
            currentIndexIndex = 0;
        }

        internal int DoDraw(GraphicsDevice device, Camera cam)
        {
            if (currentIndexIndex == 0)
            {
                return 0;
            }

            cam.BasicEffect.World = Matrix.Identity;
            cam.BasicEffect.VertexColorEnabled = hasColors;
            cam.BasicEffect.LightingEnabled = (!hasColors) && Light.HasLights;

            currentMaterial.SetTextureState(cam.BasicEffect);
            currentMaterial.SetBlendState(device);

#if DEBUG
            if (Camera.logRenderCalls)
            {
                Debug.LogFormat("==> Dyn batch draw: {0} on {1} verts {2}, indices {3}", currentMaterial.mainTexture, cam.gameObject, currentVertexIndex, currentIndexIndex);
            }
#endif

            foreach (EffectPass pass in cam.BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                if (hasColors)
                {
                    device.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        vertexColorData,
                        0,
                        currentVertexIndex,
                        indexData,
                        0,
                        currentIndexIndex / 3
                    );
                }
                else
                {
                    device.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        vertexData,
                        0,
                        currentVertexIndex,
                        indexData,
                        0,
                        currentIndexIndex / 3
                    );
                }
            }

            EndBatch();
            return 1;
        }

        private void PrepareMesh(Mesh mesh, int subMeshIndex, ref Matrix transform)
        {
            batchVertexSize += mesh._vertices.Length;
            if (vertexData.Length < batchVertexSize)
            {
                if (hasColors)
                {
                    VertexPositionColorTexture[] newVertexData = new VertexPositionColorTexture[batchVertexSize];
                    vertexColorData.CopyTo(newVertexData, 0);
                    vertexColorData = newVertexData;
                }
                else
                {
                    VertexPositionNormalTexture[] newVertexData = new VertexPositionNormalTexture[batchVertexSize];
                    vertexData.CopyTo(newVertexData, 0);
                    vertexData = newVertexData;
                }
#if DEBUG
                Debug.LogWarning("Increased size of Dynamic vertex buffer to " + batchVertexSize);
#endif
            }
            batchIndexSize += mesh.triangles.Length;
            if (indexData.Length < batchIndexSize)
            {
                short[] newIndexData = new short[batchIndexSize];
                Buffer.BlockCopy(indexData, 0, newIndexData, 0, indexData.Length);
                indexData = newIndexData;
#if DEBUG
                Debug.LogWarning("Increased size of Dynamic index buffer to " + batchIndexSize);
#endif
            }

            if (positionData.Length < mesh._vertices.Length)
            {
                positionData = new Microsoft.Xna.Framework.Vector3[mesh._vertices.Length];
#if DEBUG
                Debug.LogWarning("Increased size of Dynamic position buffer to " + mesh._vertices.Length);
#endif
            }

            if (transform != Matrix.Identity)
            {
                Microsoft.Xna.Framework.Vector3.Transform(mesh._vertices, ref transform, positionData);
            }
            else
            {
                mesh._vertices.CopyTo(positionData, 0);
            }

            for (int v = 0; v < mesh._vertices.Length; v++)
            {
                if (hasColors)
                {
                    VertexPositionColorTexture vert = new VertexPositionColorTexture();
                    vert.Position = positionData[v];
                    vert.TextureCoordinate = (mesh._uv != null) ? mesh._uv[v] : Microsoft.Xna.Framework.Vector2.Zero;
                    vert.Color = mesh.colors[v];
                    vertexColorData[currentVertexIndex + v] = vert;
                }
                else
                {
                    VertexPositionNormalTexture vert = new VertexPositionNormalTexture();
                    vert.Position = positionData[v];
                    vert.TextureCoordinate = (mesh._uv != null) ? mesh._uv[v] : Microsoft.Xna.Framework.Vector2.Zero;
                    vert.Normal = (mesh._normals != null) ? mesh._normals[v] : Microsoft.Xna.Framework.Vector3.Zero;
                    vertexData[currentVertexIndex + v] = vert;
                }
            }

            short[] tris = mesh.triangles;
            if (mesh.subMeshCount > 1)
            {
                tris = mesh.GetTriangles(subMeshIndex);
            }
            for (int t = 0; t < tris.Length; t++)
            {
                indexData[currentIndexIndex + t] = (short)(tris[t] + currentVertexIndex);
            }

            currentVertexIndex += mesh._vertices.Length;
            currentIndexIndex += tris.Length;
        }
    }
}
