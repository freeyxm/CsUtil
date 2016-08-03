using System;
using System.Collections.Generic;
using System.Threading;
using CsNet.Dispatcher;

namespace CsNet
{
    class TaskInfo
    {
        public SocketHandler handler;
        public CheckFlag check;
    }

    class SocketDispatcher : Dispatcher<TaskInfo>
    {
        public SocketDispatcher(int capacity, int initSize)
            : base(capacity, initSize)
        {
        }

        public void Dispatch(Dictionary<SocketHandler, CheckFlag> tasks)
        {
            lock (m_taskQueue)
            {
                var e = m_taskQueue.GetEnumerator();
                while (e.MoveNext())
                {
                    var h = e.Current.handler;
                    if (tasks.ContainsKey(h))
                    {
                        e.Current.check |= tasks[h];
                        tasks.Remove(h);
                    }
                }
            }
            var e2 = tasks.GetEnumerator();
            while (e2.MoveNext())
            {
                TaskInfo task = new TaskInfo();
                task.handler = e2.Current.Key;
                task.check = e2.Current.Value;
                Produce(task);
            }
        }
    }
}
