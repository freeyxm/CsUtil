using System;
using CsNet.Collections;

namespace Test
{
    class AvlTreeTest : BinaryTreeTest<AvlTree<int, int>, AvlTreeNode<int, int>>
    {
        protected override bool ValidTree(AvlTree<int, int> tree)
        {
            Console.WriteLine("Valid balance ...");
            tree._ValidBalance();

            return base.ValidTree(tree);
        }

        public override void TestPerformace()
        {
            int maxCount = 1000000;
            AvlTree<int, int> tree = new AvlTree<int, int>(maxCount);
            TestPerformace(tree, maxCount);
        }
    }
}
