using System;

namespace CsNet.Dispatcher
{
    /// <summary>
    /// Master，向Dispatcher派发任务。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Master<T> : Loopable
    {
        private Dispatcher<T> m_dispatcher;

        public Master(Dispatcher<T> dispatcher)
        {
            m_dispatcher = dispatcher;
        }

        private Master()
        {
        }

        /// <summary>
        /// 派发任务
        /// </summary>
        /// <param name="task"></param>
        protected bool Produce(T task)
        {
            return m_dispatcher.Produce(task);
        }
    }
}
