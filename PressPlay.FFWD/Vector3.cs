using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public struct Vector3
    {
        public Vector3(float x, float y, float z)
        {
            vector = new Microsoft.Xna.Framework.Vector3(x, y, z);
        }

        public Vector3(Microsoft.Xna.Framework.Vector3 v)
        {
            vector = v;
        }

        public Vector3(int p)
        {
            vector = new Microsoft.Xna.Framework.Vector3(p);
        }

        internal Microsoft.Xna.Framework.Vector3 vector;

        #region Properties
        public float x
        {
            get
            {
                return vector.X;
            }
            set
            {
                vector.X = value;
            }
        }

        public float y
        {
            get
            {
                return vector.Y;
            }
            set
            {
                vector.Y = value;
            }
        }

        public float z
        {
            get
            {
                return vector.Z;
            }
            set
            {
                vector.Z = value;
            }
        }

        public Vector3 normalized
        {
            get
            {
                if (vector == Microsoft.Xna.Framework.Vector3.Zero)
                {
                    return Vector3.zero;
                }
                return Microsoft.Xna.Framework.Vector3.Normalize(vector);
            }
        }

        public float magnitude
        {
            get
            {
                return vector.Length();
            }
        }

        public float sqrMagnitude
        {
            get
            {
                return vector.LengthSquared();
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
                        vector.X = value;
                        break;
                    case 1:
                        vector.Y = value;
                        break;
                    case 2:
                        vector.Z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("You must use 0 (x), 1 (y) or 2 (z) as index to the vector.");
                }
            }
        }
        #endregion

        #region Operators
        public static implicit operator Microsoft.Xna.Framework.Vector3(Vector3 v)
        {
            return v.vector;
        }

        public static implicit operator Vector3(Microsoft.Xna.Framework.Vector3 v)
        {
            return new Vector3(v);
        }

        public static implicit operator Microsoft.Xna.Framework.Vector2(Vector3 v)
        {
            return v.vector.To2d();
        }

        public static implicit operator Vector3(Microsoft.Xna.Framework.Vector2 v)
        {
            return new Vector3(v.To3d());
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.vector + b.vector);
        }

        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.vector);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.vector - b.vector);
        }

        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.vector * b.vector);
        }

        public static Vector3 operator *(float a, Vector3 b)
        {
            return new Vector3(a * b.vector);
        }

        public static Vector3 operator *(Vector3 a, float b)
        {
            return new Vector3(a.vector * b);
        }

        public static Vector3 operator /(Vector3 a, float b)
        {
            return new Vector3(a.vector / b);
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.vector == b.vector;
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return a.vector != b.vector;
        }
        #endregion

        #region Static constants
        private static Vector3 _zero = new Vector3(0, 0, 0);
        public static Vector3 zero
        {
            get
            {
                return _zero;
            }
        }

        private static Vector3 _one = new Vector3(1, 1, 1);
        public static Vector3 one
        {
            get
            {
                return _one;
            }
        }

        private static Vector3 _forward = new Vector3(0, 0, -1);
        public static Vector3 forward
        {
            get
            {
                return _forward;
            }
        }

        private static Vector3 _up = new Vector3(0, 1, 0);
        public static Vector3 up
        {
            get
            {
                return _up;
            }
        }

        private static Vector3 _right = new Vector3(1, 0, 0);
        public static Vector3 right
        {
            get
            {
                return _right;
            }
        }
        #endregion

        #region Overridden methods
        public override bool Equals(object obj)
        {
            if (obj is Vector3)
            {
                return vector.Equals(((Vector3)obj).vector);
            }
            if (obj is Microsoft.Xna.Framework.Vector3)
            {
                return vector.Equals((Microsoft.Xna.Framework.Vector3)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return vector.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0:F} {1:F} {2:F}", x, y, z);
        }
        #endregion

        #region Public methods
        public void Normalize()
        {
            vector.Normalize();
        }

        public void Scale(Vector3 scale)
        {
            vector.X *= scale.x;
            vector.Y *= scale.y;
            vector.Z *= scale.z;
        }
        #endregion

        #region Static methods
        public static Vector3 Lerp(Vector3 v1, Vector3 v2, float amount)
        {
            Microsoft.Xna.Framework.Vector3 o;
            Microsoft.Xna.Framework.Vector3.Lerp(ref v1.vector, ref v2.vector, amount, out o);
            return o;
        }

        public static Vector3 Slerp(Vector3 v1, Vector3 v2, float amount)
        {
            // TODO: Implement this method
            throw new NotImplementedException("Method not implemented.");
        }

        public static void OrthoNormalize(ref Vector3 normal, ref Vector3 tangent)
        {
            // TODO: Implement this method
            throw new NotImplementedException("Method not implemented.");
        }

        public static void OrthoNormalize(ref Vector3 normal, ref Vector3 tangent, ref Vector3 binormal)
        {
            // TODO: Implement this method
            throw new NotImplementedException("Method not implemented.");
        }

        public static Vector3 MoveTowards(Vector3 current, Vector3 traget, float maxDistanceDelta)
        {
            // TODO: Implement this method
            throw new NotImplementedException("Method not implemented.");
        }

        public static Vector3 RotateTowards(Vector3 current, Vector3 traget, float maxRadiansDelta, float maxMagnitudeDelta)
        {
            // TODO: Implement this method
            throw new NotImplementedException("Method not implemented.");
        }

        public static Vector3 SmoothDamp(Vector3 current, Vector3 traget, ref Vector3 currentVelocity, float smoothTime)
        {
            return SmoothDamp(current, traget, ref currentVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);
        }

        public static Vector3 SmoothDamp(Vector3 current, Vector3 traget, ref Vector3 currentVelocity, float smoothTime, float maxSpeed)
        {
            return SmoothDamp(current, traget, ref currentVelocity, smoothTime, maxSpeed, Time.deltaTime);
        }

        public static Vector3 SmoothDamp(Vector3 current, Vector3 traget, ref Vector3 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            // TODO: Implement this method
            throw new NotImplementedException("Method not implemented.");
        }

        public static Vector3 Scale(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            Microsoft.Xna.Framework.Vector3 res;
            Microsoft.Xna.Framework.Vector3.Cross(ref a.vector, ref b.vector, out res);
            return new Vector3(res);
        }

        public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal)
        {
            Microsoft.Xna.Framework.Vector3 o;
            Microsoft.Xna.Framework.Vector3.Reflect(ref inDirection.vector, ref inNormal.vector, out o);
            return o;
        }

        public static float Dot(Vector3 lhs, Vector3 rhs)
        {
            float f;
            Microsoft.Xna.Framework.Vector3.Dot(ref lhs.vector, ref rhs.vector, out f);
            return f;
        }

        public static Vector3 Project(Vector3 a, Vector3 b)
        {
            // TODO: Implement this method
            throw new NotImplementedException("Method not implemented.");
        }

        public static float Angle(Vector3 a, Vector3 b)
        {
            // TODO: Implement this method
            throw new NotImplementedException("Method not implemented.");
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            float res;
            Microsoft.Xna.Framework.Vector3.Distance(ref a.vector, ref b.vector, out res);
            return res;
        }

        public Vector3 ClampMagnitude(Vector3 vector, float maxLength)
        {
            // TODO: Implement this method
            throw new NotImplementedException("Method not implemented.");
        }

        public Vector3 Min(Vector3 lhs, Vector3 rhs)
        {
            Microsoft.Xna.Framework.Vector3 res;
            Microsoft.Xna.Framework.Vector3.Min(ref lhs.vector, ref rhs.vector, out res);
            return new Vector3(res);
        }

        public Vector3 Max(Vector3 lhs, Vector3 rhs)
        {
            Microsoft.Xna.Framework.Vector3 res;
            Microsoft.Xna.Framework.Vector3.Max(ref lhs.vector, ref rhs.vector, out res);
            return new Vector3(res);
        }
        #endregion
    }
}
