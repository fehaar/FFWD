using System;
using System.Collections.Generic;
using System.Linq;

namespace PressPlay.FFWD
{
    public class Debug
    {
        private static List<UnityObject> gosToDisplay = new List<UnityObject>();
        private static Dictionary<string, string> _debugDisplay = new Dictionary<string, string>();

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

        public static void Display(string key, object value)
        {
            _debugDisplay[key] = value.ToString();
        }

        public static IEnumerable<KeyValuePair<string, string>> DisplayStrings
        {
            get
            {
                return _debugDisplay.Concat(ObjectDisplays);
            }
        }

        public static void DisplayHierarchy(UnityObject obj)
        {
            gosToDisplay.Add(obj);
        }

        public static IEnumerable<KeyValuePair<string, string>> ObjectDisplays
        {
            get
            {
                foreach (UnityObject obj in gosToDisplay)
                {
                    Transform trans = (obj is GameObject) ? (obj as GameObject).transform : (obj as Component).transform;
                    while (trans != null)
                    {
                        yield return new KeyValuePair<string, string>(trans.name, trans.transform.position.ToString());
                        trans = trans.transform.parent;
                    }
                }
            }
        }

        public static void DrawLine(Vector3 start, Vector3 end)
        {
            DrawLine(start, end, Color.white);
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            // TODO : Add implementation of method
            //throw new NotImplementedException("Method not implemented.");
        }

        public static void DrawRay(Vector3 start, Vector3 end, Color color)
        {
            // TODO : Add implementation of method
            //throw new NotImplementedException("Method not implemented.");
        }
    }
}
