using System;

namespace CsNet.Dispatcher
{
    /// <summary>
    /// “工人”，从Dispatcher处领取并执行任务。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    abstract class Worker<T> : Loopable
    {
        private Dispatcher<T> m_dispatcher;

        public Worker(Dispatcher<T> dispatcher)
        {
            m_dispatcher = dispatcher;
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
        /// 执行具体的任务。
        /// </summary>
        /// <param name="task"></param>
        protected abstract void Work(T task);
    }
}
