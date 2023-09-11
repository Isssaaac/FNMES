using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FNMES.Utility.Core
{
    /// <summary>
    /// 使用Random类生成伪随机数
    /// </summary>
    public static class RandomHelper
    {

        /// <summary>
        /// 生成一个指定范围的随机整数，该随机数范围包括最小值，但不包括最大值
        /// </summary>
        /// <param name="minNum">最小值</param>
        /// <param name="maxNum">最大值</param>
        public static int GetRandomInt(int minNum, int maxNum)
        {
            return new Random().Next(minNum, maxNum);
        }

        /// <summary>
        /// 生成一个0.0到1.0的随机小数
        /// </summary>
        public static double GetRandomDouble()
        {
            return new Random().NextDouble();
        }

        /// <summary>
        /// 对一个数组进行随机排序
        /// </summary>
        /// <typeparam name="T">数组的类型</typeparam>
        /// <param name="arr">需要随机排序的数组</param>
        public static void GetRandomArray<T>(T[] arr)
        {
            //对数组进行随机排序的算法:随机选择两个位置，将两个位置上的值交换

            //交换的次数,这里使用数组的长度作为交换次数
            int count = arr.Length;

            //开始交换
            for (int i = 0; i < count; i++)
            {
                //生成两个随机数位置
                int targetIndex1 = GetRandomInt(0, arr.Length);
                int targetIndex2 = GetRandomInt(0, arr.Length);

                //定义临时变量
                T temp;

                //交换两个随机数位置的值
                temp = arr[targetIndex1];
                arr[targetIndex1] = arr[targetIndex2];
                arr[targetIndex2] = temp;
            }
        }
    }
}
