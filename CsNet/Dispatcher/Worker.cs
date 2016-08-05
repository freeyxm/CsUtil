using System;

namespace CsNet.Dispatcher
{
    /// <summary>
    /// Worker，从Dispatcher分配任务。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Worker<T> : Loopable
    {
        private Dispatcher<T> m_dispatcher;

        public Worker(Dispatcher<T> dispatcher)
        {
            m_dispatcher = dispatcher;
        }

        private Worker()
        {
        }

        protected sealed override void Loop()
        {
            T task = default(T);
            if (m_dispatcher.Consume(ref task))
            {
                Work(task);
            }
        }

        /// <summary>
        /// 执行任务。
        /// </summary>
        /// <param name="task"></param>
        protected abstract void Work(T task);
    }
}
