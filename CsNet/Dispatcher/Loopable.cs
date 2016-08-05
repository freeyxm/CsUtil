using System;

namespace CsNet.Dispatcher
{
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
