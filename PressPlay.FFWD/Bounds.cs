using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision;

namespace PressPlay.FFWD
{
    public struct Bounds
    {
        public Bounds(Microsoft.Xna.Framework.BoundingBox b)
        {
            _boundingSphere = null;
            box = b;
        }

        public Bounds(Vector3 center, Vector3 size)
        {
            _boundingSphere = null;
            Vector3 ext = size / 2;
            box = new Microsoft.Xna.Framework.BoundingBox(center - ext, center + ext);
        }

        private Microsoft.Xna.Framework.BoundingBox box;

        [ContentSerializer(ElementName="c")]
        public Vector3 center
        {
            get
            {
                return (box.Min + box.Max) / 2;
            }
            set
            {
                box = new Microsoft.Xna.Framework.BoundingBox(value - extents, value + extents);
                _boundingSphere = null;
            }
        }

        public Vector3 size
        {
            get
            {
                return (box.Max - box.Min);
            }
        }

        [ContentSerializer(ElementName = "e")]
        public Vector3 extents
        {
            get
            {
                return size / 2;
            }
            set
            {
                Vector3 c = center;

                box.Max = c + value;
                box.Min = c - value;
                _boundingSphere = null;
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
            box = new Microsoft.Xna.Framework.BoundingBox(min, max);
            _boundingSphere = null;
        }

        public void Encapsulate(Bounds bounds)
        {
            box = BoundingBox.CreateMerged(box, bounds.box);
            _boundingSphere = null;
        }

        public void Encapsulate(Vector3 point)
        {
            if (box.Contains(point) == ContainmentType.Disjoint)
            {
                box = BoundingBox.CreateFromPoints(new Microsoft.Xna.Framework.Vector3[] { point, box.Min, box.Max });
            }
        }

        public void Encapsulate(Microsoft.Xna.Framework.Vector3[] points)
        {
            if (points == null || points.Length == 0)
            {
                box = new BoundingBox();
                return;
            }
            box = BoundingBox.CreateFromPoints(points);
            _boundingSphere = null;
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

        public bool Contains(Vector3 point)
        {
            return box.Contains(point) != ContainmentType.Disjoint;
        }

        public void SqrDistance(Vector3 point)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");

        }

        public bool IntersectRay(Microsoft.Xna.Framework.Ray r)
        {
            float? res = box.Intersects(r);
            return res.HasValue;
        }

        public override string ToString()
        {
            return box.ToString();
        }
        #endregion

        private BoundingSphere? _boundingSphere;
        internal BoundingSphere boundingSphere
        {
            get
            {
                if (!_boundingSphere.HasValue)
                {
                    _boundingSphere = new BoundingSphere(center, Math.Max(Math.Max(size.x, size.y), size.z) / 2);
                }
                return _boundingSphere.Value;
            }
        }

        internal static Bounds FromAABB(ref AABB aabb, Physics.To2dMode to2dMode, Vector3 size)
        {
            Vector3 center = VectorConverter.Convert(aabb.Center, to2dMode);
            Vector3 sz = VectorConverter.Convert(aabb.Extents * 2, to2dMode);
            switch (to2dMode)
            {
                case Physics.To2dMode.DropX:
                    sz.x = size.x;
                    break;
                case Physics.To2dMode.DropY:
                    sz.y = size.y;
                    break;
                case Physics.To2dMode.DropZ:
                    sz.z = size.z;
                    break;
            }
            return new Bounds(center, sz);
        }
    }
}
