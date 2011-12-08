using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public enum TextAnchor { UpperLeft, UpperCenter, UpperRight, MiddleLeft, MiddleCenter, MiddleRight, LowerLeft, LowerCenter, LowerRight }

    public class GUIStyle
    {
        public static readonly GUIStyle none = new GUIStyle();

        public int fontSize;
        public TextAnchor alignment;
    }
}
