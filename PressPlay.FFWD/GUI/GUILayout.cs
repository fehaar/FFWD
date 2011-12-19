using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class GUILayout
    {
        private static Rect? currentArea;

        public static void BeginArea(Rect rect)
        {
            if (!currentArea.HasValue)
            {
                currentArea = rect;
            }
            else
            {
                throw new InvalidOperationException("We already have an area begun");
            }
        }

        public static bool Button(string text)
        {
            if (currentArea.HasValue)
            {
                Rect r = new Rect(currentArea.Value.x, currentArea.Value.y, currentArea.Value.width, GUI.skin.button.fixedHeight);
                int offset = GUI.skin.button.fixedHeight + GUI.skin.button.margin.bottom;
                currentArea = new Rect(currentArea.Value.x, currentArea.Value.y + offset, currentArea.Value.width, currentArea.Value.height - offset);
                return GUI.Button(r, text);
            }
            return false;
        }

        public static void EndArea()
        {
            currentArea = null;
        }

        public static void Label(string text)
        {
            throw new NotImplementedException();
        }

        public static void Label(string text, GUIStyle style)
        {
            throw new NotImplementedException();
        }
    }
}
