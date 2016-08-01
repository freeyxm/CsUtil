using System;
using System.Net;

namespace CsNetClient
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress addr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ep = new IPEndPoint(addr, 2016);

            Client client = new Client();
            client.Start(ep);
        }
    }
}
