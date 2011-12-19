using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public enum TextAnchor { UpperLeft, UpperCenter, UpperRight, MiddleLeft, MiddleCenter, MiddleRight, LowerLeft, LowerCenter, LowerRight }
    public enum FontStyle { Normal, Bold, Italic, BoldAndItalic }

    public class GUIStyle
    {
        public static readonly GUIStyle none = new GUIStyle();

        public int fontSize;
        public int fixedHeight;
        public RectOffset margin;
        public TextAnchor alignment;
        public GUIStyleState normal;
        public GUIStyleState hover;
        public GUIStyleState onNormal;
        public GUIStyleState onActive;
        public GUIStyleState focused;
        public GUIStyleState onFocused;
        public FontStyle fontStyle;
    }
}
