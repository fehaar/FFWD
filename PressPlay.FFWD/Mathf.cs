using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD
{
    /// <summary>
    /// Math library with float versions of the methods in the normal Math library.
    /// Better suited for games.
    /// </summary>
    public static class Mathf
    {
        public static float PI
        {
            get
            {
                return MathHelper.Pi;
            }
        }

        public static float Cos(float value)
        {
            return (float)Math.Cos(value);
        }

        public static float Sin(float value)
        {
            return (float)Math.Sin(value);
        }

        public static float Pow(float x, float y)
        {
            return (float)Math.Pow(x, y);
        }

        public static float Sqrt(float x)
        {
            return (float)Math.Sqrt(x);
        }

        public static float Abs(float x)
        {
            return (float)Math.Abs(x);
        }

        public static float Asin(float x)
        {
            return (float)Math.Asin(x);
        }

        public static float Min(float x, float y)
        {
            return Math.Min(x, y);
        }

        public static float Max(float x, float y)
        {
            return Math.Max(x, y);
        }
    }
}
