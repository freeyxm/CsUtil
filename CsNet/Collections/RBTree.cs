using System;
using System.Collections.Generic;

namespace CsNet.Collections
{
    public class RBTree<K, V> : BinaryTree<K, V>
    {
        private enum Color
        {
            Black,
            Red,
        }

        private class Node
        {
            public K key;
            public V value;
            public Node parent;
            public Node lchild;
            public Node rchild;
            public int hashCode;
            public Color color;
        }

        private static Node Nil;
        static RBTree()
        {
            Nil = new Node();
            Nil.color = Color.Black;
            Nil.lchild = Nil.rchild = Nil.parent = null;
        }

        private Node m_root;
        private Cache<Node> m_nodeCache;
        private IEqualityComparer<K> m_comparer;

        public RBTree(int capacity = 0)
            : this(null, capacity)
        {
        }

        public RBTree(IEqualityComparer<K> comparer, int capacity = 0)
        {
            m_root = Nil;
            m_comparer = comparer ?? EqualityComparer<K>.Default;
            m_nodeCache = new Cache<Node>(capacity);
        }

        public override void Add(K key, V value)
        {
            Insert(NewNode(key, value, Nil));
        }

        public override bool Remove(K key)
        {
            Node z = FindKey(key);
            if (z != Nil)
            {
                //Delete(z);
                delete_one_child(z);
                return true;
            }
            return false;
        }

        public override bool ContainsKey(K key)
        {
            return FindKey(key) != Nil;
        }

        public override V this[K key]
        {
            get
            {
                var node = FindKey(key);
                if (node == Nil)
                    throw new KeyNotFoundException(string.Format("key {0} not found", key));
                return node.value;
            }
            set
            {
                var node = FindKey(key);
                if (node == Nil)
                    throw new KeyNotFoundException(string.Format("key {0} not found", key));
                node.value = value;
            }
        }

        public override void Clear()
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
                Clear(node.lchild as Node);

            if (node.rchild != Nil)
                Clear(node.rchild as Node);

            DelNode(node);
        }

        #region Recursive Traverse
        public override void TraversePreOrder_r(TraverseAction action)
        {
            if (m_root != Nil)
            {
                TraversePreOrder_r(m_root, action);
            }
        }

        private void TraversePreOrder_r(Node node, TraverseAction action)
        {
            action(node.key, node.value);

            if (node.lchild != Nil)
                TraversePreOrder_r(node.lchild, action);

            if (node.rchild != Nil)
                TraversePreOrder_r(node.rchild, action);
        }

        public override void TraverseInOrder_r(TraverseAction action)
        {
            if (m_root != Nil)
            {
                TraverseInOrder_r(m_root, action);
            }
        }

        private void TraverseInOrder_r(Node node, TraverseAction action)
        {
            if (node.lchild != Nil)
                TraverseInOrder_r(node.lchild, action);

            action(node.key, node.value);

            if (node.rchild != Nil)
                TraverseInOrder_r(node.rchild, action);
        }

        public override void TraversePostOrder_r(TraverseAction action)
        {
            if (m_root != Nil)
            {
                TraversePostOrder_r(m_root, action);
            }
        }

        private void TraversePostOrder_r(Node node, TraverseAction action)
        {
            if (node.lchild != Nil)
                TraversePostOrder_r(node.lchild, action);

            if (node.rchild != Nil)
                TraversePostOrder_r(node.rchild, action);

            action(node.key, node.value);
        }
        #endregion Recursive Traverse

        private Node FindKey(K key)
        {
            int hashCode = m_comparer.GetHashCode(key);
            Node node = m_root;
            while (node != Nil)
            {
                int cmp = hashCode.CompareTo(node.hashCode);
                if (cmp < 0)
                    node = node.lchild;
                else if (cmp > 0)
                    node = node.rchild;
                else
                    break;
            }
            return node;
        }

        private void RotateLeft(Node A)
        {
            Node B = A.rchild;

            A.rchild = B.lchild;
            if (B.lchild != Nil)
                B.lchild.parent = A;

            B.parent = A.parent;
            if (A.parent == Nil)
                m_root = B;
            else if (A.parent.lchild == A)
                A.parent.lchild = B;
            else
                A.parent.rchild = B;

            B.lchild = A;
            A.parent = B;
        }

        private void RotateRight(Node A)
        {
            Node B = A.lchild;

            A.lchild = B.rchild;
            if (B.rchild != Nil)
                B.rchild.parent = A;

            B.parent = A.parent;
            if (A.parent == Nil)
                m_root = B;
            else if (A.parent.lchild == A)
                A.parent.lchild = B;
            else
                A.parent.rchild = B;

            B.rchild = A;
            A.parent = B;
        }

