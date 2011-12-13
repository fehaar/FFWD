using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public struct Plane
    {
        private Microsoft.Xna.Framework.Plane p;

        public Plane(Vector3 inNormal, float d)
        {
            p = new Microsoft.Xna.Framework.Plane(inNormal, d);
        }

        public Plane(Vector3 inNormal, Vector3 inPoint)
        {
            if (inNormal == Vector3.up)
            {
                p = new Microsoft.Xna.Framework.Plane(inPoint, Vector3.forward + inPoint, Vector3.right + inPoint);
            }
            else
            {
                Microsoft.Xna.Framework.Vector3 u = Microsoft.Xna.Framework.Vector3.Cross(inNormal, Microsoft.Xna.Framework.Vector3.Backward);
                Microsoft.Xna.Framework.Vector3 v = Microsoft.Xna.Framework.Vector3.Cross(inNormal, u);
                Microsoft.Xna.Framework.Vector3 pt = inPoint;
                p = new Microsoft.Xna.Framework.Plane(inPoint, pt + u, pt + v);
            }
        }

        public Vector3 normal
        {
            get
            {
                return p.Normal;
            }
            set
            {
                p.Normal = value;
            }
        }
        public float distance
        {
            get
            {
                return p.D;
            }
            set
            {
                p.D = value;
            }
        }

        public float GetDistanceToPoint(Vector3 point)
        {
            return Microsoft.Xna.Framework.Vector3.Dot(Microsoft.Xna.Framework.Vector3.Normalize(p.Normal), point) - p.D;
        }

        public bool Raycast(Ray ray, out float enter)
        {
            float? f = ray.Intersects(p);
            if (f.HasValue)
            {
                enter = f.Value;
                return true;
            }
            enter = Single.NaN;
            return false;
        }
    }
}
