using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public struct Bounds
    {
        public Bounds(Microsoft.Xna.Framework.BoundingBox b)
        {
            box = b;
        }

        public Bounds(Vector3 center, Vector3 size)
        {
            Vector3 ext = size / 2;
            box = new Microsoft.Xna.Framework.BoundingBox(center - ext, center + ext);
        }

        private Microsoft.Xna.Framework.BoundingBox box;

        public Vector3 center
        {
            get
            {
                return (box.Min + box.Max) / 2;
            }
            set
            {
                box = new Microsoft.Xna.Framework.BoundingBox(value - extents, value + extents);
            }
        }

        public Vector3 size
        {
            get
            {
                return (box.Max - box.Min);
            }
        }

        public Vector3 extents
        {
            get
            {
                return size / 2;
            }
        }

        public Vector3 min
        {
            get
            {
                return box.Min;
            }
        }

        public Vector3 max
        {
            get
            {
                return box.Max;
            }
        }

        #region Public functions
        public void SetMinMax(Vector3 min, Vector3 max)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");

        }

        public void Encapsulate(Vector3 point)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");

        }

        public void Expand(float amount)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");

        }

        public bool Intersects(Bounds b)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");

        }

        public void Contains(Vector3 point)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");

        }

        public void SqrDistance(Vector3 point)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");

        }

        public bool IntersectRay(Microsoft.Xna.Framework.Ray r)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");

        }

        public override string ToString()
        {
            return box.ToString();
        }
        #endregion
    }
}
