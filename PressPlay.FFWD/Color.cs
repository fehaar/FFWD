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

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return r;
                    case 1:
                        return g;
                    case 2:
                        return b;
                    case 3:
                        return a;
                    default:
                        throw new IndexOutOfRangeException("You must use an index between 0 and 3 to access color info.");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        r = value;
                        break;
                    case 1:
                        g = value;
                        break;
                    case 2:
                        b = value;
                        break;
                    case 3:
                        a = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("You must use an index between 0 and 3 to access color info.");
                }
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

        public static bool operator ==(Color d, Color c)
        {
            return d.a == c.a && d.r == c.r && d.g == c.g && d.b == c.b;
        }
        public static bool operator !=(Color d, Color c)
        {
            return d.a != c.a || d.r != c.r || d.g != c.g || d.b != c.b;
        }
        public static Color operator *(float d, Color c)
        {
            return new Color(c.r * d, c.g * d, c.b * d, c.a * d);
        }
        public static Color operator *(Color c, float d)
        {
            return new Color(c.r * d, c.g * d, c.b * d, c.a * d);
        }
        public static Color operator +(Color d, Color c)
        {
            return new Color(d.r + c.r, d.g + c.g, d.b + c.b, d.a + c.a);
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

        public static implicit operator Vector3(Color c)
        {
            return new Vector3(c.r, c.g, c.b);
        }

        public static implicit operator Microsoft.Xna.Framework.Vector3(Color c)
        {
            return new Microsoft.Xna.Framework.Vector3(c.r, c.g, c.b);
        }

        public static Color Lerp(Color a, Color b, float t)
        {
            return new Color(Microsoft.Xna.Framework.MathHelper.Lerp(a.r, b.r, t), Microsoft.Xna.Framework.MathHelper.Lerp(a.g, b.g, t), Microsoft.Xna.Framework.MathHelper.Lerp(a.b, b.b, t), Microsoft.Xna.Framework.MathHelper.Lerp(a.a, b.a, t));
        }

        public static Color Parse(string s)
        {
            Color c = new Color();
            if (s.Length == 8)
            {
                c.a = ParseHexData(s, 0);
                s = s.Substring(2);
            }
            c.r = ParseHexData(s, 0);
            c.g = ParseHexData(s, 2);
            c.b = ParseHexData(s, 4);
            return c;
        }

        private static float ParseHexData(string s, int start)
        {
            return ((float)Int32.Parse(s.Substring(start, 2), System.Globalization.NumberStyles.HexNumber)) / 255f;
        }

        public override string ToString()
        {
            if (A == 0)
            {
                return String.Format("{0:X}{1:X}{2:X}", R, G, B);
            }
            return String.Format("{0:X}{1:X}{2:X}{3:X}", A, R, G, B);
        }
    }
}
