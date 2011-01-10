using System;

namespace PressPlay.FFWD
{
    public struct Quaternion
    {
        #region Constructors
        public Quaternion(float x, float y, float z, float w)
        {
            quaternion = new Microsoft.Xna.Framework.Quaternion(x, y, z, w);
        }

        public Quaternion(Microsoft.Xna.Framework.Quaternion q)
        {
            quaternion = q;
        }
        #endregion

        internal Microsoft.Xna.Framework.Quaternion quaternion;

        #region Properties
		public float x
        {
            get
            {
                return quaternion.X;
            }
            set
            {
                quaternion.X = value;
            }
        }

        public float y
        {
            get
            {
                return quaternion.Y;
            }
            set
            {
                quaternion.Y = value;
            }
        }

        public float z
        {
            get
            {
                return quaternion.Z;
            }
            set
            {
                quaternion.Z = value;
            }
        }

        public float w
        {
            get
            {
                return quaternion.W;
            }
            set
            {
                quaternion.W = value;
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
                        throw new IndexOutOfRangeException("You must use 0 (x), 1 (y), 2 (z) or 3 (w) as index to the quaternion.");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        quaternion.X = value;
                        break;
                    case 1:
                        quaternion.Y = value;
                        break;
                    case 2:
                        quaternion.Z = value;
                        break;
                    case 3:
                        quaternion.W = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("You must use 0 (x), 1 (y), 2 (z) or 3 (w) as index to the quaternion.");
                }
            }
        }

        public Vector3 eulerAngles
        {
            get
            {
                return QuaternionToEulerAngleVector3(this);
            }
        }
    	#endregion    

