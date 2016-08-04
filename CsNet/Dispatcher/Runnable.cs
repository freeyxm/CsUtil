using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsNet.Dispatcher
{
    public interface IRunnable
    {
        void Run();
    }

    public abstract class Runnable : IRunnable
    {
        private bool m_running;

        public Runnable()
        {
            m_running = false;
        }

        public abstract void Run();

        public bool Running
        {
            get { return m_running; }
            protected set { m_running = value; }
        }
    }

    public abstract class Loopable : Runnable
    {
        private bool m_quit;

        public Loopable()
        {
            m_quit = false;
        }

        public sealed override void Run()
        {
            SetQuit(false);
            Running = true;

            while (!m_quit)
            {
                Loop();
            }

            Running = false;
        }

        protected abstract void Loop();

        public void Quit()
        {
            SetQuit(true);
        }

        protected void SetQuit(bool quit)
        {
            m_quit = quit;
        }
    }
}
