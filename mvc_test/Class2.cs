using System;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;
public class MesDataService
{
    // 这是一个与MES交互的方法
    public void SyncProductionOrder(string orderId, string orderData)
    {
        // 1. 获取针对 "SyncProductionOrder" 这个接口的日志记录器
        // 日志将会被写入到 D:/MESLOG/SyncProductionOrder/ 目录下
        ILog mesLogger = MesLogManager.GetMesLogger("SyncProductionOrder");

        try
        {
            mesLogger.Info($"开始同步生产订单。订单号: {orderId}");
            mesLogger.Debug($"同步的数据内容: {orderData}"); // 注意：Debug级别默认不会被记录

            // 2. 在这里执行你的MES API调用或数据交互逻辑
            // ...
            // var response = mesApiClient.PostAsync("/api/ProductionOrder", orderData).Result;

            mesLogger.Info($"生产订单 {orderId} 同步成功。");
        }
        catch (Exception ex)
        {
            // 3. 记录异常信息
            mesLogger.Error($"生产订单 {orderId} 同步失败！", ex);
        }
    }

    public void UploadQualityInspection(string inspectionData)
    {
        // 为另一个接口获取日志记录器
        // 日志将会被写入到 D:/MESLOG/UploadQualityInspection/ 目录下
        ILog mesLogger = MesLogManager.GetMesLogger("UploadQualityInspection");

        try
        {
            mesLogger.Info("开始上传质检数据。");
            // ... 执行上传逻辑 ...
            mesLogger.Info("质检数据上传成功。");
        }
        catch (Exception ex)
        {
            mesLogger.Error("质检数据上传失败！", ex);
        }
    }
}