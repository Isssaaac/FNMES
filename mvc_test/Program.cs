using SqlSugar;
using FNMES.TEST;
// See https://aka.ms/new-console-template for more information


Console.WriteLine("Hello, World!");


SqlSugarClient db = new SqlSugarClient(
    new ConnectionConfig()
    {
        ConnectionString = "Server=127.0.0.1;Database=MVC;User ID=sa;Password=gdlaser;Trusted_Connection=False;",
        DbType = DbType.SqlServer,
        IsAutoCloseConnection = true,
        InitKeyType = InitKeyType.Attribute
    });

    db.Aop.OnLogExecuting = (sql, pars) => {
        Console.WriteLine(sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
        Console.WriteLine();
       
        
    };


int test()
    {
    
        
    ////新增用户基本信息。
    SysUser model = new()
    {
        Id = SnowFlakeSingle.Instance.NextId(),
        UserNo = "TT",
        CardNo = "rt",
        Name = "rt",
        CreateTime = Convert.ToDateTime("2020-02-01")
    };
    db.Insertable(model).SplitTable().ExecuteCommand();
        return 0;
      }


    test();
    
    Console.ReadLine();
    db.Close();
