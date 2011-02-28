using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD
{
    public class TypeSet : ICollection<string>
    {
        private Dictionary<int, string> dict;

        public TypeSet()
        {
            dict = new Dictionary<int, string>();
        }

        public void Add(Type tp)
        {
            Add(tp.AssemblyQualifiedName);
        }

        public void Add(string item)
        {
            int hash = item.GetHashCode();
            if (!dict.ContainsKey(hash))
            {
                dict.Add(hash, item);
            }
        }

        public void Clear()
        {
            dict.Clear();
        }

        public bool Contains(Type tp)
        {
            return Contains(tp.AssemblyQualifiedName);
        }

        public bool Contains(string item)
        {
            return dict.ContainsKey(item.GetHashCode());
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string item)
        {
            return dict.Remove(item.GetHashCode());
        }

        public IEnumerator<string> GetEnumerator()
        {
            return dict.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.Values.GetEnumerator();
        }

        // Properties
        public int Count
        {
            get { return dict.Keys.Count(); } 
        }

        public bool IsReadOnly 
        {
            get { return false; }
        }

        public void AddRange(IEnumerable<string> types)
        {
            foreach (string item in types)
            {
                Add(item);
            }
        }
    }
}
