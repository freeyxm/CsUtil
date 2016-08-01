using System;
using System.Collections.Generic;
using System.Net;

namespace CsNetServer
{
    class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint ep = new IPEndPoint(0, 2016);
            Server server = new Server();
            server.Start(ep);
        }
    }
}
