using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CsUtil.Test
{
    public class TestUtility
    {
        public static void RunTime(string title, Stopwatch watch, Action action)
        {
            Console.Write("Start {0} ...", title);
            watch.Restart();
            action();
            watch.Stop();
            Console.WriteLine(" {0} ms", watch.ElapsedMilliseconds);
        }
    }
}
