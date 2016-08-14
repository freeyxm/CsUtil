using System;
using System.Collections.Generic;

namespace CsNet.Collections
{
    public class AvlTreeNode<K, V> : BinaryTreeNode<K, V, AvlTreeNode<K, V>>
    {
        public sbyte balance;
    }

    public class AvlTree<K, V> : BinaryTree<K, V, AvlTreeNode<K, V>>
    {
        private class Balance
        {
            public const sbyte RR = -2;
            public const sbyte RH = -1;
            public const sbyte EH = 0;
            public const sbyte LH = 1;
            public const sbyte LL = 2;
        }

        private int m_hashCode;
        private bool m_heightChanged;

        public AvlTree()
            : this(null, 0)
        {
        }

        public AvlTree(int capacity)
            : this(null, capacity)
        {
        }

        public AvlTree(IEqualityComparer<K> comparer, int capacity = 0)
            : base(comparer, capacity, null)
        {
        }

        protected override bool Insert(K key, V value)
        {
            var node = NewNode(key, value);
            if (m_root == null)
                m_root = node;
            else
                Insert(node);
            return true;
        }

        private void Insert(AvlTreeNode<K, V> node)
        {
            int cmp;
            var target = m_root;
            while (true)
            {
                cmp = node.hashCode.CompareTo(target.hashCode);
                if (cmp < 0)
                {
                    if (target.lchild == null)
                        break;
                    else
                        target = target.lchild;
                }
                else
                {
                    if (target.rchild == null)
                        break;
                    else
                        target = target.rchild;
                }
            }

            bool heightChanged;
            if (cmp < 0)
            {
                node.parent = target;
                target.lchild = node;
                target.balance++;
                heightChanged = target.balance == Balance.LH;
            }
            else
            {
                node.parent = target;
                target.rchild = node;
                target.balance--;
                heightChanged = target.balance == Balance.RH;
            }

            // insert fixup
            while (heightChanged)
            {
                var parent = target.parent;

                switch (target.balance)
                {
                    case Balance.LL:
                        {
                            heightChanged = LeftUpBalance(target);
                        }
                        break;
                    case Balance.LH:
                    case Balance.RH:
                        {
                            if (parent != null)
                            {
                                if (parent.lchild == target)
                                    parent.balance++;
                                else
                                    parent.balance--;
                            }
                        }
                        break;
                    case Balance.RR:
                        {
                            heightChanged = RightUpBalance(target);
                        }
                        break;
                    default:
                        heightChanged = false;
                        break;
                }

                if (!heightChanged || parent == null)
                    break;
                else
                    target = parent;
            }
        }

        protected override bool Delete(K key)
        {
            bool ret = false;
            if (m_root != null)
            {
                m_heightChanged = false;
                m_hashCode = m_comparer.GetHashCode(key);
                ret = Delete(m_root);
            }
            return ret;
        }

        private bool Delete(AvlTreeNode<K, V> target)
        {
            int cmp = m_hashCode.CompareTo(target.hashCode);
            if (cmp < 0)
            {
                if (target.lchild == null)
                    return false;
                bool ret = Delete(target.lchild);
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
                bool ret = Delete(target.rchild);
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
                    var toDel = DeleteRightest(target.lchild);
                    UpdateNode(target, toDel);
                    if (m_heightChanged)
                    {
                        target.balance--;
                        CheckLeftDown(target);
                    }
                    target = toDel;
                }
                else
                {
                    var toDel = DeleteLeftest(target.rchild);
                    UpdateNode(target, toDel);
                    if (m_heightChanged)
                    {
                        target.balance++;
                        CheckRightDown(target);
                    }
                    target = toDel;
                }

                DelNode(target);
                return true;
            }
        }

        private AvlTreeNode<K, V> DeleteRightest(AvlTreeNode<K, V> target)
        {
            if (target.rchild == null)
            {
                PromoteLeftChild(target);
                m_heightChanged = true;
                return target;
            }
            else
            {
                var node = DeleteRightest(target.rchild);
                if (m_heightChanged)
                {
                    target.balance++;
                    CheckRightDown(target);
                }
                return node;
            }
        }

        private AvlTreeNode<K, V> DeleteLeftest(AvlTreeNode<K, V> target)
        {
            if (target.lchild == null)
            {
                PromoteRightChild(target);
                m_heightChanged = true;
                return target;
            }
            else
            {
                var node = DeleteLeftest(target.lchild);
                if (m_heightChanged)
                {
                    target.balance--;
                    CheckLeftDown(target);
                }
                return node;
            }
        }

