using FNMES.Utility.Extension;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace CCS.WebUI
{
    public static class AppSetting
    {
        public static IConfiguration Configuration { get; private set; }
        private static Connection _connection;

        public static string DbConnectionString
        {
            get { return _connection.DbConnectionString; }
        }
        public static string DBType
        {
            get { return _connection.DBType; }
        }

        public static string WebSoftwareName { get; set; }

        public static string Copyright { get; set; }

        public static int LogOutDateDays { get; set; }
        public static int SessionTimeout { get; set; }
        public static void Init(this IServiceCollection services, IConfiguration configuration)
        {
            Configuration = configuration;
            services.Configure<Secret>(configuration.GetSection("Secret"));
            services.Configure<Connection>(configuration.GetSection("Connection"));
            var provider = services.BuildServiceProvider();
            IWebHostEnvironment environment = provider.GetRequiredService<IWebHostEnvironment>();
            _connection = provider.GetRequiredService<IOptions<Connection>>().Value;
            MyEnvironment.Init(Path.Combine(environment.ContentRootPath, ""));
            WebSoftwareName = (configuration["WebSoftwareName"] ?? "");
            Copyright = (configuration["Copyright"] ?? "");
            LogOutDateDays = Convert.ToInt32(configuration["LogOutDateDays"] ?? "30");
            SessionTimeout = Convert.ToInt32(configuration["SessionTimeout"] ?? "20");
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

    public class Connection
    {
        public string DBType { get; set; }
        public string DbConnectionString { get; set; }
    }
}
