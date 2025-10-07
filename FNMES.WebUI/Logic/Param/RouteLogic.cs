using FNMES.Entity.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Utility.Operator;
using FNMES.Utility.Security;
using FNMES.Utility.Extension;
using FNMES.Utility.Core;
using FNMES.Utility.Other;
using System.Drawing.Printing;
using Microsoft.VisualBasic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Microsoft.AspNetCore.Routing;
using static System.Collections.Specialized.BitVector32;

namespace FNMES.WebUI.Logic.Param
{
    public class RouteLogic : BaseLogic
    {

        public List<ParamLocalRoute> GetList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, string productPartNo)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamLocalRoute> queryable = db.MasterQueryable<ParamLocalRoute>().Where(it => it.ProductPartNo == productPartNo);
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.StationCode.Contains(keyWord));
                }
                return queryable.OrderBy(it => it.Step).ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<ParamLocalRoute>();
            }
        }
        public ParamLocalRoute GetRouteByStation(string stationCode, string productPartNo, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamLocalRoute> queryable = db.MasterQueryable<ParamLocalRoute>().Where(it => it.ProductPartNo == productPartNo && it.StationCode == stationCode);
                var route = queryable.First();
                return route;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }

        public int Insert(ParamLocalRoute model, long operateId)
        {
            try
            {
                var db = GetInstance(model.ConfigId);
                if(model.Id.IsNullOrEmpty())
                    model.Id = SnowFlakeSingle.instance.NextId();
                model.CreateUserId = operateId;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                return db.Insertable<ParamLocalRoute>(model).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }

        public int Update(ParamLocalRoute model, long operateId)
        {
            try
            {
                var db = GetInstance(model.ConfigId);
                model.ModifyUserId = operateId;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<ParamLocalRoute>(model).IgnoreColumns(it => new
                {
                    it.CreateTime,
                    it.CreateUserId
                }).ExecuteCommand();
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return 0;
            }
        }

        public int Delete(long primaryKey, string configId)
        {
            var db = GetInstance(configId);
            try
            {
                Db.BeginTran();
                //删除权限与角色的对应关系。
                db.Deleteable<ParamLocalRoute>().Where((it) => primaryKey == it.Id).ExecuteCommand();
                Db.CommitTran();
                return 1;
            }
            catch (Exception E)
            {
                Db.RollbackTran();
                Logger.ErrorInfo(E.Message);
                return 0;
            }
        }

        public ParamLocalRoute Get(long primaryKey, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                ParamLocalRoute entity = db.MasterQueryable<ParamLocalRoute>().Where(it => it.Id == primaryKey).First();
                var sysDb = GetInstance();
                entity.CreateUser = sysDb.MasterQueryable<SysUser>().Where(it => it.Id == entity.CreateUserId).First();
                entity.ModifyUser = sysDb.MasterQueryable<SysUser>().Where(it => it.Id == entity.ModifyUserId).First();
                return entity;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new ParamLocalRoute { Id = 0, StationCode = "查无此项" };
            }
        }

        public ParamLocalRoute Get(string station, string productPartNo ,string configId)
        {
            try
            {
                var db = GetInstance(configId);
                ParamLocalRoute entity = db.MasterQueryable<ParamLocalRoute>().Where(it => it.StationCode == station && it.ProductPartNo == productPartNo).First();
                if (entity != null)
                {
                    if (!entity.Criterion.IsNullOrEmpty())
                    {
                        entity.CheckStations = entity.Criterion.Split(',').ToList();
                    }
                }
                return entity;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }

        public List<ParamLocalRoute> Get(string productPartNo, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return  db.MasterQueryable<ParamLocalRoute>().Where(it => it.ProductPartNo == productPartNo).OrderBy(it =>it.Step).ToList();
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<ParamLocalRoute>();
            }
        }

        public bool Align(List<ParamLocalRoute> list,string productPartNo, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                List<ParamRecipeItem> recipeItems = new List<ParamRecipeItem>();
                var recipId = GetTableList<ParamRecipe>(configId).Where(it => it.ProductPartNo == productPartNo).Select(e => e.Id).First();

                for (int i=0;i<list.Count;i++)
                {
                    ParamRecipeItem item = new ParamRecipeItem();
                    item.Id = list[i].Id;
                    item.StationCode = list[i].StationCode;
                    item.StationName = list[i].StationName;
                    item.CreateTime = DateTime.Now;
                    item.RecipeId = recipId;
                    recipeItems.Add(item);
                }

                foreach (var item in recipeItems)
                {
                    var existingItem = db.Queryable<ParamRecipeItem>()
                                          .First(x => x.StationCode == item.StationCode);

                    if (existingItem != null)
                    {
                        // 更新现有记录，忽略某些字段
                        db.Updateable(item)
                          .WhereColumns(w => w.StationCode)
                          .UpdateColumns(e => new { e.StationCode, e.StationName,e.RecipeId,e.CreateTime })
                          .ExecuteCommand();
                    }
                    else
                    {
                        db.Insertable(item).ExecuteCommand();
                    }
                }

                foreach (var item in list)
                {
                    var existingItem = db.Queryable<ParamLocalRoute>()
                                          .First(x => x.StationCode == item.StationCode);

                    if (existingItem != null)
                    {
                        // 更新现有记录，忽略某些字段
                        db.Updateable(item)
                          .WhereColumns(w => w.StationCode)
                          .UpdateColumns(e => new { e.StationCode, e.StationName, e.ProductPartNo, e.CreateTime })
                          .ExecuteCommand();
                    }
                    else
                    {
                        db.Insertable(item).ExecuteCommand();
                    }
                }

                ////要更新两份表
                //var result1 = db.Storageable(recipeItems)
                //        .WhereColumns(w => w.StationCode) // 根据 Id 判断是否存在
                //            .ExecuteCommand(); // 不存在则插入，存在则更新

                //var result = db.Storageable(list)
                //        .WhereColumns(w => w.StationCode) // 根据 Id 判断是否存在
                //            .ExecuteCommand(); // 不存在则插入，存在则更新
                return true;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return false;
            }
        }


    }
}
