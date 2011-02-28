using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD
{
    public class TypeSet : ICollection<Type>
    {
        private Dictionary<Type, short> dict;

        public TypeSet()
        {
            dict = new Dictionary<Type, short>();
        }

        public void Add(Type item)
        {
            if (!dict.ContainsKey(item))
            {
                dict.Add(item, 0);
            }
        }

        public void Clear()
        {
            dict.Clear();
        }

        public bool Contains(Type item)
        {
            return dict.ContainsKey(item);
        }

        public void CopyTo(Type[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Type item)
        {
            return dict.Remove(item);
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return dict.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.Keys.GetEnumerator();
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
                Add(Type.GetType(item));
            }
        }
    }
}
