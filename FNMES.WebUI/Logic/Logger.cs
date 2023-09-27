using FNMES.Entity.Sys;
using FNMES.Utility.Logs;
using FNMES.Utility.Other;
using FNMES.WebUI.Logic.Base;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FNMES.WebUI.Logic
{
    /// <summary>
    /// 日志
    /// </summary>
    public class Logger
    {
        private static object _lock = new object();
        private static void Log(string type, string message)
        {
            lock (_lock)
            {
                SimpleClient<SysLog> client = new SimpleClient<SysLog>(BaseLogic.GetInstance());



                SysLog log = new SysLog();
                log.Id = SnowFlakeSingle.instance.NextId();
                log.Type = type;
#if !NETFRAMEWORK
                log.ThreadId = Thread.GetCurrentProcessorId();
#else
                log.ThreadId = Thread.CurrentThread.ManagedThreadId;
#endif
                log.Message = message;
                log.CreateTime = DateTime.Now;
                client.Insert(log);
            }
        }



        /// <summary>
        /// 操作日志写入
        /// </summary>
        /// <param name="message"></param>
        public static void OperateInfo(string message)
        {
            Log("Operate", message);
            LogHelper.Operate(message);
        }

        /// <summary>
        /// 运行日志写入
        /// </summary>
        /// <param name="message"></param>
        public static void RunningInfo(string message)
        {
            /*Log("Running", message);*/
            LogHelper.Info(message);
        }


        /// <summary>
        /// 错误日志写入
        /// </summary>
        /// <param name="message"></param>
        public static void ErrorInfo(string message)
        {
            Log("Error", message);
            LogHelper.Error(message);
        }
    }
}
