using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FNMES.Utility.Threads
{
    /// <summary>
    /// 等待变量加载工具类
    /// </summary>
    public class WaitUtil
    {
        #region Wait 等待变量加载完成
        /// <summary>
        /// 等待变量加载完成
        /// </summary>
        /// <param name="func">在此方法中初始化变量</param>
        /// <param name="seconds">超时时间(单位：秒)(超时后停止等待)</param>
        public static void Wait(Func<ICollection> func, int seconds = 20)
        {
            int interval = 500;
            int maxTryCount = seconds * 1000 / interval;
            int tryCount = 0;
            ICollection obj = func();
            while (IsNullOrEmpty(obj) && tryCount++ < maxTryCount)
            {
                Thread.Sleep(interval);
                obj = func();
                Console.WriteLine("等待变量初始化");
                //LogUtil.Log();
            }
            //LogUtil.Log("变量初始化完成");
        }
        #endregion

        #region Wait 等待变量加载完成
        /// <summary>
        /// 等待变量加载完成
        /// </summary>
        /// <param name="func">在此方法中初始化变量</param>
        /// <param name="seconds">超时时间(单位：秒)(超时后停止等待)</param>
        public static void Wait(Func<object> func, int seconds = 20)
        {
            int interval = 500;
            int maxTryCount = seconds * 1000 / interval;
            int tryCount = 0;
            object obj = func();
            while (obj == null && tryCount++ < maxTryCount)
            {
                Thread.Sleep(interval);
                obj = func();
                Console.WriteLine("等待变量初始化");
            }
            Console.WriteLine("变量初始化完成");
        }
        #endregion

        #region 等待变量加载完成
        /// <summary>
        /// 等待变量加载完成
        /// </summary>
        /// <param name="func">在此方法中判断条件</param>
        /// <param name="seconds">超时时间(单位：秒)(超时后停止等待)</param>
        public static void Wait(Func<bool> func, int seconds = 20)
        {
            int interval = 500;
            int maxTryCount = seconds * 1000 / interval;
            int tryCount = 0;
            bool bl = func();
            while (!bl && tryCount++ < maxTryCount)
            {
                Thread.Sleep(interval);
                bl = func();
                Console.WriteLine("等待条件为true");
            }
            Console.WriteLine("条件为true");
        }
        #endregion

        #region IsNullOrEmpty
        private static bool IsNullOrEmpty(ICollection obj)
        {
            if (obj == null) return true;
            if (obj.Count == 0) return true;
            return false;
        }
        #endregion

    }
}
