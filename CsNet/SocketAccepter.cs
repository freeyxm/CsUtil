using System;
using System.Net.Sockets;

namespace CsNet
{
    public class SocketAccepter : SocketHandler
    {
        public delegate void OnAccept(Socket socket);
        private OnAccept m_onAccept;
        private SocketBase m_socket;

        public SocketAccepter(SocketBase socket, SocketListener listener)
            : base(listener)
        {
            m_socket = socket;
            m_socketListener.Register(this, CheckFlag.Read | CheckFlag.Error);
        }

        ~SocketAccepter()
        {
            m_socketListener.UnRegister(this, CheckFlag.All);
        }

        public void SetOnAcceptSocket(OnAccept onAccept)
        {
            m_onAccept = onAccept;
        }

        public override Socket GetSocket()
        {
            return m_socket.GetSocket();
        }

        public override void OnSocketReadReady()
        {
            Socket socket = m_socket.Accept();
            if (socket != null)
            {
                m_onAccept(socket);
            }
            else
            {
                if (m_socket.State != FResult.WouldBlock)
                {
                    Logger.Error(m_socket.ErrorMsg);
                    OnError();
                }
            }
        }

        public override void OnSocketError()
        {
            OnError();
        }

        public override void OnSocketWriteReady()
        {
        }

        private void OnError()
        {
            m_socketListener.UnRegister(this, CheckFlag.All);
        }
    }
}
