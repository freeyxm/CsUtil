using System;
using System.Collections.Generic;

namespace CsNet.Util
{
    public class AvlTree<K, V>
    {
        public delegate void TraverseActionRef(K key, ref V value);
        public delegate void TraverseAction(K key, V value);

        private class Node
        {
            public K key;
            public V value;
            public Node parent;
            public Node lchild;
            public Node rchild;
            public sbyte balance;
            public bool marked;
        }
        private class Balance
        {
            public const sbyte RR = -2;
            public const sbyte RH = -1;
            public const sbyte EH = 0;
            public const sbyte LH = 1;
            public const sbyte LL = 2;
        }

        private Node m_root;

        private K m_curKey;
        private V m_curValue;
        private bool m_heightChanged;

        private Queue<Node> m_cacheQueue;
        private Queue<Node> m_traverseQueue;
        private Stack<Node> m_traverseStack;

        public AvlTree(int capacity = 0)
        {
            m_cacheQueue = new Queue<Node>(capacity);
            m_traverseQueue = new Queue<Node>();
            m_traverseStack = new Stack<Node>();
        }

        #region Insert
        public void Add(K key, V value)
        {
            if (m_root == null)
            {
                m_root = NewNode(key, value, null);
            }
            else
            {
                m_heightChanged = false;
                m_curKey = key;
                m_curValue = value;

                Add(m_root);

                m_curKey = default(K);
                m_curValue = default(V);
            }
        }

        private void Add(Node target)
        {
            int cmp = ComparKey(m_curKey, target.key);
            if (cmp < 0)
            {
                if (target.lchild == null)
                {
                    target.lchild = NewNode(m_curKey, m_curValue, target);
                    target.balance++;
                    if (target.balance == Balance.LH)
                        m_heightChanged = true;
                }
                else
                {
                    Add(target.lchild);
                    if (m_heightChanged)
                    {
                        target.balance++;
                        CheckLeftUp(target);
                    }
                }
            }
            else
            {
                if (target.rchild == null)
                {
                    target.rchild = NewNode(m_curKey, m_curValue, target);
                    target.balance--;
                    if (target.balance == Balance.RH)
                        m_heightChanged = true;
                }
                else
                {
                    Add(target.rchild);
                    if (m_heightChanged)
                    {
                        target.balance--;
                        CheckRightUp(target);
                    }
                }
            }
        }
        #endregion Insert

        #region Remove
        public bool Remove(K key)
        {
            bool ret = false;
            if (m_root != null)
            {
                m_heightChanged = false;
                m_curKey = key;

                ret = Remove(ref m_root);

                m_curKey = default(K);
            }
            return ret;
        }

        private bool Remove(ref Node root)
        {
            int cmp = ComparKey(m_curKey, root.key);
            if (cmp == 0)
            {
                var tmp = root;
                if (root.lchild == null || root.rchild == null)
                {
                    PromoteChild(root);
                    m_heightChanged = true;
                }
                else
                {
                    var node = RemoveLeftest(root.rchild);
                    ReplaceNode(root, node);
                    if (m_heightChanged)
                    {
                        node.balance++;
                        CheckRightDown(node);
                    }
                }

                DelNode(tmp);
                return true;
            }
            else if (cmp < 0)
            {
                if (root.lchild == null)
                    return false;
                bool ret = Remove(ref root.lchild);
                if (ret && m_heightChanged)
                {
                    root.balance--;
                    CheckLeftDown(root);
                }
                return ret;
            }
            else
            {
                if (root.rchild == null)
                    return false;
                bool ret = Remove(ref root.rchild);
                if (ret && m_heightChanged)
                {
                    root.balance++;
                    CheckRightDown(root);
                }
                return ret;
            }
        }

        private Node RemoveLeftest(Node target)
        {
            if (target.lchild == null)
            {
                PromoteChild(target);
                m_heightChanged = true;
                return target;
            }
            else
            {
                var node = RemoveLeftest(target.lchild);
                if (m_heightChanged)
                {
                    target.balance--;
                    CheckLeftDown(target);
                }
                return node;
            }
        }

