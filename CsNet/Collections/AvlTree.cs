using System;
using System.Collections.Generic;

namespace CsNet.Collections
{
    public class AvlTree<K, V>
    {
        public delegate void TraverseActionRef(K key, ref V value);
        public delegate void TraverseAction(K key, V value);

        private class Node
        {
            public int hashCode;
            public K key;
            public V value;
            public Node parent;
            public Node lchild;
            public Node rchild;
            public sbyte balance;
            public bool marked; // 非递归遍历时使用
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

        private Node m_newNode;
        private int m_hashCode;
        private bool m_heightChanged;

        private Queue<Node> m_cacheQueue;
        private Stack<Node> m_traverseStack;

        private IEqualityComparer<K> m_comparer;

        public AvlTree(int capacity = 0)
            : this(null, capacity)
        {
        }

        public AvlTree(IEqualityComparer<K> comparer, int capacity = 0)
        {
            m_cacheQueue = new Queue<Node>(capacity);
            m_traverseStack = new Stack<Node>();
            m_comparer = comparer ?? EqualityComparer<K>.Default;
            for (int i = 0; i < capacity; ++i)
            {
                m_cacheQueue.Enqueue(new Node());
            }
        }

        #region Add
        public void Add(K key, V value)
        {
            if (m_root == null)
            {
                m_root = NewNode(key, value);
            }
            else
            {
                m_heightChanged = false;
                m_newNode = NewNode(key, value);

                Add(m_root);

                m_newNode = null;
            }
        }

