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
    public struct Vector3 : IEquatable<Vector3>
    {
        #region Static constants

        public static readonly Vector3 zero = new Vector3(0f, 0f, 0f);
        public static readonly Vector3 one = new Vector3(1f, 1f, 1f);
        /// <summary>
        /// The forward vector is positive-z (0, 0, 1)
        /// </summary>
        public static readonly Vector3 forward = new Vector3(0f, 0f, 1f);
        /// <summary>
        /// The back vector is negative-z (0, 0, 1)
        /// </summary>
        public static readonly Vector3 back = new Vector3(0f, 0f, -1f);
        /// <summary>
        /// The up vector is positive-y (0, 1, 0)
        /// </summary>
        public static readonly Vector3 up = new Vector3(0f, 1f, 0f);
        /// <summary>
        /// The right vector is positive-x (1, 0, 0)
        /// </summary>
        public static readonly Vector3 right = new Vector3(1f, 0f, 0f);

        #endregion Static constants

        #region Public Fields

        public float x;
        public float y;
        public float z;

        #endregion Public Fields

        #region Properties

        public Vector3 normalized
        {
            get
            {
                return Vector3.Normalize(this);
            }
        }

        public float magnitude
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y + z * z);
            }
        }

        public float sqrMagnitude
        {
            get
            {
                return x * x + y * y + z * z;
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
                    default:
                        throw new IndexOutOfRangeException("You must use 0 (x), 1 (y) or 2 (z) as index to the vector.");
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
                    default:
                        throw new IndexOutOfRangeException("You must use 0 (x), 1 (y) or 2 (z) as index to the vector.");
                }
            }
        }

        #endregion Properties

        #region Constructors

        public Vector3(float value)
        {
            this.x = value;
            this.y = value;
            this.z = value;
        }

        public Vector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(Vector2 position, float depth)
        {
            switch (ApplicationSettings.to2dMode)
            {
                case ApplicationSettings.To2dMode.DropX:
                    this.x = depth;
                    this.y = position.x;
                    this.z = position.y;
                    break;
                case ApplicationSettings.To2dMode.DropY:
                    this.x = position.x;
                    this.y = depth;
                    this.z = position.y;
                    break;
                case ApplicationSettings.To2dMode.DropZ:
                    this.x = position.x;
                    this.y = position.y;
                    this.z = depth;
                    break;
                default:
                    throw new Exception("Unknown enum " + ApplicationSettings.to2dMode);
            }
        }

        public Vector3(Microsoft.Xna.Framework.Vector3 v) : this(v.X, v.Y, v.Z) { }

        #endregion Constructors

        #region Public Methods

        public static Vector3 Lerp(Vector3 from, Vector3 to, float t)
        {
            float normalizedT = Mathf.Clamp01(t);
            return new Vector3(
                MathHelper.Lerp(from.x, to.x, normalizedT),
                MathHelper.Lerp(from.y, to.y, normalizedT),
                MathHelper.Lerp(from.z, to.z, normalizedT));
        }

        //TODO
        public static Vector3 Slerp(Vector3 from, Vector3 to, float t)
        {
            throw new NotImplementedException();
        }

        //TODO
        public static void OrthoNormalize(ref Vector3 normal, ref Vector3 tangent)
        {
            throw new NotImplementedException();
        }

        //TODO
        public static void OrthoNormalize(ref Vector3 normal, ref Vector3 tangent, ref Vector3 binormal)
        {
            throw new NotImplementedException();
        }

        //TODO
        public static void MoveTowards(ref Vector3 normal, ref Vector3 tangent, ref Vector3 binormal)
        {
            throw new NotImplementedException();
        }

        //TODO
        public static Vector3 RotateTowards(Vector3 from, Vector3 to, float maxRadiansDelta, float maxMagnitudeDelta)
        {
            throw new NotImplementedException();
        }

        public static Vector3 Scale(Vector3 a, Vector3 b)
        {
            a.x *= b.x;
            a.y *= b.y;
            a.z *= b.z;

            return a;
        }

        public void Scale(Vector3 a)
        {
            this.x *= a.x;
            this.y *= a.y;
            this.z *= a.z;
        }

        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.y * rhs.z - rhs.y * lhs.z,
                               -(lhs.x * rhs.z - rhs.x * lhs.z),
                               lhs.x * rhs.y - rhs.x * lhs.y);
        }

        // TODO
        public static Vector3 Reflect(Vector3 vector, Vector3 normal)
        {
            throw new NotImplementedException();
        }

        public static float Dot(Vector3 lhs, Vector3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }

        //TODO
        public static Vector3 Project(Vector3 vector, Vector3 onNormal)
        {
            throw new NotImplementedException();
        }

        public static float Angle(Vector3 from, Vector3 to)
        {
            float dot = Vector3.Dot(from.normalized, to.normalized);

            if (dot >= 1 || dot <= -1) { return 0; }

            return MathHelper.ToDegrees((float)Math.Acos((double)dot));
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            return (float)Math.Sqrt(DistanceSquared(a, b));
        }

        public static float DistanceSquared(Vector3 a, Vector3 b)
        {
            return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z);
        }

        public static float SqrMagnitude(Vector3 v)
        {
            return v.sqrMagnitude;
        }

        public static Vector3 Max(Vector3 value1, Vector3 value2)
        {
            return new Vector3(
                MathHelper.Max(value1.x, value2.x),
                MathHelper.Max(value1.y, value2.y),
                MathHelper.Max(value1.z, value2.z));
        }

        public static Vector3 Min(Vector3 value1, Vector3 value2)
        {
            return new Vector3(
                MathHelper.Min(value1.x, value2.x),
                MathHelper.Min(value1.y, value2.y),
                MathHelper.Min(value1.z, value2.z));
        }

        public override bool Equals(object obj)
        {
            return (obj is Vector3) ? this == (Vector3)obj : false;
        }

        public bool Equals(Vector3 other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return (int)(this.x + this.y + this.z);
        }

        public void Normalize()
        {
            if (this == Vector3.zero) return;
            
            float factor = Distance(this, Vector3.zero);

            if (factor == 0)
            {
                return;
            }

            factor = 1f / factor;
            this.x *= factor;
            this.y *= factor;
            this.z *= factor;
        }

        private static Vector3 Normalize(Vector3 vector){
            vector.Normalize();
            return vector;
        }

        public override string ToString()
        {
            return String.Format("({0:F1}, {1:F1}, {2:F1})", x, y, z);
        }

        /// <summary>
        /// Converts the Vector3 to a Vector2 by dropping a given variable.
        /// If Correct Zero Data is on, we will try to use a different mode if one of the axes are 0 after converting. Used primarily for collider sizing.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="correctZeroData"></param>
        /// <returns></returns>
        public Vector2 Convert(ApplicationSettings.To2dMode mode, bool correctZeroData)
        {
            Vector2 v = Vector2.zero;
            switch (mode)
            {
                case ApplicationSettings.To2dMode.DropX:
                    v = new Vector2(y, z);
                    break;
                case ApplicationSettings.To2dMode.DropY:
                    v = new Vector2(x, z);
                    break;
                case ApplicationSettings.To2dMode.DropZ:
                    v = new Vector2(x, y);
                    break;
                default:
                    throw new Exception("Unknown enum " + ApplicationSettings.to2dMode);
            }
            if (correctZeroData && v.x == 0)
            {
                v.x = (mode == ApplicationSettings.To2dMode.DropX) ? x : y;
            }
            if (correctZeroData && v.y == 0)
            {
                v.y = (mode == ApplicationSettings.To2dMode.DropZ) ? z : y;
            }
            return v;
        }

        public Vector2 Convert(bool correctZeroData)
        {
            return Convert(ApplicationSettings.to2dMode, correctZeroData);
        }

        public Vector2 Convert(ApplicationSettings.To2dMode mode)
        {
            return Convert(mode, false);
        }
        #endregion Public methods

        #region Operators

        public static implicit operator Microsoft.Xna.Framework.Vector3(Vector3 v)
        {
            return new Microsoft.Xna.Framework.Vector3(v.x, v.y, v.z);
        }

        public static implicit operator Vector3(Microsoft.Xna.Framework.Vector3 v)
        {
            return new Vector3(v);
        }

        public static implicit operator Vector2(Vector3 v)
        {
            return v.Convert(ApplicationSettings.to2dMode, false);
        }

        public static implicit operator Microsoft.Xna.Framework.Vector2(Vector3 v)
        {
            switch (ApplicationSettings.to2dMode)
            {
                case ApplicationSettings.To2dMode.DropX:
                    return new Microsoft.Xna.Framework.Vector2(v.y, v.z);
                case ApplicationSettings.To2dMode.DropY:
                    return new Microsoft.Xna.Framework.Vector2(v.x, v.z);
                case ApplicationSettings.To2dMode.DropZ:
                    return new Microsoft.Xna.Framework.Vector2(v.x, v.y);
                default:
                    throw new Exception("Unknown enum " + ApplicationSettings.to2dMode);
            }
        }

        public static implicit operator Vector3(Microsoft.Xna.Framework.Vector2 v)
        {
            switch (ApplicationSettings.to2dMode)
            {
                case ApplicationSettings.To2dMode.DropX:
                    return new Microsoft.Xna.Framework.Vector3(0, v.X, v.Y);
                case ApplicationSettings.To2dMode.DropY:
                    return new Microsoft.Xna.Framework.Vector3(v.X, 0, v.Y);
                case ApplicationSettings.To2dMode.DropZ:
                    return new Microsoft.Xna.Framework.Vector3(v.X, v.Y, 0);
                default:
                    throw new Exception("Unknown enum " + ApplicationSettings.to2dMode);
            }
        }

        public static explicit operator float(Vector3 v)
        {
            switch (ApplicationSettings.to2dMode)
            {
                case ApplicationSettings.To2dMode.DropX:
                    return v.x;
                case ApplicationSettings.To2dMode.DropY:
                    return v.y;
                case ApplicationSettings.To2dMode.DropZ:
                    return v.z;
                default:
                    throw new Exception("Unknown enum " + ApplicationSettings.to2dMode);
            }
        }

        public static bool operator ==(Vector3 value1, Vector3 value2)
        {
            return value1.x == value2.x
                && value1.y == value2.y
                && value1.z == value2.z;
        }

        public static bool operator !=(Vector3 value1, Vector3 value2)
        {
            return !(value1 == value2);
        }

        public static Vector3 operator +(Vector3 value1, Vector3 value2)
        {
            value1.x += value2.x;
            value1.y += value2.y;
            value1.z += value2.z;
            return value1;
        }

        public static Vector3 operator -(Vector3 value)
        {
            value = new Vector3(-value.x, -value.y, -value.z);
            return value;
        }

        public static Vector3 operator -(Vector3 value1, Vector3 value2)
        {
            value1.x -= value2.x;
            value1.y -= value2.y;
            value1.z -= value2.z;
            return value1;
        }

        public static Vector3 operator *(Vector3 value1, Vector3 value2)
        {
            value1.x *= value2.x;
            value1.y *= value2.y;
            value1.z *= value2.z;
            return value1;
        }

        public static Vector3 operator *(Vector3 value, float scaleFactor)
        {
            value.x *= scaleFactor;
            value.y *= scaleFactor;
            value.z *= scaleFactor;
            return value;
        }

        public static Vector3 operator *(float scaleFactor, Vector3 value)
        {
            value.x *= scaleFactor;
            value.y *= scaleFactor;
            value.z *= scaleFactor;
            return value;
        }

        public static Vector3 operator /(Vector3 value1, Vector3 value2)
        {
            value1.x /= value2.x;
            value1.y /= value2.y;
            value1.z /= value2.z;
            return value1;
        }

        public static Vector3 operator /(Vector3 value, float divider)
        {
            float factor = 1 / divider;
            value.x *= factor;
            value.y *= factor;
            value.z *= factor;
            return value;
        }

        #endregion
    }
}
