using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FNMES.Utility.Extension.DataCache;
using FNMES.Utility.Extension.SqlSugar;
using System.Data;
using JinianNet.JNTemplate;
using FNMES.Utility.Extension;
using FNMES.Entity.Base;
using FNMES.Utility.Core;


namespace FNMES.Logic.Base
{
    public class BaseLogic
    {
        private static string ConnectionString;
        private static SqlSugar.DbType DbType;
        private static string DbName;
        private static bool DebugMode;

        public static bool InitDB(string dbType, string host, string dbName, string userName, string pasword, ref string message, bool debug = false)
        {
            DebugMode = debug;
            DbName = dbName;
            if (dbType.ToLower() == "SqlServer".ToLower())
            {
                DbType = SqlSugar.DbType.SqlServer;
                ConnectionString = $"Data Source={host};Initial Catalog={dbName};User ID={userName};Password={pasword};";
                return true;
            }
            else if (dbType.ToLower() == "MySql".ToLower())
            {
                DbType = SqlSugar.DbType.MySql;
                ConnectionString = $"server={host};Database={dbName};Uid={userName};Pwd={pasword};";
                return true;
            }
            else if (dbType.ToLower() == "Oracle".ToLower())
            {
                DbType = SqlSugar.DbType.Oracle;
                ConnectionString = $"Data Source={host}/{dbName};User ID={userName};Password={pasword};";
                return true;
            }
            else if (dbType.ToLower() == "PostgreSql".ToLower())
            {
                DbType = SqlSugar.DbType.PostgreSQL;
                ConnectionString = $"Server={host};Port=5432;Database={dbName};User Id={userName};Password={pasword};";
                return true;
            }
            else if (dbType.ToLower() == "Sqlite".ToLower())
            {
                DbType = SqlSugar.DbType.Sqlite;

                var template = Engine.CreateTemplate(dbName);
                template.Set("BaseDirectory", MyEnvironment.RootPath(""));
                var result = template.Render();

                ConnectionString = $"DataSource={result};";
                return true;
            }
            else
            {
                message = "不支持的数据库";
                return false;
            }
        }

