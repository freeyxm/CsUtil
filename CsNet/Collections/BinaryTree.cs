using System;
using System.Collections.Generic;

namespace CsNet.Collections
{
    public class BinaryTreeNode<K, V, Node> where Node : BinaryTreeNode<K, V, Node>
    {
        public int hashCode;
        public K key;
        public V value;
        public Node parent;
        public Node lchild;
        public Node rchild;
        public bool marked; // used by non recursive traversal.
    }

    public abstract class BinaryTree<K, V, Node> where Node : BinaryTreeNode<K, V, Node>, new()
    {
        protected Node m_root;
        protected Node Nil; // null node.
        protected IEqualityComparer<K> m_comparer;
        private Cache<Node> m_nodeCache;
        private Stack<Node> m_traverseStack; // used by non recursive traversal.

        public BinaryTree(IEqualityComparer<K> comparer, int capacity, Node nil)
        {
            m_root = Nil = nil;
            m_comparer = comparer ?? EqualityComparer<K>.Default;
            m_traverseStack = new Stack<Node>();
            m_nodeCache = new Cache<Node>(capacity);
            for (int i = 0; i < capacity; ++i)
            {
                m_nodeCache.FreeNode(new Node());
            }
        }

        private BinaryTree()
        {
        }

        public abstract void Add(K key, V value);

        public abstract bool Remove(K key);

        public virtual bool ContainsKey(K key)
        {
            return FindKey(key) != Nil;
        }

        public virtual V this[K key]
        {
            get
            {
                Node node = FindKey(key);
                if (node == Nil)
                    throw new KeyNotFoundException(string.Format("key {0} not found", key));
                return node.value;
            }
            set
            {
                Node node = FindKey(key);
                if (node == Nil)
                    throw new KeyNotFoundException(string.Format("key {0} not found", key));
                node.value = value;
            }
        }

        public virtual void Clear()
        {
            if (m_root != Nil)
            {
                Clear(m_root);
                m_root = Nil;
            }
        }

        private void Clear(Node node)
        {
            if (node.lchild != Nil)
                Clear(node.lchild);

            if (node.rchild != Nil)
                Clear(node.rchild);

            DelNode(node);
        }

        #region Traverse
        public delegate void TraverseActionRef(K key, ref V value);
        public delegate void TraverseAction(K key, V value);

        #region Recursive Traverse
        public virtual void TraversePreOrder_r(TraverseAction action)
        {
            if (m_root != Nil)
            {
                TraversePreOrder_r(m_root, action);
            }
        }

        public virtual void TraverseInOrder_r(TraverseAction action)
        {
            if (m_root != Nil)
            {
                TraverseInOrder_r(m_root, action);
            }
        }

        public virtual void TraversePostOrder_r(TraverseAction action)
        {
            if (m_root != Nil)
            {
                TraversePostOrder_r(m_root, action);
            }
        }

        private void TraversePreOrder_r(BinaryTreeNode<K, V, Node> node, TraverseAction action)
        {
            action(node.key, node.value);

            if (node.lchild != Nil)
                TraversePreOrder_r(node.lchild, action);

            if (node.rchild != Nil)
                TraversePreOrder_r(node.rchild, action);
        }

        private void TraverseInOrder_r(BinaryTreeNode<K, V, Node> node, TraverseAction action)
        {
            if (node.lchild != Nil)
                TraverseInOrder_r(node.lchild, action);

            action(node.key, node.value);

            if (node.rchild != Nil)
                TraverseInOrder_r(node.rchild, action);
        }

        private void TraversePostOrder_r(BinaryTreeNode<K, V, Node> node, TraverseAction action)
        {
            if (node.lchild != Nil)
                TraversePostOrder_r(node.lchild, action);

            if (node.rchild != Nil)
                TraversePostOrder_r(node.rchild, action);

            action(node.key, node.value);
        }
        #endregion Recursive Traverse

