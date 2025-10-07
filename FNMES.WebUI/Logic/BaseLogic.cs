using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using FNMES.Utility.Core;
using CCS.WebUI;
using FNMES.Utility;
using FNMES.Entity;
using FNMES.Entity.Enum;
using FNMES.Entity.Param;
using FNMES.Entity.Sys;
using FNMES.Entity.Record;
using System.Linq.Expressions;
using ServiceStack;
using System.Collections;
using System.Reflection;
using System.Data.Entity.Core.Metadata.Edm;
using FNMES.Utility.Logs;
using System.ComponentModel;
using Newtonsoft.Json;

namespace FNMES.WebUI.Logic.Base
{
    public class BaseLogic
    {
        private static SqlSugar.DbType DbType { get {
                switch (AppSetting.sysConnection.DBType.ToUpper())
                {
                    case "MYSQL": return SqlSugar.DbType.MySql;
                    case "SQLITE": return SqlSugar.DbType.Sqlite; 
                    case "SQLSERVER": return SqlSugar.DbType.SqlServer; 
                    case "MSSQL": return SqlSugar.DbType.SqlServer;
                    case "ORACLE": return SqlSugar.DbType.Oracle;
                    default: throw new Exception("配置写的TM是个什么东西？");
                }
            } }

        public static SqlSugarScope Db
        {
            get
            {
                SqlSugarScope client = (SqlSugarScope)Utility.MiddleWare.AutofacContainerModule.GetService<ISqlSugarClient>();
                if (client != null)
                {
                    //正常都是通过注入的方式获取
                    return client;
                }
                //如果没有ioc注入就再创建
                var slavaConFig = new List<SlaveConnectionConfig>();

                foreach (var item in AppSetting.sysConnection.SlaveConnections)
                {
                    slavaConFig.Add(
                    new SlaveConnectionConfig()
                    {
                        HitRate = int.Parse(item.HitRate),
                        ConnectionString = item.ConnectionString
                    });
                }

                SqlSugarScope db = new SqlSugarScope(new ConnectionConfig()
                {
                    //准备添加分表分库
                    ConfigId = AppSetting.sysConnection.ConfigId,
                    ConnectionString = AppSetting.sysConnection.DbConnectionString,
                    IsAutoCloseConnection = true,
                    DbType = DbType,
                    MoreSettings = new ConnMoreSettings()
                    {
                        DisableNvarchar = true
                    },
                    SlaveConnectionConfigs = slavaConFig,
                    //设置codefirst非空值判断
                    ConfigureExternalServices = new ConfigureExternalServices
                    {
                        EntityService = (c, p) =>
                        {
                            //// int?  decimal?这种 isnullable=true
                            if (c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                p.IsNullable = true;
                            }
                            //高版C#写法 支持string?和string  
                            //if (new NullabilityInfoContext().Create(c).WriteState is NullabilityState.Nullable)
                            //{
                            //    p.IsNullable = true;
                            //}
                        }
                    }
                }, db =>
                {
                    db.Aop.DataExecuting = (oldValue, entityInfo) =>
                    {
                        //var httpcontext = ServiceLocator.Instance.GetService<IHttpContextAccessor>().HttpContext;
                        switch (entityInfo.OperationType)
                        {
                            case DataFilterType.InsertByObject:
                                if (entityInfo.PropertyName == "CreateUser")
                                {
                                    //entityInfo.SetValue(new Guid(httpcontext.Request.Headers["Id"].ToString()));
                                }
                                else if (entityInfo.PropertyName == "CreateTime")
                                {
                                    //entityInfo.SetValue(new Guid(httpcontext.Request.Headers["TenantId"].ToString()));
                                }
                                else if (entityInfo.PropertyName == "ModifyUser")
                                {
                                    //entityInfo.SetValue(new Guid(httpcontext.Request.Headers["Id"].ToString()));
                                }
                                else if (entityInfo.PropertyName == "ModifyTime")
                                {
                                    //entityInfo.SetValue(new Guid(httpcontext.Request.Headers["TenantId"].ToString()));
                                }
                                break;
                            case DataFilterType.UpdateByObject:
                                if (entityInfo.PropertyName == "ModifyUser")
                                {
                                    //entityInfo.SetValue(new Guid(httpcontext.Request.Headers["Id"].ToString()));
                                }
                                else if (entityInfo.PropertyName == "ModifyTime")
                                {
                                    //entityInfo.SetValue(DateTime.Now);
                                }
                                break;
                        }

                    };
                    db.Aop.OnLogExecuting = (s, p) =>
                    {
                        //没用，没数据
                        //string message = s + "\r\n" + Db.Utilities.SerializeObject(p.ToDictionary(it => it.ParameterName, it => it.Value));
                        //Logger.RunningInfo(s + "\r\n" +Db.Utilities.SerializeObject(p.ToDictionary(it => it.ParameterName, it => it.Value)));
                        //Console.WriteLine(s + "\r\n" + Db.Utilities.SerializeObject(p.ToDictionary(it => it.ParameterName, it => it.Value)));

                        //using (StreamWriter writer = new StreamWriter("D:\\LOG.txt", true)) // true 表示追加到文件末尾  
                        //{
                        //    writer.WriteLine($"{DateTime.Now}: {message}"); // 写入时间戳和消息  
                        //}

                        //if (GobalModel.SqlLogEnable)
                        //{
                        //    var _logger = ServiceLocator.Instance?.GetRequiredService<ILogger<SqlSugarClient>>();
                        //StringBuilder sb = new StringBuilder();
                        //sb.Append("执行SQL:" + s.ToString());
                        //foreach (var i in p)
                        //{
                        //    sb.Append($"\r\n参数:{i.ParameterName},参数值:{i.Value}");
                        //}
                        //Console.WriteLine(sb.ToString());
                        //    _logger?.LogInformation(sb.ToString());
                        //}
                    };
                });
                return db;
            }
        }


        

