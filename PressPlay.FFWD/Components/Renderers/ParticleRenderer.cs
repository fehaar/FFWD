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

        private BasicEffect effect;
        private ParticleEmitter emitter;
        private VertexPositionColorTexture[] vertices;
        private short[] triangles;
        
        [ContentSerializerIgnore]
        public Rectangle ParticleBounds;

        public override void Awake()
        {
            base.Awake();

            emitter = gameObject.GetComponent<ParticleEmitter>();
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

        public override void Draw(GraphicsDevice device, Camera cam)
        {
            if (emitter.particles == null || emitter.particleCount == 0) return;

            if (effect == null)
            {
                effect = new BasicEffect(device);
            }

            effect.World = Matrix.Identity;
            effect.View = cam.View();
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
                triangles = new short[emitter.particles.Length * 6];
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

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
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
        }

        private void RenderParticle(int vertexIndex, int triangleIndex, ref Particle particle)
        {
            Vector3 pos = particle.Position;
            float size = particle.Size / 2;
            if (!emitter.useWorldSpace)
            {
                pos += transform.position;
            }
            vertices[vertexIndex].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 1);
            vertices[vertexIndex].Position = pos + (Vector3)new Vector2(-size, size);
            vertices[vertexIndex].Color = particle.Color;

            vertices[vertexIndex + 1].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 0);
            vertices[vertexIndex + 1].Position = pos + (Vector3)new Vector2(-size, -size);
            vertices[vertexIndex + 1].Color = particle.Color;

            vertices[vertexIndex + 2].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 1);
            vertices[vertexIndex + 2].Position = pos + (Vector3)new Vector2(size, size);
            vertices[vertexIndex + 2].Color = particle.Color;

            vertices[vertexIndex + 3].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 0);
            vertices[vertexIndex + 3].Position = pos + (Vector3)new Vector2(size, -size);
            vertices[vertexIndex + 3].Color = particle.Color;

            triangles[triangleIndex] = (short)vertexIndex;
            triangles[triangleIndex + 1] = (short)(vertexIndex + 2);
            triangles[triangleIndex + 2] = (short)(vertexIndex + 1);
            triangles[triangleIndex + 3] = (short)(vertexIndex + 1);
            triangles[triangleIndex + 4] = (short)(vertexIndex + 2);
            triangles[triangleIndex + 5] = (short)(vertexIndex + 3);
        }

    }
}
