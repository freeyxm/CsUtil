using System;
using System.Diagnostics;

namespace CsUtil.Test
{
    public class ExecTimer
    {
        public enum TimePrecision
        {
            Micro,
            Milli,
            Sec,
        }
        private static string[] m_timeUnits = { "us", "ms", "s" };

        private Stopwatch m_stopWatch;
        private long m_ticksPerMicrosecond;

        public ExecTimer()
        {
            m_stopWatch = new Stopwatch();
            m_ticksPerMicrosecond = Stopwatch.Frequency / 1000000;
        }

        public void Reset()
        {
            m_stopWatch.Reset();
        }

        public void Start()
        {
            m_stopWatch.Start();
        }

        public void Stop()
        {
            m_stopWatch.Stop();
        }

        public bool IsRunning()
        {
            return m_stopWatch.IsRunning;
        }

        public long GetElapsedMicroseconds()
        {
            return m_stopWatch.ElapsedTicks / m_ticksPerMicrosecond;
        }

        public long GetElapsedMilliseconds()
        {
            return m_stopWatch.ElapsedMilliseconds;
        }

        public long GetElapsedSeconds()
        {
            return m_stopWatch.ElapsedMilliseconds / 1000;
        }

        public long GetElapseTime(TimePrecision tp)
        {
            switch (tp)
            {
                case TimePrecision.Micro:
                    return GetElapsedMicroseconds();
                case TimePrecision.Milli:
                    return GetElapsedMilliseconds();
                case TimePrecision.Sec:
                default:
                    return GetElapsedSeconds();
            }
        }

        public long Execute(Action action)
        {
            Reset();
            Start();
            {
                action();
            }
            Stop();
            return GetElapsedMicroseconds();
        }

        public void ExecuteAndPrint(string msg, Action action, TimePrecision tp = TimePrecision.Micro)
        {
            Reset();
            Start();
            {
                action();
            }
            Stop();
            Console.WriteLine("{0}: {1} {2}", msg, GetElapseTime(tp), m_timeUnits[(int)tp]);
        }
    }
}