        public static void InitSeedData(ISqlSugarClient db,bool IsSystemTable)
        {
            try
            {

                IEnumerable<Type> entityTypes;
                if (IsSystemTable)
                {
                    entityTypes = GlobalContext.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false) && u.IsDefined(typeof(SystemTableInitAttribute), false));
                }
                else
                    entityTypes = GlobalContext.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false) && u.IsDefined(typeof(LineTableInitAttribute), false));

                if (!entityTypes.Any()) return;

                var seedDataTypes = GlobalContext.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.GetInterfaces().Any(i => i.HasImplementedRawGeneric(typeof(ISqlSugarEntitySeedData<>))));

                if (!seedDataTypes.Any()) return;
                foreach (var seedType in seedDataTypes)
                {
                    var entityType = seedType.GetInterfaces().First().GetGenericArguments().First();
                    string tableName = db.EntityMaintenance.GetEntityInfo(entityType).DbTableName;

                    bool tableExist = db.DbMaintenance.IsAnyTable(tableName, false);

                    LogHelper.Info($"实体表{tableName},是否存在:{tableExist}");
                    if (!tableExist)
                        continue;

                    var instance = Activator.CreateInstance(seedType);
                    var hasDataMethod = seedType.GetMethod("SeedData");
                    var seedData = ((IEnumerable)hasDataMethod?.Invoke(instance, null))?.Cast<object>();

                    if (seedData == null) continue;


                    var seedDataTable = seedData.ToList().ToDataTable_();//获取种子数据
                    seedDataTable.TableName = tableName;//获取表名


                    if (seedDataTable.Columns.Contains("Id"))//判断种子数据是否有主键
                    {
                        var ids = new HashSet<object>();
                        foreach (DataRow row in seedDataTable.Rows)
                        {
                            var id = row["Id"];
                            if (!ids.Add(id))
                            {
                                LogHelper.Info($"发现重复的 Id: {id}");
                                continue;
                            }
                        }
                        DataTableResult storage = db.Storageable(seedDataTable).WhereColumns("Id").ToStorage();
                        storage.AsInsertable.ExecuteCommand();

                        //codefirst暂时全部新增,根据主键更新,用户表暂不更新,只插入，即永远保留着种子数据


                        var ignoreUpdate = hasDataMethod.GetCustomAttribute<IgnoreSeedDataUpdateAttribute>();//读取忽略更新特性
                        if (ignoreUpdate == null)
                            storage.AsUpdateable.ExecuteCommand();//只有没有忽略更新的特性才执行更新
                    }
                    else // 没有主键或者不是预定义的主键(有重复的可能)
                    {
                        //全量插入
                        var storage = db.Storageable(seedDataTable).ToStorage();
                        storage.AsInsertable.ExecuteCommand();
                    }
                }

            }
            catch(Exception e)
            {
                LogHelper.Error($"初始化种子数据，是否是系统表：{IsSystemTable}", e);
            }
        }

        /// <summary>
        /// 根据名称获取数据库连接的实例
        /// </summary>
        /// <param name="dbName">默认、"1"-"5"</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ISqlSugarClient GetInstance(string dbName = DatabaseName.Sys)
        {
            if(dbName == DatabaseName.Sys)
            {
                return Db.GetConnection(DatabaseName.Sys);
            }
            else
            {
                if (!Db.IsAnyConnection(dbName))
                {
                    MyConnectionConFig[] myConnectionConFigs = AppSetting.lineConnections;
                    int length = myConnectionConFigs.Length;
                    int index = -1;
                    int i = 0;
                    for (; i < length; i++)
                    {
                        if(dbName == myConnectionConFigs[i].ConfigId)
                        {
                            index = i;
                            break;
                        }
                    }
                    if(index == -1)
                    {
                        //在外侧输出
                       // Logger.ErrorInfo($"没有找到标识为{dbName}的数据库");
                        throw new Exception($"没有找到标识为{dbName}的数据库");
                    }
                    var slava = new List<SlaveConnectionConfig>();

                    foreach (var item in myConnectionConFigs[i].SlaveConnections)
                    {
                        slava.Add(
                        new SlaveConnectionConfig()
                        {
                            HitRate = int.Parse(item.HitRate),
                            ConnectionString = item.ConnectionString
                        });
                    }

                    Db.AddConnection(new ConnectionConfig()
                    {
                        //准备添加分表分库
                        ConfigId = myConnectionConFigs[i].ConfigId,
                        ConnectionString = myConnectionConFigs[i].DbConnectionString,
                        IsAutoCloseConnection = true,
                        DbType = DbType,
                        MoreSettings = new ConnMoreSettings()
                        {
                            DisableNvarchar = true
                        },
                        SlaveConnectionConfigs = slava,
                    }); ;
                }
                return Db.GetConnection(dbName);
            }
        }

        private static void InitDb(ISqlSugarClient db,bool IsSystemTable)
        {

            IEnumerable<Type> entityTypes;
            if (IsSystemTable)
            {
                entityTypes = GlobalContext.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false) && u.IsDefined(typeof(SystemTableInitAttribute), false));
            }
            else
                entityTypes = GlobalContext.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false) && u.IsDefined(typeof(LineTableInitAttribute), false));

            if(!entityTypes.Any()) return;

            foreach (var entityType in entityTypes)
            {
                try
                {
                    var splitTable = entityType.GetCustomAttribute<SplitTableAttribute>();
                    if (splitTable == null)//如果特性是空
                    {
                        db.CodeFirst.SetStringDefaultLength(200).InitTables(entityType);//普通创建

                    }
                    else
                        db.CodeFirst.SetStringDefaultLength(200).SplitTables().InitTables(entityType);//自动分表创建
                }
                catch (Exception ex)
                {
                    Logger.ErrorInfo($"初始化{entityType}数据库表{ex.Message}");
                    continue;
                }
            }
        }

        /// <summary>
        /// 初始化所有不需要分表的表
        /// </summary>
        public void InitAllTable()
        {
            //初始化系统库参数表
            MyConnectionConFig sysConnection = AppSetting.sysConnection;
            try
            {
                ISqlSugarClient db = GetInstance("default");
                InitDb(db,true);
                if(GlobalContext.SystemConfig.IsSeedData)
                    InitSeedData(db,true);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo($"初始化主数据库表{e.Message}");
            }
            //初始化分库参数表
            foreach (var item in AppSetting.lineConnections)
            {
                try
                {
                    ISqlSugarClient db = GetInstance(item.ConfigId);
                    InitDb(db,false);
                    if (GlobalContext.SystemConfig.IsSeedData)
                        InitSeedData(db,false);
                }
                
                catch (Exception e)
                {
                    Logger.ErrorInfo($"初始化线体{item.ConfigId}数据库表{ e.Message}");
                }
            }
        }

        /**************************通用操作*********************************/
        /// <summary>
        /// 带分表表格查询,带时间
        /// </summary>
        /// <typeparam name="TTable"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <param name="totalCount"></param>
        /// <param name="index"></param>
        /// <param name="epress"></param>
        /// <returns></returns>
        public List<TTable> GetSplitTableList<TTable>(int pageIndex, int pageSize, ref int totalCount, string index,string configId, Expression<Func<TTable, bool>>? express = null) where TTable : RecordBase
        {
            try
            {
                DateTime today = DateTime.Today;
                DateTime startTime = today;
                DateTime endTime = today.AddDays(1);
                var db = GetInstance();
                ISugarQueryable<TTable> queryable = db.Queryable<TTable>();
                if (!express.IsNullOrEmpty())
                {
                    //keyword表达式
                    queryable = queryable.Where(express);
                }
                //查询当日
                if (index == "1")
                {

                }
                //近7天
                else if (index == "2")
                {
                    today = DateTime.Today;
                    startTime = today.AddDays(-6);
                    endTime = today.AddDays(1);
                }
                //近1月
                else if (index == "3")
                {
                    today = DateTime.Today;
                    startTime = today.AddDays(-29);
                    endTime = today.AddDays(1);
                }
                //近3月
                else if (index == "4")
                {
                    today = DateTime.Today;
                    startTime = today.AddDays(-91);
                    endTime = today.AddDays(1);
                }
                queryable = queryable.SplitTable(startTime, endTime).Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                return queryable.OrderByDescending(it => it.Id).ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("查询记录失败", e);
                return new List<TTable>();
            }
        }

        //按照时间查询
        public List<TTable> GetSplitTableList<TTable>(int pageIndex, int pageSize, ref int totalCount, DateTime start, DateTime end, Expression<Func<TTable, bool>>? express = null) where TTable : RecordBase
        {
            try
            {
                DateTime today = DateTime.Today;
                DateTime startTime = start;
                DateTime endTime = end;
                var db = GetInstance();
                ISugarQueryable<TTable> queryable = db.Queryable<TTable>();
                if (!express.IsNullOrEmpty())
                {
                    //keyword表达式
                    queryable = queryable.Where(express);
                }
                queryable = queryable.SplitTable(startTime, endTime).Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                return queryable.OrderByDescending(it => it.Id).ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("查询记录失败", e);
                return new List<TTable>();
            }
        }

        /// <summary>
        /// 按照时间分表的表格插入
        /// </summary>
        /// <typeparam name="TTable"></typeparam>
        /// <param name="models"></param>
        /// <returns></returns>
        public int InsertSplitTableList<TTable>(List<TTable> models ,string configId="default") where TTable : RecordBase
        {
            try
            {
                var db = GetInstance(configId);
                //是否能生效
                foreach (var model in models)
                {
                    model.Id = SnowFlakeSingle.Instance.NextId();
                    model.CreateTime = DateTime.Now;
                }
                var ret = db.Insertable(models).SplitTable().ExecuteCommand();
                return ret;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo($"上传批量数据失败", e);
                return -1;
            }
        }

        /// <summary>
        /// ID在这里面定
        /// </summary>
        /// <typeparam name="TTable"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertSplitTableRow<TTable>(TTable model) where TTable : RecordBase, new()
        {
            try
            {
                var db = GetInstance();
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateTime = DateTime.Now;
                var ret = db.Insertable(model).SplitTable().ExecuteCommand();
                return ret;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo($"上传单条数据失败", e);
                return -1;
            }
        }

        public List<TTable> GetTableList<TTable>(int pageIndex, int pageSize, ref int totalCount, string index, Expression<Func<TTable, bool>>? express = null) where TTable : ParamBase
        {
            try
            {
                DateTime today = DateTime.Today;
                DateTime startTime = today;
                DateTime endTime = today.AddDays(1);
                var db = GetInstance();
                ISugarQueryable<TTable> queryable = db.Queryable<TTable>();
                if (!express.IsNullOrEmpty())
                {
                    //keyword表达式
                    queryable = queryable.Where(express);
                }
                //查询当日
                if (index == "1")
                {

                }
                //近7天
                else if (index == "2")
                {
                    today = DateTime.Today;
                    startTime = today.AddDays(-6);
                    endTime = today.AddDays(1);
                }
                //近1月
                else if (index == "3")
                {
                    today = DateTime.Today;
                    startTime = today.AddDays(-29);
                    endTime = today.AddDays(1);
                }
                //近3月
                else if (index == "4")
                {
                    today = DateTime.Today;
                    startTime = today.AddDays(-91);
                    endTime = today.AddDays(1);
                }
                queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                return queryable.OrderByDescending(it => it.Id).ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("查询记录失败", e);
                return new List<TTable>();
            }
        }

        //无日期查找，用于显示参数表格显示在网页
        public List<TTable> GetTableList<TTable>(int pageIndex, int pageSize, ref int totalCount, Expression<Func<TTable, bool>>? express = null) where TTable : ParamBase
        {
            try
            {
                var db = GetInstance();
                ISugarQueryable<TTable> queryable = db.Queryable<TTable>();
                return queryable.OrderByDescending(it => it.Id).ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("查询记录失败", e);
                return new List<TTable>();
            }
        }

        /// <summary>
        /// 无分表的表格插入多个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        /// <returns></returns>
        public int InsertTableList<T>(List<T> models,string configId) where T : ParamBase, new()
        {
            try
            {
                var db = GetInstance(configId);
                //是否能生效
                foreach (var model in models)
                {
                    model.Id = SnowFlakeSingle.Instance.NextId();
                    model.CreateTime = DateTime.Now;
                }
                var ret = db.Insertable(models).ExecuteCommand();
                return ret;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo($"上传失败", e);
                return -1;
            }
        }

        /// <summary>
        /// 无分表的表格插入单个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        /// <returns></returns>
        public int InsertTableRow<T>(T model,string configId) where T : ParamBase, new()
        {
            try
            {
                var db = GetInstance(configId);
                if(model.Id.IsNullOrEmpty() || model.Id==0)
                    model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateTime = DateTime.Now;
                var ret = db.Insertable(model).ExecuteCommand();
                return ret;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo($"上传单个数据失败", e);
                return -1;
            }
        }



        /// <summary>
        /// 无分表表格更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateTable<T>(T model,string configId) where T : ParamBase, new()
        {
            try
            {
                var db = GetInstance(configId);
                return db.Updateable(model).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("更新出错", e);
                return -1;
            }
        }

        public int UpdateTable<T>(List<T> model) where T : ParamBase, new()
        {
            try
            {
                var db = GetInstance();
                return db.Updateable(model).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("更新出错", e);
                return -1;
            }
        }

        /// <summary>
        /// 分表表格更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateSplitTable<T>(T model) where T : RecordBase, new()
        {
            try
            {
                var db = GetInstance();
                return db.Updateable(model).SplitTable(tabs => tabs.Take(3)).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("更新出错", e);
                return -1;
            }
        }

        /// <summary>
        /// 无分表表格删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public int DeleteTableRowByID<T>(string primaryKey,string configId) where T : ParamBase, new()
        {
            try
            {
                var db = GetInstance(configId);
                return db.Deleteable<T>(it => it.Id == long.Parse(primaryKey)).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("删除单行出错", e);
                return -1;
            }
        }

        /// <summary>
        /// 无分表表格删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public int DeleteSplitTableRowByID<T>(string primaryKey) where T : RecordBase, new()
        {
            try
            {
                var db = GetInstance();
                return db.Deleteable<T>(it => it.Id == long.Parse(primaryKey)).SplitTable(tabs => tabs.Take(3)).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("删除单行出错", e);
                return -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public T GetTableRowByID<T>(string primaryKey,string configId) where T : ParamBase, new()
        {
            try
            {
                var db = GetInstance(configId);
                return db.Queryable<T>().Where(it => it.Id == long.Parse(primaryKey)).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("删除单行出错", e);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public T GetSplitTableRowByID<T>(string primaryKey) where T : RecordBase, new()
        {
            try
            {
                var db = GetInstance();
                return db.Queryable<T>().SplitTable(tabs => tabs.Take(3)).Where(it => it.Id == long.Parse(primaryKey)).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("删除单行出错", e);
                return null;
            }
        }

        /// <summary>
        /// 获取ParamPLC表格数据
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public List<T> GetTableList<T>() where T : ParamBase, new()
        {
            try
            {
                var db = GetInstance();
                //业务逻辑强制走主库
                var paramlist = db.MasterQueryable<T>().OrderBy(it => it.Id, OrderByType.Desc).ToList();
                return paramlist;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("查询P出错", e);
                return null;
            }
        }

        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public List<T> GetTableList<T>(string configId)
        {
            try
            {
                var db = GetInstance(configId);
                //业务逻辑强制走主库
                var paramlist = db.MasterQueryable<T>().ToList();
                return paramlist;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("查询出错", e);
                return null;
            }
        }

        public List<T> GetSplitPageList<T>(int pageIndex, int pageSize, string configId, string startDate, string endDate,string conditions, ref int totalCount) where T:RecordBase
        {
            try
            {
                
                var db = GetInstance(configId);
                ISugarQueryable<T> queryable = db.Queryable<T>();

                if (startDate.IsNullOrEmpty())
                {
                    DateTime nowTime = DateTime.Now;
                    startDate = nowTime.AddDays(-30).ToString();
                }
                    
                if (endDate.IsNullOrEmpty())
                {
                    endDate = DateTime.Now.ToString();
                }
                    
                DateTime start = Convert.ToDateTime(startDate);
                DateTime end = Convert.ToDateTime(endDate);
                TimeSpan daysSpan = new TimeSpan(end.Ticks - start.Ticks);
                if (daysSpan.TotalDays > 90)
                    end = start.AddDays(-90);
                queryable = queryable.SplitTable(start, end);
                if (!conditions.IsNullOrEmpty())
                {
                    List<Condition> conditionList = JsonConvert.DeserializeObject<List<Condition>>(conditions);
                    queryable = BuildQuery(queryable, conditionList);
                }
                var ret = queryable.ToPageList(pageIndex, pageSize, ref totalCount);
                return ret;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<T>();
            }
        }

        public ISugarQueryable<T> BuildQuery<T>(ISugarQueryable<T> query, List<Condition> conditions)
        {
            if (conditions == null || !conditions.Any())
                return query;

            foreach (var condition in conditions)
            {
                query = ApplyCondition(query, condition);
            }

            return query;
        }

        public class Condition
        {
            public string Field { get; set; }    // 字段名
            public string Operator { get; set; } // 操作符：equals, contains, greater, less, like等
            public string Value { get; set; }    // 值
        }

        private ISugarQueryable<T> ApplyCondition<T>(ISugarQueryable<T> query, Condition condition)
        {
            if (string.IsNullOrEmpty(condition.Field) || condition.Value == null)
                return query;

            switch (condition.Operator?.ToLower())
            {
                case "equals": // 等于
                    query = query.Where($"{condition.Field} = @value", new { value = condition.Value });
                    break;

                case "greater": // 大于
                    query = query.Where($"{condition.Field} > @0", condition.Value);
                    break;

                case "less": // 小于
                    query = query.Where($"{condition.Field} < @0", condition.Value);
                    break;

                case "contains": // 模糊查询
                    query = query.Where($"{condition.Field} like @value", new { value = $"%{condition.Value}%" });
                    break;

                case "isnull": // 为空
                    query = query.Where($"{condition.Field} IS NULL");
                    break;

                case "isnotnull": // 不为空
                    query = query.Where($"{condition.Field} IS NOT NULL");
                    break;

                default:
                    // 默认使用等于
                    query = query.Where($"{condition.Field} = @0", condition.Value);
                    break;
            }

            //// 构建查询
            //var query = db.Queryable<User>()
            //    .Where(it => it.IsDeleted == false);
            //query = BuildQuery(query, conditions);
            //// 执行查询
            //var result = query.ToList();

            return query;
        }
    }
}
