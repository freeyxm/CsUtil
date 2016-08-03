using System;
using System.Net.Sockets;

namespace CsNet
{
    /// <summary>
    /// Socket状态监听接口
    /// </summary>
    public abstract class SocketHandler
    {
        protected SocketListener m_socketListener;

        public SocketHandler(SocketListener listener)
        {
            m_socketListener = listener;
        }

        private SocketHandler()
        {
        }

        public abstract Socket GetSocket();
        public abstract void OnSocketReadReady();
        public abstract void OnSocketWriteReady();
        public abstract void OnSocketError();

        #region busy
        private bool m_busy = false; // 同一时刻，只能有一个线程占用，以解决多线程同步问题。
        private object m_busyLock = new object();

        public bool Busy
        {
            get { lock (m_busyLock) { return m_busy; } }
        }

        public bool SetBusy(bool busy)
        {
            lock (m_busyLock)
            {
                if (busy)
                {
                    if (m_busy)
                        return false;
                    m_busy = true;
                    return true;
                }
                else
                {
                    m_busy = false;
                    return true;
                }
            }
        }
        #endregion busy
    }
}
