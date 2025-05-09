using CCS.WebUI;
using FNMES.Entity.DTO.ApiParam;
using FNMES.Entity.Param;
using FNMES.Utility.Core;
using FNMES.Utility.Network;
using FNMES.WebUI.API;
using FNMES.WebUI.Logic;
using FNMES.WebUI.Logic.Base;
using FNMES.WebUI.Logic.Param;
using FNMES.WebUI.Logic.Sys;
using Microsoft.Extensions.Hosting;
using SqlSugar;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using FNMES.Entity.DTO.ApiData;

namespace FNMES.WebUI
{
    //后台服务
    public class BackgroundServices : IHostedService, IDisposable
    {
        private Timer _timer;
        private SysLineLogic sysLineLogic;
        private FactoryStatusLogic factoryLogic;
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
            Logger.RunningInfo("日志框架初始化");
            //初始化表
            sysLineLogic = new SysLineLogic();
            factoryLogic = new FactoryStatusLogic();

            BaseLogic baseLogic = new BaseLogic();
            baseLogic.InitAllTable();

            if (AppSetting.WorkId == 1)
            {
                _timer = new Timer(DoHeartbeat, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            }
            return Task.CompletedTask;
        }

        private void DoHeartbeat(object state)
        {
            System.Collections.Generic.List<Entity.Sys.SysLine> sysLines = sysLineLogic.GetList();
            
            foreach (var item in sysLines)
            {
                if (item.IsEnabled)
                {
                    FactoryStatus status = GetStatus(item.ConfigId);
                    HeartbeatParam param = new()
                    {
                        productionLine = item.EnCode,
                        ipAddress = "10.11.13.4",
                        sourceSys = "GD-111"
                    };
                    string ret = APIMethod.Call(Url.HeartbeatUrl, param, item.ConfigId,true);
                    RetMessage<object> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<object>>();
                    //没有响应，根据状态当前状态处理，
                    if (retMessage == null || retMessage.messageType != "S")
                    {
                        if (status.Status == 1 && status.Retry < 3)
                        {
                            status.Retry++;
                            factoryLogic.Update(status);
                        }
                        else if (status.Status == 1 && status.Retry == 3)
                        {
                            factoryLogic.Insert(new FactoryStatus()
                            {
                                Id = SnowFlakeSingle.instance.NextId(),
                                Status = 0,
                                ConfigId = item.ConfigId,
                                CreateTime = DateTime.Now,
                                Retry = 0
                            });
                        }
                        else if (status.Status == 0)
                        {
                            //之前没连上，什么都不用做
                        }
                    }
                    else if (retMessage.messageType == "S")
                    {
                        if (status.Status == 0 && status.Retry < 3)
                        {
                            status.Retry++;
                            factoryLogic.Update(status);
                        }
                        else if (status.Status == 0 && status.Retry == 3)
                        {
                            factoryLogic.Insert(new FactoryStatus()
                            {
                                Id = SnowFlakeSingle.instance.NextId(),
                                Status = 1,
                                ConfigId = item.ConfigId,
                                CreateTime = DateTime.Now,
                                Retry = 0
                            });
                            //todo此处需要异步处理离线接口程序。
                        }
                        else if (status.Status == 1)
                        {
                            //之前已连上，什么都不用做
                        }
                    } 
                }
            }
        }
        private FactoryStatus GetStatus(string configId)
        {
            FactoryStatus status = factoryLogic.Get(configId);
            if (status == null)
            {
                status = new FactoryStatus()
                {
                    Id = SnowFlakeSingle.instance.NextId(),
                    Status = 0,
                    ConfigId= configId,
                    CreateTime = DateTime.Now,
                    Retry = 0
                };
                factoryLogic.Insert(status);
            }
            status.ConfigId = configId;
            return status;
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
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
