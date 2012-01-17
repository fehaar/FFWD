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
            if (materials.Length > indexBuffer.Length)
            {
                throw new Exception("The static batch renderer does not have enough submeshes for all materials!");
            }
        }

        internal bool AddMesh(Mesh m, Matrix transform)
        {
            if (m == null)
            {
                throw new Exception("Trying to add null mesh to static batch");
            }
            if (m._vertices == null)
            {
                throw new Exception("Mesh " + m.name + " does not have any vertices and is being added to a static batch");
            }

            int vertexOffset = 0;
            if (vertices == null)
            {
                vertices = new VertexPositionNormalTexture[m._vertices.Length];
            }
            else
            {
                if (vertices.Length + m._vertices.Length > UInt16.MaxValue)
                {
                    return false;
                }

                VertexPositionNormalTexture[] oldVerts = vertices;
                vertices = new VertexPositionNormalTexture[oldVerts.Length + m._vertices.Length];
                oldVerts.CopyTo(vertices, 0);
                vertexOffset = oldVerts.Length;
            }
            for (int i = 0; i < m._vertices.Length; i++)
            {                
                vertices[vertexOffset + i].Position = Microsoft.Xna.Framework.Vector3.Transform(m._vertices[i], transform);
                vertices[vertexOffset + i].Normal = Microsoft.Xna.Framework.Vector3.Normalize(Microsoft.Xna.Framework.Vector3.TransformNormal(m._normals[i], transform));
                vertices[vertexOffset + i].TextureCoordinate = m._uv[i];
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
            return true;
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

            device.SetVertexBuffer(vertexBuffer);
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetTextureState(cam.BasicEffect);
                materials[i].SetBlendState(device);
                device.Indices = indexBuffer[i];
                foreach (EffectPass pass in cam.BasicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        0,
                        0,
                        vertexBuffer.VertexCount,
                        0,
                        indexBuffer[i].IndexCount / 3
                    );
                }
            }
            return indexBuffer.Length;
        }
    }
}
