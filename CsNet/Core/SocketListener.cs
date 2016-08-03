using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace CsNet
{
    public enum CheckFlag
    {
        Read = 0x01 << 0,
        Write = 0x01 << 1,
        Error = 0x01 << 2,

        RW = Read | Write,
        All = Read | Write | Error,
    }

    /// <summary>
    /// 监听Socket状态
    /// </summary>
    public class SocketListener
    {
        private Dictionary<Socket, SocketHandler> m_readSockets;
        private Dictionary<Socket, SocketHandler> m_writeSockets;
        private Dictionary<Socket, SocketHandler> m_errorSockets;
        private Dictionary<SocketHandler, CheckFlag> m_socketStates;
        private List<Socket> m_checkRead;
        private List<Socket> m_checkWrite;
        private List<Socket> m_checkError;
        private int m_timeout;

        public delegate void Dispatch(Dictionary<SocketHandler, CheckFlag> source);
        private Dispatch m_dispatch;

        /// <summary>
        /// Listening Socket state.
        /// </summary>
        /// <param name="capacity">init capacity</param>
        /// <param name="timeout">Select timeout (microsecond).</param>
        public SocketListener(int capacity, int timeout, Dispatch dispatch)
        {
            m_readSockets = new Dictionary<Socket, SocketHandler>(capacity);
            m_writeSockets = new Dictionary<Socket, SocketHandler>(capacity);
            m_errorSockets = new Dictionary<Socket, SocketHandler>(capacity);
            m_socketStates = new Dictionary<SocketHandler, CheckFlag>();
            m_checkRead = new List<Socket>(capacity);
            m_checkWrite = new List<Socket>(capacity);
            m_checkError = new List<Socket>(capacity);
            m_timeout = timeout;
            m_dispatch = dispatch;
        }

        /// <summary>
        /// 注册监听
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="flag"></param>
        public void Register(SocketHandler handler, CheckFlag flag)
        {
            Socket socket = handler.GetSocket();

            if ((flag & CheckFlag.Read) != 0)
            {
                lock (m_readSockets)
                {
                    if (!m_readSockets.ContainsKey(socket))
                        m_readSockets.Add(socket, handler);
                }
            }
            if ((flag & CheckFlag.Write) != 0)
            {
                lock (m_writeSockets)
                {
                    if (!m_writeSockets.ContainsKey(socket))
                        m_writeSockets.Add(socket, handler);
                }
            }
            if ((flag & CheckFlag.Error) != 0)
            {
                lock (m_errorSockets)
                {
                    if (!m_errorSockets.ContainsKey(socket))
                        m_errorSockets.Add(socket, handler);
                }
            }
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="flag"></param>
        public void UnRegister(SocketHandler handler, CheckFlag flag)
        {
            Socket socket = handler.GetSocket();

            if ((flag & CheckFlag.Read) != 0)
            {
                lock (m_readSockets)
                {
                    m_readSockets.Remove(socket);
                }
            }
            if ((flag & CheckFlag.Write) != 0)
            {
                lock (m_writeSockets)
                {
                    m_writeSockets.Remove(socket);
                }
            }
            if ((flag & CheckFlag.Error) != 0)
            {
                lock (m_errorSockets)
                {
                    m_errorSockets.Remove(socket);
                }
            }
        }

        public void Run()
        {
            while (true)
            {
                BuildCheckList(ref m_checkRead, m_readSockets);
                BuildCheckList(ref m_checkWrite, m_writeSockets);
                BuildCheckList(ref m_checkError, m_errorSockets);

                if (m_checkRead.Count > 0 || m_checkWrite.Count > 0 || m_checkError.Count > 0)
                {
                    try
                    {
                        Socket.Select(m_checkRead, m_checkWrite, m_checkError, m_timeout);

                        m_socketStates.Clear();
                        ExecuteCheckList(m_checkRead, m_readSockets, CheckFlag.Read);
                        ExecuteCheckList(m_checkWrite, m_writeSockets, CheckFlag.Write);
                        ExecuteCheckList(m_checkError, m_errorSockets, CheckFlag.Error);
                        m_dispatch(m_socketStates);
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

        /// <summary>
        /// 构建Select所需的checkList
        /// </summary>
        /// <param name="checkList"></param>
        /// <param name="source"></param>
        void BuildCheckList(ref List<Socket> checkList, Dictionary<Socket, SocketHandler> source)
        {
            checkList.Clear();
            lock (source)
            {
                var e = source.GetEnumerator();
                while (e.MoveNext())
                {
                    if (!e.Current.Value.Busy)
                    {
                        checkList.Add(e.Current.Key);
                    }
                }
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

                            if (!h.SetBusy(true))
                                continue;

                            if (m_socketStates.ContainsKey(h))
                                m_socketStates[h] |= flag;
                            else
                                m_socketStates.Add(h, flag);
                        }
                    }
                } // end lock
            }
        } // end ExecuteCheckList
    }
}
