using System;
using System.Collections.Generic;

namespace CsUtil.Pool
{
    public class DataPoolFactory
    {
        private static List<IDataPool> m_poolList = new List<IDataPool>();

        public static void CreatePool<T>(int capacity) where T : new()
        {
            if (DataPool<T>.Instance != null)
            {
                DataPool<T>.Instance.SetCapacity(capacity);
                if (m_poolList.Contains(DataPool<T>.Instance) == false)
                {
                    m_poolList.Add(DataPool<T>.Instance);
                }
            }
            else
            {
                m_poolList.Add(new DataPool<T>(capacity));
            }
        }

        public static void DeletePool<T>() where T : new()
        {
            if (DataPool<T>.Instance != null)
            {
                m_poolList.Remove(DataPool<T>.Instance);
                DataPool<T>.Instance.Release();
            }
        }

        public static void Clear()
        {
            for (int i = 0; i < m_poolList.Count; ++i)
            {
                m_poolList[i].Clear();
            }
        }

        public static void Release()
        {
            for (int i = 0; i < m_poolList.Count; ++i)
            {
                m_poolList[i].Release();
            }
            m_poolList.Clear();
        }
    }
}
