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
        internal VertexPositionNormalTexture[] buffer { private get; set; }

        public short[] triangles;
        public float[] vertices;
        public float[] normals;
        public float[] uv;

        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private BasicEffect effect;

        public override void Awake()
        {
            base.Awake();
            if (vertexBuffer == null)
            {
                buffer = new VertexPositionNormalTexture[vertices.Length / 3];
                for (int i = 0; i < vertices.Length / 3; i++)
                {
                    int vertexIndex = i * 3;
                    int texCoordIndex = i * 2;
                    buffer[i] = new VertexPositionNormalTexture(
                        new Microsoft.Xna.Framework.Vector3(vertices[vertexIndex], -vertices[vertexIndex + 1], vertices[vertexIndex + 2]),
                        new Microsoft.Xna.Framework.Vector3(normals[vertexIndex], normals[vertexIndex + 1], normals[vertexIndex + 2]),
                        new Microsoft.Xna.Framework.Vector2(uv[texCoordIndex], 1 - uv[texCoordIndex + 1])
                        );
                }

                //vertexBuffer = new VertexBuffer(Application.screenManager.GraphicsDevice, typeof(VertexPositionNormalTexture), buffer.Length, BufferUsage.WriteOnly);
                //vertexBuffer.SetData(buffer);
                //indexBuffer = new IndexBuffer(Application.screenManager.GraphicsDevice, IndexElementSize.SixteenBits, triangles.Length, BufferUsage.WriteOnly);
                //indexBuffer.SetData(triangles);
            }
            if (effect == null)
            {
                effect = new BasicEffect(Application.screenManager.GraphicsDevice);
            }
        }

        public override void Draw(GraphicsDevice device, Camera cam)
        {
            RasterizerState oldrasterizerState = device.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            device.RasterizerState = rasterizerState;

            effect.World = Matrix.Identity;
            effect.View = cam.View();
            effect.Projection = cam.projectionMatrix;
            if (materials != null && materials.Length > 0 && materials[0].texture != null)
            {
                effect.TextureEnabled = true;
                effect.Texture = materials[0].texture;
                device.BlendState = materials[0].blendState;
            }
            effect.VertexColorEnabled = false;
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    buffer,
                    0,
                    buffer.Length,
                    triangles,
                    0,
                    triangles.Length / 3
                );
                //device.DrawIndexedPrimitives(
                //    PrimitiveType.TriangleList,
                //    0,
                //    0,
                //    vertexBuffer.VertexCount,
                //    0,
                //    indexBuffer.IndexCount
                //);
            }

            device.RasterizerState = oldrasterizerState;
            
        }
    }
}
