using System;
using System.Collections.Generic;

namespace CsNet.Collections
{
    public class RBTreeNode<K, V> : BinaryTreeNode<K, V, RBTreeNode<K, V>>
    {
        public RBTree<K, V>.Color color;
    }

    public class RBTree<K, V> : BinaryTree<K, V, RBTreeNode<K, V>>
    {
        public enum Color
        {
            Black,
            Red,
        }

        private new static RBTreeNode<K, V> Nil;
        static RBTree()
        {
            Nil = new RBTreeNode<K, V>();
            Nil.color = Color.Black;
            Nil.parent = Nil;
            Nil.lchild = Nil.rchild = Nil;
            Nil.key = default(K);
            Nil.value = default(V);
            Nil.hashCode = -1;
        }

        public RBTree(int capacity = 0)
            : this(null, capacity)
        {
        }

        public RBTree(IEqualityComparer<K> comparer, int capacity = 0)
            : base(comparer, capacity, Nil)
        {
        }

        protected override bool Insert(K key, V value)
        {
            Insert(NewNode(key, value, Nil));
            return true;
        }

        protected override bool Delete(K key)
        {
            var node = FindKey(key);
            if (node != Nil)
            {
                Delete(node);
                return true;
            }
            return false;
        }

        #region Insert
        private void Insert(RBTreeNode<K, V> n)
        {
            var tmp = m_root;
            var p = Nil;

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

            //insert_case1(n);
            InsertFixup(n);
        }

        #region insert fix up (recursive)
        private void insert_case1(RBTreeNode<K, V> n)
        {
            if (n.parent == Nil)
                n.color = Color.Black;
            else
                insert_case2(n);
        }

        private void insert_case2(RBTreeNode<K, V> n)
        {
            if (n.parent.color == Color.Black)
                return;
            else
                insert_case3(n);
        }

