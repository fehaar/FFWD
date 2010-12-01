using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class Debug
    {
        public static void Log(object message)
        {
            System.Diagnostics.Debug.WriteLine(message.ToString());
        }

        private static Dictionary<string, string> _debugDisplay = new Dictionary<string, string>();

        public static void Display(string key, object value)
        {
            _debugDisplay[key] = value.ToString();
        }

        public static IEnumerable<KeyValuePair<string, string>> DisplayStrings
        {
            get
            {
                return _debugDisplay;
            }
        }
    }
}
