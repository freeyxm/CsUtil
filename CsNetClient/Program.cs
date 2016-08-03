using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using CsNet;

namespace CsNetClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.LogLevel = Logger.Level.Info;

            IPAddress addr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ep = new IPEndPoint(addr, 2016);

            List<Client> clients = new List<Client>();
            List<Thread> threads = new List<Thread>();

            ControlManager socketMgr = new ControlManager(1);
            socketMgr.Start();

            for (int i = 0; i < 100; ++i)
            {
                Client client = new Client();
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    client.Start(ep, 10);
                }));
                clients.Add(client);
                threads.Add(thread);
                thread.Start();
            }

            for (int i = 0; i < threads.Count; ++i)
            {
                threads[i].Join();
            }
        }
    }
}
