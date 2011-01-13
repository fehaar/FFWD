using System;

namespace PressPlay.FFWD
{
    public struct Ray
    {
        private Microsoft.Xna.Framework.Ray ray;

        public Vector3 origin
        {
            get
            {
                return ray.Position;
            }
            set
            {
                ray.Position = value;
            }
        }
        public Vector3 direction {
            get {
                return ray.Direction;
            }
            set {
                ray.Direction = value;
            }
        }

        public Ray(Vector3 origin, Vector3 direction)
        {
            ray = new Microsoft.Xna.Framework.Ray();
            this.origin = origin;
            this.direction = direction;
        }

        public Vector3 GetPoint(float distance)
        {
            return origin + direction * distance;
        }

        public float? Intersects(Microsoft.Xna.Framework.Plane plane)
        {
            return ray.Intersects(plane);
        }
    }
}
