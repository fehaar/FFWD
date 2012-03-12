using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.Components
{
    internal class RenderQueue : IComparer<RenderItem>
    {
        public RenderQueue()
        {
            list = new List<RenderItem>(ApplicationSettings.DefaultCapacities.RenderQueues);
        }

        private List<RenderItem> list;

        public void Add(RenderItem item)
        {
            int index = list.BinarySearch(item, this);
            if (index < 0)
            {
                list.Insert(~index, item);
            }
            else
            {
                list.Insert(index, item);
            }
        }

        public RenderItem this[int index]
        {
            get
            {
                return list[index];
            }
        }

        public int Count
        {
            get 
            {
                return list.Count;
            }
        }

        public int Compare(RenderItem x, RenderItem y)
        {
            return x.Priority.CompareTo(y.Priority);
        }
    }
}
