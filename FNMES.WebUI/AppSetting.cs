using FNMES.Utility.Core;
using FNMES.Utility.Extension;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SqlSugar;
using System;
using System.IO;
using  FNMES.Utility;

namespace CCS.WebUI
{
    public static class AppSetting
    {
        public static IConfiguration Configuration { get; private set; }
        private static MyConnectionConFig _connection;
        private static MyConnectionConFig[] _LineConnections;
        
        public static MyConnectionConFig sysConnection 
        {
            get { return _connection; } 
        }
        public static MyConnectionConFig[] lineConnections
        {
            get { return _LineConnections; }
        }
        public static string DbConnectionString
        {
            get { return _connection.DbConnectionString; }
        }
        public static string DBType
        {
            get { return _connection.DBType; }
        }

        public static string WebSoftwareName { get; set; }
        public static string FactoryUrl { get; set; }
        public static string PlantCode { get; set; }
        public static int WorkId { get; set; }
        public static string Copyright { get; set; }
        public static int LogOutDateDays { get; set; }
        public static int SessionTimeout { get; set; }
        public static FTPparam FTPparam { get; set; }
        public static void Init(this IServiceCollection services, IConfiguration configuration)
        {
            Configuration = configuration;
            services.Configure<Secret>(configuration.GetSection("Secret"));
            services.Configure<MyConnectionConFig>(configuration.GetSection("SysConnection"));
            var provider = services.BuildServiceProvider();
            IWebHostEnvironment environment = provider.GetRequiredService<IWebHostEnvironment>();
            _connection = provider.GetRequiredService<IOptions<MyConnectionConFig>>().Value;
            //string s = _connection.ToJson();
            _LineConnections =  configuration.GetSection("LineConnections").Get<MyConnectionConFig[]>();
            FTPparam = configuration.GetSection("FTP").Get<FTPparam>();
            //s = _LineConnections.ToJson();
            MyEnvironment.Init(Path.Combine(environment.ContentRootPath, ""));
            WebSoftwareName = (configuration["WebSoftwareName"] ?? "");
            Copyright = (configuration["Copyright"] ?? "");
            LogOutDateDays = Convert.ToInt32(configuration["LogOutDateDays"] ?? "30");
            SessionTimeout = Convert.ToInt32(configuration["SessionTimeout"] ?? "20");
            FactoryUrl = (configuration["FactoryUrl"] ?? "");
            PlantCode = (configuration["PlantCode"] ?? "");//20240418 添加
            WorkId = Convert.ToInt32(configuration["WorkId"] ?? "http://221.230.79.84:9199");
            if (string.IsNullOrEmpty(_connection.DbConnectionString))
                throw new Exception("未配置好数据库默认连接");
        }
        // 多个节点name格式 ：["key:key1"]
        public static string GetSettingString(string key)
        {
            return Configuration[key];
        }
        // 多个节点,通过.GetSection("key")["key1"]获取
        public static IConfigurationSection GetSection(string key)
        {
            return Configuration.GetSection(key);
        }
    }

    public class FTPparam
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
   
}
