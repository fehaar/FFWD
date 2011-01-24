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
        private VertexPositionTexture[] vertices;
        private short[] triangles;
        
        [ContentSerializerIgnore]
        public Rectangle ParticleBounds;

        //private Texture2D colorMultipliedTexture;
        //private Texture2D[] colorMultipliedTextures;
        
        public override void Awake()
        {
            base.Awake();

            emitter = gameObject.GetComponent<ParticleEmitter>();
            //animator = gameObject.GetComponent<ParticleAnimator>();
            //if (TileScale == Vector2.zero)
            //    TileScale = Vector2.one;

            //if (TileOffset == Vector2.zero)
            //    TileOffset = Vector2.one;
        }


        public override void Start()
        {
            base.Start();
            // TODO: Make pre colored textures!
            //colorMultipliedTexture = ContentHelper.GetColoredTexture(animator.ColorAnimation0, Texture);
            //if (animator.DoesAnimateColor)
            //{
            //    Color[] colors = animator.GetAnimationColors();
            //    colorMultipliedTextures = new Texture2D[colors.Length];
            //    for (int i = 0; i < colors.Length; i++)
            //    {
            //        colorMultipliedTextures[i] = ContentHelper.GetColoredTexture(colors[i], Texture);
            //    }
            //}
        }

        public Rectangle GetSourceRect()
        {
            //if (colorMultipliedTexture == null)
            //{
            //    colorMultipliedTexture = ContentHelper.GetColoredTexture(animator.ColorAnimation0, Texture);
            //}
            //return new Rectangle((int)((float)colorMultipliedTexture.Width * material.mainTextureOffset.x + 1),
            //                    (int)((float)colorMultipliedTexture.Height * ((1 - material.mainTextureOffset.y - material.mainTextureOffset.y) + 1)),
            //                    (int)Math.Ceiling((float)colorMultipliedTexture.Width * TileScale.x - 2),
            //                    (int)Math.Ceiling((float)colorMultipliedTexture.Height * TileScale.y - 2));

            return new Rectangle(0, 0, material.texture.Width, material.texture.Height);
        }

        //private Texture2D GetTexture(int index)
        //{
        //    if (!animator.doesAnimateColor)
        //        return colorMultipliedTexture;

        //    float colorScale = 1 - (emitter.particles[index].Energy / emitter.particles[index].StartingEnergy);
        //    int colorIndex = (int)(colorMultipliedTextures.Length * colorScale);
        //    if (colorIndex == colorMultipliedTextures.Length)
        //        colorIndex -= 1;

        //    return colorMultipliedTextures[colorIndex];
        //}


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
            if (emitter.particles == null || emitter.particlesInUse == 0) return;

            if (effect == null)
            {
                effect = new BasicEffect(device);
            }

            RasterizerState oldrasterizerState = device.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
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

            if (vertices == null)
	        {
                vertices = new VertexPositionTexture[emitter.particles.Length * 4];
                triangles = new short[emitter.particles.Length * 6];
	        }

            int particlesRendered = 0;
            for (int i = 0; i < emitter.particles.Length && particlesRendered < emitter.particlesInUse; i++)
            {
                if (emitter.particles[i].Energy > 0)
                {
                    RenderParticle(particlesRendered * 4, particlesRendered * 6, ref emitter.particles[i]);
                    particlesRendered++;
                    if (particlesRendered == 1)
                    {
                        Debug.Display("First part at " + ToString(), emitter.particles[i].Position);
                    }
                }
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives<VertexPositionTexture>(
                    PrimitiveType.TriangleList,
                    vertices,
                    0,
                    particlesRendered * 4,
                    triangles,
                    0,
                    particlesRendered * 2
                );
            }

            device.RasterizerState = oldrasterizerState;
        }

        private void RenderParticle(int vertexIndex, int triangleIndex, ref Particle particle)
        {
            Vector3 pos = particle.Position;
            float size = particle.Size / 2;
            if (!emitter.useWorldSpace)
            {
                pos += transform.position;
            }
            vertices[vertexIndex] = new VertexPositionTexture() { 
                TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 1),
                Position = pos + (Vector3)new Vector2(-size, size)
            };
            vertices[vertexIndex + 1] = new VertexPositionTexture()
            {
                TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 0),
                Position = pos + (Vector3)new Vector2(-size, -size)
            };
            vertices[vertexIndex + 2] = new VertexPositionTexture()
            {
                TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 1),
                Position = pos + (Vector3)new Vector2(size, size)
            };
            vertices[vertexIndex + 3] = new VertexPositionTexture()
            {
                TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 0),
                Position = pos + (Vector3)new Vector2(size, -size)
            };
            triangles[triangleIndex] = (short)vertexIndex;
            triangles[triangleIndex + 1] = (short)(vertexIndex + 1);
            triangles[triangleIndex + 2] = (short)(vertexIndex + 2);
            triangles[triangleIndex + 3] = (short)(vertexIndex + 2);
            triangles[triangleIndex + 4] = (short)(vertexIndex + 1);
            triangles[triangleIndex + 5] = (short)(vertexIndex + 3);
        }

        //public float GetLayer()
        //{
        //    if (drawLayer < 0)
        //        drawLayer = (.5f - (.5f * (float)Layer * 0.001f));
        //    return drawLayer;
        //}
        
    }
}