        #region Non Recursive Traverse
        public virtual void TraversePreOrder(TraverseAction action)
        {
            if (m_root == Nil)
                return;

            Node node = m_root;
            m_traverseStack.Clear();
            do
            {
                action(node.key, node.value);

                if (node.rchild != Nil)
                {
                    m_traverseStack.Push(node.rchild);
                }

                if (node.lchild != Nil)
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

        public virtual void TraverseInOrder(TraverseAction action)
        {
            if (m_root == Nil)
                return;

            m_traverseStack.Clear();
            PushInOrder(m_root as Node);

            Node node;
            while (m_traverseStack.Count > 0)
            {
                node = m_traverseStack.Peek();

                while (!node.marked)
                {
                    node.marked = true;
                    if (node.lchild != Nil)
                    {
                        PushInOrder(node.lchild);
                        node = node.lchild;
                    }
                }

                m_traverseStack.Pop();
                action(node.key, node.value);

                if (node.rchild != Nil)
                {
                    PushInOrder(node.rchild);
                }
            }
        }

        public virtual void TraversePostOrder(TraverseAction action)
        {
            if (m_root == Nil)
                return;

            m_traverseStack.Clear();
            PushPostOrder(m_root as Node);

            Node node;
            bool hasChild;
            while (m_traverseStack.Count > 0)
            {
                node = m_traverseStack.Peek();
                hasChild = false;

                if (!node.marked)
                {
                    if (node.rchild != Nil)
                    {
                        PushPostOrder(node.rchild);
                        hasChild = true;
                    }
                    if (node.lchild != Nil)
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

        private void PushInOrder(Node node)
        {
            node.marked = false;
            m_traverseStack.Push(node);
        }

        private void PushPostOrder(Node node)
        {
            node.marked = false;
            m_traverseStack.Push(node);
        }
        #endregion Non Recursive Traverse
        #endregion Traverse

        protected virtual void RotateLeft(Node A)
        {
            Node B = A.rchild;

            A.rchild = B.lchild;
            if (B.lchild != Nil)
                B.lchild.parent = A;
            B.lchild = A;

            B.parent = A.parent;
            if (m_root == A)
                m_root = B;
            else if (A.parent.lchild == A)
                A.parent.lchild = B;
            else
                A.parent.rchild = B;
            A.parent = B;
        }

        protected virtual void RotateRight(Node A)
        {
            Node B = A.lchild;

            A.lchild = B.rchild;
            if (B.rchild != Nil)
                B.rchild.parent = A;
            B.rchild = A;

            B.parent = A.parent;
            if (m_root == A)
                m_root = B;
            else if (A.parent.lchild == A)
                A.parent.lchild = B;
            else
                A.parent.rchild = B;
            A.parent = B;
        }

        protected virtual Node FindKey(K key)
        {
            Node node = m_root;
            int hashCode = m_comparer.GetHashCode(key);
            int cmp;
            while (node != Nil)
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

        protected virtual Node GetLeftestNode(Node node)
        {
            while (node.lchild != Nil)
            {
                node = node.lchild;
            }
            return node;
        }

        protected virtual Node GetRightestNode(Node node)
        {
            while (node.rchild != Nil)
            {
                node = node.rchild;
            }
            return node;
        }

        protected virtual void ReplaceNode(Node from, Node to)
        {
            to.lchild = from.lchild;
            if (from.lchild != Nil)
            {
                from.lchild.parent = to;
                from.lchild = Nil;
            }

            to.rchild = from.rchild;
            if (from.rchild != Nil)
            {
                from.rchild.parent = to;
                from.rchild = Nil;
            }

            to.parent = from.parent;
            if (m_root == from)
                m_root = to;
            else if (from.parent.lchild == from)
                from.parent.lchild = to;
            else
                from.parent.rchild = to;
            from.parent = Nil;
        }

        protected virtual Node NewNode(K key, V value, Node parent)
        {
            Node node = m_nodeCache.AllocNode();
            node.key = key;
            node.value = value;
            node.parent = parent;
            node.lchild = Nil;
            node.rchild = Nil;
            node.hashCode = m_comparer.GetHashCode(key);
            return node;
        }

        protected virtual void DelNode(Node node)
        {
            m_nodeCache.FreeNode(node);
        }
    }
}
