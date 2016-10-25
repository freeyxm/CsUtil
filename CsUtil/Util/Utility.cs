using System;
using System.Collections.Generic;

namespace CsUtil.Util
{
    public class Utility
    {
        #region Equals
        public const float FLOAT_PRECISION = 1E-7f;
        public const double DOUBLE_PRECISION = 1E-15d;

        public static bool IsEqualZero(float value)
        {
            return -FLOAT_PRECISION <= value && value <= FLOAT_PRECISION;
        }

        public static bool IsEqualZero(double value)
        {
            return -DOUBLE_PRECISION <= value && value <= DOUBLE_PRECISION;
        }

        public static bool IsEqual(float v1, float v2)
        {
            return IsEqualZero(v1 - v2);
        }

        public static bool IsEqual(double v1, double v2)
        {
            return IsEqualZero(v1 - v2);
        }

        public static bool IsEqual<T>(IList<T> list1, IList<T> list2) where T : IComparable
        {
            if (list1.Count != list2.Count)
                return false;

            for (int i = 0; i < list1.Count; ++i)
            {
                if (list1[i].CompareTo(list2[i]) != 0)
                    return false;
            }
            return true;
        }
        #endregion Equals

        public static void RandomOrder<T>(IList<T> list, Random random)
        {
            for (int i = list.Count - 1; i >= 0; --i)
            {
                int index = random.Next(0, i + 1);
                T tmp = list[i];
                list[i] = list[index];
                list[index] = tmp;
            }
        }

        public delegate void MergeDelegate<T>(ref T a, T b);
        public static void MergeList<T>(IList<T> list, Func<T, T, bool> equal, MergeDelegate<T> merge)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                var elem = list[i];
                for (int j = list.Count - 1; j > i; --j)
                {
                    if (equal(elem, list[j]))
                    {
                        merge(ref elem, list[j]);
                        list.RemoveAt(j);
                    }
                }
            }
        }

        public static int GetCharCount(string str, char ch)
        {
            int count = 0;
            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] == ch)
                    ++count;
            }
            return count;
        }
    }
}
