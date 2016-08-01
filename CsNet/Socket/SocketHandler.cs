using System;
using System.Net.Sockets;

namespace FNet
{
    abstract class SocketHandler
    {
        public abstract Socket GetSocket();
        public abstract void OnSocketReadReady();
        public abstract void OnSocketWriteReady();
        public abstract void OnSocketError();
    }
}
