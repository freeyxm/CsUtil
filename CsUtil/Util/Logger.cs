using System;

namespace CsUtil.Util
{
    public class Logger
    {
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error,
        }
        public static LogLevel logLevel { get; set; }

        private static void Write(LogLevel level, string msg)
        {
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_EDITOR
            switch (level)
            {
                case LogLevel.Error:
                    Debuger.LogError(msg);
                    break;
                case LogLevel.Warning:
                    Debuger.LogWarning(msg);
                    break;
                default:
                    Debuger.Log(msg);
                    break;
            }
#else
            Console.WriteLine("[{0}] {1}", level, msg);
#endif
        }

        public static void Log(LogLevel level, string msg)
        {
            if (logLevel <= level)
            {
                Write(level, msg);
            }
        }

        public static void Log(LogLevel level, string format, params object[] args)
        {
            if (logLevel <= level)
            {
                if (args.Length > 0)
                    format = string.Format(format, args);
                Write(level, format);
            }
        }

        public static void Debug(string msg)
        {
            Log(LogLevel.Debug, msg);
        }

        public static void Debug(string format, params object[] args)
        {
            Log(LogLevel.Debug, format, args);
        }

        public static void Info(string msg)
        {
            Log(LogLevel.Info, msg);
        }

        public static void Info(string format, params object[] args)
        {
            Log(LogLevel.Info, format, args);
        }

        public static void Warning(string msg)
        {
            Log(LogLevel.Warning, msg);
        }

        public static void Warning(string format, params object[] args)
        {
            Log(LogLevel.Warning, format, args);
        }

        public static void Error(string msg)
        {
            Log(LogLevel.Error, msg);
        }

        public static void Error(string format, params object[] args)
        {
            Log(LogLevel.Error, format, args);
        }
    }
}