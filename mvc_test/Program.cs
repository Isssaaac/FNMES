using System;
using System.IO;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using log4net.Core;

namespace DynamicLog4NetDemo
{
    class Program
    {
        // 1. 通用日志记录器（可模拟已有的其他日志配置）
        private static readonly ILog generalLogger = LogManager.GetLogger(typeof(Program));

        // 2. 专门用于动态文件夹的日志记录器
        private static ILog dynamicFileLogger;

        static void Main(string[] args)
        {
            try
            {
                MesLogManager.LogInfo("AAA", "A1");
                MesLogManager.LogInfo("BBB", "B1");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n程序运行出错：{ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Console.WriteLine("\n按任意键退出...");
                Console.ReadKey();
            }
        }


  
    }
}