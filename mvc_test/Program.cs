

using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using SqlSugar;
using FNMES.TEST;


var db = new SqlSugarClient(new ConnectionConfig
{
    ConnectionString = "Server=127.0.0.1;Database=test;User ID=sa;Password=123456;Trusted_Connection=False;TrustServerCertificate=True;",
    DbType = DbType.SqlServer,
    IsAutoCloseConnection = true,
});


db.CodeFirst.InitTables(typeof(SysUser));


//for (int i = 0; i < 10; i++)
//{
//    var newUser = new SysUser
//    {
//        Id = SnowFlakeSingle.instance.NextId(),
//        Name = "John Doe",
//    };
//    db.Insertable(newUser).SplitTable().ExecuteCommand();
//}
var users = db.Queryable<SysUser>().SplitTable(tabs => tabs.Take(3)).ToList();
Console.WriteLine(JsonConvert.SerializeObject(users));