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
        SocketBase m_socket;
        bool m_bRun;

        public Client()
        {
            m_socket = new SocketTcp(AddressFamily.InterNetwork);
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

            var mgr = new MsgManager(m_socket);
            mgr.SetOnSocketError((m) => { m_bRun = false; });
            mgr.SetOnRecvedData(OnRecvedData);
            mgr.Register();

            while (m_bRun)
            {
                mgr.SendMsg(bytes, () =>
                {
                    Logger.Debug("Send finished.");
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

        void OnRecvedData(MsgManager mgr, byte[] data)
        {
            string msg = Encoding.UTF8.GetString(data);
            Logger.Debug(string.Format("Recv msg: {0}", msg));
        }
    }
}
