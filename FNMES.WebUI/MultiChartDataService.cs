// 1. Hub 类
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using System;

public class MultiChartHub : Hub { }

// 2. 数据推送服务
public class MultiChartDataService : BackgroundService
{
    private readonly IHubContext<MultiChartHub> _hubContext;
    private readonly Random _random = new Random();

    public MultiChartDataService(IHubContext<MultiChartHub> hubContext)
    {
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // 推送温度数据（单值时间序列格式）
            await PushToChart(
                chartId: "temperature-chart",
                data: new
                {
                    time = DateTime.Now.ToString("HH:mm:ss"),
                    value = Math.Round(50 + _random.NextDouble() * 30, 1)
                }
            );

            // 推送销售数据（多系列格式）
            await PushToChart(
                chartId: "sales-chart",
                data: new
                {
                    today = new[] { 100 + _random.Next(50), 80 + _random.Next(40), 120 + _random.Next(60) },
                    yesterday = new[] { 90 + _random.Next(40), 70 + _random.Next(30), 110 + _random.Next(50) }
                }
            );

            // 推送状态数据（饼图格式）
            await PushToChart(
                chartId: "status-chart",
                data: new[] {
                    new { name = "运行中", value = 3 + _random.Next(2) },
                    new { name = "停机", value = 1 + _random.Next(1) },
                    new { name = "维护中", value = 0 + _random.Next(1) }
                }
            );

            await Task.Delay(3000, stoppingToken); // 每3秒推送一次
        }
    }

    // 通用推送方法（不关心数据格式，只负责传递）
    private async Task PushToChart(string chartId, object data)
    {
        await _hubContext.Clients.All.SendAsync("UpdateChart", chartId, data);
    }
}
