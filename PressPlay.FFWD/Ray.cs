using System;

namespace PressPlay.FFWD
{
    

    struct Ray
    {
        private Microsoft.Xna.Framework.Ray ray = new Microsoft.Xna.Framework.Ray();

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

        public Ray()
        {
            this.origin = Vector3.zero;
            this.direction = Vector3.zero;
        }

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }

        public Vector3 GetPoint(float distance)
        {
            return origin + direction * distance;
        }
    }
}
