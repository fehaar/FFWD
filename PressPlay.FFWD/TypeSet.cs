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
        private Dictionary<string, short> dict;

        public TypeSet()
        {
            dict = new Dictionary<string, short>();
        }

        public void Add(Type tp)
        {
            Add(tp.Name);
        }

        public void Add(string item)
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

        public bool Contains(Type tp)
        {
            return Contains(tp.Name);
        }

        public bool Contains(string item)
        {
            return dict.ContainsKey(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string item)
        {
            return dict.Remove(item);
        }

        public IEnumerator<string> GetEnumerator()
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
                Add(item);
            }
        }
    }
}
