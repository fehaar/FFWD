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
        internal VertexPositionTexture[] vertices;
        [ContentSerializer]
        internal short[] indices;

        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

        public override void  Awake()
        {
 	        base.Awake();

            vertexBuffer = new VertexBuffer(Application.screenManager.GraphicsDevice, typeof(VertexPositionTexture), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);
            indexBuffer = new IndexBuffer(Application.screenManager.GraphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
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
            return 1;
        }
    }
}