        private void ReplaceNode(Node from, Node to)
        {
            to.lchild = from.lchild;
            if (from.lchild != null)
            {
                from.lchild.parent = to;
                from.lchild = null;
            }

            to.rchild = from.rchild;
            if (from.rchild != null)
            {
                from.rchild.parent = to;
                from.rchild = null;
            }

            to.parent = from.parent;
            if (from.parent != null)
            {
                if (from.parent.lchild == from)
                    from.parent.lchild = to;
                else
                    from.parent.rchild = to;
            }

            to.balance = from.balance;

            if (m_root == from)
            {
                m_root = to;
            }
        }

        private bool PromoteChild(Node target)
        {
            Node child = null;
            if (target.rchild == null)
            {
                child = target.lchild;
                target.lchild = null;
            }
            else if (target.lchild == null)
            {
                child = target.rchild;
                target.rchild = null;
            }
            else
            {
                return false;
            }

            if (child != null)
            {
                child.parent = target.parent;
            }

            if (target.parent != null)
            {
                if (target.parent.lchild == target)
                    target.parent.lchild = child;
                else
                    target.parent.rchild = child;
                target.parent = null;
            }

            if (m_root == target)
            {
                m_root = child;
            }

            return true;
        }
        #endregion Remove

        #region Recursive Traverse
        public void TraversePreOrder(TraverseActionRef action)
        {
            if (m_root != null)
            {
                TraversePreOrder(m_root, action);
            }
        }

        private void TraversePreOrder(Node node, TraverseActionRef action)
        {
            action(node.key, ref node.value);

            if (node.lchild != null)
                TraversePreOrder(node.lchild, action);

            if (node.rchild != null)
                TraversePreOrder(node.rchild, action);
        }

        public void TraversePreOrder_nf(TraverseAction action)
        {
            if (m_root != null)
            {
                TraversePreOrder_nf(m_root, action);
            }
        }

        private void TraversePreOrder_nf(Node node, TraverseAction action)
        {
            action(node.key, node.value);

            if (node.lchild != null)
                TraversePreOrder_nf(node.lchild, action);

            if (node.rchild != null)
                TraversePreOrder_nf(node.rchild, action);
        }

        public void TraverseInOrder(TraverseActionRef action)
        {
            if (m_root != null)
            {
                TraverseInOrder(m_root, action);
            }
        }

        private void TraverseInOrder(Node node, TraverseActionRef action)
        {
            if (node.lchild != null)
                TraverseInOrder(node.lchild, action);

            action(node.key, ref node.value);

            if (node.rchild != null)
                TraverseInOrder(node.rchild, action);
        }

        public void TraversePostOrder(TraverseActionRef action)
        {
            if (m_root != null)
            {
                TraversePostOrder(m_root, action);
            }
        }

        private void TraversePostOrder(Node node, TraverseActionRef action)
        {
            if (node.lchild != null)
                TraversePostOrder(node.lchild, action);

            if (node.rchild != null)
                TraversePostOrder(node.rchild, action);

            action(node.key, ref node.value);
        }
        #endregion Recursive Traverse

        #region Non Recursive Traverse
        public void TraversePreOrder_nr(TraverseActionRef action)
        {
            m_traverseStack.Clear();
            if (m_root != null)
            {
                m_traverseStack.Push(m_root);
            }

            Node node;
            while (m_traverseStack.Count > 0)
            {
                node = m_traverseStack.Pop();
                action(node.key, ref node.value);

                if (node.rchild != null)
                {
                    m_traverseStack.Push(node.rchild);
                }

                if (node.lchild != null)
                {
                    m_traverseStack.Push(node.lchild);
                }
            }
        }

