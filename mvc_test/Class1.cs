using System;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;

public static class MesLogManager
{
    private const string MesLogRepositoryName = "MES_LOG_REPOSITORY";
    private static readonly ILoggerRepository _mesLogRepository;

    static MesLogManager()
    {
        _mesLogRepository = LogManager.CreateRepository(MesLogRepositoryName) ?? LogManager.GetRepository(MesLogRepositoryName);

        var hierarchy = (Hierarchy)_mesLogRepository;
        hierarchy.Root.Level = Level.Info;
        hierarchy.Configured = true;
    }

    public static ILog GetMesLogger(string interfaceName)
    {
        if (string.IsNullOrWhiteSpace(interfaceName))
        {
            throw new ArgumentException("接口名称不能为空", nameof(interfaceName));
        }

        string loggerName = $"MES.{interfaceName}";
        ILog logger = LogManager.GetLogger(MesLogRepositoryName, loggerName);

        var loggerImpl = logger.Logger as Logger;
        if (loggerImpl != null && loggerImpl.Appenders.Count == 0)
        {
            AddAppenderToLogger(loggerImpl, interfaceName);
        }

        return logger;
    }

    /// <summary>
    /// 为指定的Logger动态添加一个FileAppender（已修正文件名问题）
    /// </summary>
    private static void AddAppenderToLogger(Logger logger, string interfaceName)
    {
        // 1. 构建日志文件存放的目录
        string logDirectory = Path.Combine(@"D:\MESLOG", interfaceName);

        // 确保目录存在
        Directory.CreateDirectory(logDirectory);

        // 2. 创建RollingFileAppender
        var appender = new RollingFileAppender
        {
            Name = $"MesAppender.{interfaceName}",

            // *** 关键修正点 1 ***
            // 将File设置为目录 + 一个固定的基础文件名（例如 "log"）
            // 这个基础文件名会在滚动时被使用
            File = Path.Combine(logDirectory, "log"),

            AppendToFile = true,
            RollingStyle = RollingFileAppender.RollingMode.Date,

            // *** 关键修正点 2 ***
            // DatePattern定义了日期部分的格式以及文件后缀
            // 当滚动时，它会被追加到 "File" 属性的值后面
            // 最终形成 log.20230121.log
            DatePattern = "yyyyMMdd'.log'",

            StaticLogFileName = false, // 因为文件名是动态变化的，所以设为false
            LockingModel = new FileAppender.MinimalLock(),
        };//
        // 3. 设置日志布局
        var layout = new PatternLayout
        {
            ConversionPattern = "%date{yyyy-MM-dd HH:mm:ss,fff} [%level] - %message%newline"
        };
        layout.ActivateOptions();
        appender.Layout = layout;
        // 4. 激活Appender配置
        appender.ActivateOptions();
        // 5. 将Appender添加到Logger中
        logger.AddAppender(appender);
    }

    public static void LogInfo(string interfaceName, string message)
    {
        ILog mesLogger = MesLogManager.GetMesLogger(interfaceName);

        try
        {
            mesLogger.Info(message);
        }
        catch (Exception ex)
        {
            // 3. 记录异常信息
            mesLogger.Error($"{message}", ex);
        }
    }
}