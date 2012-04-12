using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class Texture : Asset
    {
        protected Microsoft.Xna.Framework.Graphics.Texture2D tex;

        public Texture()
        {
        }

        internal Texture(Microsoft.Xna.Framework.Graphics.Texture2D t)
        {
            tex = t;
        }

        internal Texture(int _width, int _height)
        {
            tex = new Microsoft.Xna.Framework.Graphics.Texture2D(Application.Instance.GraphicsDevice, _width, _height, false, Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color);
            CheckPowerOfTwoSize();
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

        public int width
        {
            get
            {
                return (tex == null) ? 0 : tex.Width;
            }
        }

        public int height
        {
            get
            {
                return (tex == null) ? 0 : tex.Height;
            }
        }

        public int mipmapCount
        {
            get
            {
                return (tex == null) ? 0 : tex.LevelCount;
            }
        }

        public void SetPixel(int x, int y, Color color)
        {
            // Unity takes Wrap mode into account and so should we. By default, wrap.
            int modX = x > 0 && x < tex.Width ? x : x % tex.Width;
            // % means remainder, not modulus, so we can get negative values.
            if (modX < 0)
            {
                modX += tex.Width;
            }

            int modY = y > 0 && y < tex.Height ? y : y % tex.Height;
            if (modY < 0)
            {
                modY += tex.Height;
            }

            Microsoft.Xna.Framework.Color[] xnaColor = new Microsoft.Xna.Framework.Color[] { color };
            tex.SetData<Microsoft.Xna.Framework.Color>(0, new Microsoft.Xna.Framework.Rectangle(modX, tex.Height - modY - 1, 1, 1), xnaColor, 0, 1);
        }

        public void SetPixels(Color[] colors, int miplevel = 0)
        {
            Microsoft.Xna.Framework.Color[] xnaColors = new Microsoft.Xna.Framework.Color[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                xnaColors[i] = colors[i];
            }

            tex.SetData<Microsoft.Xna.Framework.Color>(miplevel, null, xnaColors, 0, xnaColors.Length);
        }

        public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, int miplevel = 0)
        {
            Microsoft.Xna.Framework.Color[] xnaColors = new Microsoft.Xna.Framework.Color[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                xnaColors[i] = colors[i];
            }

            tex.SetData<Microsoft.Xna.Framework.Color>(
                miplevel,
                new Microsoft.Xna.Framework.Rectangle(x, y, blockWidth, blockHeight),
                xnaColors,
                0,
                blockWidth * blockHeight);
        }

        public Color GetPixel (int x, int y)
        {
            // By default, textures in Unity are set to Wrap = Repeat.
            // Use that as default.
            // Calculate the modulus of x and y, so we don't go out of bounds.
            int modX = x > 0 && x < tex.Width ? x : x % tex.Width;
            // % means remainder, not modulus, so we can get negative values.
            if (modX < 0)
            {
                modX += tex.Width;
            }

            int modY = y > 0 && y < tex.Height ? y : y % tex.Height;
            if (modY < 0)
            {
                modY += tex.Height;
            }

            Microsoft.Xna.Framework.Color[] color = new Microsoft.Xna.Framework.Color[1];
            tex.GetData<Microsoft.Xna.Framework.Color>(0, new Microsoft.Xna.Framework.Rectangle(modX, modY, 1, 1), color, 0, 1);

            // Implicit conversion to FFWD.Color
            return color[0];
        }

        public Color[] GetPixels(int miplevel = 0)
        {
            int mipSize = GetMipmapSize(miplevel);

            Microsoft.Xna.Framework.Color[] xnaColors = new Microsoft.Xna.Framework.Color[mipSize];
            tex.GetData<Microsoft.Xna.Framework.Color>(miplevel, null, xnaColors, 0, mipSize);

            Color[] colors = new Color[mipSize];
            for (int i = 0; i < mipSize; i++)
            {
                colors[i] = xnaColors[i];
            }

            return colors;
        }

        public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight, int miplevel = 0)
        {
            Microsoft.Xna.Framework.Color[] xnaColors = new Microsoft.Xna.Framework.Color[blockWidth * blockHeight];
            
            tex.GetData<Microsoft.Xna.Framework.Color>(
                miplevel,
                new Microsoft.Xna.Framework.Rectangle(x, y, blockWidth, blockHeight),
                xnaColors,
                0,
                blockWidth * blockHeight);

            Color[] colors = new Color[xnaColors.Length];
            for (int i = 0; i < xnaColors.Length; i++)
            {
                colors[i] = xnaColors[i];
            }

            return colors;
        }

        public void Apply()
        {
            Apply(false, false);
        }

        public void Apply(bool updateMipmaps)
        {
            Apply(updateMipmaps, false);
        }

        public void Apply(bool updateMipmaps, bool makeNoLongerReadable)
        {
            // TODO: Implement this
        }

        protected override void DoLoadAsset(AssetHelper assetHelper)
        {
            if (name == null)
            {
                return;
            }
            if (tex == null)
            {
                tex = assetHelper.LoadAsset<Microsoft.Xna.Framework.Graphics.Texture2D>(name);
                CheckPowerOfTwoSize();
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

        internal bool IsPowerOfTwoSize { get; private set; }

        protected void CheckPowerOfTwoSize()
        {
            // NOTE: There must be some genius math function that can do this better?! 
            IsPowerOfTwoSize = ((tex.Width == 1) || (tex.Width == 2) || (tex.Width == 4) || (tex.Width == 8) || (tex.Width == 16) || (tex.Width == 32) || (tex.Width == 64) || (tex.Width == 128) || (tex.Width == 256) || (tex.Width == 512) || (tex.Width == 1024) || (tex.Width == 2048) || (tex.Width == 4096)) && ((tex.Height == 1) || (tex.Height == 2) || (tex.Height == 4) || (tex.Height == 8) || (tex.Height == 16) || (tex.Height == 32) || (tex.Height == 64) || (tex.Height == 128) || (tex.Height == 256) || (tex.Height == 512) || (tex.Height == 1024) || (tex.Height == 2048) || (tex.Height == 4096));
        }

        protected int GetMipmapSize(int miplevel)
        {
            if (miplevel < 0 || miplevel >= mipmapCount)
            {
                throw new ArgumentOutOfRangeException("miplevel", "Texture has " + mipmapCount + " mipmap levels. Mipmap " + miplevel + " is out of range.");
            }

            // Each mip level is one fourth the size of the previous one
            return (int)(tex.Width * tex.Height / Math.Pow(4, miplevel));
        }

        protected int GetMipmapWidth(int miplevel = 0)
        {
            if (miplevel < 0 || miplevel >= mipmapCount)
            {
                throw new ArgumentOutOfRangeException("miplevel", "Texture has " + mipmapCount + " mipmap levels. Mipmap " + miplevel + " is out of range.");
            }

            // Each miplevel is half the width and hwight of the previous one
            return (int)(tex.Width / Math.Pow(2, miplevel));
        }

        protected int GetMipmapHeight(int miplevel = 0)
        {
            if (miplevel < 0 || miplevel >= mipmapCount)
            {
                throw new ArgumentOutOfRangeException("miplevel", "Texture has " + mipmapCount + " mipmap levels. Mipmap " + miplevel + " is out of range.");
            }

            // Each miplevel is half the width and hwight of the previous one
            return (int)(tex.Height / Math.Pow(2, miplevel));
        }
    }
}
