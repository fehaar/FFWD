using System;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD
{
    public static class GUI
    {
        public static GUISkin skin { get; set; }
        public static Color backgroundColor { get; set; }
        public static Color color { get; set; }

        public static void DrawTexture(Rect rect, Microsoft.Xna.Framework.Graphics.Texture m_HealthBarBackgroundTexture)
        {
            throw new NotImplementedException();
        }

        public static void Label(Rect rect, string text)
        {
            Label(rect, text, GUIStyle.none);
        }

        public static void Label(Rect rect, string text, GUIStyle style)
        {
            throw new NotImplementedException();
        }

        public static bool Button(Rect rect, string text)
        {
            throw new NotImplementedException();
        }

        public static bool Button(Rect rect, Texture2D tex)
        {
            return Button(rect, tex, GUIStyle.none);
        }

        public static bool Button(Rect rect, Texture2D tex, GUIStyle style)
        {
            throw new NotImplementedException();
        }

        public static string TextField(Rect rect, string m_strLevelName)
        {
            throw new NotImplementedException();
        }
    }
}
