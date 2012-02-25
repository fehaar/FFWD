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
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD
{
    public struct Vector4 : IEquatable<Vector4>
    {
        #region Static constants

        public static readonly Vector4 zero = new Vector4();
        public static readonly Vector4 one = new Vector4(1f, 1f, 1f, 1f);

        #endregion Static constants


        #region Public Fields

        public float x;
        public float y;
        public float z;
        public float w;

        #endregion Public Fields


        #region Properties

        public Vector4 normalized
        {
            get
            {
                return Vector4.Normalize(this);
            }
        }

        public float magnitude
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y + z * z + w * w);
            }
        }

        public float sqrMagnitude
        {
            get
            {
                return x * x + y * y + z * z + w * w;
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
                    case 2:
                        return z;
                    case 3:
                        return w;
                    default:
                        throw new IndexOutOfRangeException("You must use 0 (x), 1 (y), 2 (z) or 3 (w) as index to the vector.");
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
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("You must use 0 (x), 1 (y), 2 (z) or 3 (w) as index to the vector.");
                }
            }
        }

        #endregion Properties


        #region Constructors

        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vector4(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = 0;
        }

        public Vector4(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
            this.w = 0;
        }

        public Vector4(float value)
        {
            this.x = value;
            this.y = value;
            this.z = value;
            this.w = value;
        }

        public Vector4(Microsoft.Xna.Framework.Vector4 v)
        {
            this.x = v.X;
            this.y = v.Y;
            this.z = v.Z;
            this.w = v.W;
        }

        #endregion


        #region Public Methods

        public static Vector4 Lerp(Vector4 from, Vector4 to, float t)
        {
            float normalizedT = Mathf.Clamp01(t);
            return new Vector4(
                MathHelper.Lerp(from.x, to.x, t),
                MathHelper.Lerp(from.y, to.y, t),
                MathHelper.Lerp(from.z, to.z, t),
                MathHelper.Lerp(from.w, to.w, t));
        }

        //TODO
        public static void MoveTowards(ref Vector4 normal, ref Vector4 tangent, ref Vector4 binormal)
        {
            throw new NotImplementedException();
        }


        public static Vector4 Scale(Vector4 a, Vector4 b)
        {
            a.x *= b.x;
            a.y *= b.y;
            a.z *= b.z;
            a.w *= b.w;

            return a;
        }

        public void Scale(Vector4 a)
        {
            this.x *= a.x;
            this.y *= a.y;
            this.z *= a.z;
            this.w *= a.w;
        }

        public override int GetHashCode()
        {
            return (int)(this.w + this.x + this.y + this.y);
        }

        public override bool Equals(object obj)
        {
            return (obj is Vector4) ? this == (Vector4)obj : false;
        }

        public bool Equals(Vector4 other)
        {
            return this == other;
        }

        public void Normalize()
        {
            float factor = DistanceSquared(this, Vector4.zero);

            if (factor == 0)
            {
                return;
            }

            factor = 1f / (float)Math.Sqrt(factor);
            this.w *= factor;
            this.x *= factor;
            this.y *= factor;
            this.z *= factor;
        }

        public static Vector4 Normalize(Vector4 vector)
        {
            vector.Normalize();
            return vector;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(32);
            sb.Append("{X:");
            sb.Append(this.x);
            sb.Append(" Y:");
            sb.Append(this.y);
            sb.Append(" Z:");
            sb.Append(this.z);
            sb.Append(" W:");
            sb.Append(this.w);
            sb.Append("}");
            return sb.ToString();
        }

        public static float Dot(Vector4 a, Vector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        //TODO
        public static Vector4 Project(Vector4 a, Vector4 b)
        {
            throw new NotImplementedException();
        }

        public static float Distance(Vector4 a, Vector4 b)
        {
            return (float)Math.Sqrt(DistanceSquared(a, b));
        }

        public static float DistanceSquared(Vector4 a, Vector4 b)
        {            
            float result = (a.w - b.w) * (a.w - b.w) +
                           (a.x - b.x) * (a.x - b.x) +
                           (a.y - b.y) * (a.y - b.y) +
                           (a.z - b.z) * (a.z - b.z);
            return result;
        }

        public static float Magnitude(Vector4 a)
        {
            return a.magnitude;
        }

        public static float SqrMagnitude(Vector4 a)
        {
            return a.sqrMagnitude;
        }

        public float SqrMagnitude()
        {
            return this.sqrMagnitude;
        }

        public static Vector4 Min(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(
               MathHelper.Min(lhs.x, rhs.x),
               MathHelper.Min(lhs.y, rhs.y),
               MathHelper.Min(lhs.z, rhs.z),
               MathHelper.Min(lhs.w, rhs.w));
        }

        public static Vector4 Max(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(
               MathHelper.Max(lhs.x, rhs.x),
               MathHelper.Max(lhs.y, rhs.y),
               MathHelper.Max(lhs.z, rhs.z),
               MathHelper.Max(lhs.w, rhs.w));
        }

        #endregion Public Methods


        #region Operators

        public static implicit operator Microsoft.Xna.Framework.Vector4(Vector4 v)
        {
            return new Microsoft.Xna.Framework.Vector4(v.x, v.y, v.z, v.w);
        }

        public static implicit operator Vector4(Microsoft.Xna.Framework.Vector4 v)
        {
            return new Vector4(v);
        }

        public static implicit operator Vector4(Vector3 v)
        {
            return new Vector4(v.x, v.y, v.z);
        }

        public static bool operator ==(Vector4 value1, Vector4 value2)
        {
            return value1.w == value2.w
                && value1.x == value2.x
                && value1.y == value2.y
                && value1.z == value2.z;
        }

        public static bool operator !=(Vector4 value1, Vector4 value2)
        {
            return !(value1 == value2);
        }

        public static Vector4 operator +(Vector4 value1, Vector4 value2)
        {
            value1.w += value2.w;
            value1.x += value2.x;
            value1.y += value2.y;
            value1.z += value2.z;
            return value1;
        }

        public static Vector4 operator -(Vector4 value1, Vector4 value2)
        {
            value1.w -= value2.w;
            value1.x -= value2.x;
            value1.y -= value2.y;
            value1.z -= value2.z;
            return value1;
        }

        public static Vector4 operator *(Vector4 value1, Vector4 value2)
        {
            value1.w *= value2.w;
            value1.x *= value2.x;
            value1.y *= value2.y;
            value1.z *= value2.z;
            return value1;
        }

        public static Vector4 operator *(Vector4 value1, float scaleFactor)
        {
            value1.w *= scaleFactor;
            value1.x *= scaleFactor;
            value1.y *= scaleFactor;
            value1.z *= scaleFactor;
            return value1;
        }

        public static Vector4 operator *(float scaleFactor, Vector4 value1)
        {
            value1.w *= scaleFactor;
            value1.x *= scaleFactor;
            value1.y *= scaleFactor;
            value1.z *= scaleFactor;
            return value1;
        }

        public static Vector4 operator /(Vector4 value1, Vector4 value2)
        {
            value1.w /= value2.w;
            value1.x /= value2.x;
            value1.y /= value2.y;
            value1.z /= value2.z;
            return value1;
        }

        public static Vector4 operator /(Vector4 value1, float divider)
        {
            float factor = 1f / divider;
            value1.w *= factor;
            value1.x *= factor;
            value1.y *= factor;
            value1.z *= factor;
            return value1;
        }

        #endregion Operators
    }
}