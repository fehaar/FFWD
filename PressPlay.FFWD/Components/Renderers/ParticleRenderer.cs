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

        [ContentSerializerIgnore]
        public bool doViewportCulling = false;

        private ParticleEmitter emitter;
        private VertexPositionColorTexture[] vertices;
        private short[] triangles;
        
        [ContentSerializerIgnore]
        public Rectangle ParticleBounds;

        Microsoft.Xna.Framework.Vector3 camPosition = new Microsoft.Xna.Framework.Vector3();
        Microsoft.Xna.Framework.Vector3 camUpVector = new Microsoft.Xna.Framework.Vector3();
        Microsoft.Xna.Framework.Vector3 camForwardVector = new Microsoft.Xna.Framework.Vector3();
        Matrix m = new Matrix();

        public override void Awake()
        {
            base.Awake();

            emitter = gameObject.GetComponent<ParticleEmitter>();
            emitter.onParticleCountChanged = ParticleCountChanged;
            ParticleCountChanged();
        }

        void ParticleCountChanged()
        {
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
            return new Rectangle(0, 0, material.mainTexture.Width, material.mainTexture.Height);
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

            camPosition = (Microsoft.Xna.Framework.Vector3)cam.transform.position;
            camUpVector = (Microsoft.Xna.Framework.Vector3)cam.transform.up;
            camForwardVector = (Microsoft.Xna.Framework.Vector3)cam.transform.forward;

            cam.BasicEffect.World = Matrix.Identity;
            cam.BasicEffect.VertexColorEnabled = true;

            material.SetTextureState(cam.BasicEffect);
            material.SetBlendState(device);

            int particlesRendered = 0;
            for (int i = 0; i < emitter.particles.Length && particlesRendered < emitter.particleCount; i++)
            {
                Particle part = emitter.particles[i];
                if (part.Energy > 0)
                {
                    if (RenderParticle(cam, particlesRendered * 4, particlesRendered * 6, ref part))
                    {
                        particlesRendered++;
                    }
                }
            }

            if (particlesRendered == 0)
            {
                return 0;
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

        private bool RenderParticle(Camera cam, int vertexIndex, int triangleIndex, ref Particle particle)
        {
            float size = particle.Size / 2;
            if (size <= 0)
            {
                return false;
            }

            Microsoft.Xna.Framework.Vector3 pos = particle.Position;
            if (!emitter.useWorldSpace)
            {
                pos += (Microsoft.Xna.Framework.Vector3)transform.position;
            }

            if (doViewportCulling)
            {
                BoundingSphere sphere = new BoundingSphere(pos, size);
                if (cam.DoFrustumCulling(ref sphere))
                {
                    return false;
                }
            }
            
            vertices[vertexIndex].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(particle.TextureOffset.x, particle.TextureOffset.y + particle.TextureScale.y);
            vertices[vertexIndex].Color = particle.Color;
            vertices[vertexIndex + 1].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(particle.TextureOffset.x, particle.TextureOffset.y);
            vertices[vertexIndex + 1].Color = particle.Color;
            vertices[vertexIndex + 2].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(particle.TextureOffset.x + particle.TextureScale.x, particle.TextureOffset.y + particle.TextureScale.y);
            vertices[vertexIndex + 2].Color = particle.Color;
            vertices[vertexIndex + 3].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(particle.TextureOffset.x + particle.TextureScale.x, particle.TextureOffset.y);
            vertices[vertexIndex + 3].Color = particle.Color;

            //XZ plane particles
            //if (particle.Rotation != 0)
            //{
            //    Matrix m = Matrix.CreateRotationY(particle.Rotation);
            //    Microsoft.Xna.Framework.Vector3 p = new Microsoft.Xna.Framework.Vector3(size, vertexIndex * 0.0001f, size);
            //    Microsoft.Xna.Framework.Vector3.Transform(ref p, ref m, out p);
            //    vertices[vertexIndex].Position = pos + new Microsoft.Xna.Framework.Vector3(-p.Z, vertexIndex * 0.0001f, p.X);
            //    vertices[vertexIndex + 1].Position = pos + new Microsoft.Xna.Framework.Vector3(-p.X, vertexIndex * 0.0001f, -p.Z);
            //    vertices[vertexIndex + 2].Position = pos + new Microsoft.Xna.Framework.Vector3(p.X, vertexIndex * 0.0001f, p.Z);
            //    vertices[vertexIndex + 3].Position = pos + new Microsoft.Xna.Framework.Vector3(p.Z, vertexIndex * 0.0001f, -p.X);
            //}
            //else
            //{
            //    vertices[vertexIndex].Position = pos + new Microsoft.Xna.Framework.Vector3(-size, vertexIndex * 0.0001f, size);
            //    vertices[vertexIndex + 1].Position = pos + new Microsoft.Xna.Framework.Vector3(-size, vertexIndex * 0.0001f, -size);
            //    vertices[vertexIndex + 2].Position = pos + new Microsoft.Xna.Framework.Vector3(size, vertexIndex * 0.0001f, size);
            //    vertices[vertexIndex + 3].Position = pos + new Microsoft.Xna.Framework.Vector3(size, vertexIndex * 0.0001f, -size);
            //}

            //rotated particles
            if (particle.Rotation == 0)
            {
                Microsoft.Xna.Framework.Vector3 p = particle.Position;

                Matrix.CreateBillboard(
                    ref p,
                    ref camPosition,
                    ref camUpVector,
                    camForwardVector,
                    out m
                    );
                p = new Microsoft.Xna.Framework.Vector3(size, vertexIndex * 0.0001f, size);
                Microsoft.Xna.Framework.Vector3.Transform(ref p, ref m, out p);
                
                vertices[vertexIndex].Position = pos + new Microsoft.Xna.Framework.Vector3(-p.Z, vertexIndex * 0.0001f, p.X);
                vertices[vertexIndex + 1].Position = pos + new Microsoft.Xna.Framework.Vector3(-p.X, vertexIndex * 0.0001f, -p.Z);
                vertices[vertexIndex + 2].Position = pos + new Microsoft.Xna.Framework.Vector3(p.X, vertexIndex * 0.0001f, p.Z);
                vertices[vertexIndex + 3].Position = pos + new Microsoft.Xna.Framework.Vector3(p.Z, vertexIndex * 0.0001f, -p.X);
            }
            else
            {
                vertices[vertexIndex].Position = pos + new Microsoft.Xna.Framework.Vector3(-size, vertexIndex * 0.0001f, size);
                vertices[vertexIndex + 1].Position = pos + new Microsoft.Xna.Framework.Vector3(-size, vertexIndex * 0.0001f, -size);
                vertices[vertexIndex + 2].Position = pos + new Microsoft.Xna.Framework.Vector3(size, vertexIndex * 0.0001f, size);
                vertices[vertexIndex + 3].Position = pos + new Microsoft.Xna.Framework.Vector3(size, vertexIndex * 0.0001f, -size);
            }
            return true;
        }

    }
}
