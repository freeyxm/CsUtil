using System;
using System.Collections.Generic;
using System.Threading;

namespace CsNet.Dispatcher
{
    /// <summary>
    /// 分发中心，使用生产者-消费者模型。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    abstract class Dispatcher<T>
    {
        protected Queue<T> m_taskQueue;
        private Semaphore m_producter;
        private Semaphore m_consumer;

        /// <summary>
        /// </summary>
        /// <param name="capacity">容量</param>
        /// <param name="initSize">初始队列容量</param>
        public Dispatcher(int capacity, int initSize)
        {
            m_taskQueue = new Queue<T>(initSize);
            m_producter = new Semaphore(capacity, capacity);
            m_consumer = new Semaphore(0, capacity);
        }

        public void Produce(T task)
        {
            m_producter.WaitOne();
            lock (m_taskQueue)
            {
                m_taskQueue.Enqueue(task);
            }
            m_consumer.Release();
        }

        public bool Consume(ref T task)
        {
            m_consumer.WaitOne();

            bool hasTask = false;
            lock (m_taskQueue)
            {
                if (m_taskQueue.Count > 0)
                {
                    task = m_taskQueue.Dequeue();
                    hasTask = true;
                }
            }
            if (hasTask)
            {
                m_producter.Release();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
