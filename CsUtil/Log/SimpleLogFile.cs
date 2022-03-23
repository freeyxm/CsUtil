using System;
using System.IO;
using System.Text;

namespace CsUtil.Log
{
    public class SimpleLogFile
    {
        private StreamWriter m_streamWriter;
        private string m_path;
        public string Path => m_path;

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

        #region Write

        public void Write(string value)
        {
            m_streamWriter.Write(value);
        }

        public void Write(string format, params object[] args)
        {
            m_streamWriter.Write(format, args);
        }

        public void WriteLine(string value)
        {
            m_streamWriter.WriteLine(value);
        }

        public void WriteLine(string format, params object[] args)
        {
            m_streamWriter.WriteLine(format, args);
        }

        public void WriteLine()
        {
            m_streamWriter.WriteLine();
        }

        #endregion

        #region Append

        public SimpleLogFile Append(int value)
        {
            m_streamWriter.Write(value);
            return this;
        }

        public SimpleLogFile Append(uint value)
        {
            m_streamWriter.Write(value);
            return this;
        }

        public SimpleLogFile Append(long value)
        {
            m_streamWriter.Write(value);
            return this;
        }

        public SimpleLogFile Append(ulong value)
        {
            m_streamWriter.Write(value);
            return this;
        }

        public SimpleLogFile Append(float value)
        {
            m_streamWriter.Write(value);
            return this;
        }

        public SimpleLogFile Append(double value)
        {
            m_streamWriter.Write(value);
            return this;
        }

        public SimpleLogFile Append(decimal value)
        {
            m_streamWriter.Write(value);
            return this;
        }

        public SimpleLogFile Append(bool value)
        {
            m_streamWriter.Write(value);
            return this;
        }

        public SimpleLogFile Append(char value)
        {
            m_streamWriter.Write(value);
            return this;
        }

        public SimpleLogFile Append(char[] buffer)
        {
            m_streamWriter.Write(buffer);
            return this;
        }

        public SimpleLogFile Append(char[] buffer, int index, int count)
        {
            m_streamWriter.Write(buffer, index, count);
            return this;
        }

        public SimpleLogFile Append(string value)
        {
            m_streamWriter.Write(value);
            return this;
        }

        public SimpleLogFile Append(string format, params object[] args)
        {
            m_streamWriter.Write(format, args);
            return this;
        }

        public SimpleLogFile AppendLine(string value)
        {
            m_streamWriter.WriteLine(value);
            return this;
        }

        public SimpleLogFile AppendLine(string format, params object[] args)
        {
            m_streamWriter.WriteLine(format, args);
            return this;
        }

        public SimpleLogFile AppendLine()
        {
            m_streamWriter.WriteLine();
            return this;
        }

        #endregion

        #region Unity Like

        public void Log(string value)
        {
            string time = GetTime();
#if UNITY_EDITOR
            UnityEngine.Debug.LogFormat("{0} [f:{1}] {2}", time, UnityEngine.Time.frameCount, value);
#endif
            Append(time);
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE
            Append(" [f:").Append(UnityEngine.Time.frameCount).Append("]");
#endif
            Append(" [-Debug-] ").AppendLine(value);
        }

        public void LogFormat(string format, params object[] args)
        {
            string time = GetTime();
#if UNITY_EDITOR
            UnityEngine.Debug.LogFormat("{0} [f:{1}] {2}", time, UnityEngine.Time.frameCount, string.Format(format, args));
#endif
            Append(time);
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE
            Append(" [f:").Append(UnityEngine.Time.frameCount).Append("]");
#endif
            Append(" [-Debug-] ").AppendLine(format, args);
        }

        public void LogError(string value)
        {
            string time = GetTime();
#if UNITY_EDITOR
            UnityEngine.Debug.LogErrorFormat("{0} [f:{1}] {2}", time, UnityEngine.Time.frameCount, value);
#endif
            Append(time);
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE
            Append(" [f:").Append(UnityEngine.Time.frameCount).Append("]");
#endif
            Append(" [-Error-] ").AppendLine(value);
        }

        public void LogErrorFormat(string format, params object[] args)
        {
            string time = GetTime();
#if UNITY_EDITOR
            UnityEngine.Debug.LogErrorFormat("{0} [f:{1}] {2}", time, UnityEngine.Time.frameCount, string.Format(format, args));
#endif
            Append(time);
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE
            Append(" [f:").Append(UnityEngine.Time.frameCount).Append("]");
#endif
            Append(" [-Error-] ").AppendLine(format, args);
        }

        public void LogWarning(string value)
        {
            string time = GetTime();
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarningFormat("{0} [f:{1}] {2}", time, UnityEngine.Time.frameCount, value);
#endif
            Append(time);
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE
            Append(" [f:").Append(UnityEngine.Time.frameCount).Append("]");
#endif
            Append(" [Warning] ").AppendLine(value);
        }

        public void LogWarningFormat(string format, params object[] args)
        {
            string time = GetTime();
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarningFormat("{0} [f:{1}] {2}", time, UnityEngine.Time.frameCount, string.Format(format, args));
#endif
            Append(time);
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE
            Append(" [f:").Append(UnityEngine.Time.frameCount).Append("]");
#endif
            Append(" [Warning] ").AppendLine(format, args);
        }

        #endregion

        private string GetTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
        }

        public static bool OpenLogFile(string path, string namePrefix, out SimpleLogFile logFile)
        {
            try
            {
                logFile = new SimpleLogFile();

                path = string.Format("{0}/{1}_{2}.log", path, namePrefix, DateTime.Now.ToString("yyyyMMdd-HHmmss-fff"));

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