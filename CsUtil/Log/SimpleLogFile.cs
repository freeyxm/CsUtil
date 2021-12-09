using System;
using System.IO;
using System.Text;

namespace CsUtil.Log
{
    public class SimpleLogFile
    {
        private StreamWriter m_streamWriter;
        private string m_path;

        public string Path { get { return m_path; } }

        public bool Open(string path)
        {
            m_path = path;

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

        public void WriteLine(string format, params object[] args)
        {
            if (m_streamWriter != null)
            {
                m_streamWriter.WriteLine(format, args);
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

        public SimpleLogFile AppendLine(string format, params object[] args)
        {
            WriteLine(format, args);
            return this;
        }

        public SimpleLogFile AppendLine()
        {
            WriteLine();
            return this;
        }


        public void Log(string content)
        {
            Append("[-Debug-] ").AppendLine(content);
        }

        public void LogFormat(string format, params object[] args)
        {
            Append("[-Debug-] ").AppendLine(format, args);
        }

        public void LogError(string content)
        {
            Append("[-Error-] ").AppendLine(content);
        }

        public void LogErrorFormat(string format, params object[] args)
        {
            Append("[-Error-] ").AppendLine(format, args);
        }

        public void LogWarning(string content)
        {
            Append("[Warning] ").AppendLine(content);
        }

        public void LogWarningFormat(string format, params object[] args)
        {
            Append("[Warning] ").AppendLine(format, args);
        }


        public static bool OpenLogFile(string path, string namePrefix, out SimpleLogFile logFile)
        {
            try
            {
                logFile = new SimpleLogFile();

                path = string.Format("{0}/{1}_{2}.log", path, namePrefix, DateTime.Now.ToString("yyyMMdd-HHmmss.ffff"));

                string dir = System.IO.Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                return logFile.Open(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                logFile = null;
                return false;
            }
        }
    }
}