        #region Insert
        private void Insert(Node n)
        {
            Node tmp = m_root;
            Node p = Nil;

            while (tmp != Nil)
            {
                p = tmp;
                if (n.hashCode.CompareTo(tmp.hashCode) < 0)
                    tmp = tmp.lchild;
                else
                    tmp = tmp.rchild;
            }

            n.parent = p;
            if (m_root == Nil)
            {
                m_root = n;
            }
            else
            {
                if (n.hashCode.CompareTo(p.hashCode) < 0)
                    p.lchild = n;
                else
                    p.rchild = n;
            }

            insert_case1(n);
        }

        private void insert_case1(Node n)
        {
            if (n.parent == Nil)
                n.color = Color.Black;
            else
                insert_case2(n);
        }

        private void insert_case2(Node n)
        {
            if (n.parent.color == Color.Black)
                return;
            else
                insert_case3(n);
        }

        private void insert_case3(Node n)
        {
            Node u = Uncle(n);
            if (u != Nil && u.color == Color.Red)
            {
                Node g2 = Grandparent(n);
                n.parent.color = Color.Black;
                u.color = Color.Black;
                g2.color = Color.Red;
                insert_case1(g2);
            }
            else
            {
                insert_case4(n);
            }
        }

        private void insert_case4(Node n)
        {
            Node g = Grandparent(n);
            if (n == n.parent.rchild && n.parent == g.lchild)
            {
                RotateLeft(n.parent);
                n = n.lchild;
            }
            else if (n == n.parent.lchild && n.parent == g.rchild)
            {
                RotateRight(n.parent);
                n = n.rchild;
            }
            insert_case5(n);
        }

        private void insert_case5(Node n)
        {
            Node g = Grandparent(n);
            n.parent.color = Color.Black;
            g.color = Color.Red;
            if (n == n.parent.lchild)
                RotateRight(g);
            else
                RotateLeft(g);
        }

        private void InsertFixup(Node n)
        {
            while (n.parent.color == Color.Red)
            {
                if (n.parent == n.parent.parent.lchild)
                {
                    Node y = n.parent.parent.rchild;

                    if (y.color == Color.Red)
                    {
                        y.color = Color.Black;
                        n.parent.color = Color.Black;
                        n.parent.parent.color = Color.Red;
                        n = n.parent.parent;
                    }
                    else if (n == n.parent.rchild)
                    {
                        n = n.parent;
                        RotateLeft(n);
                    }

                    n.parent.color = Color.Black;
                    n.parent.parent.color = Color.Red;
                    RotateRight(n.parent.parent);
                }
            }

            m_root.color = Color.Black;
        }
        #endregion Insert

        #region Delete
        private void delete_one_child(Node n)
        {
            Node child = n.rchild == Nil ? n.lchild : n.rchild;

            replace_node(n, child);

            if (n.color == Color.Black)
            {
                if (child.color == Color.Red)
                {
                    child.color = Color.Black;
                }
                else
                {
                    delete_case1(child);
                }
            }

            DelNode(n);
        }

        private void delete_case1(Node n)
        {
            if (n.parent != Nil)
                delete_case2(n);
        }

        private void delete_case2(Node n)
        {
            Node s = Sibling(n);
            if (s.color == Color.Red)
            {
                s.color = Color.Black;
                n.parent.color = Color.Red;
                if (n == n.parent.lchild)
                    RotateLeft(n.parent);
                else
                    RotateRight(n.parent);
            }
            delete_case3(n);
        }

        private void delete_case3(Node n)
        {
            Node s = Sibling(n);
            if (n.parent.color == Color.Black
                && s.color == Color.Black
                && s.lchild.color == Color.Black
                && s.rchild.color == Color.Black)
            {
                s.color = Color.Red;
                delete_case1(n.parent);
            }
            else
            {
                delete_case4(n);
            }
        }

        private void delete_case4(Node n)
        {
            Node s = Sibling(n);
            if (n.parent.color == Color.Red
                && s.color == Color.Black
                && s.lchild.color == Color.Black
                && s.rchild.color == Color.Black)
            {
                s.color = Color.Red;
                n.parent.color = Color.Black;
            }
            else
            {
                delete_case5(n);
            }
        }

