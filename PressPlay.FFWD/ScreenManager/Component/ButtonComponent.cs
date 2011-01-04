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
    public class ButtonComponent : Component, Interfaces.IUpdateable
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
        private bool isTapped = false;
        #endregion

        #region properties
        public Rectangle bounds
        {
            get
            {
                return new Rectangle((int)transform.position.x, (int)transform.position.y, _bounds.Width, _bounds.Height);
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
            if (!isTapped)
            {
                onSelectTime = Time.time;
                isTapped = true;
            }
            ((SpriteRenderer)gameObject.renderer).texture = activeTexture;
        }

        public ButtonComponent() : base()
        {

        }

        public ButtonComponent(string normalTextureSrc, string activeTextureSrc) : this()
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
                gameObject.renderer.material = new Material();
                gameObject.renderer.material.renderQueue = 1000;
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

            if (gameObject.renderer == null)
            {
                gameObject.AddComponent(new SpriteRenderer());
            }

            ((SpriteRenderer)gameObject.renderer).texture = normalTexture;
        }

        public void OnTweenUpdate(float value)
        {
            Debug.Log("OnTweenUpdate: "+value);
        }

        #region IUpdateable Members

        public void Update()
        {

            if (isTapped && Time.time > onSelectTime + delayBeforeActivation)
            {
                if (Selected != null)
                {
                    Selected(this, new PlayerIndexEventArgs(PlayerIndex.One));
                }

                isTapped = false;
            }
        }

        #endregion
    }
}
