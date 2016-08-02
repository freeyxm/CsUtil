using System;
using System.Text;

namespace CsNet
{
    public class Logger
    {
        public enum Level
        {
            All,
            Debug,
            Info,
            Warning,
            Error,
            Fatal,
            Off,
        }
        public static Level LogLevel { get; set; }

        private static StringBuilder m_buffer = new StringBuilder();

        public static void Log(Level level, string msg)
        {
            if (LogLevel > level)
                return;
            lock(m_buffer)
            {
                m_buffer.Clear();
                m_buffer.Append("[").Append(level).Append("] ");
                m_buffer.AppendLine(msg);
                Console.Write(m_buffer);
            }
        }

        public static void Log(Level level, string format, params object[] args)
        {
            if (LogLevel > level)
                return;
            Log(level, string.Format(format, args));
        }

        public static void Debug(string msg)
        {
            Log(Level.Debug, msg);
        }

        public static void Debug(string format, params object[] args)
        {
            Log(Level.Debug, format, args);
        }

        public static void Info(string msg)
        {
            Log(Level.Info, msg);
        }

        public static void Info(string format, params object[] args)
        {
            Log(Level.Info, format, args);
        }

        public static void Warning(string msg)
        {
            Log(Level.Warning, msg);
        }

        public static void Warning(string format, params object[] args)
        {
            Log(Level.Warning, format, args);
        }

        public static void Error(string msg)
        {
            Log(Level.Error, msg);
        }

        public static void Error(string format, params object[] args)
        {
            Log(Level.Error, format, args);
        }
    }
}
