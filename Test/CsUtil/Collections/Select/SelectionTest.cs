using System;
using System.Collections.Generic;
using System.Diagnostics;
using CsUtil.Collections.Select;

namespace Test
{
    class SelectionTest : TestBase
    {
        public override void Test()
        {
            //TestValidity();
            TestSelectKth2();
        }

        private void TestValidity()
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            Stopwatch watch = new Stopwatch();

            int maxCount = 10000000;
            int[] data = null; // { 1, 2, 3, 2, 2, 0, 3, 3, 3, 6 };

            TestUtility.RunTime("Generate data", watch, () =>
            {
                if (data == null || data.Length == 0)
                {
                    data = new int[maxCount];
                    for (int i = 0; i < maxCount; ++i)
                    {
                        data[i] = random.Next(maxCount);
                    }
                }
            });

            int min = 0, max = 0;
            TestUtility.RunTime("SelectMinMax", watch, () =>
            {
                Selection.SelectMinMax(data, out min, out max);
            });

            int min2 = -1;
            TestUtility.RunTime("SelectMin", watch, () =>
            {
                min2 = Selection.SelectMin(data);
            });
            Debug.Assert(min == min2, "min not equal");

            int max2 = -1;
            TestUtility.RunTime("SelectMax", watch, () =>
            {
                max2 = Selection.SelectMax(data);
            });
            Debug.Assert(max == max2, "max not equal");

            int kth = -2, k = 0;
            TestUtility.RunTime("SelectKth min", watch, () =>
            {
                kth = Selection.SelectKth(data, k = 0);
            });
            Debug.Assert(kth == min, "kth != min");

            TestUtility.RunTime("SelectKth max", watch, () =>
            {
                kth = Selection.SelectKth(data, k = data.Length - 1);
            });
            Debug.Assert(kth == max, "kth != max");

            TestUtility.RunTime("SelectKth mid", watch, () =>
            {
                kth = Selection.SelectKth(data, k = data.Length / 2);
            });
            int kcount = 0;
            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i] <= kth)
                    ++kcount;
            }
            Debug.Assert(kcount >= k, "kth wrong!");

            k = data.Length / 10 * 7;
            TestUtility.RunTime("SelectKth", watch, () =>
            {
                kth = Selection.SelectKth(data, k);
            });
            int kth2 = -3;
            TestUtility.RunTime("SelectKthE", watch, () =>
            {
                kth2 = Selection.SelectKthE(data, k, 0, maxCount);
            });
            Debug.Assert(kth == kth2, "kth != kth2");

            Console.WriteLine("Test done.");
        }

        private void TestSelectKth2()
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            Stopwatch watch = new Stopwatch();

            int maxCount = 10000000;
            int[] data = null, input = null;

            TestUtility.RunTime("Generate data", watch, () =>
            {
                if (input == null || input.Length == 0)
                {
                    input = new int[maxCount];
                    for (int i = 0; i < maxCount; ++i)
                    {
                        input[i] = random.Next(maxCount);
                    }
                }
                data = new int[input.Length];
            });

            int kth1 = -1, kth2 = -2, k = data.Length / 10 * 5;

            input.CopyTo(data, 0);
            TestUtility.RunTime("SelectKth", watch, () =>
            {
                kth1 = Selection.SelectKth(data, k);
            });

            input.CopyTo(data, 0);
            TestUtility.RunTime("SelectKth2", watch, () =>
            {
                kth2 = Selection.SelectKthE(data, k, 0, maxCount);
            });

            Debug.Assert(kth1 == kth2, "kth1 != kth2");
        }
    }
}