        public void TraverseInOrder_nr(TraverseActionRef action)
        {
            m_traverseStack.Clear();
            if (m_root != null)
            {
                PushInOrder(m_root);
            }

            Node node;
            while (m_traverseStack.Count > 0)
            {
                node = m_traverseStack.Peek();

                while (node.lchild != null && !node.lchild.marked)
                {
                    PushInOrder(node.lchild);
                    node = node.lchild;
                }

                m_traverseStack.Pop();
                action(node.key, ref node.value);
                node.marked = true;

                if (node.rchild != null)
                {
                    PushInOrder(node.rchild);
                }
            }
        }

        private void PushInOrder(Node node)
        {
            if (node.lchild != null)
            {
                node.lchild.marked = false;
            }
            node.marked = false;
            m_traverseStack.Push(node);
        }

        public void TraversePostOrder_nr(TraverseActionRef action)
        {
            m_traverseStack.Clear();
            if (m_root != null)
            {
                PushPostOrder(m_root);
            }

            Node node;
            while (m_traverseStack.Count > 0)
            {
                node = m_traverseStack.Peek();
                bool hasChild = false;

                if (node.rchild != null && !node.rchild.marked)
                {
                    PushPostOrder(node.rchild);
                    hasChild = true;
                }

                if (node.lchild != null && !node.lchild.marked)
                {
                    PushPostOrder(node.lchild);
                    hasChild = true;
                }

                if (!hasChild)
                {
                    m_traverseStack.Pop();
                    action(node.key, ref node.value);
                }
            }
        }

        private void PushPostOrder(Node node)
        {
            if (node.lchild != null)
            {
                node.lchild.marked = false;
            }
            if (node.rchild != null)
            {
                node.rchild.marked = false;
            }
            node.marked = true;
            m_traverseStack.Push(node);
        }

        private void Enqueue(Node node)
        {
            node.marked = true;

            if (node.lchild != null)
                node.lchild.marked = false;
            if (node.rchild != null)
                node.rchild.marked = false;

            m_traverseQueue.Enqueue(node);
        }
        #endregion Non Recursive Traverse

        public bool ContainsKey(K key)
        {
            return FindKey(key) != null;
        }

        public V this[K key]
        {
            get
            {
                var node = FindKey(key);
                if (node == null)
                    throw new KeyNotFoundException(string.Format("key {0} not found", key));
                return node.value;
            }
            set
            {
                var node = FindKey(key);
                if (node == null)
                    throw new KeyNotFoundException(string.Format("key {0} not found", key));
                node.value = value;
            }
        }

        public void Clear()
        {
            if (m_root != null)
            {
                Clear(m_root);
                m_root = null;
            }
        }

        private void Clear(Node node)
        {
            DelNode(node); // !!!

            if (node.lchild != null)
                Clear(node.lchild);

            if (node.rchild != null)
                Clear(node.rchild);
        }

        #region Rotation
        private void LeftRotation(Node A)
        {
            Node B = A.rchild;

            A.rchild = B.lchild;
            if (B.lchild != null)
            {
                B.lchild.parent = A;
            }
            B.lchild = A;

            B.parent = A.parent;
            if (A.parent != null)
            {
                if (A.parent.lchild == A)
                    A.parent.lchild = B;
                else
                    A.parent.rchild = B;
            }
            A.parent = B;

            if (m_root == A)
            {
                m_root = B;
            }
        }

        private void RightRotation(Node A)
        {
            Node B = A.lchild;

            A.lchild = B.rchild;
            if (B.rchild != null)
            {
                B.rchild.parent = A;
            }
            B.rchild = A;

            B.parent = A.parent;
            if (A.parent != null)
            {
                if (A.parent.lchild == A)
                    A.parent.lchild = B;
                else
                    A.parent.rchild = B;
            }
            A.parent = B;

            if (m_root == A)
            {
                m_root = B;
            }
        }

