using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public struct Plane
    {
        public Plane(Vector3 inNormal, float d)
        {
            normal = inNormal;
            distance = d;
        }

        public Plane(Vector3 inNormal, Vector3 inPoint)
        {
            // TODO: Implement this!
            normal = inNormal;
            distance = 0;
        }

        public Vector3 normal;
        public float distance;

        public float GetDistanceToPoint(Vector3 tl)
        {
            // TODO: Implement this!
            return 1;
        }
    }
}
