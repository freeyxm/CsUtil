using System;
using System.Collections.Generic;
using System.Threading;

namespace CsNet
{
    public class ControlManager
    {
        private SocketDispatcher m_dispatcher;
        private SocketListener m_listener;
        private Thread m_listenerThread;

        private int m_workerNum;
        private List<SocketWorker> m_workers;
        private List<Thread> m_workerThreads;

        public ControlManager(int workerNum)
        {
            m_workerNum = workerNum;
            m_workers = new List<SocketWorker>(workerNum);
            m_workerThreads = new List<Thread>(workerNum);

            m_dispatcher = new SocketDispatcher(1000, 100);
            m_listener = new SocketListener(1, 10000, m_dispatcher.Dispatch);
        }

        public void Start()
        {
            InitWorkers();
            StartWorkers();

            m_listenerThread = new Thread(new ThreadStart(m_listener.Run));
            m_listenerThread.Start();
        }

        public SocketListener GetSocketListener()
        {
            return m_listener;
        }

        private void InitWorkers()
        {
            for (int i = 0; i < m_workerNum; ++i)
            {
                SocketWorker worker = new SocketWorker(m_dispatcher);
                m_workers.Add(worker);
                Thread thread = new Thread(new ThreadStart(worker.Run));
                m_workerThreads.Add(thread);
            }
        }

        private void StartWorkers()
        {
            for (int i = 0; i < m_workerThreads.Count; ++i)
            {
                m_workerThreads[i].Start();
            }
        }

        private void StopWorkers()
        {
            for (int i = 0; i < m_workers.Count; ++i)
            {
                m_workers[i].Quit();
            }
        }
    }
}
