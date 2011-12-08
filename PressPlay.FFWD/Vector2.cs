#region License
/*
MIT License
Copyright © 2006 The Mono.Xna Team

All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License

using System;
using System.ComponentModel;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD
{
    public struct Vector2 : IEquatable<Vector2>
    {
        #region Static constants

        public static readonly Vector2 zero = new Vector2(0f, 0f);
        public static readonly Vector2 one = new Vector2(1f, 1f);
        public static readonly Vector2 up = new Vector2(0f, 1f);
        public static readonly Vector2 right = new Vector2(1f, 0f);

        #endregion Static constants

        #region Public Fields

        public float x;
        public float y;

        #endregion Public Fields

        #region Properties

        public float magnitude
        {
            get 
            {
                return (float)Math.Sqrt(x * x + y * y);
            }
        }

        public float sqrMagnitude
        {
            get
            {
                return x * x + y * y;
            }
        }

        public Vector2 normalized
        {
            get
            {
                float factor = 1f / magnitude;
                return new Vector2(this.x *= factor, this.y *= factor);
            }
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    default:
                        throw new IndexOutOfRangeException("You must use 0 (x), 1 (y) as index to the vector.");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("You must use 0 (x), 1 (y) as index to the vector.");
                }
            }
        }

        #endregion Properties

        #region Constructors

        public Vector2(float value)
        {
            this.x = value;
            this.y = value;
        }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2(Microsoft.Xna.Framework.Vector2 v) : this(v.X, v.Y) { }

        #endregion Constructors

        #region Public Methods

        public void Scale(Vector2 a)
        {
            this.x *= a.x;
            this.y *= a.y;
        }

        public static float Dot(Vector2 lhs, Vector2 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y;
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            return (float)Math.Sqrt(DistanceSquared(a, b));
        }

        public static float DistanceSquared(Vector2 a, Vector2 b)
        {
            return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
        }

        public static Vector2 Max(Vector2 value1, Vector2 value2)
        {
            return new Vector2(
                MathHelper.Max(value1.x, value2.x),
                MathHelper.Max(value1.y, value2.y));
        }

        public static Vector2 Min(Vector2 value1, Vector2 value2)
        {
            return new Vector2(
                MathHelper.Min(value1.x, value2.x),
                MathHelper.Min(value1.y, value2.y));
        }

        public override bool Equals(object obj)
        {
            return (obj is Vector2) ? this == (Vector2)obj : false;
        }

        public bool Equals(Vector2 other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return (int)(this.x + this.y);
        }

        public void Normalize()
        {
            float factor = Distance(this, Vector2.zero);
            factor = 1f / factor;
            this.x *= factor;
            this.y *= factor;
        }

        private static Vector2 Normalize(Vector2 vector){
            vector.Normalize();
            return vector;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(32);
            sb.Append("{X=");
            sb.Append(this.x + "f,");
            sb.Append(" Y=");
            sb.Append(this.y + "f");
            sb.Append("}");
            return sb.ToString();
        }

        #endregion Public methods

        #region Operators

        public static implicit operator Microsoft.Xna.Framework.Vector2(Vector2 v)
        {
            return new Microsoft.Xna.Framework.Vector2(v.x, v.y);
        }

        public static implicit operator Vector2(Microsoft.Xna.Framework.Vector2 v)
        {
            return new Vector2(v);
        }

        public static implicit operator Vector3(Vector2 v)
        {
            switch (ApplicationSettings.to2dMode)
            {
                case ApplicationSettings.To2dMode.DropX:
                    return new Vector3(0, v.x, v.y);
                case ApplicationSettings.To2dMode.DropY:
                    return new Vector3(v.x, 0, v.y);
                case ApplicationSettings.To2dMode.DropZ:
                    return new Vector3(v.x, v.y, 0);
                default:
                    throw new Exception("Unknown enum " + ApplicationSettings.to2dMode);
            }
        }

        public static bool operator ==(Vector2 value1, Vector2 value2)
        {
            return value1.x == value2.x
                && value1.y == value2.y;
        }

        public static bool operator !=(Vector2 value1, Vector2 value2)
        {
            return !(value1 == value2);
        }

        public static Vector2 operator +(Vector2 value1, Vector2 value2)
        {
            value1.x += value2.x;
            value1.y += value2.y;
            return value1;
        }

        public static Vector2 operator -(Vector2 value)
        {
            value = new Vector2(-value.x, -value.y);
            return value;
        }

        public static Vector2 operator -(Vector2 value1, Vector2 value2)
        {
            value1.x -= value2.x;
            value1.y -= value2.y;
            return value1;
        }

        public static Vector2 operator *(Vector2 value1, Vector2 value2)
        {
            value1.x *= value2.x;
            value1.y *= value2.y;
            return value1;
        }

        public static Vector2 operator *(Vector2 value, float scaleFactor)
        {
            value.x *= scaleFactor;
            value.y *= scaleFactor;
            return value;
        }

        public static Vector2 operator *(float scaleFactor, Vector2 value)
        {
            value.x *= scaleFactor;
            value.y *= scaleFactor;
            return value;
        }

        public static Vector2 operator /(Vector2 value1, Vector2 value2)
        {
            value1.x /= value2.x;
            value1.y /= value2.y;
            return value1;
        }

        public static Vector2 operator /(Vector2 value, float divider)
        {
            float factor = 1 / divider;
            value.x *= factor;
            value.y *= factor;
            return value;
        }

        #endregion
    }
}
