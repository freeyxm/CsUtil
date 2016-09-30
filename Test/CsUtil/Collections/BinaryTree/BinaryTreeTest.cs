using System;
using System.Collections.Generic;
using System.Diagnostics;
using CsUtil.Util;
using CsUtil.Collections.BinaryTree;

namespace Test
{
    class BinaryTreeTest<Tree, Node> : TestBase where Tree : BinaryTree<int, int, Node>, new()
        where Node : BinaryTreeNode<int, int, Node>, new()
    {
        List<int> m_result;
        List<int> m_result1 = new List<int>();
        List<int> m_result2 = new List<int>();
        int m_count;

        public override void Test()
        {
            TestValidity();
            TestPerformace();
        }

        public virtual void TestValidity()
        {
            TestValidity(null, 1000, int.MaxValue);
        }

        protected void TestValidity(string inputStr, int maxCount, int maxValue)
        {
            Console.WriteLine("------------------------------------");
            Console.WriteLine("TestValidity:\n");

            string[] inputs = null;
            if (!string.IsNullOrEmpty(inputStr))
            {
                inputs = inputStr.Split(',');
                maxCount = inputs.Length;
            }
            Random random = new Random((int)DateTime.Now.Ticks);

            // 构造测试数据
            Console.WriteLine("Generate data {0} ...", maxCount);
            List<int> input = new List<int>(maxCount);
            if (inputs == null || inputs.Length == 0)
            {
                for (int i = 0; i < maxCount; ++i)
                {
                    input.Add(random.Next(maxValue));
                }
            }
            else
            {
                for (int i = 0; i < inputs.Length; ++i)
                {
                    input.Add(int.Parse(inputs[i]));
                }
            }
            if (maxCount <= 10)
            {
                Console.WriteLine("Input: {0}", inputStr);
            }

            // 插入
            Console.WriteLine("Insert ...");
            var tree = new Tree();
            for (int i = 0; i < input.Count; ++i)
            {
                tree.Add(input[i], 0);
            }
            // 插入校验
            m_count = 0;
            tree.TraverseInOrder(TraverseCount);
            Debug.Assert(m_count == maxCount, "Valid Insert failed!");

            // 平衡校验
            Console.WriteLine("Valid Tree ...");
            ValidTree(tree);

            // 校验前序
            Console.WriteLine("Valid PreOrder ...");
            m_result = m_result1;
            m_result.Clear();
            tree.TraversePreOrder_r(Traverse);
            m_result = m_result2;
            m_result.Clear();
            tree.TraversePreOrder(Traverse);
            Debug.Assert(Utility.IsEqual(m_result1, m_result2), "Valid PreOrder failed!");

            // 校验中序
            Console.WriteLine("Valid InOrder ...");
            m_result = m_result1;
            m_result.Clear();
            tree.TraverseInOrder_r(Traverse);
            m_result = m_result2;
            m_result.Clear();
            tree.TraverseInOrder(Traverse);
            Debug.Assert(Utility.IsEqual(m_result1, m_result2), "Valid InOrder failed!");

            // 校验后序
            Console.WriteLine("Valid PostOrder ...");
            m_result = m_result1;
            m_result.Clear();
            tree.TraversePostOrder_r(Traverse);
            m_result = m_result2;
            m_result.Clear();
            tree.TraversePostOrder(Traverse);
            Debug.Assert(Utility.IsEqual(m_result1, m_result2), "Valid PostOrder failed!");

            // 校验删除
            Console.WriteLine("Valid Remove ...");
            Utility.RandomOrder(input, random);
            int rcount = 0;
            foreach (var key in input)
            {
                if (tree.Remove(key))
                {
                    ++rcount;
                    // 平衡校验
                    ValidTree(tree);
                }
                else
                    Debug.Assert(false, "Valid Remove failed!");
            }
            Debug.Assert(rcount == maxCount && tree.Count == 0, "Valid Remove failed!");

            Console.WriteLine("Test passed.\n");
            m_result = null;
            m_result1.Clear();
            m_result2.Clear();
        }

        protected virtual bool ValidTree(Tree tree)
        {
            if (!tree._CheckBalance())
                return false;

            m_result = m_result1;
            m_result.Clear();
            tree.TraverseInOrder(Traverse);

            for (int i = 1; i < m_result.Count; ++i)
            {
                if (m_result[i].CompareTo(m_result[i - 1]) < 0)
                {
                    Debug.Assert(false, "Tree broken!");
                    return false;
                }
            }
            return true;
        }

        private void Traverse(int key, int value)
        {
            m_result.Add(key);
        }

        private void TraverseCount(int key, int value)
        {
            ++m_count;
        }

        public virtual void TestPerformace()
        {
            Tree tree = new Tree();
            TestPerformace(tree, 1000000);
        }

        protected void TestPerformace(Tree tree, int maxCount)
        {
            Console.WriteLine("------------------------------------");
            Console.WriteLine("TestPerformace:\n");

            Stopwatch watch = new Stopwatch();
            List<int> keys = new List<int>();
            Random random = new Random((int)DateTime.Now.Ticks);

            TestUtility.RunTime(string.Format("Generate data {0}", maxCount), watch, () =>
            {
                for (int i = 0; i < maxCount; ++i)
                {
                    keys.Add(random.Next());
                }
            });

            TestUtility.RunTime("Insert", watch, () =>
            {
                foreach (var k in keys)
                {
                    tree.Add(k, 0);
                }
            });
            Console.WriteLine("Count {0}", tree.Count);

            TestUtility.RunTime("TraversePreOrder_r", watch, () =>
            {
                tree.TraversePreOrder_r(Traverse2);
            });

            TestUtility.RunTime("TraversePreOrder", watch, () =>
            {
                tree.TraversePreOrder(Traverse2);
            });

            TestUtility.RunTime("TraverseInOrder_r", watch, () =>
            {
                tree.TraverseInOrder_r(Traverse2);
            });

            TestUtility.RunTime("TraverseInOrder", watch, () =>
            {
                tree.TraverseInOrder(Traverse2);
            });

            TestUtility.RunTime("TraversePostOrder_r", watch, () =>
            {
                tree.TraversePostOrder_r(Traverse2);
            });

            TestUtility.RunTime("TraversePostOrder", watch, () =>
            {
                tree.TraversePostOrder(Traverse2);
            });

            TestUtility.RunTime("Random", watch, () =>
            {
                Utility.RandomOrder(keys, random);
            });

            TestUtility.RunTime("Delete", watch, () =>
            {
                int dcount = 0;
                foreach (var k in keys)
                {
                    if (tree.Remove(k))
                        ++dcount;
                    //Debug.Assert(tree.Remove(k), "Delete error.");
                }
                Console.Write(" {0},", dcount);
            });

            Console.WriteLine("Done.\n");
        }

        private void Traverse2(int key, int value)
        {
        }
    }
}
