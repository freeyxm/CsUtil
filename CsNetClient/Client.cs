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

        public void Start(EndPoint ep)
        {
            var ret = m_socket.Connect(ep);
            if (ret != FResult.Success)
                return;

            Thread listner = new Thread(new ThreadStart(SocketListener.Instance.Run));
            listner.Start();

            string msg = "hi server!";
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

                Console.ReadLine();
                //Thread.Sleep(10);
            }
        }

        void OnRecvedData(MsgManager mgr, byte[] data)
        {
            string msg = Encoding.UTF8.GetString(data);
            Logger.Debug(string.Format("Recv msg: {0}", msg));
        }
    }
}
