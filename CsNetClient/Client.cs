using System;
using System.Collections.Generic;
using CsNet;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CsNetClient
{
    class Client
    {
        private ControlManager m_ctlMgr;
        private SocketBase m_socket;
        private bool m_bRun;
        private bool m_bReconnect;
        private int m_reconnectCount;

        public Client(ControlManager mgr)
        {
            m_socket = new SocketTcp(AddressFamily.InterNetwork);
            m_ctlMgr = mgr;
        }

        public void Start(EndPoint ep, int sleep)
        {
            var ret = m_socket.Connect(ep);
            if (ret != FResult.Success)
            {
                Logger.Error("Connect error: {0}", m_socket.ErrorMsg);
                return;
            }

            string msg = "Hi server!";
            byte[] bytes = Encoding.UTF8.GetBytes(msg);

            m_bRun = true;
            m_bReconnect = false;
            m_reconnectCount = 0;

            var socket = new SocketMsg(m_socket, m_ctlMgr.GetSocketListener());
            socket.SetOnSocketError(OnSocketError);
            socket.SetOnRecvedData(OnRecvedData);
            socket.Register();

            while (m_bRun)
            {
                if (m_bReconnect)
                {
                    ++m_reconnectCount;
                    if (socket.GetSocket().Reconnect() == FResult.Success)
                    {
                        Logger.Info("Reconnect Success.");
                        socket.Register();
                        m_bReconnect = false;
                    }
                    else
                    {
                        Logger.Info("Reconnect failed ({0}): {1}", m_reconnectCount, socket.GetSocket().ErrorMsg);
                        if (m_reconnectCount >= 5)
                        {
                            m_bRun = false;
                            break;
                        }
                        Thread.Sleep(2000);
                    }
                    continue;
                }

                socket.SendMsg(bytes, () =>
                {
                    //Logger.Debug("Send finished.");
                }, () =>
                {
                    Logger.Debug("Send error.");
                });

                if (sleep > 0)
                    Thread.Sleep(sleep);
                else
                    Console.ReadLine();
            }
        }

        void OnRecvedData(SocketMsg socket, byte[] data)
        {
            string msg = Encoding.UTF8.GetString(data);
            //Logger.Debug(string.Format("Recv msg: {0}", msg));
        }

        void OnSocketError(SocketMsg socket)
        {
            m_bReconnect = true;
        }
    }
}