        private void Add(Node target)
        {
            int cmp = m_newNode.hashCode.CompareTo(target.hashCode);
            if (cmp < 0)
            {
                if (target.lchild == null)
                {
                    m_newNode.parent = target;
                    target.lchild = m_newNode;
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
                    m_newNode.parent = target;
                    target.rchild = m_newNode;
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
        #endregion Add

        #region Remove
        public bool Remove(K key)
        {
            bool ret = false;
            if (m_root != null)
            {
                m_heightChanged = false;
                m_hashCode = m_comparer.GetHashCode(key);
                ret = Remove(m_root);
            }
            return ret;
        }

        private bool Remove(Node target)
        {
            int cmp = m_hashCode.CompareTo(target.hashCode);
            if (cmp < 0)
            {
                if (target.lchild == null)
                    return false;
                bool ret = Remove(target.lchild);
                if (ret && m_heightChanged)
                {
                    target.balance--;
                    CheckLeftDown(target);
                }
                return ret;
            }
            else if (cmp > 0)
            {
                if (target.rchild == null)
                    return false;
                bool ret = Remove(target.rchild);
                if (ret && m_heightChanged)
                {
                    target.balance++;
                    CheckRightDown(target);
                }
                return ret;
            }
            else
            {
                if (target.lchild == null)
                {
                    PromoteRightChild(target);
                    m_heightChanged = true;
                }
                else if (target.rchild == null)
                {
                    PromoteLeftChild(target);
                    m_heightChanged = true;
                }
                else if (target.balance == Balance.LH) // 从较高的子树上选择替换节点
                {
                    var node = RemoveRightest(target.lchild);
                    ReplaceNode(target, node);
                    if (m_heightChanged)
                    {
                        node.balance--;
                        CheckLeftDown(node);
                    }
                }
                else
                {
                    var node = RemoveLeftest(target.rchild);
                    ReplaceNode(target, node);
                    if (m_heightChanged)
                    {
                        node.balance++;
                        CheckRightDown(node);
                    }
                }

                DelNode(target);
                return true;
            }
        }

        private Node RemoveRightest(Node target)
        {
            if (target.rchild == null)
            {
                PromoteLeftChild(target);
                m_heightChanged = true;
                return target;
            }
            else
            {
                var node = RemoveRightest(target.rchild);
                if (m_heightChanged)
                {
                    target.balance++;
                    CheckRightDown(target);
                }
                return node;
            }
        }

        private Node RemoveLeftest(Node target)
        {
            if (target.lchild == null)
            {
                PromoteRightChild(target);
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
                from.parent = null;
            }

            to.balance = from.balance;

            if (m_root == from)
            {
                m_root = to;
            }
        }

        private void PromoteLeftChild(Node target)
        {
            Node child = target.lchild;

            if (child != null)
            {
                child.parent = target.parent;
                target.lchild = null;
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
        }

        private void PromoteRightChild(Node target)
        {
            Node child = target.rchild;

            if (child != null)
            {
                child.parent = target.parent;
                target.rchild = null;
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
        }
        #endregion Remove

        #region Recursive Traverse
        public void TraversePreOrder_r(TraverseAction action)
        {
            if (m_root != null)
            {
                TraversePreOrder_r(m_root, action);
            }
        }

        private void TraversePreOrder_r(Node node, TraverseAction action)
        {
            action(node.key, node.value);

            if (node.lchild != null)
                TraversePreOrder_r(node.lchild, action);

            if (node.rchild != null)
                TraversePreOrder_r(node.rchild, action);
        }

        public void TraverseInOrder_r(TraverseAction action)
        {
            if (m_root != null)
            {
                TraverseInOrder_r(m_root, action);
            }
        }

        private void TraverseInOrder_r(Node node, TraverseAction action)
        {
            if (node.lchild != null)
                TraverseInOrder_r(node.lchild, action);

            action(node.key, node.value);

            if (node.rchild != null)
                TraverseInOrder_r(node.rchild, action);
        }

        public void TraversePostOrder_r(TraverseAction action)
        {
            if (m_root != null)
            {
                TraversePostOrder_r(m_root, action);
            }
        }

        private void TraversePostOrder_r(Node node, TraverseAction action)
        {
            if (node.lchild != null)
                TraversePostOrder_r(node.lchild, action);

            if (node.rchild != null)
                TraversePostOrder_r(node.rchild, action);

            action(node.key, node.value);
        }
        #endregion Recursive Traverse

        #region Non Recursive Traverse
        public void TraversePreOrder(TraverseAction action)
        {
            if (m_root == null)
                return;

            Node node = m_root;
            m_traverseStack.Clear();
            do
            {
                action(node.key, node.value);

                if (node.rchild != null)
                {
                    m_traverseStack.Push(node.rchild);
                }

                if (node.lchild != null)
                {
                    node = node.lchild;
                }
                else
                {
                    if (m_traverseStack.Count > 0)
                        node = m_traverseStack.Pop();
                    else
                        break;
                }
            } while (true);
        }

        public void TraverseInOrder(TraverseAction action)
        {
            if (m_root == null)
                return;

            m_traverseStack.Clear();
            PushInOrder(m_root);

            Node node;
            while (m_traverseStack.Count > 0)
            {
                node = m_traverseStack.Peek();

                while (!node.marked)
                {
                    node.marked = true;
                    if (node.lchild != null)
                    {
                        PushInOrder(node.lchild);
                        node = node.lchild;
                    }
                }

                m_traverseStack.Pop();
                action(node.key, node.value);

                if (node.rchild != null)
                {
                    PushInOrder(node.rchild);
                }
            }
        }

        private void PushInOrder(Node node)
        {
            node.marked = false;
            m_traverseStack.Push(node);
        }

        public void TraversePostOrder(TraverseAction action)
        {
            if (m_root == null)
                return;

            m_traverseStack.Clear();
            PushPostOrder(m_root);

            Node node;
            bool hasChild;
            while (m_traverseStack.Count > 0)
            {
                node = m_traverseStack.Peek();
                hasChild = false;

                if (!node.marked)
                {
                    if (node.rchild != null)
                    {
                        PushPostOrder(node.rchild);
                        hasChild = true;
                    }
                    if (node.lchild != null)
                    {
                        PushPostOrder(node.lchild);
                        hasChild = true;
                    }
                    node.marked = true;
                }

                if (!hasChild)
                {
                    m_traverseStack.Pop();
                    action(node.key, node.value);
                }
            }
        }

        private void PushPostOrder(Node node)
        {
            node.marked = false;
            m_traverseStack.Push(node);
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

        #region Balance
        /// <summary>
        /// 左旋
        /// </summary>
        /// <param name="A"></param>
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

        /// <summary>
        /// 右旋
        /// </summary>
        /// <param name="A"></param>
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

        /// <summary>
        /// 左子树升高后平衡调整
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
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
                                throw new InvalidBalanceException("left up, rd", rd.balance);
                        }
                        rd.balance = Balance.EH;
                        LeftRotation(lc);
                        RightRotation(target);
                    }
                    break;
                default:
                    throw new InvalidBalanceException("left up, lc", lc.balance);
            }
            return false;
        }

        /// <summary>
        /// 右子树升高后平衡调整
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
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
                                throw new InvalidBalanceException("right up, ld", ld.balance);
                        }
                        ld.balance = Balance.EH;
                        RightRotation(rc);
                        LeftRotation(target);
                    }
                    break;
                default:
                    throw new InvalidBalanceException("right up, rc", rc.balance);
            }
            return false;
        }

