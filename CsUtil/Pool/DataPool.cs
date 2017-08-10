using System;
using System.Collections.Generic;
using CsUtil.Util;

namespace CsUtil.Pool
{
    public interface IDataPool
    {
        void Clear();
        void Release();
    }

    public class DataPool<T> : Singleton<DataPool<T>>, IDataPool
        where T : new()
    {
        private Queue<T> m_cache = new Queue<T>();
        private int m_capacity;

        public DataPool(int capacity)
        {
            SetCapacity(capacity);
        }

        public void SetCapacity(int capacity)
        {
            m_capacity = Math.Max(0, capacity);
            while (m_cache.Count > m_capacity)
            {
                m_cache.Dequeue();
            }
        }

        public T Create()
        {
            if (m_cache.Count > 0)
            {
                return m_cache.Dequeue();
            }
            else
            {
                return new T();
            }
        }

        public void Delete(T data)
        {
            if (m_cache.Count < m_capacity)
            {
                m_cache.Enqueue(data);
            }
        }

        public void Clear()
        {
            m_cache.Clear();
        }

        public void Release()
        {
            Clear();
            if (m_instance == this)
            {
                m_instance = null;
            }
        }
    }
}