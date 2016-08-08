using System;
using System.Net;
using CsNet.Util;

namespace CsNetServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.LogLevel = Logger.Level.Info;

            IPEndPoint ep = new IPEndPoint(0, 2016);
            Server server = new Server();
            server.Start(ep);
        }
    }
}
