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
        public int lengthScale;
        public int velocityScale;
        public float maxParticleSize;
        public Vector3 uvAnimation;

        private ParticleEmitter emitter;
        private ParticleAnimator animator;
        
        
        [ContentSerializerIgnore]
        public Rectangle ParticleBounds;

        private Texture2D colorMultipliedTexture;
        private Texture2D[] colorMultipliedTextures;
        
        public override void Awake()
        {
            base.Awake();

            emitter = gameObject.GetComponent<ParticleEmitter>();
            animator = gameObject.GetComponent<ParticleAnimator>();
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

        private Texture2D GetTexture(int index)
        {
            if (!animator.doesAnimateColor)
                return colorMultipliedTexture;

            float colorScale = 1 - (emitter.particles[index].Energy / emitter.particles[index].StartingEnergy);
            int colorIndex = (int)(colorMultipliedTextures.Length * colorScale);
            if (colorIndex == colorMultipliedTextures.Length)
                colorIndex -= 1;

            return colorMultipliedTextures[colorIndex];
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
            if (emitter.particles == null) return;
            //Rectangle sourceRect = GetSourceRect();
            //Vector2 drawScale;
            //Vector2 drawPosition;
            //float angle;
            //gameObject.GetDrawPositionalInfo(out drawPosition, out drawScale, out angle);
            //drawScale /= new Vector2(sourceRect.Width, sourceRect.Height);
            //Vector2 origin = new Vector2((float)sourceRect.Width / 2, (float)sourceRect.Height / 2);

            //Vector2 xBounds = new Vector2(Single.PositiveInfinity, Single.NegativeInfinity);
            //Vector2 yBounds = new Vector2(Single.PositiveInfinity, Single.NegativeInfinity);
            //bool isVisible = IsVisible();

            //for (int i = 0; i < emitter.particles.Length; i++)
            //{
            //    if (emitter.particles[i].Energy > 0)
            //    {
            //        Vector2 pos = emitter.particles[i].Position;
            //        Vector2 size = new Vector2(emitter.particles[i].Size, emitter.particles[i].Size) * 0.1f;
            //        pos.y *= -1;
            //        if (!emitter.useWorldSpace)
            //        {
            //            pos += drawPosition;
            //        }

            //        Texture2D tex = GetTexture(i);

            //        if (isVisible)
            //        {
            //            spriteBatch.Draw(tex, pos, sourceRect, Microsoft.Xna.Framework.Color.White, angle, origin, size, gameObject.FlipSprite ? SpriteEffects.FlipHorizontally : SpriteEffects.None, .5f - (.5f * (float)(Layer + i * 0.01) / 1000f));
            //        }

            //        size.x *= colorMultipliedTexture.Width;
            //        size.y *= colorMultipliedTexture.Height;
            //        if (xBounds.x > (pos.x - size.x))
            //        {
            //            xBounds.x = pos.x - size.x;
            //        }
            //        if (xBounds.y < (pos.x + size.x))
            //        {
            //            xBounds.y = pos.x + size.x;
            //        }
            //        if (yBounds.x > (pos.y - size.y))
            //        {
            //            yBounds.x = pos.y - size.y;
            //        }
            //        if (yBounds.y < (pos.y + size.y))
            //        {
            //            yBounds.y = pos.y + size.y;
            //        }
            //    }
            //}
            //ParticleBounds = new Rectangle((int)xBounds.x, (int)yBounds.x, (int)Math.Ceiling(xBounds.y - xBounds.x), (int)Math.Ceiling(yBounds.y - yBounds.x));
        }

        //public float GetLayer()
        //{
        //    if (drawLayer < 0)
        //        drawLayer = (.5f - (.5f * (float)Layer * 0.001f));
        //    return drawLayer;
        //}
        
    }
}
