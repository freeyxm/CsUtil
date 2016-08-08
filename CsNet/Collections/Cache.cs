using System;
using System.Collections.Generic;

namespace CsNet.Collections
{
    class Cache<T> where T : new()
    {
        protected Queue<T> m_cacheQueue;

        public Cache(int capacity)
        {
            m_cacheQueue = new Queue<T>(capacity);
            for (int i = 0; i < capacity; ++i)
            {
                m_cacheQueue.Enqueue(NewNode());
            }
        }

        public virtual T AllocNode()
        {
            if (m_cacheQueue.Count > 0)
                return m_cacheQueue.Dequeue();
            else
                return NewNode();
        }

        public virtual void FreeNode(T obj)
        {
            m_cacheQueue.Enqueue(obj);
        }

        protected virtual T NewNode()
        {
            return new T();
        }
    }
}
