using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class GUISkin : ScriptableObject
    {
        private Dictionary<string, GUIStyle> styles = new Dictionary<string, GUIStyle>();

        public GUIStyle GetStyle(string name)
        {
            if (!styles.ContainsKey(name))
            {
                styles.Add(name, new GUIStyle());
            }
            return styles[name];
        }
    }
}
