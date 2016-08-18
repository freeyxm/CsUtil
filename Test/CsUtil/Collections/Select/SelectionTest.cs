﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using CsUtil.Collections.Select;

namespace Test
{
    class SelectionTest : TestBase
    {
        public override void Test()
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

            Console.WriteLine("Test done.");
        }
    }
}
