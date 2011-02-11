using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Components
{
    public class ParticleRenderer : Renderer
    {
        public float lengthScale;
        public float velocityScale;
        public float maxParticleSize;
        public Vector3 uvAnimation;

        private static BasicEffect effect;
        private ParticleEmitter emitter;
        private VertexPositionColorTexture[] vertices;
        private short[] triangles;

        private DynamicVertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

        [ContentSerializerIgnore]
        public Rectangle ParticleBounds;

        public override void Awake()
        {
            emitter = gameObject.GetComponent<ParticleEmitter>();
            CreateBuffers();
        }

        public override void Start()
        {
            CreateBuffers();
        }

        private void CreateBuffers()
        {
            if (vertexBuffer == null && (emitter.particles != null))
            {
                vertexBuffer = new DynamicVertexBuffer(Application.screenManager.GraphicsDevice, typeof(VertexPositionColorTexture), emitter.particles.Length * 4, BufferUsage.WriteOnly);
                indexBuffer = new IndexBuffer(Application.screenManager.GraphicsDevice, IndexElementSize.SixteenBits, emitter.particles.Length * 6, BufferUsage.WriteOnly);

                triangles = new short[emitter.particles.Length * 6];
                int j = 0;
                for (int i = 0; i < emitter.particles.Length * 6; i += 6, j += 4)
                {
                    triangles[i] = (short)j;
                    triangles[i + 1] = (short)(j + 2);
                    triangles[i + 2] = (short)(j + 1);
                    triangles[i + 3] = (short)(j + 1);
                    triangles[i + 4] = (short)(j + 2);
                    triangles[i + 5] = (short)(j + 3);
                }
                indexBuffer.SetData(triangles, 0, triangles.Length);
            }
        }

        public Rectangle GetSourceRect()
        {
            return new Rectangle(0, 0, material.texture.Width, material.texture.Height);
        }

        public bool IsVisible(Viewport viewport)
        {
            if (ParticleBounds.Left == Int32.MinValue)
            {
                return false;
            }

            bool visible = true;
            viewport.Bounds.Intersects(ref ParticleBounds, out visible);
            return visible;
        }

        public override int Draw(GraphicsDevice device, Camera cam)
        {
#if DEBUG
            Application.particleDrawTimer.Start();
#endif
            if (emitter.particles == null || emitter.particleCount == 0) return 0;

            if (effect == null)
            {
                effect = new BasicEffect(device);
            }

            effect.World = Matrix.Identity;
            effect.View = cam.view;
            effect.Projection = cam.projectionMatrix;
            if (materials != null && materials.Length > 0 && materials[0].texture != null)
            {
                effect.TextureEnabled = true;
                effect.Texture = materials[0].texture;
                materials[0].SetBlendState(device);
            }
            effect.VertexColorEnabled = true;

            if (vertices == null)
	        {
                vertices = new VertexPositionColorTexture[emitter.particles.Length * 4];
	        }

            int particlesRendered = 0;
            for (int i = 0; i < emitter.particles.Length && particlesRendered < emitter.particleCount; i++)
            {
                if (emitter.particles[i].Energy > 0)
                {
                    RenderParticle(particlesRendered * 4, particlesRendered * 6, ref emitter.particles[i]);
                    particlesRendered++;
                }
            }
            vertexBuffer.SetData(vertices, 0, particlesRendered * 4);

            device.Indices = indexBuffer;
            device.SetVertexBuffer(vertexBuffer);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    particlesRendered * 4,
                    0,
                    particlesRendered * 2
                );
            }
            device.Indices = null;
            device.SetVertexBuffer(null);

#if DEBUG
            Application.particleDrawTimer.Stop();
#endif
            return 1;
        }

        private void RenderParticle(int vertexIndex, int triangleIndex, ref Particle particle)
        {
            Vector3 pos = particle.Position;
            float size = particle.Size / 2;
            if (!emitter.useWorldSpace)
            {
                pos += transform.position;
            }
            // NOTE: Implies that y is depth
            vertices[vertexIndex].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 1);
            vertices[vertexIndex].Position = new Microsoft.Xna.Framework.Vector3(pos.x - size, pos.y, pos.z + size);
            vertices[vertexIndex].Color = particle.Color;

            vertices[vertexIndex + 1].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 0);
            vertices[vertexIndex + 1].Position = new Microsoft.Xna.Framework.Vector3(pos.x - size, pos.y, pos.z - size);
            vertices[vertexIndex + 1].Color = particle.Color;

            vertices[vertexIndex + 2].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 1);
            vertices[vertexIndex + 2].Position = new Microsoft.Xna.Framework.Vector3(pos.x + size, pos.y, pos.z + size);
            vertices[vertexIndex + 2].Color = particle.Color;

            vertices[vertexIndex + 3].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 0);
            vertices[vertexIndex + 3].Position = new Microsoft.Xna.Framework.Vector3(pos.x + size, pos.y, pos.z - size);
            vertices[vertexIndex + 3].Color = particle.Color;
        }

    }
}
