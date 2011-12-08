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
            // TODO: Test that this is true
            p = new Microsoft.Xna.Framework.Plane(inNormal, inPoint.magnitude);
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

        public float GetDistanceToPoint(Vector3 tl)
        {            
            // TODO: Implement this!
            return 1;
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
