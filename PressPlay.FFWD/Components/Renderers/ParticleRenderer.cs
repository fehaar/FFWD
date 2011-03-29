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

        private ParticleEmitter emitter;
        private VertexPositionColorTexture[] vertices;
        private short[] triangles;
        
        [ContentSerializerIgnore]
        public Rectangle ParticleBounds;

        public override void Awake()
        {
            base.Awake();

            emitter = gameObject.GetComponent<ParticleEmitter>();
            vertices = new VertexPositionColorTexture[emitter.particlesToAllocate() * 4];

            // Create all triangles as they will not change
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
            Application.particleDraws++;
#endif
            if (emitter.particles == null || emitter.particleCount == 0) return 0;

            cam.BasicEffect.World = Matrix.Identity;
            cam.BasicEffect.VertexColorEnabled = true;

            material.SetTextureState(cam.BasicEffect);
            material.SetBlendState(device);

            int particlesRendered = 0;
            for (int i = 0; i < emitter.particles.Length && particlesRendered < emitter.particleCount; i++)
            {
                if (emitter.particles[i].Energy > 0)
                {
                    RenderParticle(particlesRendered * 4, particlesRendered * 6, ref emitter.particles[i]);
                    particlesRendered++;
                }
            }

#if DEBUG
            if (Camera.logRenderCalls)
            {
                Debug.LogFormat("Particle: {0} on {1}. Count {2}", gameObject, cam.gameObject, particlesRendered);
            }
#endif

            foreach (EffectPass pass in cam.BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
                    PrimitiveType.TriangleList,
                    vertices,
                    0,
                    particlesRendered * 4,
                    triangles,
                    0,
                    particlesRendered * 2
                );
            }

#if DEBUG
            Application.particleDrawTimer.Stop();
#endif

            return 1;
        }

        private void RenderParticle(int vertexIndex, int triangleIndex, ref Particle particle)
        {
            Microsoft.Xna.Framework.Vector3 pos = particle.Position;
            float size = particle.Size / 2;
            if (!emitter.useWorldSpace)
            {
                pos += (Microsoft.Xna.Framework.Vector3)transform.position;
            }

            if (particle.Rotation != 0)
            {
                Matrix m = Matrix.CreateRotationY(particle.Rotation);

                vertices[vertexIndex].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 1);
                Microsoft.Xna.Framework.Vector3 p = new Microsoft.Xna.Framework.Vector3(-size, vertexIndex * 0.0001f, size);
                Microsoft.Xna.Framework.Vector3.Transform(ref p, ref m, out vertices[vertexIndex].Position);
                vertices[vertexIndex].Position += pos;
                vertices[vertexIndex].Color = particle.Color;

                vertices[vertexIndex + 1].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 0);
                p = new Microsoft.Xna.Framework.Vector3(-size, vertexIndex * 0.0001f, -size);
                Microsoft.Xna.Framework.Vector3.Transform(ref p, ref m, out vertices[vertexIndex + 1].Position);
                vertices[vertexIndex + 1].Position += pos;
                vertices[vertexIndex + 1].Color = particle.Color;

                vertices[vertexIndex + 2].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 1);
                p = new Microsoft.Xna.Framework.Vector3(size, vertexIndex * 0.0001f, size);
                Microsoft.Xna.Framework.Vector3.Transform(ref p, ref m, out vertices[vertexIndex + 2].Position);
                vertices[vertexIndex + 2].Position += pos;
                vertices[vertexIndex + 2].Color = particle.Color;

                vertices[vertexIndex + 3].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 0);
                p = new Microsoft.Xna.Framework.Vector3(size, vertexIndex * 0.0001f, -size);
                Microsoft.Xna.Framework.Vector3.Transform(ref p, ref m, out vertices[vertexIndex + 3].Position);
                vertices[vertexIndex + 3].Position += pos;
                vertices[vertexIndex + 3].Color = particle.Color;
            }
            else
            {
                vertices[vertexIndex].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 1);
                vertices[vertexIndex].Position = pos + new Microsoft.Xna.Framework.Vector3(-size, vertexIndex * 0.0001f, size);
                vertices[vertexIndex].Color = particle.Color;

                vertices[vertexIndex + 1].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 0);
                vertices[vertexIndex + 1].Position = pos + new Microsoft.Xna.Framework.Vector3(-size, vertexIndex * 0.0001f, -size);
                vertices[vertexIndex + 1].Color = particle.Color;

                vertices[vertexIndex + 2].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 1);
                vertices[vertexIndex + 2].Position = pos + new Microsoft.Xna.Framework.Vector3(size, vertexIndex * 0.0001f, size);
                vertices[vertexIndex + 2].Color = particle.Color;

                vertices[vertexIndex + 3].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 0);
                vertices[vertexIndex + 3].Position = pos + new Microsoft.Xna.Framework.Vector3(size, vertexIndex * 0.0001f, -size);
                vertices[vertexIndex + 3].Color = particle.Color;
            }

        }

    }
}
