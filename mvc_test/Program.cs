
using SqlSugar;
using FNMES.Utility.Core;
using FNMES.Utility.Files;
using System.Data;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using System;


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
    DbType = SqlSugar.DbType.SqlServer,
    IsAutoCloseConnection = true,
});

var types = new List<Type>();
types.Add(typeof(User));
types.Add(typeof(Order));
types.Add(typeof(Info));

db.CodeFirst.InitTables(types.ToArray());

//User user1 = new User() {
//    Id = 1,
//    Name = "Test"
//};
//db.Insertable<User>(user1).ExecuteCommand();

//Order order = new Order() {
//    Id = 2,
//    UserId = 1,
//    Product = "111",
//    Price = "123"
//};
//db.Insertable<Order>(order).ExecuteCommand();

//var info = new Info()
//{
//    Id = 2,
//    Age = "12",
//};
//db.Insertable<Info>(info).ExecuteCommand();

string callTime = DateTime.Now.ToString();
string currentStamp = ExtDateTime.GetTimeStamp(DateTime.Now);
DateTime n = ExtDateTime.TimeStampToDateTime("1702339200000");
Console.WriteLine($"时间戳:{currentStamp},测试时间:{n.ToString()}");
var t = new test();
Console.WriteLine(t.nono!="2");
class test
{
    public string? nono;
}




//var tab = db.Queryable<User>().Includes(it => it.Orders);
//// 创建一个 DataTable 用于存储数据  
//DataTable dataTable = tab.ToDataTable();

//// 导出 DataTable 到 Excel  
//ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
//using (ExcelPackage excelPackage = new ExcelPackage())
//{
//    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1"); // 创建工作表  

//    // 将列名写入第一行  
//    for (int i = 0; i < dataTable.Columns.Count; i++)
//    {
//        worksheet.Cells[1, i + 1].Value = dataTable.Columns[i].ColumnName;
//    }

//    // 将数据写入 Excel  
//    for (int i = 0; i < dataTable.Rows.Count; i++)
//    {
//        for (int j = 0; j < dataTable.Columns.Count; j++)
//        {
//            worksheet.Cells[i + 2, j + 1].Value = dataTable.Rows[i][j];
//        }
//    }

//    // 设置 Excel 文件的路径  
//    string filePath = @"D:\output.xlsx"; // 替换为您的文件路径  

//    // 保存 Excel 文件  
//    excelPackage.SaveAs(new System.IO.FileInfo(filePath));
//}

//Console.WriteLine("数据已成功导出到 Excel 文件。");






//Dictionary<string, string> outkeyValuePairs = new Dictionary<string, string>() {
//                    {"Id", "Id"},
//                    {"Name", "名称"}};

//// 填充数据到工作表
//List<Dictionary<string, string>> keyValues = new List<Dictionary<string, string>>();
//keyValues.Add(outkeyValuePairs);
//List<string> sheetNames = new List<string> { "OutStation"};
//List<DataTable> tables = new List<DataTable>();
//DataTable dt2 = tab.ToDataTable();
////tables.Add(dt1);
//tables.Add(dt2);


////转换称文件流传出去
//var bytes = ExcelUtils.DtExportExcel(tables, keyValues, sheetNames);
//// 创建文件流
//var stream = new MemoryStream(bytes);



////为什么会失效
//var sql = db.Deleteable<SysUser>().Where(it=>it.Name == "nono").SplitTable(tabs => tabs.Take(3)).ExecuteCommand();
//Console.WriteLine(sql);
//var i = db.Queryable<SysUser>().Where(it => it.Name == "nono").SplitTable(tabs => tabs.Take(3)).ToList();
//Console.WriteLine(JsonConvert.SerializeObject(i));

public class User
{
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
    public int Id { get; set; }
    public string Name { get; set; }

    [Navigate(NavigateType.OneToMany, nameof(Order.UserId))]//一对多
    public List<Order> Orders { get; set; }
}

public class Info
{
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
    public int Id { get; set; }
    public string Age { get; set; }
}

public class Order
{
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
    public int Id { get; set; }

    public int UserId { get; set; }
    public string Product { get; set; }
    public string Price { get; set; }
}




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