        private bool LeftUpBalance(Node target)
        {
            Node lc = target.lchild;
            switch (lc.balance)
            {
                case Balance.LH:
                    {
                        lc.balance = Balance.EH;
                        target.balance = Balance.EH;
                        RightRotation(target);
                    }
                    break;
                case Balance.RH:
                    {
                        Node rd = lc.rchild;
                        switch (rd.balance)
                        {
                            case Balance.LH:
                                lc.balance = Balance.EH;
                                target.balance = Balance.RH;
                                break;
                            case Balance.RH:
                                lc.balance = Balance.LH;
                                target.balance = Balance.EH;
                                break;
                            case Balance.EH:
                                lc.balance = Balance.EH;
                                target.balance = Balance.EH;
                                break;
                            default:
                                throw new Exception("balance invalid.");
                        }
                        rd.balance = Balance.EH;
                        LeftRotation(lc);
                        RightRotation(target);
                    }
                    break;
                default:
                    throw new Exception("balance invalid.");
            }
            return false;
        }

        private bool RightUpBalance(Node target)
        {
            Node rc = target.rchild;
            switch (rc.balance)
            {
                case Balance.RH:
                    {
                        rc.balance = Balance.EH;
                        target.balance = Balance.EH;
                        LeftRotation(target);
                    }
                    break;
                case Balance.LH:
                    {
                        Node ld = rc.lchild;
                        switch (ld.balance)
                        {
                            case Balance.LH:
                                rc.balance = Balance.RH;
                                target.balance = Balance.EH;
                                break;
                            case Balance.RH:
                                rc.balance = Balance.EH;
                                target.balance = Balance.LH;
                                break;
                            case Balance.EH:
                                rc.balance = Balance.EH;
                                target.balance = Balance.EH;
                                break;
                            default:
                                throw new Exception("balance invalid.");
                        }
                        ld.balance = Balance.EH;
                        RightRotation(rc);
                        LeftRotation(target);
                    }
                    break;
                default:
                    throw new Exception("balance invalid.");
            }
            return false;
        }

        private bool LeftDownBalance(Node target)
        {
            bool heightChanged = false;

            Node rc = target.rchild;
            switch (rc.balance)
            {
                case Balance.EH:
                    {
                        rc.balance = Balance.LH;
                        target.balance = Balance.RH;
                        LeftRotation(target);
                        heightChanged = false;
                    }
                    break;
                case Balance.RH:
                    {
                        rc.balance = Balance.EH;
                        target.balance = Balance.EH;
                        LeftRotation(target);
                        heightChanged = true;
                    }
                    break;
                case Balance.LH:
                    {
                        Node ld = rc.lchild;
                        switch (ld.balance)
                        {
                            case Balance.LH:
                                rc.balance = Balance.RH;
                                target.balance = Balance.EH;
                                break;
                            case Balance.RH:
                                rc.balance = Balance.EH;
                                target.balance = Balance.LH;
                                break;
                            case Balance.EH:
                                rc.balance = Balance.EH;
                                target.balance = Balance.EH;
                                break;
                            default:
                                throw new Exception("balance invalid.");
                        }
                        ld.balance = Balance.EH;
                        RightRotation(rc);
                        LeftRotation(target);
                        heightChanged = true;
                    }
                    break;
                default:
                    throw new Exception("balance invalid.");
            }
            return heightChanged;
        }

        private bool RightDownBalance(Node target)
        {
            bool heightChanged = false;

            Node lc = target.lchild;
            switch (lc.balance)
            {
                case Balance.EH:
                    {
                        lc.balance = Balance.RH;
                        target.balance = Balance.LH;
                        RightRotation(target);
                        heightChanged = false;
                    }
                    break;
                case Balance.LH:
                    {
                        lc.balance = Balance.EH;
                        target.balance = Balance.EH;
                        RightRotation(target);
                        heightChanged = true;
                    }
                    break;
                case Balance.RH:
                    {
                        Node rd = lc.rchild;
                        switch (rd.balance)
                        {
                            case Balance.LH:
                                lc.balance = Balance.EH;
                                target.balance = Balance.RH;
                                break;
                            case Balance.RH:
                                lc.balance = Balance.LH;
                                target.balance = Balance.EH;
                                break;
                            case Balance.EH:
                                lc.balance = Balance.EH;
                                target.balance = Balance.EH;
                                break;
                            default:
                                throw new Exception("balance invalid.");
                        }
                        rd.balance = Balance.EH;
                        LeftRotation(lc);
                        RightRotation(target);
                        heightChanged = true;
                    }
                    break;
                default:
                    throw new Exception("balance invalid.");
            }
            return heightChanged;
        }

