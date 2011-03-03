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
        public short[] triangles;
        public float[] vertices;
        [ContentSerializer(Optional=true)]
        public float[] normals;
        public float[] uv;

        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

        public override void Awake()
        {
            base.Awake();
            if (vertexBuffer == null)
            {
                VertexPositionTexture[] buffer = new VertexPositionTexture[vertices.Length / 3];
                for (int i = 0; i < vertices.Length / 3; i++)
                {
                    int vertexIndex = i * 3;
                    int texCoordIndex = i * 2;
                    buffer[i] = new VertexPositionTexture(
                        new Microsoft.Xna.Framework.Vector3(vertices[vertexIndex], -vertices[vertexIndex + 1], vertices[vertexIndex + 2]),
                        new Microsoft.Xna.Framework.Vector2(uv[texCoordIndex], 1 - uv[texCoordIndex + 1])
                        );
                }

                vertexBuffer = new VertexBuffer(Application.screenManager.GraphicsDevice, typeof(VertexPositionTexture), buffer.Length, BufferUsage.WriteOnly);
                vertexBuffer.SetData(buffer);
                indexBuffer = new IndexBuffer(Application.screenManager.GraphicsDevice, IndexElementSize.SixteenBits, triangles.Length, BufferUsage.WriteOnly);
                indexBuffer.SetData(triangles);

                triangles = null;
                vertices = null;
                normals = null;
                uv = null;
            }
        }

        public override int Draw(GraphicsDevice device, Camera cam)
        {
            //RasterizerState oldrasterizerState = device.RasterizerState;
            //RasterizerState rasterizerState = new RasterizerState();
            //rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            //device.RasterizerState = rasterizerState;

#if DEBUG
            if (Camera.logRenderCalls)
            {
                Debug.LogFormat("Static batch: {0} on {1}", gameObject, cam.gameObject);
            }
#endif

            cam.BasicEffect.World = Matrix.Identity;
            cam.BasicEffect.View = cam.view;
            cam.BasicEffect.Projection = cam.projectionMatrix;
            cam.BasicEffect.VertexColorEnabled = false;

            material.SetTextureState(cam.BasicEffect);
            material.SetBlendState(device);

            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;

            foreach (EffectPass pass in cam.BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    vertexBuffer.VertexCount,
                    0,
                    indexBuffer.IndexCount / 3
                );
            }

            //device.RasterizerState = oldrasterizerState;
            return 1;
        }
    }
}
