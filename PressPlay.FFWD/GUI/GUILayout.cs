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
            // TODO: Implement this
        }

        public static void Label(string text, GUIStyle style)
        {
            // TODO: Implement this
        }

        public static bool Toggle(bool value, string text, params GUILayoutOption[] options)
        {
            return DoToggle(value, GUIContent.Temp(text), GUI.skin.toggle, options);
        }

        // TODO:
        //public static bool Toggle(bool value, GUIContent content, GUIStyle style, GUILayoutOption[] options)
        //public static bool Toggle(bool value, Texture image, GUIStyle style, GUILayoutOption[] options)
        //public static bool Toggle(bool value, Texture image, GUILayoutOption[] options)
        //public static bool Toggle(bool value, string text, GUIStyle style, GUILayoutOption[] options)
        //public static bool Toggle(bool value, GUIContent content, GUILayoutOption[] options)

        private static bool DoToggle(bool value, GUIContent content, GUIStyle style, GUILayoutOption[] options)
        {
            // TODO: Implement this
            return false;
        }
        
    }
}