        #region Public methods
        public void ToAngleAxis(out float angle, out Vector3 axis)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");
        }

        public void SetFromToRotation(Vector3 fromDirection, Vector3 toDirection)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");

        }

        public void SetLookRotation(Vector3 view)
        {
            SetLookRotation(view, Vector3.up);
        }

        public void SetLookRotation(Vector3 view, Vector3 up)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");
        }
        #endregion

        private static Quaternion _identity = new Quaternion(Microsoft.Xna.Framework.Quaternion.Identity);
        public static Quaternion identity
        {
            get
            {
                return _identity;
            }
        }

        #region Operators
        public static implicit operator Microsoft.Xna.Framework.Quaternion(Quaternion q)
        {
            return q.quaternion;
        }

        public static implicit operator Quaternion(Microsoft.Xna.Framework.Quaternion q)
        {
            return new Quaternion(q);
        }

        public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
        {
            return new Quaternion(lhs.quaternion * rhs.quaternion);
        }

        public static Vector3 operator *(Quaternion rotation, Vector3 point)
        {
            return new Vector3(Microsoft.Xna.Framework.Vector3.Transform(point, rotation.quaternion));
        }

        public static bool operator ==(Quaternion lhs, Quaternion rhs)
        {
            return lhs.quaternion == rhs.quaternion;
        }

        public static bool operator !=(Quaternion lhs, Quaternion rhs)
        {
            return lhs.quaternion != rhs.quaternion;
        }
        #endregion

        #region Overrides
        public override bool Equals(object obj)
        {
            if (obj is Quaternion)
	        {
                return ((Quaternion)obj).Equals(quaternion);
	        }
            if (obj is Microsoft.Xna.Framework.Quaternion)
            {
                return ((Microsoft.Xna.Framework.Quaternion)obj).Equals(quaternion);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return quaternion.GetHashCode();
        }

        public override string ToString()
        {
            return quaternion.ToString();
        }
        #endregion

        #region Private methods
        // Source: http://www.innovativegames.net/blog/blog/2009/03/18/matrices-quaternions-and-euler-angle-vectors/
        private Vector3 AngleTo(Vector3 from, Vector3 location)
        {
            Vector3 angle = new Vector3();
            Vector3 v3 = (location - from).normalized;

            angle.x = (float)Math.Asin(v3.y);
            angle.y = (float)Math.Atan2((double)-v3.x, (double)-v3.z);

            return angle;
        }

        // Source: http://www.innovativegames.net/blog/blog/2009/03/18/matrices-quaternions-and-euler-angle-vectors/
        Vector3 QuaternionToEulerAngleVector3(Quaternion rotation)
        {
            Vector3 rotationaxes = new Vector3();
            Vector3 forward = rotation * Vector3.forward;
            Vector3 up = rotation * Vector3.up;

            rotationaxes = AngleTo(new Vector3(), forward);

            if (rotationaxes.x == Microsoft.Xna.Framework.MathHelper.PiOver2)
            {
                rotationaxes.y = (float)Math.Atan2((double)up.x, (double)up.z);
                rotationaxes.z = 0;
            }
            else if (rotationaxes.x == -Microsoft.Xna.Framework.MathHelper.PiOver2)
            {
                rotationaxes.y = (float)Math.Atan2((double)-up.x, (double)-up.z);
                rotationaxes.z = 0;
            }
            else
            {
                up = Microsoft.Xna.Framework.Vector3.Transform(up, Microsoft.Xna.Framework.Matrix.CreateRotationY(-rotationaxes.y));
                up = Microsoft.Xna.Framework.Vector3.Transform(up, Microsoft.Xna.Framework.Matrix.CreateRotationX(-rotationaxes.x));

                rotationaxes.z = (float)Math.Atan2((double)-up.x, (double)up.y);
            }

            return new Vector3(Microsoft.Xna.Framework.MathHelper.ToDegrees(rotationaxes.x), Microsoft.Xna.Framework.MathHelper.ToDegrees(rotationaxes.y), Microsoft.Xna.Framework.MathHelper.ToDegrees(rotationaxes.z));
        }
        #endregion

        #region Static methods
        public static float Dot(Quaternion a, Quaternion b)
        {
            float dot;
            Microsoft.Xna.Framework.Quaternion.Dot(ref a.quaternion, ref b.quaternion, out dot);
            return dot;
        }

        public static Quaternion AngleAxis(float angle, Vector3 axis)
        {
            return new Quaternion(Microsoft.Xna.Framework.Quaternion.CreateFromAxisAngle(axis, Microsoft.Xna.Framework.MathHelper.ToRadians(angle)));
        }

        public static Quaternion FromToRotation(Vector3 from, Vector3 to)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");
        }

        public static Quaternion Slerp(Quaternion from, Quaternion to, float t)
        {
            Microsoft.Xna.Framework.Quaternion q;
            Microsoft.Xna.Framework.Quaternion.Slerp(ref from.quaternion, ref to.quaternion, t, out q);
            return new Quaternion(q);
        }

        public static Quaternion Lerp(Quaternion from, Quaternion to, float t)
        {
            Microsoft.Xna.Framework.Quaternion q;
            Microsoft.Xna.Framework.Quaternion.Lerp(ref from.quaternion, ref to.quaternion, t, out q);
            return new Quaternion(q);
        }

        public static Quaternion RotateTowards(Quaternion from, Quaternion to, float maxDegreesDelta)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");
        }

        public static Quaternion Inverse(Quaternion q)
        {
            return new Quaternion(Microsoft.Xna.Framework.Quaternion.Inverse(q.quaternion));
        }

        public static Quaternion Angle(Quaternion from, Quaternion to)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");
        }

        public static Quaternion Euler(float x, float y, float z)
        {
            return new Quaternion(Microsoft.Xna.Framework.Quaternion.CreateFromYawPitchRoll(Microsoft.Xna.Framework.MathHelper.ToRadians(y), Microsoft.Xna.Framework.MathHelper.ToRadians(x), Microsoft.Xna.Framework.MathHelper.ToRadians(z)));
        }

        public static Quaternion Euler(Vector3 v)
        {
            return Euler(v.x, v.y, v.z);
        }

        public static Quaternion LookRotation(Vector3 forward)
        {
            return LookRotation(forward, Vector3.up);
        }

        public static Quaternion LookRotation(Vector3 forward, Vector3 up)
        {
            return new Quaternion(Microsoft.Xna.Framework.Quaternion.CreateFromRotationMatrix(Microsoft.Xna.Framework.Matrix.CreateWorld(Microsoft.Xna.Framework.Vector3.Zero, forward, up)));
        }
        #endregion
    }
}
