using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public struct Rect
    {
        public Rect(float left, float top, float w, float h)
        {
            _xMin = x = left;
            _yMin = y = top;
            width = w;
            height = h;
            _xMax = x + width;
            _yMax = y + height;
        }

        public float x;
        public float y;
        public float width;
        public float height;

        private float _xMin;
        public float xMin
        {
            get
            {
                return _xMin;
            }
            set
            {
                _xMin = value;
            }
        }

        private float _yMin;
        public float yMin
        {
            get
            {
                return _yMin;
            }
            set
            {
                _yMin = value;
            }
        }

        private float _xMax;
        public float xMax
        {
            get
            {
                return _xMax;
            }
            set
            {
                _xMax = value;
            }
        }

        private float _yMax;
        public float yMax
        {
            get
            {
                return _yMax;
            }
            set
            {
                _yMax = value;
            }
        }

        public bool Contains(Vector2 point)
        {
            return (point.x >= _xMin && point.y >= _yMin && point.x <= _xMax && point.y <= _yMax);
        }

        public bool Contains(Vector3 point)
        {
            return (point.x >= _xMin && point.y >= _yMin && point.x <= _xMax && point.y <= _yMax);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public static implicit operator Microsoft.Xna.Framework.Rectangle(Rect r)
        {
            return new Microsoft.Xna.Framework.Rectangle((int)r.x, (int)r.y, (int)r.width, (int)r.height);
        }

        public static bool operator ==(Rect a, Rect b)
        {
            return (a._xMax == b._xMax
                && a._yMax == b._yMax
                && a.width == b.width
                && a.height == b.height                
                && a.x == b.x
                && a.y == b.y);
        }

        public static bool operator !=(Rect a, Rect b)
        {
            return !(a == b);
        }
    }
}
