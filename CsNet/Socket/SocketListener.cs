using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace CsNet
{
    public class SocketListener
    {
        private static SocketListener m_instance;
        public static SocketListener Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new SocketListener(1, 10000);
                return m_instance;
            }
        }

        public enum CheckFlag
        {
            Read = 0x01 << 0,
            Write = 0x01 << 1,
            Error = 0x01 << 2,

            RW = Read | Write,
            All = Read | Write | Error,
        }

        private Dictionary<Socket, SocketHandler> m_readSocks;
        private Dictionary<Socket, SocketHandler> m_writeSocks;
        private Dictionary<Socket, SocketHandler> m_errorSocks;
        private List<Socket> m_checkRead;
        private List<Socket> m_checkWrite;
        private List<Socket> m_checkError;
        private int m_timeout;

        /// <summary>
        /// Listening Socket state.
        /// </summary>
        /// <param name="capacity">init capacity</param>
        /// <param name="timeout">Select timeout (microsecond).</param>
        public SocketListener(int capacity, int timeout)
        {
            m_readSocks = new Dictionary<Socket, SocketHandler>();
            m_writeSocks = new Dictionary<Socket, SocketHandler>();
            m_errorSocks = new Dictionary<Socket, SocketHandler>();
            m_checkRead = new List<Socket>(capacity);
            m_checkWrite = new List<Socket>(capacity);
            m_checkError = new List<Socket>(capacity);
            m_timeout = timeout;
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
                ResetCheckList(ref m_checkRead, m_readSocks);
                ResetCheckList(ref m_checkWrite, m_writeSocks);
                ResetCheckList(ref m_checkError, m_errorSocks);

                if (m_checkRead.Count > 0 || m_checkWrite.Count > 0 || m_checkError.Count > 0)
                {
                    try
                    {
                        Socket.Select(m_checkRead, m_checkWrite, m_checkError, m_timeout);

                        ExecuteCheckList(m_checkRead, m_readSocks, CheckFlag.Read);
                        ExecuteCheckList(m_checkWrite, m_writeSocks, CheckFlag.Write);
                        ExecuteCheckList(m_checkError, m_errorSocks, CheckFlag.Error);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e.ToString());
                    }
                }
                else
                {
                    System.Threading.Thread.Sleep(m_timeout / 1000);
                }
            } // end while
        }

        void ResetCheckList(ref List<Socket> checkList, Dictionary<Socket, SocketHandler> source)
        {
            checkList.Clear();
            lock (source)
            {
                checkList.AddRange(source.Keys);
            }
        }

        void ExecuteCheckList(List<Socket> checkList, Dictionary<Socket, SocketHandler> source, CheckFlag flag)
        {
            if (checkList.Count > 0)
            {
                lock (source)
                {
                    for (int i = 0; i < checkList.Count; ++i)
                    {
                        var s = checkList[i];
                        if (source.ContainsKey(s))
                        {
                            var h = source[s];

                            if ((flag & CheckFlag.Read) != 0)
                                h.OnSocketReadReady();

                            if ((flag & CheckFlag.Write) != 0)
                                h.OnSocketWriteReady();

                            if ((flag & CheckFlag.Error) != 0)
                            {
                                UnRegister(h, CheckFlag.All);
                                h.OnSocketError();
                            }
                        }
                    }
                } // end lock
            }
        } // end ExecuteCheckList
    }
}
