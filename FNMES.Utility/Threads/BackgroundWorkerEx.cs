using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Utility.Threads
{
    /// <summary>
    /// 封装 BackgroundWorker
    /// 建议用BackWork
    /// </summary>
    public class BackgroundWorkerEx
    {
        /// <summary>
        /// 执行
        /// 例：BackWork.Run(() => { }, () => { }, (ex) => { });
        /// </summary>
        /// <param name="doWork">DoWork</param>
        /// <param name="workCompleted">RunWorkerCompleted</param>
        /// <param name="errorAction">错误处理</param>
        public static BackgroundWorker Run(Action doWork, Action workCompleted = null, Action<Exception> errorAction = null)
        {
            bool isDoWorkError = false;
            Exception doWorkException = null;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, e) =>
            {
                try
                {
                    doWork();
                }
                catch (Exception ex)
                {
                    isDoWorkError = true;
                    doWorkException = ex;
                    Console.WriteLine(ex.Message);
                }
            };
            worker.RunWorkerCompleted += (s, e) =>
            {
                if (!isDoWorkError)
                {
                    try
                    {
                        if (workCompleted != null) workCompleted();
                    }
                    catch (Exception ex)
                    {
                        if (errorAction != null) errorAction(ex);
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    if (errorAction != null) errorAction(doWorkException);
                }
            };
            worker.RunWorkerAsync();
            return worker;
        }
    }
}
