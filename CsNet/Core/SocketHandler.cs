using System;
using System.Net.Sockets;
using System.Threading;

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
        private int m_busy = 0; // 同一时刻，只能有一个线程占用，以解决多线程同步问题。

        public bool Busy
        {
            get { return m_busy != 0; }
        }

        public bool SetBusy(bool busy)
        {
            if (busy)
            {
                return Interlocked.CompareExchange(ref m_busy, 1, 0) == 0;
            }
            else
            {
                m_busy = 0;
                return true;
            }
        }
        #endregion busy
    }
}
