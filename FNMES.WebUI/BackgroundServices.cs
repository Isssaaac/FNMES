using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FNMES.WebUI
{
    public class BackgroundServices : IHostedService, IDisposable
    {
        /// <summary>
        /// 启动任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            //日志配置 
            Utility.Logs.LogHelper.Init(File.ReadAllText(Utility.Extension.MyEnvironment.RootPath("Configs/log4net.config")));
            return Task.CompletedTask;
        }

        /// <summary>
        /// 停止任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            //停止时处理，如果有线程，需要中断
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
