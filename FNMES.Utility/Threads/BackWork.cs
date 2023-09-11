using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Utility.Threads
{
    /// <summary>
    /// 异步执行方法
    /// 替代BackgroundWorker
    /// </summary>
    public class BackWork
    {
        /// <summary>
        /// 异步执行
        /// </summary>
        public static void RunAsync(Action action, Action complete = null, Action<Exception> errorAction = null)
        {
            RunAsync((obj) => action(), null, complete, errorAction);
        }

        /// <summary>
        /// 异步执行
        /// </summary>
        public static async void RunAsync(Action<object> action, object arg = null, Action complete = null, Action<Exception> errorAction = null)
        {
            Exception exception = null;

            Task task = Task.Run(() =>
            {
                try
                {
                    action(arg);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });
            await task;

            if (exception == null)
            {
                if (complete != null)
                {
                    try
                    {
                        complete();
                    }
                    catch (Exception ex)
                    {
                        if (errorAction != null)
                        {
                            errorAction(ex);
                        }
                    }
                }
            }
            else
            {
                if (errorAction != null)
                {
                    errorAction(exception);
                }
            }
        }
    }
}
