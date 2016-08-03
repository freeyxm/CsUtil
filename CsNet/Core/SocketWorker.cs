﻿using System;
using CsNet.Dispatcher;

namespace CsNet
{
    class SocketWorker : Worker<TaskInfo>
    {
        public SocketWorker(SocketDispatcher dispatcher)
            : base(dispatcher)
        {
        }

        protected override void Work(TaskInfo task)
        {
            if ((task.check & CheckFlag.Read) != 0)
                task.handler.OnSocketReadReady();
            if ((task.check & CheckFlag.Write) != 0)
                task.handler.OnSocketWriteReady();
            if ((task.check & CheckFlag.Error) != 0)
                task.handler.OnSocketError();

            task.handler.SetBusy(false);
        }
    }
}