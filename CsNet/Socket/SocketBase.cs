using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace CsNet
{
    public class SocketBase
    {
        public delegate void FCallback(FResult ret, int code, string msg);

        protected delegate FResult FAction(ref int errorCode, ref string errorMsg, out SocketError socketError);

        protected Socket m_socket;
        protected int m_errorCode;
        protected string m_errorMsg;

        public SocketBase(AddressFamily af, SocketType st, ProtocolType pt)
        {
            Init(new Socket(af, st, pt));
        }

        public SocketBase(Socket socket)
        {
            Init(socket);
        }

        private SocketBase()
        {
        }

        ~SocketBase()
        {
            Close();
            m_socket.Dispose();
        }

        private void Init(Socket socket)
        {
            m_socket = socket;
            m_errorCode = 0;
            m_errorMsg = null;
        }

        public virtual FResult Bind(EndPoint ep)
        {
            return DoAction(() =>
            {
                m_socket.Bind(ep);
            });
        }

        public virtual FResult Listen(int backlog)
        {
            return DoAction(() =>
            {
                m_socket.Listen(backlog);
            });
        }

        public virtual Socket Accept()
        {
            try
            {
                return m_socket.Accept();
            }
            catch (SocketException e)
            {
                m_errorCode = e.ErrorCode;
                m_errorMsg = e.Message;
                return null;
            }
            catch (Exception e)
            {
                m_errorCode = (int)FResult.Exception;
                m_errorMsg = e.Message;
                return null;
            }
        }

        public virtual FResult Connect(EndPoint ep)
        {
            return DoAction(() =>
            {
                m_socket.Connect(ep);
            });
        }

        public virtual FResult Send(byte[] buffer, int offset, int size)
        {
            return DoAction((ref int errorCode, ref string errorMsg, out SocketError socketError) =>
            {
                FResult ret = FResult.Success;
                socketError = SocketError.Success;
                try
                {
                    int nsend = m_socket.Send(buffer, offset, size, SocketFlags.None, out socketError);
                    if (socketError == SocketError.Success)
                    {
                        if (nsend != size)
                        {
                            ret = FResult.WouldBlock;
                            errorCode = nsend;
                            errorMsg = "";
                        }
                    }
                }
                catch (SocketException e)
                {
                    if (e.ErrorCode == (int)SocketError.WouldBlock)
                    {
                        ret = FResult.WouldBlock;
                        errorCode = 0;
                        errorMsg = "";
                    }
                    else
                    {
                        throw e;
                    }
                }
                return ret;
            });
        }

        public virtual FResult Recv(byte[] buffer, int offset, int size)
        {
            return DoAction((ref int errorCode, ref string errorMsg, out SocketError socketError) =>
            {
                FResult ret = FResult.Success;
                socketError = SocketError.Success;
                try
                {
                    int nrecv = m_socket.Receive(buffer, offset, size, SocketFlags.None, out socketError);
                    if (socketError == SocketError.Success)
                    {
                        if (nrecv == 0)
                        {
                            ret = FResult.SocketClosed;
                            m_errorCode = (int)FResult.Error;
                            m_errorMsg = "";
                        }
                        else if (nrecv != size)
                        {
                            ret = FResult.WouldBlock;
                            errorCode = nrecv;
                            errorMsg = "";
                        }
                    }
                }
                catch (SocketException e)
                {
                    if (e.ErrorCode == (int)SocketError.WouldBlock)
                    {
                        ret = FResult.WouldBlock;
                        errorCode = 0;
                        errorMsg = "";
                    }
                    else
                    {
                        throw e;
                    }
                }
                return ret;
            });
        }

        public virtual FResult BeginConnect(EndPoint ep, FCallback callback)
        {
            AsyncCallback cb = new AsyncCallback((IAsyncResult ar) =>
            {
                DoAction(() =>
                {
                    Socket socket = (Socket)ar.AsyncState;
                    socket.EndConnect(ar);
                }, callback);
            });

            return DoAction(() =>
            {
                m_socket.BeginConnect(ep, cb, m_socket);
            });
        }

        public virtual FResult BeginSend(byte[] buffer, int offset, int size, FCallback callback)
        {
            AsyncCallback cb = new AsyncCallback((IAsyncResult ar) =>
            {
                DoAction((ref int errorCode, ref string errorMsg, out SocketError socketError) =>
                {
                    FResult ret = FResult.Success;
                    Socket socket = (Socket)ar.AsyncState;
                    int nsend = socket.EndSend(ar, out socketError);
                    if (socketError == SocketError.Success)
                    {
                        if (nsend != size)
                        {
                            ret = FResult.WouldBlock;
                            errorCode = nsend;
                            errorMsg = "";
                        }
                    }
                    return ret;
                }, callback);
            });

            return DoAction((ref int errorCode, ref string errorMsg, out SocketError socketError) =>
            {
                m_socket.BeginSend(buffer, offset, size, SocketFlags.None, out socketError, cb, m_socket);
                return FResult.Success;
            });
        }

        public virtual FResult BeginRecv(byte[] buffer, int offset, int size, FCallback callback)
        {
            AsyncCallback cb = new AsyncCallback((IAsyncResult ar) =>
            {
                DoAction((ref int errorCode, ref string errorMsg, out SocketError socketError) =>
                {
                    FResult ret = FResult.Success;
                    Socket socket = (Socket)ar.AsyncState;
                    int nrecv = socket.EndReceive(ar, out socketError);
                    if (socketError == SocketError.Success)
                    {
                        if (nrecv != size)
                        {
                            ret = FResult.WouldBlock;
                            errorCode = nrecv;
                            errorMsg = "";
                        }
                    }
                    return ret;
                }, callback);
            });

            return DoAction((ref int errorCode, ref string errorMsg, out SocketError socketError) =>
            {
                m_socket.BeginSend(buffer, offset, size, SocketFlags.None, out socketError, cb, m_socket);
                return FResult.Success;
            });
        }

        public virtual FResult Close()
        {
            return DoAction(() =>
            {
                m_socket.Shutdown(SocketShutdown.Both);
                m_socket.Close();
            });
        }

        protected FResult DoAction(System.Action action)
        {
            FResult ret = FResult.Success;
            try
            {
                action();
            }
            catch (SocketException e)
            {
                ret = FResult.SocketException;
                m_errorCode = e.ErrorCode;
                m_errorMsg = e.Message;
            }
            catch (Exception e)
            {
                ret = FResult.Exception;
                m_errorCode = (int)FResult.Exception;
                m_errorMsg = e.Message;
            }
            return ret;
        }

        protected FResult DoAction(FAction action)
        {
            FResult ret = FResult.Success;
            try
            {
                SocketError socketError;
                ret = action(ref m_errorCode, ref m_errorMsg, out socketError);
                if (socketError != SocketError.Success)
                {
                    ret = FResult.SocketError;
                    m_errorCode = (int)socketError;
                    m_errorMsg = "";
                }
            }
            catch (SocketException e)
            {
                ret = FResult.SocketException;
                m_errorCode = e.ErrorCode;
                m_errorMsg = e.Message;
            }
            catch (Exception e)
            {
                ret = FResult.Exception;
                m_errorCode = (int)FResult.Exception;
                m_errorMsg = e.Message;
            }
            return ret;
        }

        protected void DoAction(System.Action action, FCallback callback)
        {
            FResult ret = FResult.Success;
            int errorCode = 0;
            string errorMsg = null;

            try
            {
                action();
            }
            catch (SocketException e)
            {
                ret = FResult.SocketException;
                errorCode = e.ErrorCode;
                errorMsg = e.Message;
            }
            catch (Exception e)
            {
                ret = FResult.Exception;
                errorCode = (int)FResult.Exception;
                errorMsg = e.Message;
            }
            finally
            {
                callback(ret, errorCode, errorMsg);
            }
        }

        protected void DoAction(FAction action, FCallback callback)
        {
            FResult ret = FResult.Success;
            int errorCode = 0;
            string errorMsg = null;
            SocketError socketError = SocketError.Success;
            try
            {
                ret = action(ref errorCode, ref errorMsg, out socketError);
                if (socketError != SocketError.Success)
                {
                    ret = FResult.SocketError;
                    errorCode = (int)socketError;
                }
            }
            catch (SocketException e)
            {
                ret = FResult.SocketException;
                errorCode = e.ErrorCode;
                errorMsg = e.Message;
            }
            catch (Exception e)
            {
                ret = FResult.Exception;
                errorCode = (int)FResult.Exception;
                errorMsg = e.Message;
            }
            finally
            {
                callback(ret, errorCode, errorMsg);
            }
        }

        public Socket GetSocket() { return m_socket; }

        public int ErrorCode { get { return m_errorCode; } }
        public string ErrorMsg { get { return m_errorMsg; } }

        public int SendLength { get { return m_errorCode; } }
        public int RecvLength { get { return m_errorCode; } }
    }
}
