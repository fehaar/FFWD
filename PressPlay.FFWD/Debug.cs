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

        public static void LogError(string message)
        {
            System.Diagnostics.Debug.WriteLine("ERROR: " + message.ToString());
        }

        public static void LogWarning(string message)
        {
            System.Diagnostics.Debug.WriteLine("Warning: " + message.ToString());
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
