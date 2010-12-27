using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Components
{
    public class SpriteRenderer : Renderer, IRenderable, Interfaces.IUpdateable
    {
        #region Content properties
        [ContentSerializer(Optional=true)]
        public string Texture;
        #endregion

        [ContentSerializerIgnore]
        private Texture2D _texture;

        public Texture2D texture
        {
            get
            {
                return _texture;
            }
            set
            {
                _texture = value;

                if (_texture != null)
                {
                    bounds = _texture.Bounds;
                }
            }
        }

        public Vector2 Position = Vector2.zero;
        public Rectangle bounds = Rectangle.Empty;
        public Vector2 Origin = Vector2.zero;
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
            if (texture == null)
            {
                texture = ContentHelper.GetTexture(Texture);
            }

            if(texture != null){
                bounds = texture.Bounds;
            }

            if (material == null)
            {
                material = new Material();
            }
        }

        #region IUpdateable Members

        public void Update()
        {
            Position.x = transform.localPosition.x;
            Position.y = transform.localPosition.y;
        }

        #endregion

        #region IRenderable Members
        public override void Draw(SpriteBatch batch)
        {

            if (texture == null)
            {
                return;
            }

            batch.Begin();
            batch.Draw(texture, Position, bounds, material.color, transform.eulerAngles.y, Origin, Scale, Effects, LayerDepth);
            batch.End();
        }
        #endregion
    }
}
