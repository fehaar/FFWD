using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Components
{
    public abstract class Renderer : Component, IRenderable
    {
        [ContentSerializerIgnore]
        public Material sharedMaterial { get; set; }
        
        #region IRenderable Members
        public abstract void Draw(SpriteBatch batch);
        #endregion
    }
}
