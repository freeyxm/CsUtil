using System;
using CsNet.Collections.Tree.BinaryTree;

namespace Test
{
    class RBTreeTest : BinaryTreeTest<RBTree<int, int>, RBTreeNode<int, int>>
    {
        public override void TestValidity()
        {
            TestValidity("", 1000, 1000);
        }

        public override void TestPerformace()
        {
            int maxCount = 1000000;
            RBTree<int, int> tree = new RBTree<int, int>(maxCount);
            TestPerformace(tree, maxCount);
        }
    }
}
