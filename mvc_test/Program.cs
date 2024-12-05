

using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using SqlSugar;
using FNMES.Utility;

using FNMES.Utility.Logs;
using log4net.Config;
using log4net;
using ServiceStack;

//string logDirectory = "D:/logs";
//if (!Directory.Exists(logDirectory))
//{
//    Directory.CreateDirectory(logDirectory);
//}

////初始化 log4net  
//log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));

//ILog errorLogger = LogManager.GetLogger("logerror");
//ILog warningLogger = LogManager.GetLogger("logoperate");
//ILog infoLogger = LogManager.GetLogger("loginfo");

////记录日志示例  
//errorLogger.Error("This is an error message.");
//warningLogger.Warn("This is a warning message.");
//infoLogger.Info("This is an info message.");

var db = new SqlSugarClient(new ConnectionConfig
{
    ConnectionString = "Server=127.0.0.1;Database=test;User ID=sa;Password=123456;Trusted_Connection=False;TrustServerCertificate=True;",
    DbType = DbType.SqlServer,
    IsAutoCloseConnection = true,
});
//为什么会失效
var sql = db.Deleteable<SysUser>().Where(it=>it.Name == "nono").SplitTable(tabs => tabs.Take(3)).ExecuteCommand();
Console.WriteLine(sql);
var i = db.Queryable<SysUser>().Where(it => it.Name == "nono").SplitTable(tabs => tabs.Take(3)).ToList();
Console.WriteLine(JsonConvert.SerializeObject(i));


//db.CodeFirst.InitTables(typeof(SysUser));

////for (int i = 0; i < 10; i++)
////{
////    var newUser = new SysUser
////    {
////        Id = SnowFlakeSingle.instance.NextId(),
////        Name = "John Doe",
////    };
////    db.Insertable(newUser).SplitTable().ExecuteCommand();
////}
//var users = db.Queryable<SysUser>().SplitTable(tabs => tabs.Take(3)).ToList();

//var newUser = new SysUser
//{
//    Id = SnowFlakeSingle.instance.NextId(),
//    Name = "nono",
//};
//db.Insertable(newUser).SplitTable().ExecuteCommand();

////单字段去重
//var i = db.MasterQueryable<SysUser>().SplitTable(tabs => tabs.Take(4)).Select(s => s.Name).Distinct().ToList();
//// 查询 Name 字段去重的原始行数据  
////var distinctUsers = db.Queryable<SysUser>().SplitTable(tabs=>tabs.Take(4))
////    .GroupBy(u => u.Name)  // 按 Name 字段进行分组  
////    .ToList();  // 执行查询并转为 List<User>  

//Console.WriteLine($"<{JsonConvert.SerializeObject(i)}>");
//Console.WriteLine($"ret:<{i}>");

//var station = "M310,M320,M330";
//List<string> strArray = new List<string>(station.Split(","));
//Console.WriteLine($"<{JsonConvert.SerializeObject(strArray)}>");

//Person person = new Person { Name = "Alice", Age = 30 };
//string jsonString = JsonConvert.SerializeObject(person);
//Console.WriteLine($"序列化后{jsonString}");

//string newJsonString = "{\"Name\":\"Bob\",\"Age\":25}";

//Person newPerson = JsonConvert.DeserializeObject<Person>(newJsonString);
//Console.WriteLine($"反序列{newPerson}");

//public class Person
//{ 
//    public string Name { get; set; }
//    public int Age { get; set; }
//}

