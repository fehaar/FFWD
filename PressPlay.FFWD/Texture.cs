using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class Texture : Asset
    {
        private Microsoft.Xna.Framework.Graphics.Texture2D tex;

        public Texture()
        {
        }

        internal Texture(Microsoft.Xna.Framework.Graphics.Texture2D t)
        {
            tex = t;
        }

        public int Width
        {
            get
            {
                return (tex == null) ? 0 : tex.Width;
            }
        }

        public int Height
        {
            get
            {
                return (tex == null) ? 0 : tex.Height;
            }
        }

        protected override void DoLoadAsset(AssetHelper assetHelper)
        {
            if (tex == null)
            {
                tex = assetHelper.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Textures/" + name);
            }
        }

        public static implicit operator Microsoft.Xna.Framework.Graphics.Texture2D(Texture t)
        {
            if (t == null)
            {
                return null;
            }
            return t.tex;
        }
    }
}
