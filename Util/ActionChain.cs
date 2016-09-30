using System;
using System.Collections.Generic;

namespace CsUtil.Util
{
    public class ActionChain
    {
        public delegate void ActionNode(System.Action next);
        private Queue<ActionNode> m_queue;
        private bool m_bDone;

        public ActionChain()
        {
            m_queue = new Queue<ActionNode>();
            m_bDone = true;
        }

        /// <summary>
        /// action must call next function, otherwise chain will broken.
        /// </summary>
        /// <param name="action"></param>
        public void Enqueue(ActionNode action)
        {
            m_queue.Enqueue(action);
        }

        public void Execute()
        {
            m_bDone = false;
            Enqueue(OnActionDone);
            DoAction();
        }

        public bool IsDone()
        {
            return m_bDone;
        }

        private void OnActionDone(System.Action next)
        {
            m_bDone = true;
        }

        private void DoAction()
        {
            if (m_queue.Count > 0)
            {
                ActionNode action = m_queue.Dequeue();
                action(DoAction);
            }
        }

        public void Demo()
        {
            ActionNode act1 = (System.Action next) =>
            {
                // do something ...
                if (next != null) next();
            };

            ActionNode act2 = (System.Action next) =>
            {
                // do something ...
                if (next != null) next();
            };

            ActionChain chain = new ActionChain();
            chain.Enqueue(act1);
            chain.Enqueue(act2);
            chain.Execute();
        }
    }
}