        #region Balance
        /// <summary>
        /// 左子树升高后平衡调整
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool LeftUpBalance(AvlTreeNode<K, V> target)
        {
            var lc = target.lchild;
            switch (lc.balance)
            {
                case Balance.LH:
                    {
                        lc.balance = Balance.EH;
                        target.balance = Balance.EH;
                        RotateRight(target);
                    }
                    break;
                case Balance.RH:
                    {
                        var rd = lc.rchild;
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
                        RotateLeft(lc);
                        RotateRight(target);
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
        private bool RightUpBalance(AvlTreeNode<K, V> target)
        {
            var rc = target.rchild;
            switch (rc.balance)
            {
                case Balance.RH:
                    {
                        rc.balance = Balance.EH;
                        target.balance = Balance.EH;
                        RotateLeft(target);
                    }
                    break;
                case Balance.LH:
                    {
                        var ld = rc.lchild;
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
                        RotateRight(rc);
                        RotateLeft(target);
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
        private bool LeftDownBalance(AvlTreeNode<K, V> target)
        {
            bool heightChanged = false;

            var rc = target.rchild;
            switch (rc.balance)
            {
                case Balance.EH:
                    {
                        rc.balance = Balance.LH;
                        target.balance = Balance.RH;
                        RotateLeft(target);
                        heightChanged = false;
                    }
                    break;
                case Balance.RH:
                    {
                        rc.balance = Balance.EH;
                        target.balance = Balance.EH;
                        RotateLeft(target);
                        heightChanged = true;
                    }
                    break;
                case Balance.LH:
                    {
                        var ld = rc.lchild;
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
                        RotateRight(rc);
                        RotateLeft(target);
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
        private bool RightDownBalance(AvlTreeNode<K, V> target)
        {
            bool heightChanged = false;

            var lc = target.lchild;
            switch (lc.balance)
            {
                case Balance.EH:
                    {
                        lc.balance = Balance.RH;
                        target.balance = Balance.LH;
                        RotateRight(target);
                        heightChanged = false;
                    }
                    break;
                case Balance.LH:
                    {
                        lc.balance = Balance.EH;
                        target.balance = Balance.EH;
                        RotateRight(target);
                        heightChanged = true;
                    }
                    break;
                case Balance.RH:
                    {
                        var rd = lc.rchild;
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
                        RotateLeft(lc);
                        RotateRight(target);
                        heightChanged = true;
                    }
                    break;
                default:
                    throw new InvalidBalanceException("right down, lc", lc.balance);
            }
            return heightChanged;
        }

        /// <summary>
        /// 左子树降低后平衡检查
        /// </summary>
        /// <param name="target"></param>
        private void CheckLeftDown(AvlTreeNode<K, V> target)
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
        /// 右子树降低后平衡检查
        /// </summary>
        /// <param name="target"></param>
        private void CheckRightDown(AvlTreeNode<K, V> target)
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

        #region _CheckBalance
        public override bool _CheckBalance()
        {
            if (m_root == null)
                return true;

            int height = 0;
            return _CheckBalance(m_root, ref height);
        }

        private bool _CheckBalance(AvlTreeNode<K, V> node, ref int height)
        {
            if (node.balance < Balance.RH || node.balance > Balance.LH)
            {
                //throw new InvalidBalanceException(string.Format("node[{0}]", node.hashCode), node.balance);
                System.Diagnostics.Debug.Assert(false, "balance invalid.");
                return false;
            }

            bool ret = true;
            int lh = 0, rh = 0;

            if (ret && node.lchild != null)
            {
                ret = _CheckBalance(node.lchild, ref lh);
            }

            if (ret && node.rchild != null)
            {
                ret = _CheckBalance(node.rchild, ref rh);
            }

            if (ret && (lh - rh) != node.balance)
            {
                System.Diagnostics.Debug.Assert(false, "balance not sync with height.");
                return false;
            }

            height = Math.Max(lh, rh) + 1;

            return ret;
        }
        #endregion _CheckBalance

        protected override AvlTreeNode<K, V> NewNode(K key, V value, AvlTreeNode<K, V> parent = null)
        {
            var node = base.NewNode(key, value, parent);
            node.balance = 0;
            return node;
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
