using System;
using System.IO;
using System.Text;

namespace CsUtil.Log
{
    public class SimpleLogFile
    {
        private StreamWriter m_streamWriter;

        public bool Open(string path)
        {
            Close();

            try
            {
                m_streamWriter = new StreamWriter(path, true, Encoding.UTF8);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public void Close()
        {
            if (m_streamWriter != null)
            {
                m_streamWriter.Close();
                m_streamWriter = null;
            }
        }

        public void Flush()
        {
            if (m_streamWriter != null)
            {
                m_streamWriter.Flush();
            }
        }


        public void Write(string content)
        {
            if (m_streamWriter != null)
            {
                m_streamWriter.Write(content);
            }
        }

        public void Write(string format, params object[] args)
        {
            if (m_streamWriter != null)
            {
                m_streamWriter.Write(format, args);
            }
        }

        public void WriteLine(string content)
        {
            if (m_streamWriter != null)
            {
                m_streamWriter.WriteLine(content);
            }
        }

        public void WriteLine()
        {
            if (m_streamWriter != null)
            {
                m_streamWriter.WriteLine();
            }
        }


        public SimpleLogFile Append(string content)
        {
            Write(content);
            return this;
        }

        public SimpleLogFile Append(string format, params object[] args)
        {
            Write(format, args);
            return this;
        }

        public SimpleLogFile AppendLine(string content)
        {
            WriteLine(content);
            return this;
        }

        public SimpleLogFile AppendLine()
        {
            WriteLine();
            return this;
        }


        public void LogFormat(string format, params object[] args)
        {
            string content = string.Format(format, args);
            Append("[-Debug-] ").AppendLine(content).AppendLine();
        }

        public void LogErrorFormat(string format, params object[] args)
        {
            string content = string.Format(format, args);
            Append("[-Error-] ").AppendLine(content).AppendLine();
        }

        public void LogWarningFormat(string format, params object[] args)
        {
            string content = string.Format(format, args);
            Append("[Warning] ").AppendLine(content).AppendLine();
        }
    }
}
