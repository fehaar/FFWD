using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD.ScreenManager
{
    public class Sprite : Component, Interfaces.IUpdateable
    {

        #region Content properties
        [ContentSerializer(Optional = true)]
        public string Texture;
        #endregion

        #region fields
        private Rectangle _bounds;

        private Texture2D texture;
        #endregion

        #region properties
        public Rectangle bounds
        {
            get
            {
                return new Rectangle((int)transform.position.X, (int)transform.position.Y, _bounds.Width, _bounds.Height);
            }
        }
        #endregion

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;

        public Sprite() : base()
        {

        }

        public Sprite(string textureSrc)
            : this()
        {
            Texture = textureSrc;
        }

        public override void Awake()
        {
            base.Awake();
            ContentHelper.LoadTexture(Texture);

            if (gameObject.renderer == null)
            {
                gameObject.AddComponent(new SpriteRenderer());
            }
        }

        public override void Start()
        {
            base.Start();
            texture = ContentHelper.GetTexture(Texture);

            if (texture != null)
            {
                _bounds = texture.Bounds;
            }

            if (gameObject.renderer == null)
            {
                gameObject.AddComponent(new SpriteRenderer());
            }

            ((SpriteRenderer)gameObject.renderer).texture = texture;
        }

        #region IUpdateable Members

        public void Update()
        {

        }

        #endregion
    }
}
