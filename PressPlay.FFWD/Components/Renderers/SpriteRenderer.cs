using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Animation;
using PressPlay.FFWD.Interfaces;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Components
{
    public class SpriteRenderer : Component, IRenderable, Interfaces.IUpdateable
    {
        #region Content properties
        [ContentSerializer(Optional=true)]
        public string Texture;
        #endregion

        [ContentSerializerIgnore]
        public Texture2D texture;

        public Vector2 Position = Vector2.Zero;
        public Rectangle? SourceRect = null;
        public Color Color = Color.White;
        public Vector2 Origin = Vector2.Zero;
        public float Scale = 1f;
        public SpriteEffects Effects = SpriteEffects.None;
        public float LayerDepth = 0;

        public SpriteRenderer()
        {

        }

        public SpriteRenderer(string texture)
        {
            this.Texture = texture;
        }

        public override void Awake()
        {
            base.Awake();
            ContentHelper.LoadTexture(Texture);
        }

        public override void Start()
        {
            base.Start();
            texture = ContentHelper.GetTexture(Texture);

            if(texture != null){
                SourceRect = texture.Bounds;
            }
        }

        #region IUpdateable Members

        public void Update(GameTime gameTime)
        {
            Position.X = transform.localPosition.X;
            Position.Y = transform.localPosition.Y;
        }

        #endregion

        #region IRenderable Members
        public void Draw(SpriteBatch batch)
        {

            if (texture == null)
            {
                return;
            }

            batch.Begin();
            batch.Draw(texture, Position, SourceRect, Color, transform.angleY, Origin, Scale, Effects, LayerDepth);
            batch.End();
        }
        #endregion
    }
}
