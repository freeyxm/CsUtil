using System;

namespace CsNet.Dispatcher
{
    /// <summary>
    /// “工人”，从Dispatcher处领取并执行任务。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    abstract class Worker<T>
    {
        private Dispatcher<T> m_dispatcher;
        private bool m_quit;
        private bool m_running;

        public Worker(Dispatcher<T> dispatcher)
        {
            m_dispatcher = dispatcher;
            m_running = false;
            m_quit = false;
        }

        public void Run()
        {
            m_running = true;

            while (!m_quit)
            {
                T task = default(T);
                if (m_dispatcher.Consume(ref task))
                {
                    Work(task);
                }
            }

            m_running = false;
        }

        /// <summary>
        /// 指定退出标记，当前任务完成后退出。
        /// </summary>
        public void Quit()
        {
            m_quit = true;
        }

        public bool Running { get { return m_running; } }

        /// <summary>
        /// 执行具体的任务。
        /// </summary>
        /// <param name="task"></param>
        protected abstract void Work(T task);
    }
}
