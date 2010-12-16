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

        private Microsoft.Xna.Framework.Vector3 vector;

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

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.vector - b.vector);
        }

        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.vector * b.vector);
        }

        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            return new Vector3(a.vector / b.vector);
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.vector == b.vector;
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return a.vector != b.vector;
        }

        public Vector3 normalized
        {
            get
            {
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

        public void Normalize()
        {
            vector.Normalize();
        }

        public static float Dot(Vector3 lhs, Vector3 rhs)
        {
            return Microsoft.Xna.Framework.Vector3.Dot(lhs.vector, rhs.vector);
        }

        public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal)
        {
            return Microsoft.Xna.Framework.Vector3.Reflect(inDirection.vector, inNormal.vector);
        }

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

        private static Vector3 _forward = new Vector3(0, 0, 1);
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

        public override bool Equals(object obj)
        {
            if (obj is Vector3)
	        {
                return vector.Equals(((Vector3)obj).vector);
	        }
            return false;
        }

        public override int GetHashCode()
        {
            return vector.GetHashCode();
        }
    }
}
