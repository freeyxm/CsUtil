using System;
using CsNet.Collections;

namespace Test
{
    class AvlTreeTest : BinaryTreeTest<AvlTree<int, int>, AvlTreeNode<int, int>>
    {
        public override void TestValidity()
        {
            TestValidity("", 1000, 1000);
        }

        public override void TestPerformace()
        {
            int maxCount = 1000000;
            AvlTree<int, int> tree = new AvlTree<int, int>(maxCount);
            TestPerformace(tree, maxCount);
        }
    }
}
