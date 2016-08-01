using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace CsNet
{
    class SocketListener
    {
        private static SocketListener m_instance;
        public static SocketListener Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new SocketListener(1);
                return m_instance;
            }
        }

        public enum CheckFlag
        {
            Read = 0x01 << 0,
            Write = 0x01 << 1,
            Error = 0x01 << 2,
        }

        private Dictionary<Socket, SocketHandler> m_readSocks;
        private Dictionary<Socket, SocketHandler> m_writeSocks;
        private Dictionary<Socket, SocketHandler> m_errorSocks;
        private List<Socket> m_checkRead;
        private List<Socket> m_checkWrite;
        private List<Socket> m_checkError;

        public SocketListener(int capacity)
        {
            m_readSocks = new Dictionary<Socket, SocketHandler>();
            m_writeSocks = new Dictionary<Socket, SocketHandler>();
            m_errorSocks = new Dictionary<Socket, SocketHandler>();
            m_checkRead = new List<Socket>(capacity);
            m_checkWrite = new List<Socket>(capacity);
            m_checkError = new List<Socket>(capacity);
        }

        public void Register(SocketHandler h, CheckFlag flag)
        {
            Socket socket = h.GetSocket();

            if ((flag & CheckFlag.Read) != 0)
            {
                lock (m_readSocks)
                {
                    if (!m_readSocks.ContainsKey(socket))
                        m_readSocks.Add(socket, h);
                }
            }
            if ((flag & CheckFlag.Write) != 0)
            {
                lock (m_writeSocks)
                {
                    if (!m_writeSocks.ContainsKey(socket))
                        m_writeSocks.Add(socket, h);
                }
            }
            if ((flag & CheckFlag.Error) != 0)
            {
                lock (m_errorSocks)
                {
                    if (!m_errorSocks.ContainsKey(socket))
                        m_errorSocks.Add(socket, h);
                }
            }
        }

        public void UnRegister(SocketHandler h, CheckFlag flag)
        {
            Socket socket = h.GetSocket();

            if ((flag & CheckFlag.Read) != 0)
            {
                lock (m_readSocks)
                {
                    m_readSocks.Remove(socket);
                }
            }
            if ((flag & CheckFlag.Write) != 0)
            {
                lock (m_writeSocks)
                {
                    m_writeSocks.Remove(socket);
                }
            }
            if ((flag & CheckFlag.Error) != 0)
            {
                lock (m_errorSocks)
                {
                    m_errorSocks.Remove(socket);
                }
            }
        }

        public void Run()
        {
            while (true)
            {
                m_checkRead.Clear();
                lock (m_readSocks)
                {
                    m_checkRead.AddRange(m_readSocks.Keys);
                }
                m_checkWrite.Clear();
                lock (m_writeSocks)
                {
                    m_checkWrite.AddRange(m_writeSocks.Keys);
                }
                m_checkError.Clear();
                lock (m_errorSocks)
                {
                    m_checkError.AddRange(m_errorSocks.Keys);
                }

                Socket.Select(m_checkRead, m_checkWrite, m_checkError, 10000);

                if (m_checkRead.Count > 0)
                {
                    lock (m_readSocks)
                    {
                        for (int i = 0; i < m_checkRead.Count; ++i)
                        {
                            var s = m_checkRead[i];
                            if (m_readSocks.ContainsKey(s))
                                m_readSocks[s].OnSocketReadReady();
                        }
                    }
                }
                if (m_checkWrite.Count > 0)
                {
                    lock (m_writeSocks)
                    {
                        for (int i = 0; i < m_checkWrite.Count; ++i)
                        {
                            var s = m_checkWrite[i];


                            if (m_writeSocks.ContainsKey(s))
                                m_writeSocks[s].OnSocketWriteReady();

                        }
                    }
                }
                if (m_checkError.Count > 0)
                {
                    lock (m_errorSocks)
                    {
                        for (int i = 0; i < m_checkError.Count; ++i)
                        {
                            var s = m_checkError[i];
                            if (m_errorSocks.ContainsKey(s))
                                m_errorSocks[s].OnSocketError();
                        }
                    }
                }
            } // end while
        }
    }
}
