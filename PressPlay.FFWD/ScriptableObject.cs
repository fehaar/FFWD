using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class ScriptableObject : UnityObject
    {
        public static ScriptableObject CreateInstance<T>() where T : ScriptableObject
        {
            throw new NotImplementedException();
        }

        public static ScriptableObject CreateInstance(Type type)
        {
            throw new NotImplementedException();
        }

        public static ScriptableObject CreateInstance(string typeName)
        {
            throw new NotImplementedException();
        }
    }
}
