using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
    public class StaticBatchRenderer : Renderer
    {
        [ContentSerializer]
        internal BoundingSphere boundingSphere;

        [ContentSerializer]
        internal VertexPositionNormalTexture[] vertices;
        [ContentSerializer]
        internal short[][] indices;

        private VertexBuffer vertexBuffer;
        private IndexBuffer[] indexBuffer;

        public override void  Awake()
        {
 	        base.Awake();

            vertexBuffer = new VertexBuffer(Application.screenManager.GraphicsDevice, vertices.GetType().GetElementType(), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);
            indexBuffer = new IndexBuffer[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                indexBuffer[i] = new IndexBuffer(Application.screenManager.GraphicsDevice, IndexElementSize.SixteenBits, indices[i].Length, BufferUsage.WriteOnly);
                indexBuffer[i].SetData(indices[i]);
            }
        }

        internal void AddMesh(Mesh m, Matrix transform)
        {
            // TODO: Add a test for too many verts
            int vertexOffset = 0;
            if (vertices == null)
            {
                vertices = new VertexPositionNormalTexture[m.vertices.Length];
            }
            else
            {
                VertexPositionNormalTexture[] oldVerts = vertices;
                vertices = new VertexPositionNormalTexture[oldVerts.Length + m.vertices.Length];
                oldVerts.CopyTo(vertices, 0);
                vertexOffset = oldVerts.Length;
            }
            for (int i = 0; i < m.vertices.Length; i++)
            {                
                vertices[vertexOffset + i].Position = Microsoft.Xna.Framework.Vector3.Transform(m.vertices[i], transform);
                vertices[vertexOffset + i].Normal = Microsoft.Xna.Framework.Vector3.Normalize(Microsoft.Xna.Framework.Vector3.TransformNormal(m.normals[i], transform));
                vertices[vertexOffset + i].TextureCoordinate = m.uv[i];
            }
            if (indices == null)
            {
                indices = new short[m.subMeshCount][];
            }
            int indexOffset = 0;
            for (int i = 0; i < m.subMeshCount; i++)
            {
                short[] tris = m.GetTriangles(i);
                if (indices[i] == null)
                {
                    indices[i] = (short[])tris.Clone();
                }
                else
                {
                    short[] newTris = new short[indices[i].Length + tris.Length];
                    indices[i].CopyTo(newTris, 0);
                    indexOffset = indices[i].Length;
                    for (int t = 0; t < tris.Length; t++)
                    {
                        newTris[indexOffset + t] = (short)(tris[t] + vertexOffset);
                    }
                    indices[i] = newTris;
                }
            }
        }

        public override int Draw(GraphicsDevice device, Camera cam)
        {
            if (cam.DoFrustumCulling(ref boundingSphere))
            {
#if DEBUG
                if (Camera.logRenderCalls)
                {
                    Debug.LogFormat("VP cull static batch {0} with radius {1} pos {2} cam {3} at {4}", gameObject, boundingSphere.Radius, transform.position, cam.gameObject, cam.transform.position);
                }
#endif
                return 0;
            }

#if DEBUG
            if (Camera.logRenderCalls)
            {
                Debug.LogFormat("Static batch: {0} on {1}", gameObject, cam.gameObject);
            }
#endif

            cam.BasicEffect.World = Matrix.Identity;
            cam.BasicEffect.VertexColorEnabled = false;
            cam.BasicEffect.LightingEnabled = Light.HasLights;

            material.SetTextureState(cam.BasicEffect);
            material.SetBlendState(device);

            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer[0];

            foreach (EffectPass pass in cam.BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    vertexBuffer.VertexCount,
                    0,
                    indexBuffer[0].IndexCount / 3
                );
            }
            return 1;
        }
    }
}
