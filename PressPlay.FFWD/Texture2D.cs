using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class Texture2D : Texture
    {
        public Texture2D()
        {
        }

        internal Texture2D(Microsoft.Xna.Framework.Graphics.Texture2D t)
            : base(t)
        {
        }
    }
}
