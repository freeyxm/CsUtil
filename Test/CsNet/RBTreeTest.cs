using System;
using System.Collections.Generic;
using System.Diagnostics;
using CsNet.Util;
using CsNet.Collections;

namespace Test
{
    class RBTreeTest
    {
        List<int> m_result;
        List<int> m_result1 = new List<int>();
        List<int> m_result2 = new List<int>();

        public void TestValidity()
        {
            Random random = new Random((int)DateTime.Now.Ticks);

            Console.WriteLine("------------------------------------");
            Console.WriteLine("TestValidity:\n");

            string input = "";
            int maxCount = !string.IsNullOrEmpty(input) ? input.Length : 10000;
            int maxValue = int.MaxValue;
            int delTarget = random.Next(maxValue);

            m_result = m_result1;

            // 构造测试数据
            Console.WriteLine("Generate data {0} ...", maxCount);
            m_result.Clear();
            if (string.IsNullOrEmpty(input))
            {
                m_result.Add(delTarget); // 确保至少有一个供删除的目标
                for (int i = 1; i < maxCount; ++i)
                {
                    m_result.Add(random.Next(maxValue));
                }
            }
            else
            {
                for (int i = 0; i < input.Length; ++i)
                {
                    m_result.Add((int)input[i]);
                }
            }
            if (maxCount <= 10)
            {
                Console.WriteLine("Input: {0}", input);
            }

            // 插入
            Console.WriteLine("Insert ...");
            var tree = new RBTree<int, int>();
            for (int i = 0; i < m_result.Count; ++i)
            {
                tree.Add(m_result[i], 0);
            }

            // 校验
            ValidTree(tree);

            /*
            // 校验前序
            Console.WriteLine("Valid PreOrder ...");
            m_result = m_result1;
            m_result.Clear();
            tree.TraversePreOrder_r(Traverse);
            m_result = m_result2;
            m_result.Clear();
            tree.TraversePreOrder(Traverse);
            Debug.Assert(Utility.Equals(m_result1, m_result2), "Valid PreOrder failed!");

            // 校验中序
            Console.WriteLine("Valid InOrder ...");
            m_result = m_result1;
            m_result.Clear();
            tree.TraverseInOrder_r(Traverse);
            m_result = m_result2;
            m_result.Clear();
            tree.TraverseInOrder(Traverse);
            Debug.Assert(Utility.Equals(m_result1, m_result2), "Valid InOrder failed!");

            // 校验后序
            Console.WriteLine("Valid PostOrder ...");
            m_result = m_result1;
            m_result.Clear();
            tree.TraversePostOrder_r(Traverse);
            m_result = m_result2;
            m_result.Clear();
            tree.TraversePostOrder(Traverse);
            Debug.Assert(Utility.Equals(m_result1, m_result2), "Valid PostOrder failed!");
            */

            // 校验删除
            Console.WriteLine("Valid Remove ...");
            int rcount = 0;
            while (tree.Remove(delTarget))
            {
                ++rcount;
            }
            ValidTree(tree);
            Debug.Assert(rcount > 0, "Valid Remove failed!");
            Console.WriteLine("Remove count: {0}", rcount);

            Console.WriteLine("Test passed.\n");
            m_result = null;
            m_result1.Clear();
            m_result2.Clear();
        }

        private bool ValidTree(RBTree<int, int> tree)
        {
            //Console.WriteLine("Valid balance ...");
            //tree._ValidBalance();

            Console.WriteLine("Valid tree ...");
            m_result.Clear();
            tree.TraverseInOrder_r(Traverse);

            for (int i = 1; i < m_result.Count; ++i)
            {
                if (m_result[i].CompareTo(m_result[i - 1]) < 0)
                {
                    Debug.Assert(false, "tree broken!");
                    return false;
                }
            }
            return true;
        }

        private void Traverse(int key, int value)
        {
            m_result.Add(key);
        }

        public void TestPerformace()
        {
            const int maxCount = 1000000;

            RBTree<int, int> tree = new RBTree<int, int>(maxCount);
            Stopwatch watch = new Stopwatch();
            List<int> keys = new List<int>();
            Random random = new Random((int)DateTime.Now.Ticks);

            Console.WriteLine("------------------------------------");
            Console.WriteLine("TestPerformace:\n");

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

            TestUtility.RunTime("TraversePreOrder_r", watch, () =>
            {
                tree.TraversePreOrder_r(Traverse2);
            });

            //TestUtility.RunTime("TraversePreOrder", watch, () =>
            //{
            //    tree.TraversePreOrder(Traverse2);
            //});

            TestUtility.RunTime("TraverseInOrder_r", watch, () =>
            {
                tree.TraverseInOrder_r(Traverse2);
            });

            //TestUtility.RunTime("TraverseInOrder", watch, () =>
            //{
            //    tree.TraverseInOrder(Traverse2);
            //});

            TestUtility.RunTime("TraversePostOrder_r", watch, () =>
            {
                tree.TraversePostOrder_r(Traverse2);
            });

            //TestUtility.RunTime("TraversePostOrder", watch, () =>
            //{
            //    tree.TraversePostOrder(Traverse2);
            //});

            TestUtility.RunTime("Random", watch, () =>
            {
                int count = keys.Count;
                for (int i = keys.Count - 1; i >= 0; --i)
                {
                    int index = random.Next(0, i + 1);
                    int tmp = keys[i];
                    keys[i] = keys[index];
                    keys[index] = tmp;
                }
            });

            TestUtility.RunTime("Delete", watch, () =>
            {
                foreach (var k in keys)
                {
                    tree.Remove(k);
                }
            });

            Console.WriteLine("Done.\n");
        }

        static void Traverse2(int key, int value)
        {
        }
    }
}
