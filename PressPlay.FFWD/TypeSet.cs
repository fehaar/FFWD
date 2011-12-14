using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework.Content;
using System.Reflection;
using PressPlay.FFWD.Components;
using PressPlay.FFWD.Attributes;

namespace PressPlay.FFWD
{
    public class TypeSet
    {
        [Flags]
        public enum TypeCapabilities { None = 0, Update = 1, LateUpdate = 2, FixedUpdate = 4, Awake = 8, GUI = 16, FixReferences = 32 }

        private Dictionary<string, TypeCapabilities> dict;

        public TypeSet()
        {
            dict = new Dictionary<string, TypeCapabilities>();
        }

        public TypeSet(int capacity)
        {
            dict = new Dictionary<string, TypeCapabilities>(capacity);
        }

        public TypeCapabilities Add(Type tp)
        {
            TypeCapabilities caps = TypeCapabilities.None;
            if (tp.IsSubclassOf(typeof(Component)))
            {
                MethodInfo info = tp.GetMethod("Update");
                if (info != null && info.DeclaringType != typeof(MonoBehaviour))
                {
                    caps |= TypeCapabilities.Update;
                }

                info = tp.GetMethod("LateUpdate");
                if (info != null && info.DeclaringType != typeof(MonoBehaviour))
                {
                    caps |= TypeCapabilities.LateUpdate;
                }

                info = tp.GetMethod("FixedUpdate");
                if (info != null && info.DeclaringType != typeof(MonoBehaviour))
                {
                    caps |= TypeCapabilities.FixedUpdate;
                }

                info = tp.GetMethod("Awake");
                if (info != null && info.DeclaringType != typeof(Component))
                {
                    caps |= TypeCapabilities.Awake;
                }

                info = tp.GetMethod("OnGUI");
                if (info != null && info.DeclaringType != typeof(MonoBehaviour))
                {
                    caps |= TypeCapabilities.GUI;
                }
                Add(tp.Name, caps);
            }
            else
            {
                // MonoBehaviours and components does not need to check for FixReferences and we do not wnat other random types to muck up the type set
                if (tp.GetCustomAttributes(typeof(FixReferencesAttribute), true).Length > 0)
                {
                    caps |= TypeCapabilities.FixReferences;
                    Add(tp.Name, caps);
                }
            }
            return caps;
        }

        private void Add(string item, TypeCapabilities caps)
        {
            if (!dict.ContainsKey(item))
            {
                dict.Add(item, caps);
            }
        }

        public bool HasCaps(Type tp, TypeCapabilities caps)
        {
            if (dict.ContainsKey(tp.Name))
            {
                return (dict[tp.Name] & caps) == caps;
            }
            TypeCapabilities c = Add(tp);
            return (c & caps) == caps;
        }

        public bool Remove(string item)
        {
            return dict.Remove(item);
        }

        // Properties
        public int Count
        {
            get { return dict.Keys.Count(); } 
        }

        public void Add(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                string s = list[i];
                int index = s.IndexOf('#');
                string name = s.Substring(0, index);
                dict.Add(name, (TypeCapabilities)Enum.Parse(typeof(TypeCapabilities), s.Substring(index + 1), true));
            }
        }

        internal List<string> ToList()
        {
            List<string> l = new List<string>();
            foreach (var item in dict)
            {
                l.Add(item.Key + "#" + item.Value);
            }
            return l;
        }
    }
}
