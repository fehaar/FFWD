using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.U2X.Xna.Interfaces;

namespace PressPlay.U2X.Xna.Test.Core_framework
{
    internal class TestComponent : Component, IFixedUpdateable, IUpdateable, IRenderable
    {
        internal Action onAwake { get; set; }
        internal Action onStart { get; set; }
        internal Action onFixedUpdate { get; set; }
        internal Action onUpdate { get; set; }
        internal Action onDraw { get; set; }

        public override void Awake()
        {
            if (onAwake != null)
            {
                onAwake();
            }
        }

        public override void Start()
        {
            if (onStart != null)
            {
                onStart();
            }
        }

        #region IRenderable Members
        public void Draw(SpriteBatch batch)
        {
            if (onDraw != null)
            {
                onDraw();
            }
        }
        #endregion

        #region IUpdateable Members
        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (onUpdate != null)
            {
                onUpdate();
            }
        }
        #endregion

        #region IFixedUpdateable Members
        public void FixedUpdate()
        {
            if (onFixedUpdate != null)
            {
                onFixedUpdate();
            }
        }
        #endregion
    }
}