        private void delete_case5(Node n)
        {
            Node s = Sibling(n);
            if (s.color == Color.Black)
            {
                if (n == n.parent.lchild
                    && s.lchild.color == Color.Red
                    && s.rchild.color == Color.Black)
                {
                    s.color = Color.Red;
                    s.lchild.color = Color.Black;
                    RotateRight(s);
                }
                else if (n == n.parent.rchild
                    && s.lchild.color == Color.Black
                    && s.rchild.color == Color.Red)
                {
                    s.color = Color.Red;
                    s.rchild.color = Color.Black;
                    RotateLeft(s);
                }
            }
            delete_case6(n);
        }

        private void delete_case6(Node n)
        {
            Node s = Sibling(n);
            s.color = n.parent.color;
            n.parent.color = Color.Black;
            if (n == n.parent.lchild)
            {
                s.rchild.color = Color.Black;
                RotateLeft(n.parent);
            }
            else
            {
                s.lchild.color = Color.Black;
                RotateRight(n.parent);
            }
        }

        private void replace_node(Node n, Node child)
        {
            child.parent = n.parent;
            if (n.parent == Nil)
                m_root = child;
            else if (n.parent.lchild == n)
                n.parent.lchild = child;
            else
                n.parent.rchild = child;
        }

        private void Delete(Node z)
        {
            Node x = Nil;
            Node y = z;
            Color yColor = y.color;

            if (z.lchild == Nil)
            {
                x = z.rchild;
                Transplant(z, z.rchild);
            }
            else if (z.rchild == Nil)
            {
                x = z.lchild;
                Transplant(z, z.lchild);
            }
            else
            {
                y = Minimum(z.rchild);
                yColor = y.color;
                x = y.rchild;

                if (y.parent == z)
                {
                    x.parent = y;
                }
                else
                {
                    Transplant(y, y.rchild);
                    y.rchild = z.rchild;
                    y.rchild.parent = y;
                }

                Transplant(z, y);

                y.lchild = z.lchild;
                y.lchild.parent = y;
                y.color = z.color;
            }

            if (yColor == Color.Black)
            {
                DeleteFixup(x);
            }
        }

        private void DeleteFixup(Node x)
        {
            Node w = Nil;

            while (x != m_root && x.color == Color.Black)
            {
                if (x == x.parent.lchild)
                {
                    w = x.parent.rchild;
                    if (w.color == Color.Red)
                    {
                        // case 1
                        w.color = Color.Black;
                        x.parent.color = Color.Red;
                        RotateLeft(x.parent);
                        w = x.parent.rchild;
                    }
                    if (w.lchild.color == Color.Black && w.rchild.color == Color.Black)
                    {
                        // case 2
                        w.color = Color.Red;
                        x = x.parent;
                    }
                    else if (w.rchild.color == Color.Black)
                    {
                        // case 3
                        w.color = Color.Red;
                        w.lchild.color = Color.Black;
                        RotateRight(w);
                        w = x.parent.rchild;
                        // case 4
                        w.color = x.parent.color;
                        x.parent.color = Color.Black;
                        w.rchild.color = Color.Black;
                        RotateLeft(x.parent);
                        x = m_root;
                    }
                }
            }

            x.color = Color.Black;
        }

        private void Transplant(Node u, Node v)
        {
            if (u == Nil)
                m_root = v;
            else if (u.parent.lchild == u)
                u.parent.lchild = v;
            else
                u.parent.rchild = v;

            v.parent = u.parent;
        }

        private Node Minimum(Node z)
        {
            while (z.lchild != Nil)
            {
                z = z.lchild;
            }
            return z;
        }
        #endregion Delete

        #region Kinship
        private Node Grandparent(Node n)
        {
            if (n.parent == Nil)
                return Nil;
            else
                return n.parent.parent;
        }

        private Node Uncle(Node n)
        {
            Node g = Grandparent(n);
            if (g == Nil)
                return Nil;
            if (g.lchild == n.parent)
                return g.rchild;
            else
                return g.lchild;
        }

        private Node Sibling(Node n)
        {
            if (n.parent == Nil)
                return Nil;
            if (n == n.parent.lchild)
                return n.parent.rchild;
            else
                return n.parent.lchild;
        }
        #endregion Kinship

        private Node NewNode(K key, V value, Node parent)
        {
            Node node = m_nodeCache.AllocNode();
            node.key = key;
            node.value = value;
            node.parent = parent;
            node.lchild = Nil;
            node.rchild = Nil;
            node.color = Color.Red;
            node.hashCode = m_comparer.GetHashCode(key);
            return node;
        }

        private void DelNode(Node node)
        {
            m_nodeCache.FreeNode(node);
        }
    }
}
