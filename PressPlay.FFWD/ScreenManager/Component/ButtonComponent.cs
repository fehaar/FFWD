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
    public class ButtonComponent : Component
    {

        #region Content properties
        [ContentSerializer(Optional = true)]
        public string OnNormalTexture;
        public string OnActiveTexture;
        #endregion

        #region fields
        private Rectangle _bounds;

        private Texture2D normalTexture;
        private Texture2D activeTexture;

        public float delayBeforeActivation = 0.1f;
        private float onSelectTime;

        #endregion

        #region properties
        public Rectangle bounds
        {
            get
            {
                return _bounds;
            }
        }
        #endregion

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));

            //onSelectTime = 

            //((SpriteRenderer)gameObject.renderer).texture = activeTexture;
        }

        public ButtonComponent()
        {

        }

        public ButtonComponent(string normalTextureSrc, string activeTextureSrc)
        {
            OnNormalTexture = normalTextureSrc;
            OnActiveTexture = activeTextureSrc;
        }

        public override void Awake()
        {
            base.Awake();
            ContentHelper.LoadTexture(OnNormalTexture);
            ContentHelper.LoadTexture(OnActiveTexture);

            if (gameObject.renderer == null)
            {
                gameObject.AddComponent(new SpriteRenderer());
            }
        }

        public override void Start()
        {
            base.Start();
            normalTexture = ContentHelper.GetTexture(OnNormalTexture);
            activeTexture = ContentHelper.GetTexture(OnActiveTexture);

            if (normalTexture != null)
            {
                _bounds = normalTexture.Bounds;
            }

            ((SpriteRenderer)gameObject.renderer).texture = normalTexture;
        }
    }
}
