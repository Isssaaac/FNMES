using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Text;

namespace FNMES.Utility.Logs
{
    /// <summary>     
    /// Log4Net日志框架辅助类。     
    /// </summary>     
    public static class LogHelper
    {
        public static readonly ILog loginfo = LogManager.GetLogger("loginfo");
        public static readonly ILog logerror = LogManager.GetLogger("logerror");
        public static readonly ILog logoperate = LogManager.GetLogger("logoperate");

        //private static readonly ILog mesLogger = LogManager.GetLogger("MESLogger");

        public static void Init(string configContent)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(configContent));
            XmlConfigurator.Configure(stream);
        }

        public static void MESInfo(string interfaceName, string message)
        {
            
            log4net.GlobalContext.Properties["InterfaceName"] = interfaceName;
            log4net.ThreadContext.Properties["InterfaceName"] = interfaceName;
            object s = ThreadContext.Properties["InterfaceName"];
            var mesLogger = log4net.LogManager.GetLogger("MESLogger");
            mesLogger.Info(message);
        }

        public static void Info(string message)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(message);
            }
        }

        public static void Info(string message, Exception ex)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(message, ex);
            }
        }

        public static void Operate(string message)
        {
            if (logoperate.IsDebugEnabled)
            {
                logoperate.Debug(message);
            }
        }
        public static void Operate(string message, Exception ex)
        {
            if (logoperate.IsDebugEnabled)
            {
                logoperate.Debug(message, ex);
            }
        }

        public static void Error(string message)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(message);
            }
        }

        public static void Error(string message, Exception ex)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(message, ex);
            }
        }
    }
}

