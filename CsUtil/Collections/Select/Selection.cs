using System;
using System.Collections.Generic;

namespace CsUtil.Collections.Select
{
    public class Selection
    {
        /// <summary>
        /// 获取数组中第K大的值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static T SelectKth<T>(T[] data, int k) where T : IComparable
        {
            if (data == null)
                throw new NullReferenceException();
            if (k < 0 || k >= data.Length)
                throw new IndexOutOfRangeException();

            if (data.Length == 1)
                return data[0];

            return SelectKth(data, 0, data.Length - 1, k);
        }

        public static T SelectKth<T>(T[] data, int left, int right, int k) where T : IComparable
        {
            while (true)
            {
                if (left == right - 1)
                {
                    if (data[left].CompareTo(data[right]) > 0)
                    {
                        T tmp = data[left];
                        data[left] = data[right];
                        data[right] = tmp;
                    }
                    break;
                }

                int mid = (left + right) / 2;
                int index;
                if (data[left].CompareTo(data[right]) < 0)
                    index = data[left].CompareTo(data[mid]) > 0 ? left : mid;
                else
                    index = data[right].CompareTo(data[mid]) > 0 ? right : mid;

                T target = data[index];
                data[index] = data[right];

                int i = Partition(data, left, right - 1, target);
                if (i < right)
                {
                    data[right] = data[i];
                    data[i] = target;
                }
                else
                {
                    data[right] = target;
                }

                if (i < k)
                    left = i + 1;
                else if (i > k)
                    right = i - 1;
                else
                    break;
            }

            return data[k];
        }

        public static int SelectKth(int[] data, int k)
        {
            if (data == null)
                throw new NullReferenceException();
            if (k < 0 || k >= data.Length)
                throw new IndexOutOfRangeException();

            if (data.Length == 1)
                return data[0];

            return SelectKth(data, 0, data.Length - 1, k);
        }

        public static int SelectKth(int[] data, int left, int right, int k)
        {
            while (left < right)
            {
                if (left == right - 1)
                {
                    if (data[left] > data[right])
                    {
                        int tmp = data[left];
                        data[left] = data[right];
                        data[right] = tmp;
                    }
                    break;
                }

                int mid = (left + right) / 2;
                int index;
                if (data[left] < data[right])
                    index = data[left] > data[mid] ? left : mid;
                else
                    index = data[right] > data[mid] ? right : mid;

                int target = data[index];
                data[index] = data[right];

                int i = Partition(data, left, right - 1, target);
                if (i < right)
                {
                    data[right] = data[i];
                    data[i] = target;
                }
                else
                {
                    data[right] = target;
                }

                if (i < k)
                    left = i + 1;
                else if (i > k)
                    right = i - 1;
                else
                    break;
            }

            return data[k];
        }

