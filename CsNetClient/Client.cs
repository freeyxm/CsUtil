﻿using System;
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

            var socket = new SocketMsg(m_socket);
            socket.SetOnSocketError((m) => { m_bRun = false; });
            socket.SetOnRecvedData(OnRecvedData);
            socket.Register();

            while (m_bRun)
            {
                socket.SendMsg(bytes, () =>
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

        void OnRecvedData(SocketMsg mgr, byte[] data)
        {
            string msg = Encoding.UTF8.GetString(data);
            Logger.Debug(string.Format("Recv msg: {0}", msg));
        }
    }
}