        private void insert_case3(RBTreeNode<K, V> n)
        {
            var u = Uncle(n);
            if (u != Nil && u.color == Color.Red)
            {
                var g2 = Grandparent(n);
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

        private void insert_case4(RBTreeNode<K, V> n)
        {
            var g = Grandparent(n);
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

        private void insert_case5(RBTreeNode<K, V> n)
        {
            var g = Grandparent(n);
            n.parent.color = Color.Black;
            g.color = Color.Red;
            if (n == n.parent.lchild)
                RotateRight(g);
            else
                RotateLeft(g);
        }
        #endregion insert fix up (recursive)

        private void InsertFixup(RBTreeNode<K, V> n)
        {
            while (true)
            {
                // case 1
                if (n.parent == Nil)
                {
                    n.color = Color.Black;
                    break;
                }

                // case 2
                if (n.parent.color == Color.Black)
                    break;

                // case 3
                var u = Uncle(n);
                if (u != Nil && u.color == Color.Red)
                {
                    var g2 = Grandparent(n);
                    n.parent.color = Color.Black;
                    u.color = Color.Black;
                    g2.color = Color.Red;
                    n = g2;
                    continue;
                }

                var g = Grandparent(n);
                // case 4
                {
                    if (n == n.parent.rchild && n.parent == g.lchild)
                    {
                        RotateLeft(n.parent);
                        n = n.lchild;
                        g = Grandparent(n);
                    }
                    else if (n == n.parent.lchild && n.parent == g.rchild)
                    {
                        RotateRight(n.parent);
                        n = n.rchild;
                        g = Grandparent(n);
                    }
                }

                // case 5
                {
                    n.parent.color = Color.Black;
                    g.color = Color.Red;
                    if (n == n.parent.lchild)
                        RotateRight(g);
                    else
                        RotateLeft(g);
                }

                break;
            }
        }
        #endregion Insert

        #region Delete
        private void Delete(RBTreeNode<K, V> target)
        {
            var toDel = Nil;
            var toFix = Nil;
            if (target.lchild != Nil && target.rchild != Nil)
            {
                toDel = GetLeftestNode(target.rchild);
                target.key = toDel.key;
                target.value = toDel.value;
                target.hashCode = toDel.hashCode;
                toFix = toDel.rchild;
                PromoteRightChild(toDel);
            }
            else if (target.lchild != Nil)
            {
                toDel = target;
                toFix = target.lchild;
                PromoteLeftChild(target);
            }
            else
            {
                toDel = target;
                toFix = target.rchild;
                PromoteRightChild(target);
            }

            if (toDel.color == Color.Black)
            {
                if (toFix.color == Color.Red)
                {
                    toFix.color = Color.Black;
                }
                else
                {
                    delete_case1(toFix);
                    //DeleteFixup(toFix);
                }
            }

            DelNode(toDel);
        }

        #region delete fix up (recursive)
        private void delete_case1(RBTreeNode<K, V> n)
        {
            if (n.parent != Nil)
                delete_case2(n);
        }

        private void delete_case2(RBTreeNode<K, V> n)
        {
            var s = Sibling(n);
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

        private void delete_case3(RBTreeNode<K, V> n)
        {
            var s = Sibling(n);
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

        private void delete_case4(RBTreeNode<K, V> n)
        {
            var s = Sibling(n);
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

        private void delete_case5(RBTreeNode<K, V> n)
        {
            var s = Sibling(n);
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

        private void delete_case6(RBTreeNode<K, V> n)
        {
            var s = Sibling(n);
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
        #endregion delete fix up (recursive)

        private void DeleteFixup(RBTreeNode<K, V> n)
        {
            RBTreeNode<K, V> s;
            while (true)
            {
                // case 1
                if (n.parent == Nil)
                    break;

                // case 2
                s = Sibling(n);
                if (s.color == Color.Red)
                {
                    s.color = Color.Black;
                    n.parent.color = Color.Red;
                    if (n == n.parent.lchild)
                        RotateLeft(n.parent);
                    else
                        RotateRight(n.parent);
                    s = Sibling(n);
                }

                // case 3
                if (n.parent.color == Color.Black
                    && s.color == Color.Black
                    && s.lchild.color == Color.Black
                    && s.rchild.color == Color.Black)
                {
                    s.color = Color.Red;
                    n = n.parent;
                    continue;
                }

                // case 4
                if (n.parent.color == Color.Red
                    && s.color == Color.Black
                    && s.lchild.color == Color.Black
                    && s.rchild.color == Color.Black)
                {
                    s.color = Color.Red;
                    n.parent.color = Color.Black;
                }

                // case 5
                if (s.color == Color.Black)
                {
                    if (n == n.parent.lchild
                        && s.lchild.color == Color.Red
                        && s.rchild.color == Color.Black)
                    {
                        s.color = Color.Red;
                        s.lchild.color = Color.Black;
                        RotateRight(s);
                        s = Sibling(n);
                    }
                    else if (n == n.parent.rchild
                        && s.lchild.color == Color.Black
                        && s.rchild.color == Color.Red)
                    {
                        s.color = Color.Red;
                        s.rchild.color = Color.Black;
                        RotateLeft(s);
                        s = Sibling(n);
                    }
                }

                // case 6
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

                break;
            }
        }

        private void Transplant(RBTreeNode<K, V> u, RBTreeNode<K, V> v)
        {
            if (u == Nil)
                m_root = v;
            else if (u.parent.lchild == u)
                u.parent.lchild = v;
            else
                u.parent.rchild = v;

            v.parent = u.parent;
        }

        private RBTreeNode<K, V> Minimum(RBTreeNode<K, V> z)
        {
            while (z.lchild != Nil)
            {
                z = z.lchild;
            }
            return z;
        }
        #endregion Delete

        #region Kinship
        private RBTreeNode<K, V> Grandparent(RBTreeNode<K, V> n)
        {
            if (n.parent == Nil)
                return Nil;
            else
                return n.parent.parent;
        }

        private RBTreeNode<K, V> Uncle(RBTreeNode<K, V> n)
        {
            var g = Grandparent(n);
            if (g == Nil)
                return Nil;
            if (g.lchild == n.parent)
                return g.rchild;
            else
                return g.lchild;
        }

        private RBTreeNode<K, V> Sibling(RBTreeNode<K, V> n)
        {
            if (n.parent == Nil)
                return Nil;
            if (n == n.parent.lchild)
                return n.parent.rchild;
            else
                return n.parent.lchild;
        }
        #endregion Kinship

        protected override RBTreeNode<K, V> NewNode(K key, V value, RBTreeNode<K, V> parent)
        {
            var node = base.NewNode(key, value, parent);
            node.color = Color.Red;
            return node;
        }
    }
}
