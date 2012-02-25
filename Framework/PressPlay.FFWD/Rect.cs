using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD
{
    public struct Rect
    {
        public static readonly Rect unit = new Rect(0f, 0f, 1f, 1f);

        public Rect(float left, float top, float w, float h)
        {
            x = left;
            y = top;
            width = w;
            height = h;
        }

        public float x;
        public float y;
        public float width;
        public float height;

        [ContentSerializerIgnore]
        public float xMin
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        [ContentSerializerIgnore]
        public float yMin
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        [ContentSerializerIgnore]
        public float xMax
        {
            get
            {
                return x + width;
            }
            set
            {
                width = value - x;
            }
        }

        [ContentSerializerIgnore]
        public float yMax
        {
            get
            {
                return y + height;
            }
            set
            {
                height = value - y;
            }
        }

        public bool Contains(Vector2 point)
        {
            return (point.x >= xMin && point.y >= yMin && point.x <= xMax && point.y <= yMax);
        }

        public bool Contains(Vector3 point)
        {
            return (point.x >= xMin && point.y >= yMin && point.x <= xMax && point.y <= yMax);
        }

        public override string ToString()
        {
            return String.Format("(left:{0:F2}, top:{1:F2}, width:{2:F2}, height:{3:F2})", x, y, width, height);
        }

        public static implicit operator Microsoft.Xna.Framework.Rectangle(Rect r)
        {
            return new Microsoft.Xna.Framework.Rectangle((int)r.x, (int)r.y, (int)r.width, (int)r.height);
        }

        public static bool operator ==(Rect a, Rect b)
        {
            return (a.width == b.width
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
