using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    /// <summary>
    /// This is a special case priority queue that holds prioritized items that are pooled to minimize the amount of
    /// memory that is allocated when adding items.
    /// </summary>
    internal class PooledPriorityQueue : IComparer<PooledPriorityQueue.PooledPriorityQueueItem>
    {
        internal class PooledPriorityQueueItem 
        {
            public int id;
            public float priority;
        }

        private static Queue<PooledPriorityQueueItem> itemPool = null;

        private List<PooledPriorityQueueItem> internalList = null;

        public PooledPriorityQueue(int capacity)
        {
            internalList = new List<PooledPriorityQueueItem>(capacity);
            if (itemPool == null)
            {
                // Initialize the item pool
                itemPool = new Queue<PooledPriorityQueueItem>(ApplicationSettings.DefaultCapacities.PriorityQueuePoolSize);
                for (int i = 0; i < ApplicationSettings.DefaultCapacities.PriorityQueuePoolSize; i++)
                {
                    itemPool.Enqueue(new PooledPriorityQueueItem());
                }
            }
        }

        public void Add(int id, float priority)
        {
            PooledPriorityQueueItem item = TryGetItemFromPool();
            item.id = id;
            item.priority = priority;
            int index = internalList.BinarySearch(item, this);
            if (index < 0)
            {
                internalList.Insert(~index, item);
            }
            else
            {
                internalList.Insert(index, item);
            }
        }

        private PooledPriorityQueueItem TryGetItemFromPool()
        {
            if (itemPool.Count > 0)
            {
                return itemPool.Dequeue();
            }
            return new PooledPriorityQueueItem();
        }

        public void Clear()
        {
            for (int i = 0; i < internalList.Count; i++)
            {
                itemPool.Enqueue(internalList[i]);
            }
            internalList.Clear();
        }

        public int Count
        {
            get
            {
                return internalList.Count;
            }
        }

        public int this[int index]
        {
            get
            {
                return internalList[index].id;
            }
        }

        public int Compare(PooledPriorityQueue.PooledPriorityQueueItem x, PooledPriorityQueue.PooledPriorityQueueItem y)
        {
            return x.priority.CompareTo(y.priority);
        }

        internal void Remove(int transformId)
        {
            for (int i = internalList.Count - 1; i >= 0; i--)
            {
                if (internalList[i].id == transformId)
                {
                    internalList.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