        /// <summary>
        /// 通过估值的方式缩小搜索范围。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="k"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int SelectKthE(int[] data, int k, int min, int max)
        {
            int count = data.Length;
            int left = 0, right = count - 1;
            int index = -1;
            int offset = 0; // 修正估计区间，以防止在某一侧小幅度逼近。
            int limit = (int)Math.Sqrt(data.Length);
            int iterCount = 0;
            while (right - left > limit && ++iterCount <= 6)
            {
                // offset减半是为了防止在左右对称时，无谓的迭代。
                int target = min + (int)((long)(k - left + offset / 2) * (max - min) / count);
                index = Partition(data, left, right, target);
                if (index < k)
                {
                    offset = k - index; // 实际位置比估计位置靠左，下次估计应往右偏移。
                    min = target;
                    count -= index - left;
                    left = index;
                }
                else if (index > k)
                {
                    if (index > right)
                        index = right;
                    offset = k - index; // 实际位置比估计位置靠右，下次估计应往左偏移。
                    max = target;
                    count -= right - index;
                    right = index;
                }
                else
                {
                    if (index == left) // 此时，区间内的所有值都大于target，需要继续迭代。
                    {
                        index = -1;
                        min = target;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            //Console.Write(" iterCount = {0}, range = {1}, ", iterCount, right - left);
            if (index == k)
                return data[k];
            else
                return SelectKth(data, left, right, k);
        }

        /// <summary>
        /// 按升序分割数组（左小右大），并返回分割点索引。
        /// 若返回值>right，则数组所有数据均小于target。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int Partition<T>(T[] data, int left, int right, T target) where T : IComparable
        {
            while (true)
            {
                while (left <= right && data[left].CompareTo(target) <= 0)
                    ++left;
                while (right >= left && data[right].CompareTo(target) >= 0)
                    --right;
                if (left < right)
                {
                    T tmp = data[left];
                    data[left] = data[right];
                    data[right] = tmp;
                    ++left;
                    --right;
                }
                else
                {
                    break;
                }
            }
            return left;
        }

        public static int Partition(int[] data, int left, int right, int target)
        {
            while (true)
            {
                while (left <= right && data[left] <= target)
                    ++left;
                while (right >= left && data[right] >= target)
                    --right;
                if (left < right)
                {
                    int tmp = data[left];
                    data[left] = data[right];
                    data[right] = tmp;
                    ++left;
                    --right;
                }
                else
                {
                    break;
                }
            }
            return left;
        }

        /// <summary>
        /// 获取数组最大/最小值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static void SelectMinMax<T>(T[] data, out T min, out T max) where T : IComparable
        {
            if (data == null)
                throw new NullReferenceException();
            if (data.Length == 0)
                throw new IndexOutOfRangeException();

            if (data.Length == 1)
            {
                min = max = data[0];
                return;
            }

            if (data[0].CompareTo(data[1]) <= 0)
            {
                min = data[0];
                max = data[1];
            }
            else
            {
                min = data[1];
                max = data[0];
            }
            int i;
            for (i = 2; i < data.Length - 2; i += 2)
            {
                T d1 = data[i];
                T d2 = data[i + 1];
                if (d1.CompareTo(d2) <= 0)
                {
                    if (d1.CompareTo(min) < 0)
                        min = d1;
                    if (d2.CompareTo(max) > 0)
                        max = d2;
                }
                else
                {
                    if (d2.CompareTo(min) < 0)
                        min = d2;
                    if (d1.CompareTo(max) > 0)
                        max = d1;
                }
            }
            if (i < data.Length - 1)
            {
                T d = data[data.Length - 1];
                if (d.CompareTo(min) < 0)
                    min = d;
                else if (d.CompareTo(max) > 0)
                    max = d;
            }
        }

        public static void SelectMinMax(int[] data, out int min, out int max)
        {
            if (data == null)
                throw new NullReferenceException();
            if (data.Length == 0)
                throw new IndexOutOfRangeException();

            if (data.Length == 1)
            {
                min = max = data[0];
                return;
            }

            if (data[0] <= data[1])
            {
                min = data[0];
                max = data[1];
            }
            else
            {
                min = data[1];
                max = data[0];
            }
            int i;
            for (i = 2; i < data.Length - 2; i += 2)
            {
                int d1 = data[i];
                int d2 = data[i + 1];
                if (d1 <= d2)
                {
                    if (d1 < min)
                        min = d1;
                    if (d2 > max)
                        max = d2;
                }
                else
                {
                    if (d2 < min)
                        min = d2;
                    if (d1 > max)
                        max = d1;
                }
            }
            if (i < data.Length - 1)
            {
                int d = data[data.Length - 1];
                if (d < min)
                    min = d;
                else if (d > max)
                    max = d;
            }
        }

        /// <summary>
        /// 获取数组最小值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T SelectMin<T>(T[] data) where T : IComparable
        {
            if (data == null)
                throw new NullReferenceException();
            if (data.Length == 0)
                throw new IndexOutOfRangeException();

            T min = data[0];
            for (int i = 1; i < data.Length; ++i)
            {
                if (min.CompareTo(data[i]) > 0)
                    min = data[i];
            }
            return min;
        }

        /// <summary>
        /// 获取数组最大值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T SelectMax<T>(T[] data) where T : IComparable
        {
            if (data == null)
                throw new NullReferenceException();
            if (data.Length == 0)
                throw new IndexOutOfRangeException();

            T max = data[0];
            for (int i = 1; i < data.Length; ++i)
            {
                if (max.CompareTo(data[i]) < 0)
                    max = data[i];
            }
            return max;
        }
    }
}