        public static SqlSugarClient GetInstance()
        {
#if !NETFRAMEWORK && !WINDOWS
            SqlSugarScope client = (SqlSugar.SqlSugarScope)Utility.MiddleWare.AutofacContainerModule.GetService<ISqlSugarClient>();
            if (client != null)
                return client.ScopedContext;
#endif

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnectionString,
                DbType = DbType,
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    SqlFuncServices = ExtMethods.GetExpMethods,
                    DataInfoCacheService = new HttpRuntimeCache()
                }
            });
            //用来打印Sql方便你调式    
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                if (DebugMode)
                {
                    Console.WriteLine(sql + "\r\n" +
                    db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                    Console.WriteLine();
                }
            };
            return db;
        }

        public DataTable GetTableColumnInfo(string tableName)
        {
            if (DbType == SqlSugar.DbType.SqlServer)
            {
                using (var db = GetInstance())
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("     SELECT                                                                  ").AppendLine();
                    strSql.Append("         A.Name AS TableName,                                                ").AppendLine();
                    strSql.Append("         B.Name AS ColumnName,                                               ").AppendLine();
                    strSql.Append("         D.Name AS TypeName,                                                 ").AppendLine();
                    strSql.Append("         B.Max_Length AS TypeLength,                                         ").AppendLine();
                    strSql.Append("         CASE WHEN                                                           ").AppendLine();
                    strSql.Append("             (                                                               ").AppendLine();
                    strSql.Append("                 SELECT                                                      ").AppendLine();
                    strSql.Append("                     F.Name                                                  ").AppendLine();
                    strSql.Append("                 FROM                                                        ").AppendLine();
                    strSql.Append("                     information_schema.key_column_usage E,sys.indexes F     ").AppendLine();
                    strSql.Append("                 WHERE                                                       ").AppendLine();
                    strSql.Append("                     F.object_id=B.object_id                                 ").AppendLine();
                    strSql.Append("                     AND F.name=E.constraint_name                            ").AppendLine();
                    strSql.Append("                     AND F.is_primary_key=1                                  ").AppendLine();
                    strSql.Append("                     AND E.table_name=A.Name                                 ").AppendLine();
                    strSql.Append("                     AND E.column_name =B.Name                               ").AppendLine();
                    strSql.Append("             ) IS NULL THEN 0 ELSE 1 END AS IsPrimaryKey,                    ").AppendLine();
                    strSql.Append("       '' AS ClassName,                                                      ").AppendLine();
                    strSql.Append("       '' AS PropertyName,                                                   ").AppendLine();
                    strSql.Append("       '' AS CSType,                                                         ").AppendLine();
                    strSql.Append("       C.VALUE AS Other                                                      ").AppendLine();
                    strSql.Append("     FROM sys.tables A                                                       ").AppendLine();
                    strSql.Append("     LEFT JOIN sys.columns B                                                 ").AppendLine();
                    strSql.Append("     ON B.object_id = A.object_id                                            ").AppendLine();
                    strSql.Append("     LEFT JOIN sys.extended_properties C                                     ").AppendLine();
                    strSql.Append("     ON C.major_id = B.object_id AND C.minor_id = B.column_id                ").AppendLine();
                    strSql.Append("     LEFT JOIN sys.types D ON D.system_type_id=B.system_type_id              ").AppendLine();
                    strSql.Append("     WHERE A.Name = '" + tableName + "'                                      ").AppendLine();
                    DataTable dt = db.Ado.GetDataTable(strSql.ToString());
                    return dt;
                }
            }
            else if (DbType == SqlSugar.DbType.MySql)
            {
                using (var db = GetInstance())
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("     SELECT                                                                     ").AppendLine();
                    strSql.Append("     '" + tableName + "' AS TableName,                                          ").AppendLine();
                    strSql.Append("     COLUMN_NAME AS ColumnName,                                                 ").AppendLine();
                    strSql.Append("     DATA_TYPE AS TypeName,                                                     ").AppendLine();
                    strSql.Append("     CHARACTER_MAXIMUM_LENGTH AS TypeLength,                                    ").AppendLine();
                    strSql.Append("     case when COLUMN_KEY = 'PRI' THEN 1 ELSE 0 END AS IsPrimaryKey,            ").AppendLine();
                    strSql.Append("     '' AS ClassName,                                                           ").AppendLine();
                    strSql.Append("     '' PropertyName,                                                           ").AppendLine();
                    strSql.Append("     '' CSType   ,                                                              ").AppendLine();
                    strSql.Append("     COLUMN_COMMENT AS Other                                                    ").AppendLine();
                    strSql.Append("     FROM                                                                       ").AppendLine();
                    strSql.Append("         INFORMATION_SCHEMA.COLUMNS                                             ").AppendLine();
                    strSql.Append("     WHERE                                                                      ").AppendLine();
                    strSql.Append("         table_schema ='" + DbName + "' AND table_name = '" + tableName + "'    ").AppendLine();
                    strSql.Append("     ORDER BY ORDINAL_POSITION                                                  ").AppendLine();
                    DataTable dt = db.Ado.GetDataTable(strSql.ToString());
                    return dt;
                }
            }
            else if (DbType == SqlSugar.DbType.Sqlite)
            {
                using (var db = GetInstance())
                {

                    string sql = "SELECT * from sqlite_master where type = 'table' AND tbl_name='" + tableName + "'";
                    DataTable dt = db.Ado.GetDataTable(sql);
                    //创建个新的DataTable,把dt中的数据放进去
                    DataTable newDt = new DataTable();
                    newDt.Columns.Add("TableName");
                    newDt.Columns.Add("ColumnName");
                    newDt.Columns.Add("TypeName");
                    newDt.Columns.Add("TypeLength");
                    newDt.Columns.Add("IsPrimaryKey");
                    newDt.Columns.Add("ClassName");
                    newDt.Columns.Add("PropertyName");
                    newDt.Columns.Add("CSType");
                    newDt.Columns.Add("Other");
                    if (dt == null || dt.Rows.Count == 0)
                        return newDt;
                    string sql1 = dt.Rows[0]["sql"].ToString().Replace("\"", "");

                    List<CodeGenerator> list = new List<CodeGenerator>();
                    //第一个(
                    int index1 = sql1.IndexOf("(");
                    int index2 = sql1.LastIndexOf(")");
                    string content = sql1.Substring(index1 + 1, index2 - index1 - 1);
                    List<string> array = content.Split('\n').Select(it => it.Trim()).ToList();
                    List<string> primaryKey = new List<string>();
                    foreach (string item in array)
                    {
                        if (item.Trim() == "")
                            continue;
                        if (item.ToUpper().StartsWith("PRIMARY KEY"))
                        {
                            int index3 = item.IndexOf("(");
                            int index4 = item.IndexOf(")");
                            string[] keyArray = item.Substring(index3 + 1, index4 - index3 - 1).Split(',');
                            foreach (string key in keyArray)
                                primaryKey.Add(key.Trim());
                            continue;
                        }
                        string[] itemText = item.Trim().Split(' ');
                        CodeGenerator codeGenerator = new CodeGenerator();
                        codeGenerator.TableName = tableName;
                        codeGenerator.ColumnName = itemText[0];
                        string type = itemText[1];
                        int index5 = type.IndexOf("(");
                        int index6 = type.IndexOf(")");
                        codeGenerator.TypeName = index5 == -1 ? type : type.Substring(0, index5);
                        codeGenerator.TypeLength = index5 == -1 ? "0" : type.Substring(index5 + 1, index6 - index5 - 1);
                        codeGenerator.IsPrimaryKey = "0";
                        codeGenerator.ClassName = "";
                        codeGenerator.PropertyName = "";
                        codeGenerator.CSType = "";
                        codeGenerator.Other = "";
                        list.Add(codeGenerator);
                    }
                    foreach (CodeGenerator code in list)
                    {
                        if (primaryKey.Contains(code.ColumnName))
                            code.IsPrimaryKey = "1";
                    }
                    return list.ToJson().ToDataTable();
                }
            }


            return null;
        }

        public List<string> GetTableList()
        {
            if (DbType == SqlSugar.DbType.SqlServer)
            {
                using (var db = GetInstance())
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("         SELECT                ").AppendLine();
                    strSql.Append("             A.Name            ").AppendLine();
                    strSql.Append("         FROM                  ").AppendLine();
                    strSql.Append("             sysobjects  A     ").AppendLine();
                    strSql.Append("         WHERE                 ").AppendLine();
                    strSql.Append("             A.xtype = 'U'     ").AppendLine();
                    DataTable dt = db.Ado.GetDataTable(strSql.ToString());
                    if (dt == null)
                        return new List<string>();
                    List<string> list = new List<string>();
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(row["Name"].ToString());
                    }
                    return list;
                }
            }
            else if (DbType == SqlSugar.DbType.MySql)
            {
                using (var db = GetInstance())
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("         SELECT                                 ").AppendLine();
                    strSql.Append("             TABLE_NAME AS Name                 ").AppendLine();
                    strSql.Append("         FROM                                   ").AppendLine();
                    strSql.Append("             information_schema.TABLES          ").AppendLine();
                    strSql.Append("         WHERE                                  ").AppendLine();
                    strSql.Append("             TABLE_SCHEMA = '" + DbName + "'    ").AppendLine();
                    DataTable dt = db.Ado.GetDataTable(strSql.ToString());
                    if (dt == null)
                        return new List<string>();
                    List<string> list = new List<string>();
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(row["Name"].ToString());
                    }
                    return list;
                }
            }
            else if (DbType == SqlSugar.DbType.Sqlite)
            {
                using (var db = GetInstance())
                {
                    string sql = "SELECT name from sqlite_master where type='table'";
                    DataTable dt = db.Ado.GetDataTable(sql);
                    if (dt == null)
                        return new List<string>();
                    List<string> list = new List<string>();
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(row["name"].ToString());
                    }
                    return list;
                }
            }
            return new List<string>();
        }
    }
}
