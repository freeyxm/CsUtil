using System;
using System.Collections.Generic;

namespace CsUtil.Util
{
    public class ActionChain
    {
        private Queue<Func<bool>> m_queue;
        private bool m_bDone;

        public ActionChain()
        {
            m_queue = new Queue<Func<bool>>();
            m_bDone = true;
        }

        public void Add(Func<bool> action)
        {
            m_queue.Enqueue(action);
        }

        public void Execute(Action onFinish = null)
        {
            m_bDone = false;
            DoAction();
            m_bDone = true;

            onFinish?.Invoke();
        }

        public bool IsDone()
        {
            return m_bDone;
        }

        public void Clear()
        {
            m_queue.Clear();
            m_bDone = true;
        }

        private void DoAction()
        {
            if (m_queue.Count > 0)
            {
                Func<bool> action = m_queue.Dequeue();
                if (action())
                {
                    DoAction();
                    return;
                }
            }
        }

        public void Demo()
        {
            Func<bool> act1 = () =>
            {
                Console.WriteLine("action 1");
                return true;
            };

            Func<bool> act2 = () =>
            {
                Console.WriteLine("action 2");
                return true;
            };

            ActionChain chain = new ActionChain();
            chain.Add(act1);
            chain.Add(act2);
            chain.Execute(() =>
            {
                Console.WriteLine("action done");
            });
        }
    }
}