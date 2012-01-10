using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class GUIContent
    {
        private static GUIContent s_Image;
        private static GUIContent s_Text;
        private static GUIContent s_TextImage;

        internal protected string m_Text;

        internal static protected GUIContent Temp(string t)
        {
            s_Text.m_Text = t;
            return s_Text;
        }

        public GUIContent(string text)
        {         

        }
    }
}
