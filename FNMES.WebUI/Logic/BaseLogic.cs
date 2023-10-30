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
using CCS.WebUI;
using FNMES.Utility;
using Microsoft.Extensions.Hosting;
using System.Xml.Linq;
using FNMES.Entity.Enum;
using OfficeOpenXml.Drawing.Slicer.Style;
using System.Data.Entity.ModelConfiguration.Configuration;

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
    }
}
