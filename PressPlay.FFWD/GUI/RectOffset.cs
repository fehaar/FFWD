using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public struct RectOffset
    {
        public int left;
        public int right;
        public int top;
        public int bottom;

        public int horizontal
        {
            get
            {
                return left + right;
            }
        }

        public int vertical
        {
            get
            {
                return top + bottom;
            }
        }
    }
}
