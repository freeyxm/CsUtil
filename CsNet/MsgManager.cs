using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace FNet
{
    class MsgManager : SocketHandler
    {
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct Header
        {
            public byte sign;
            public int totalSize;
        }

        public const int MAX_MSG_SIZE = 10 * 1024;
        private const byte HEADER_SIGN = 0x7F;

        private SocketBase m_socket;

        private Queue<byte[]> m_sendQueue;
        private byte[] m_sendBuffer;
        private int m_sendLength;

        private Header m_recvHeader;
        private byte[] m_recvBuffer;
        private int m_recvLength;
        private int m_headerSize;

        public MsgManager(SocketBase socket)
        {
            m_socket = socket;
            m_socket.GetSocket().Blocking = false;

            m_sendQueue = new Queue<byte[]>();
            m_sendLength = 0;

            m_recvHeader = new Header();
            m_recvBuffer = new byte[MAX_MSG_SIZE];
            m_recvLength = 0;
            m_headerSize = Marshal.SizeOf(m_recvHeader);

            SocketListener.Instance.Register(this, SocketListener.CheckFlag.Read);
            SocketListener.Instance.Register(this, SocketListener.CheckFlag.Error);
        }

        private MsgManager()
        {
        }

        ~MsgManager()
        {
            SocketListener.Instance.UnRegister(this,
                SocketListener.CheckFlag.Write
                | SocketListener.CheckFlag.Read
                | SocketListener.CheckFlag.Error);
        }

        public void SendData(byte[] data)
        {
            byte[] bytes = PackMsg(data, 0, data.Length);
            lock (m_sendQueue)
            {
                m_sendQueue.Enqueue(bytes);
                if (m_sendQueue.Count == 1)
                {
                    SocketListener.Instance.Register(this, SocketListener.CheckFlag.Write);
                }
            }
        }

        public override Socket GetSocket()
        {
            return m_socket.GetSocket();
        }

        public override void OnSocketWriteReady()
        {
            bool bSend = true;
            if (m_sendBuffer != null && m_sendLength < m_sendBuffer.Length)
            {
                bSend = SendBuffer();
            }
            while (bSend && m_sendQueue.Count > 0)
            {
                m_sendBuffer = m_sendQueue.Dequeue();
                m_sendLength = 0;
                bSend = SendBuffer();
            }
            if (bSend)
            {
            }
        }

        bool SendBuffer()
        {
            int size = m_sendBuffer.Length - m_sendLength;
            var result = m_socket.Send(m_sendBuffer, m_sendLength, size);
            if (result == FResult.Success)
            {
                m_sendLength += size;
                m_sendBuffer = null;
                m_sendLength = 0;
                return true;
            }
            else if (result == FResult.WouldBlock)
            {
                m_sendLength += m_socket.SendLength;
                return false;
            }
            else
            {
                OnSocketError();
                return false;
            }
        }

        public override void OnSocketReadReady()
        {
            int size = 0;
            while ((size = m_socket.GetSocket().Available) > 0)
            {
                if (m_recvLength < m_headerSize)
                {
                    size = Math.Min(size, m_headerSize - m_recvLength);
                    var result = m_socket.Recv(m_recvBuffer, m_recvLength, size);
                    if (result == FResult.Success)
                    {
                        m_recvLength += size;
                        if (m_recvLength >= m_headerSize)
                        {
                            Util.BytesToStuct(m_recvBuffer, 0, ref m_recvHeader);
                            if (m_recvHeader.sign != HEADER_SIGN)
                            {
                                ResetRecvBuffer();
                            }
                        }
                    }
                    else
                    {
                        OnSocketError();
                        break;
                    }
                }
                else
                {
                    size = Math.Min(size, m_recvHeader.totalSize - m_headerSize);
                    var result = m_socket.Recv(m_recvBuffer, m_recvLength, size);
                    if (result == FResult.Success)
                    {
                        m_recvLength += size;
                        if (m_recvLength >= m_recvHeader.totalSize)
                        {
                            OnRecvData(m_recvBuffer, m_headerSize, m_recvHeader.totalSize - m_headerSize);
                            ResetRecvBuffer();
                        }
                    }
                    else
                    {
                        OnSocketError();
                        break;
                    }
                }
            }
        }

        public override void OnSocketError()
        {
            throw new NotImplementedException();
        }

        public void OnRecvData(byte[] buffer, int offset, int size)
        {
            byte[] data = new byte[size];
            Array.Copy(buffer, offset, data, 0, size);
        }

        byte[] PackMsg(byte[] data, int offset, int size)
        {
            Header header;
            header.sign = HEADER_SIGN;
            header.totalSize = m_headerSize + size;

            int headerSize = m_headerSize;
            byte[] buffer = new byte[headerSize + size];

            Util.StructToBytes(header, ref buffer, 0);
            Array.Copy(data, offset, buffer, headerSize, size);

            return buffer;
        }

        void ResetRecvBuffer()
        {
            m_recvLength = 0;
            m_recvHeader.totalSize = 0;
        }
    }
}
