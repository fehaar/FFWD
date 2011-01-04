using System;

namespace PressPlay.FFWD
{
    public struct Color
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public byte R
        {
            get
            {
                return (byte)(r * byte.MaxValue);
            }
            set
            {
                r = (float)(value) / 255;
            }
        }

        public byte G
        {
            get
            {
                return (byte)(g * byte.MaxValue);
            }
            set
            {
                g = (float)(value) / 255;
            }
        }

        public byte B
        {
            get
            {
                return (byte)(b * byte.MaxValue);
            }
            set
            {
                b = (float)(value) / 255;
            }
        }

        public byte A
        {
            get
            {
                return (byte)(a * byte.MaxValue);
            }
            set
            {
                a = (float)(value) / 255;
            }
        }

        public float greyscale
        {
            get
            {
                // TODO: Implement this
                throw new NotImplementedException("Not implemented");
            }
        }

        #region Colors

        /// <summary>
        /// Solid red. RGBA is (1, 0, 0, 1).
        /// </summary>
        public static Color red
        {
            get
            {
                return new Color(1, 0, 0, 1);
            }
        }

        /// <summary>
        /// Solid green. RGBA is (0, 1, 0, 1).
        /// </summary>
        static public Color green
        {
            get
            {
                return new Color(0, 1, 0, 1);
            }
        }

        /// <summary>
        /// Solid blue. RGBA is (0, 0, 1, 1).
        /// </summary>
        static public Color blue
        {
            get
            {
                return new Color(0, 0, 1, 1);
            }
        }

        /// <summary>
        /// Solid white. RGBA is (1, 1, 1, 1).
        /// </summary>
        public static Color white
        {
            get
            {
                return new Color(1, 1, 1, 1);
            }
        }

        /// <summary>
        /// Solid black. RGBA is (0, 0, 0, 1).
        /// </summary>
        static public Color black
        {
            get
            {
                return new Color(0, 0, 0, 1);
            }
        }

        /// <summary>
        /// Yellow. RGBA is weird (1, 235/255, 4/255, 1), but the color is nice to look at!
        /// </summary>
        static public Color yellow
        {
            get
            {
                return new Color(1, 235 / 255, 4 / 255, 1);
            }
        }

        /// <summary>
        /// Cyan. RGBA is (0, 1, 1, 1).
        /// </summary>
        static public Color cyan
        {
            get
            {
                return new Color(0, 1, 1, 1);
            }
        }

        /// <summary>
        /// Magenta. RGBA is (1, 0, 1, 1).
        /// </summary>
        static public Color magenta
        {
            get
            {
                return new Color(1, 0, 1, 1);
            }
        }

        /// <summary>
        /// Gray. RGBA is (0.5, 0.5, 0.5, 1).
        /// </summary>
        static public Color gray
        {
            get
            {
                return new Color(0.5f, 0.5f, 0.5f, 1);
            }
        }

        /// <summary>
        /// English spelling for gray. RGBA is the same (0.5, 0.5, 0.5, 1).
        /// </summary>
        static public Color grey
        {
            get
            {
                return new Color(0.5f, 0.5f, 0.5f, 1);
            }
        }

        /// <summary>
        /// Completely transparent. RGBA is (0, 0, 0, 0).
        /// </summary>
        static public Color clear
        {
            get
            {
                return new Color(0, 0, 0, 0);
            }
        }
        #endregion

        #region constructors
        public Color(float r, float g, float b) : this(r, g, b, 1)
        {

        }

        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        #endregion

        public static Color operator *(float d, Color c)
        {
            return new Color(c.r * d, c.g * d, c.b * d, c.a * d);
        }
        public static Color operator *(Color c, float d)
        {
            return new Color(c.r * d, c.g * d, c.b * d, c.a * d);
        }

        public static implicit operator Color(Microsoft.Xna.Framework.Vector4 v)
        {
            return new Color(v.X, v.Y, v.Z, v.W);
        }

        public static implicit operator Microsoft.Xna.Framework.Color(Color c)
        {
            return Microsoft.Xna.Framework.Color.FromNonPremultiplied(new Microsoft.Xna.Framework.Vector4(c.r, c.g, c.b, c.a));
        }

        public static implicit operator Color(Microsoft.Xna.Framework.Color c)
        {
            return c.ToVector4();
        }

        public static Color Lerp(Color a, Color b, float t)
        {
            return new Color(Microsoft.Xna.Framework.MathHelper.Lerp(a.r, b.r, t), Microsoft.Xna.Framework.MathHelper.Lerp(a.g, b.g, t), Microsoft.Xna.Framework.MathHelper.Lerp(a.b, b.b, t), Microsoft.Xna.Framework.MathHelper.Lerp(a.a, b.a, t));
        }
    }
}
