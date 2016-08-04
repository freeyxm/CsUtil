﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace CsNet
{
    public class SocketMsg : SocketHandler
    {
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        class Header
        {
            public short sign;
            public int totalSize;
        }
        class Msg
        {
            public byte[] data;
            public Action onFinished;
            public Action onError;
        }

        public const int MAX_MSG_SIZE = 10 * 1024;
        private const byte HEADER_SIGN = 0x7F;

        private SocketBase m_socket;

        private Queue<Msg> m_sendQueue;
        private Msg m_sendMsg;
        private int m_sendLength;

        private Header m_recvHeader;
        private byte[] m_recvBuffer;
        private int m_recvLength;
        private int m_headerSize;

        private Action<SocketMsg, byte[]> m_onRecvedData;
        private Action<SocketMsg> m_onSocketError;

        public SocketMsg(SocketBase socket, SocketListener listener)
            : base(listener)
        {
            m_socket = socket;
            m_socket.GetSocket().Blocking = false;

            m_sendQueue = new Queue<Msg>();
            m_sendLength = 0;

            m_recvHeader = new Header();
            m_recvBuffer = new byte[MAX_MSG_SIZE];
            m_recvLength = 0;
            m_headerSize = Marshal.SizeOf(m_recvHeader);
        }

        ~SocketMsg()
        {
            UnRegister();
        }

        public void SetOnRecvedData(Action<SocketMsg, byte[]> cb)
        {
            m_onRecvedData = cb;
        }

        public void SetOnSocketError(Action<SocketMsg> cb)
        {
            m_onSocketError = cb;
        }

        public void SendMsg(byte[] data, Action onFinished, Action onError)
        {
            Msg msg = new Msg();
            msg.data = PackMsg(data, 0, data.Length);
            msg.onFinished = onFinished;
            msg.onError = onError;

            lock (m_sendQueue)
            {
                m_sendQueue.Enqueue(msg);
                if (m_sendQueue.Count == 1)
                {
                    m_socketListener.Register(this, CheckFlag.Write);
                }
            }
        }

        public void OnRecvMsg(byte[] buffer, int offset, int size)
        {
            byte[] data = new byte[size];
            Array.Copy(buffer, offset, data, 0, size);
            m_onRecvedData(this, data);
        }

        public override Socket GetSocket()
        {
            return m_socket.GetSocket();
        }

        public override void OnSocketWriteReady()
        {
            var ret = FResult.Success;
            if (m_sendMsg != null && m_sendLength < m_sendMsg.data.Length)
            {
                ret = SendBuffer();
            }
            while (ret == FResult.Success)
            {
                lock (m_sendQueue)
                {
                    if (m_sendQueue.Count == 0)
                        break;
                    m_sendMsg = m_sendQueue.Dequeue();
                    m_sendLength = 0;
                }
                ret = SendBuffer();
            }

            if (m_sendMsg != null || m_sendQueue.Count > 0)
            {
                m_socketListener.Register(this, CheckFlag.Write);
            }
            else
            {
                m_socketListener.UnRegister(this, CheckFlag.Write);
            }
        }

        FResult SendBuffer()
        {
            int size = m_sendMsg.data.Length - m_sendLength;
            var ret = m_socket.Send(m_sendMsg.data, m_sendLength, size);
            if (ret == FResult.Success)
            {
                m_sendMsg.onFinished?.Invoke();
                ResetSendBuffer();
            }
            else if (ret == FResult.WouldBlock)
            {
                m_sendLength += m_socket.RealSend;
            }
            else
            {
                m_sendMsg.onError?.Invoke();
                ResetSendBuffer();
                OnSocketError();
            }
            return ret;
        }

        public override void OnSocketReadReady()
        {
            int size = m_socket.GetSocket().Available;
            if (size == 0) // remote socket closed.
            {
                OnSocketError();
                return;
            }

            while (size > 0)
            {
                if (m_recvLength < m_headerSize)
                {
                    size = Math.Min(size, m_headerSize - m_recvLength);
                    var ret = m_socket.Recv(m_recvBuffer, m_recvLength, size);
                    if (ret == FResult.Success)
                    {
                        m_recvLength += size;
                        if (m_recvLength >= m_headerSize)
                        {
                            Util.BytesToStruct(m_recvBuffer, 0, m_recvHeader);
                            if (m_recvHeader.sign != HEADER_SIGN)
                            {
                                ResetRecvBuffer();
                            }
                        }
                    }
                    else if (ret == FResult.WouldBlock)
                    {
                        m_recvLength += m_socket.RealRecv;
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
                    var ret = m_socket.Recv(m_recvBuffer, m_recvLength, size);
                    if (ret == FResult.Success)
                    {
                        m_recvLength += size;
                        if (m_recvLength >= m_recvHeader.totalSize)
                        {
                            OnRecvMsg(m_recvBuffer, m_headerSize, m_recvHeader.totalSize - m_headerSize);
                            ResetRecvBuffer();
                        }
                    }
                    else if (ret == FResult.WouldBlock)
                    {
                        m_recvLength += m_socket.RealRecv;
                    }
                    else
                    {
                        OnSocketError();
                        break;
                    }
                }
                size = m_socket.GetSocket().Available;
            }
        }

        public override void OnSocketError()
        {
            UnRegister();
            m_onSocketError(this);
        }

        public void Register()
        {
            m_socketListener.Register(this, CheckFlag.Read | CheckFlag.Error);
        }

        public void UnRegister()
        {
            m_socketListener.UnRegister(this, CheckFlag.All);
        }

        byte[] PackMsg(byte[] data, int offset, int size)
        {
            Header header = new Header();
            header.sign = HEADER_SIGN;
            header.totalSize = m_headerSize + size;

            byte[] buffer = new byte[m_headerSize + size];
            Util.StructToBytes(header, ref buffer, 0);
            Array.Copy(data, offset, buffer, m_headerSize, size);

            return buffer;
        }

        void ResetSendBuffer()
        {
            m_sendMsg = null;
            m_sendLength = 0;
        }

        void ResetRecvBuffer()
        {
            m_recvLength = 0;
            m_recvHeader.totalSize = 0;
        }
    }
}