        /// <summary>
        /// 左子树降低后平衡调整
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
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
                                throw new InvalidBalanceException("left down, ld", ld.balance);
                        }
                        ld.balance = Balance.EH;
                        RightRotation(rc);
                        LeftRotation(target);
                        heightChanged = true;
                    }
                    break;
                default:
                    throw new InvalidBalanceException("left down, rc", rc.balance);
            }
            return heightChanged;
        }

        /// <summary>
        /// 右子树降低后平衡调整
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
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
                                throw new InvalidBalanceException("right down, rd", rd.balance);
                        }
                        rd.balance = Balance.EH;
                        LeftRotation(lc);
                        RightRotation(target);
                        heightChanged = true;
                    }
                    break;
                default:
                    throw new InvalidBalanceException("right down, lc", lc.balance);
            }
            return heightChanged;
        }

        /// <summary>
        /// 左子树升高后平衡检查
        /// </summary>
        /// <param name="target"></param>
        private void CheckLeftUp(Node target)
        {
            switch (target.balance)
            {
                case Balance.LL:
                    m_heightChanged = LeftUpBalance(target);
                    break;
                case Balance.LH:
                    m_heightChanged = true; // 左子树升高，高度增大
                    break;
                case Balance.EH:
                    m_heightChanged = false;
                    break;
                default:
                    m_heightChanged = false;
                    throw new InvalidBalanceException("left up, t", target.balance);
            }
        }

        /// <summary>
        /// 左子树降低后平衡检查
        /// </summary>
        /// <param name="target"></param>
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
                    throw new InvalidBalanceException("left down, t", target.balance);
            }
        }

        /// <summary>
        /// 右子树升高后平衡检查
        /// </summary>
        /// <param name="target"></param>
        private void CheckRightUp(Node target)
        {
            switch (target.balance)
            {
                case Balance.RR:
                    m_heightChanged = RightUpBalance(target);
                    break;
                case Balance.RH:
                    m_heightChanged = true; // 右子树升高，高度增大
                    break;
                case Balance.EH:
                    m_heightChanged = false;
                    break;
                default:
                    m_heightChanged = false;
                    throw new InvalidBalanceException("right up, t", target.balance);
            }
        }

        /// <summary>
        /// 右子树降低后平衡检查
        /// </summary>
        /// <param name="target"></param>
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
                    throw new InvalidBalanceException("right down, t", target.balance);
            }
        }
        #endregion Balance

        private Node FindKey(K key)
        {
            Node node = m_root;
            int hashCode = m_comparer.GetHashCode(key);
            int cmp;
            while (node != null)
            {
                cmp = hashCode.CompareTo(node.hashCode);
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
            node.hashCode = m_comparer.GetHashCode(key);
            return node;
        }

        private void DelNode(Node node)
        {
            m_cacheQueue.Enqueue(node);
        }

        public bool _ValidBalance()
        {
            if (m_root == null)
                return true;
            else
                return _ValidBalance(m_root);
        }

        private bool _ValidBalance(Node node)
        {
            if (node == null)
                return true;

            if (node.balance < Balance.RH || node.balance > Balance.LH)
            {
                throw new InvalidBalanceException(string.Format("node[{0}]", node.hashCode), node.balance);
                //return false;
            }

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

        public class InvalidBalanceException : Exception
        {
            public InvalidBalanceException(string where, int value)
                : base(string.Format("balance invalid. {0} = {1}", where, value))
            {
            }
        }
    }
}