        private void CheckLeftUp(Node target)
        {
            switch (target.balance)
            {
                case Balance.LL:
                    m_heightChanged = LeftUpBalance(target);
                    break;
                case Balance.LH:
                    m_heightChanged = true; // already true.
                    break;
                case Balance.EH:
                    m_heightChanged = false;
                    break;
                default:
                    m_heightChanged = false;
                    throw new Exception("balance invalid.");
            }
        }

        private void CheckLeftDown(Node target)
        {
            switch (target.balance)
            {
                case Balance.RR:
                    m_heightChanged = LeftDownBalance(target);
                    break;
                case Balance.RH:
                    m_heightChanged = false;
                    break;
                case Balance.EH:
                    m_heightChanged = true; // 左子树降低后等高，高度减小
                    break;
                default:
                    m_heightChanged = false;
                    throw new Exception("balance invalid.");
            }
        }

        private void CheckRightUp(Node target)
        {
            switch (target.balance)
            {
                case Balance.RR:
                    m_heightChanged = RightUpBalance(target);
                    break;
                case Balance.RH:
                    m_heightChanged = true; // already true.
                    break;
                case Balance.EH:
                    m_heightChanged = false;
                    break;
                default:
                    m_heightChanged = false;
                    throw new Exception("balance invalid.");
            }
        }

        private void CheckRightDown(Node target)
        {
            switch (target.balance)
            {
                case Balance.LL:
                    m_heightChanged = RightDownBalance(target);
                    break;
                case Balance.LH:
                    m_heightChanged = false;
                    break;
                case Balance.EH:
                    m_heightChanged = true; // 右子树降低后等高，高度减小
                    break;
                default:
                    m_heightChanged = false;
                    throw new Exception("balance invalid.");
            }
        }
        #endregion Rotation

        private int ComparKey(K k1, K key2)
        {
            return k1.GetHashCode().CompareTo(key2.GetHashCode());
        }

        private Node FindKey(K key)
        {
            Node node = m_root;
            int cmp;
            while (node != null)
            {
                cmp = ComparKey(key, node.key);
                if (cmp == 0)
                    break;
                else if (cmp < 0)
                    node = node.lchild;
                else
                    node = node.rchild;
            }
            return node;
        }

        private Node GetLeftestNode(Node node)
        {
            while (node.lchild != null)
            {
                node = node.lchild;
            }
            return node;
        }

        private Node GetRightestNode(Node node)
        {
            while (node.rchild != null)
            {
                node = node.rchild;
            }
            return node;
        }

        private Node NewNode(K key, V value, Node parent = null)
        {
            Node node = m_cacheQueue.Count > 0 ? m_cacheQueue.Dequeue() : new Node();
            node.key = key;
            node.value = value;
            node.parent = parent;
            node.lchild = null;
            node.rchild = null;
            node.balance = 0;
            return node;
        }

        private void DelNode(Node node)
        {
            m_cacheQueue.Enqueue(node);
        }

        private bool _ValidBalance(Node node)
        {
            if (node == null)
                return true;

            if (node.balance < Balance.RH || node.balance > Balance.LH)
                return false;

            bool ret = true;
            if (ret && node.lchild != null)
            {
                ret = _ValidBalance(node.lchild);
            }
            if (ret && node.rchild != null)
            {
                ret = _ValidBalance(node.rchild);
            }
            return ret;
        }
    }
}
