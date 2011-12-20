using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class GUISkin : ScriptableObject
    {
        private static Dictionary<string, GUIStyle> styles = new Dictionary<string, GUIStyle>();

        public GUIStyle button
        {
            get
            {
                return GetStyle("button");
            }
        }

        public GUIStyle GetStyle(string name)
        {
            if (!styles.ContainsKey(name))
            {
                switch (name)
                {
                    case "button":
                        styles.Add("button", new GUIStyle()
                        {
                            alignment = TextAnchor.MiddleCenter,
                            fixedHeight = 21,
                            margin = new RectOffset() { bottom = 4, top = 4},
                            normal = new GUIStyleState() { background = Texture2D.LoadFromResource("button.PNG") }
                        });
                        break;
                    default:
                        styles.Add(name, new GUIStyle());
                        break;
                }
            }
            return styles[name];
        }
    }
}
