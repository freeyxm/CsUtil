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

        public RBTree()
            : this(null, 0)
        {
        }

        public RBTree(int capacity)
            : this(null, capacity)
        {
        }

        public RBTree(IEqualityComparer<K> comparer, int capacity = 0)
            : base(comparer, capacity, new RBTreeNode<K, V>())
        {
            Nil.color = Color.Black;
            Nil.parent = null;
            Nil.lchild = Nil.rchild = null;
            Nil.key = default(K);
            Nil.value = default(V);
            Nil.hashCode = -1;
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
            if (m_root == Nil)
            {
                m_root = n;
            }
            else
            {
                var tmp = m_root;
                var p = Nil;
                int cmp = 0;

                while (tmp != Nil)
                {
                    p = tmp;
                    cmp = n.hashCode.CompareTo(tmp.hashCode);
                    if (cmp < 0)
                        tmp = tmp.lchild;
                    else
                        tmp = tmp.rchild;
                }

                n.parent = p;
                if (cmp < 0)
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
            if (n == m_root)
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
            if (u != null && u.color == Color.Red)
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
                if (n == m_root)
                {
                    n.color = Color.Black;
                    break;
                }

                // case 2
                if (n.parent.color == Color.Black)
                    break;

                // case 3
                var u = Uncle(n);
                if (u != null && u.color == Color.Red)
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
            var toDel = target;
            if (target.lchild != Nil && target.rchild != Nil)
            {
                toDel = GetLeftestNode(target.rchild);
                UpdateNode(target, toDel);
            }

            delete_one_child(toDel);

            DelNode(toDel);
        }

        private void delete_one_child(RBTreeNode<K, V> n)
        {
            RBTreeNode<K, V> child;
            // replace node
            if (n.rchild == Nil)
            {
                child = n.lchild;
                PromoteLeftChild(n);
            }
            else
            {
                child = n.rchild;
                PromoteRightChild(n);
            }

            if (n.color == Color.Black)
            {
                if (child.color == Color.Red)
                {
                    child.color = Color.Black;
                }
                else
                {
                    //delete_case1(child);
                    DeleteFixup(child);
                }
            }
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
                    break;
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
        #endregion Delete

        #region Kinship
        private RBTreeNode<K, V> Grandparent(RBTreeNode<K, V> n)
        {
            if (n.parent == Nil)
                return null;
            else
                return n.parent.parent;
        }

        private RBTreeNode<K, V> Uncle(RBTreeNode<K, V> n)
        {
            var g = Grandparent(n);
            if (g == null)
                return null;
            if (g.lchild == n.parent)
                return g.rchild;
            else
                return g.lchild;
        }

        private RBTreeNode<K, V> Sibling(RBTreeNode<K, V> n)
        {
            if (n.parent == Nil)
                return null;
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

        #region _CheckBalance
        public override bool _CheckBalance()
        {
            if (m_root == Nil)
                return true;

            // condition 2
            if (m_root.color != Color.Black)
            {
                System.Diagnostics.Debug.Assert(false, "Cond2: Root node should black.");
                return false;
            }

            // condition 3
            if (Nil.color != Color.Black)
            {
                System.Diagnostics.Debug.Assert(false, "Cond3: Nil node should black.");
                return false;
            }

            // condition 4
            if (!_CheckBalanceCond4(m_root))
                return false;

            // condition 5
            int count = 0;
            if (!_CheckBalanceCond5(m_root, ref count))
                return false;

            return true;
        }

        private bool _CheckBalanceCond4(RBTreeNode<K, V> node)
        {
            if (node.color == Color.Red)
            {
                if (node.lchild.color != Color.Black || node.rchild.color != Color.Black)
                {
                    System.Diagnostics.Debug.Assert(false, "Cond4: Red node's child should black.");
                    return false;
                }
            }
            if (node.lchild != Nil)
            {
                if (!_CheckBalanceCond4(node.lchild))
                    return false;
            }
            if (node.rchild != Nil)
            {
                if (!_CheckBalanceCond4(node.rchild))
                    return false;
            }
            return true;
        }

        private bool _CheckBalanceCond5(RBTreeNode<K, V> node, ref int count)
        {
            if (node.color == Color.Black)
            {
                ++count;
            }

            int countL = 0, countR = 0;
            bool ret = true;

            if (ret)
            {
                if (node.lchild == Nil)
                    countL = 1;
                else
                    ret = _CheckBalanceCond5(node.lchild, ref countL);
            }

            if (ret)
            {
                if (node.rchild == Nil)
                    countR = 1;
                else
                    ret = _CheckBalanceCond5(node.rchild, ref countR);
            }

            if (!ret)
                return false;

            if (countL != countR)
            {
                System.Diagnostics.Debug.Assert(countL == countR, "Cond5: black node num to leaf should equal.");
                return false;
            }
            else
            {
                count += countL;
                return true;
            }
        }
        #endregion _CheckBalance
    }
}
