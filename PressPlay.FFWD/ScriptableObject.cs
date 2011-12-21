using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class ScriptableObject : UnityObject
    {
        public static T CreateInstance<T>() where T : ScriptableObject
        {
            return Activator.CreateInstance<T>();
        }

        public static ScriptableObject CreateInstance(Type type)
        {
            return (ScriptableObject)Activator.CreateInstance(type);
        }

        public static ScriptableObject CreateInstance(string typeName)
        {
            throw new NotImplementedException("Somethings is off here. We don't find the types involved...");
            Type t = Type.GetType(typeName);
            return CreateInstance(t);
        }

        public virtual void OnEnable()
        {
            // NOTE: Do not make any code here. Typically base method is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void OnDisable()
        {
            // NOTE: Do not make any code here. Typically base method is NOT called in MonoScripts so this will not be called either!!!!!
        }
    }
}
