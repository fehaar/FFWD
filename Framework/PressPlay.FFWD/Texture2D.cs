using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PressPlay.FFWD
{
    public class Texture2D : Texture
    {
        public Texture2D()
        {
        }

        public Texture2D(int width, int height)
            : base(width, height)
        {
        }

        internal Texture2D(Microsoft.Xna.Framework.Graphics.Texture2D t)
            : base(t)
        {
        }

        internal static Texture2D LoadFromResource(string name)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var t = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(Application.Instance.GraphicsDevice, asm.GetManifestResourceStream(String.Format("PressPlay.FFWD.{0}", name))); 
            return new Texture2D(t);
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

            byte[] buffer = new byte[]
                {
                    color.R,
                    color.G,
                    color.B,
                    color.A
                };

            tex.SetData<byte>(0, new Microsoft.Xna.Framework.Rectangle(modX, modY, 1, 1), buffer, 0, buffer.Length);
        }

        public void SetPixels(Color[] colors)
        {
            SetPixels(colors, 0);
        }

        public void SetPixels(Color[] colors, int miplevel)
        {
            byte[] buffer = new byte[colors.Length << 2];

            for (int i = 0; i < colors.Length; ++i)
            {
                buffer[(i << 2)] = colors[i].R;
                buffer[(i << 2) + 1] = colors[i].G;
                buffer[(i << 2) + 2] = colors[i].B;
                buffer[(i << 2) + 3] = colors[i].A;
            }

            tex.SetData<byte>(miplevel, null, buffer, 0, buffer.Length);
        }

        public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors)
        {
            SetPixels(x, y, blockWidth, blockHeight, colors, 0);
        }
        
        public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, int miplevel)
        {
            byte[] buffer = new byte[colors.Length << 2];

            for (int i = 0; i < colors.Length; ++i)
            {
                buffer[(i << 2)] = colors[i].R;
                buffer[(i << 2) + 1] = colors[i].G;
                buffer[(i << 2) + 2] = colors[i].B;
                buffer[(i << 2) + 3] = colors[i].A;
            }

            tex.SetData<byte>(miplevel, new Microsoft.Xna.Framework.Rectangle(x, y, blockWidth, blockHeight), buffer, 0, blockWidth * blockHeight << 2);
        }

        public Color GetPixel(int x, int y)
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

            byte[] buffer = new byte[4];
            tex.GetData<byte>(0, new Microsoft.Xna.Framework.Rectangle(modX, modY, 1, 1), buffer, 0, 4);

            Color color = new Color();
            color.R = buffer[0];
            color.G = buffer[1];
            color.B = buffer[2];
            color.A = buffer[3];
            
            return color;
        }

        public Color[] GetPixels()
        {
            return GetPixels(0);
        }

        public Color[] GetPixels(int miplevel)
        {
            int mipSize = GetMipmapSize(miplevel);

            byte[] buffer = new byte[mipSize << 2];
            tex.GetData<byte>(miplevel, null, buffer, 0, mipSize << 2);

            Color[] colors = new Color[mipSize];
            for (int i = 0; i < mipSize; i++)
            {
                colors[i] = new Color();
                colors[i].R = buffer[(i << 2)];
                colors[i].G = buffer[(i << 2) + 1];
                colors[i].B = buffer[(i << 2) + 2];
                colors[i].A = buffer[(i << 2) + 3];
            }

            return colors;
        }

        public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight)
        {
            return GetPixels(x, y, blockWidth, blockHeight, 0);
        }

        public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight, int miplevel)
        {
            byte[] buffer = new byte[(blockWidth * blockHeight) << 2];

            tex.GetData<byte>(miplevel, new Microsoft.Xna.Framework.Rectangle(x, y, blockWidth, blockHeight), buffer, 0, buffer.Length);

            Color[] colors = new Color[blockWidth * blockHeight];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color();
                colors[i].R = buffer[(i << 2)];
                colors[i].G = buffer[(i << 2) + 1];
                colors[i].B = buffer[(i << 2) + 2];
                colors[i].A = buffer[(i << 2) + 3];
            }

            return colors;
        }

        protected int GetMipmapSize()
        {
            return GetMipmapSize(0);
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

        protected int GetMipmapWidth()
        {
            return GetMipmapWidth(0);
        }

        protected int GetMipmapWidth(int miplevel)
        {
            if (miplevel < 0 || miplevel >= mipmapCount)
            {
                throw new ArgumentOutOfRangeException("miplevel", "Texture has " + mipmapCount + " mipmap levels. Mipmap " + miplevel + " is out of range.");
            }

            // Each miplevel is half the width and hwight of the previous one
            return (int)(tex.Width / Math.Pow(2, miplevel));
        }

        protected int GetMipmapHeight()
        {
            return GetMipmapHeight(0);
        }

        protected int GetMipmapHeight(int miplevel)
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
