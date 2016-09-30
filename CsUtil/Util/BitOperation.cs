using System;
using System.Diagnostics;

namespace CsUtil.Util
{
    class BitOperation
    {
        public const int INT_BIT_NUM = sizeof(int) * 8;
        public const int INT_MAX_1 = (int)((0x01U << (INT_BIT_NUM - 1)) - 1); // int.max - 1;

        /// <summary>
        /// 循环左移
        /// </summary>
        /// <param name="value"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int RotateLeft(int value, int num)
        {
            num &= INT_MAX_1; // %= INT_BIT_NUM
            return (value << num) | (value >> (INT_BIT_NUM - num));
        }

        /// <summary>
        /// 循环左移1位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int RotateLeft(int value)
        {
            return RotateLeft(value, 1);
        }

        /// <summary>
        /// 循环左移，指定位长
        /// </summary>
        /// <param name="value">目标值</param>
        /// <param name="num">左移位数</param>
        /// <param name="maxBitNum">数据位长</param>
        /// <returns></returns>
        public static int RotateLeft(int value, int num, int maxBitNum)
        {
            Debug.Assert(0 < maxBitNum && maxBitNum <= INT_BIT_NUM);
            num &= (int)((0x01U << (maxBitNum - 1)) - 1); // %= maxBitNum
            return (value << num) | (value >> (maxBitNum - num));
        }

        /// <summary>
        /// 循环右移
        /// </summary>
        /// <param name="value"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int RotateRight(int value, int num)
        {
            num &= INT_MAX_1; // %= INT_BIT_NUM
            return (value >> num) | (value << (INT_BIT_NUM - num));
        }

        /// <summary>
        /// 循环右移1位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int RotateRight(int value)
        {
            return RotateRight(value, 1);
        }

        /// <summary>
        /// 循环右移，指定位长
        /// </summary>
        /// <param name="value">目标值</param>
        /// <param name="num">右移位数</param>
        /// <param name="maxBitNum">数据位长</param>
        /// <returns></returns>
        public static int RotateRight(int value, int num, int maxBitNum)
        {
            Debug.Assert(0 < maxBitNum && maxBitNum <= INT_BIT_NUM);
            num &= (int)((0x01U << (maxBitNum - 1)) - 1); // %= maxBitNum
            return (value >> num) | (value << (maxBitNum - num));
        }
    }
}