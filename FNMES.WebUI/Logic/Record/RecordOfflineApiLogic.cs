using SqlSugar;
using System;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Record;
using System.Collections.Generic;
using FNMES.Utility.Core;
using System.Linq;
using FNMES.WebUI.API;
using FNMES.Utility.Network;
using System.Diagnostics;
using System.Security.Policy;
using System.Threading.Tasks;
using FNMES.Entity.Param;
using FNMES.WebUI.Logic.Param;

namespace FNMES.WebUI.Logic.Record
{
    public class RecordOfflineApiLogic : BaseLogic
    {
        //注意，分表数据需要加SplitTable()

        public int Insert(RecordOfflineApi model,string configId)
        {
            try
            {
                var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateTime = DateTime.Now;
                return db.Insertable<RecordOfflineApi>(model).SplitTable().ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }
        public List<RecordOfflineApi> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount, string configId, string index, string index1)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordOfflineApi> queryable = db.Queryable<RecordOfflineApi>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.Url.Contains(keyWord));
                }
                //查询当日
                if (index == "1")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today;
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                //近7天
                else if (index == "2")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-6);
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                //近1月
                else if (index == "3")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-29);
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                //近3月
                else if (index == "4")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-91);
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                if(index1 == "1") {
                    queryable = queryable.Where(it => it.ReUpload == 0);
                }

                //按季分表三个月取2张表
                //2024.5.9增加倒序排序
                return queryable.SplitTable(tabs => tabs.Take(2))
                    .MergeTable()
                    .OrderByDescending(it=>it.Id)
                    .ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordOfflineApi>();
            }
        }

        public List<RecordOfflineApi> GetUnload(string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.Queryable<RecordOfflineApi>().Where(it => it.ReUpload == 0).SplitTable(tabs => tabs.Take(2)).ToList();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordOfflineApi>();
            }
        }
         
        public int Update(List<RecordOfflineApi> models,string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.Updateable<RecordOfflineApi>(models).SplitTable().ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }

        public int Update(RecordOfflineApi model, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.Updateable<RecordOfflineApi>(model).SplitTable().ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }

        #region 20240414添加
        public int Upload(RecordOfflineApi model, string configId)
        {
            FactoryStatus factoryStatus = GetStatus(configId);
            if (factoryStatus.isOnline)
            {
                var result = APIMethod.Call(model.Url, model.RequestBody, configId).ToObject<RetMessage<object>>();
                if (result != null && result.messageType == "S")
                {
                    return this.Delete(model.Id, configId);
                }
            }
                return 0;
        }

        private FactoryStatus GetStatus(string configId)
        {
            FactoryStatusLogic factoryLogic = new FactoryStatusLogic();
            FactoryStatus status = factoryLogic.Get(configId);
            if (status == null)
            {
                status = new FactoryStatus()
                {
                    Id = SnowFlakeSingle.instance.NextId(),
                    Status = 0,
                    CreateTime = DateTime.Now,
                    Retry = 0,
                    ConfigId = configId

                };
                factoryLogic.Insert(status);
            }
            status.ConfigId = configId;

            return status;
        }

        public int UploadAll( List<RecordOfflineApi> models, string configId)
        {
            FactoryStatus factoryStatus = GetStatus(configId);
            var noUploads = models.Where(e=>e.ReUpload == 0).OrderBy(e=>e.CreateTime).ToList();
            int v = 0;
            foreach (var model in noUploads)
            {
                if (factoryStatus.isOnline)
                {
                    var result = APIMethod.Call(model.Url, model.RequestBody, configId).ToObject<RetMessage<object>>();
                    if (result != null && result.messageType == "S")
                    {
                        v += this.Delete(model.Id, configId);
                    }
                }
               
            }
            return v;
            
        }

        public int Delete(long primaryKey, string configId)
        {
            var db = GetInstance(configId);
            try
            {
                Db.BeginTran();
                db.Deleteable<RecordOfflineApi>().Where((it) => primaryKey == it.Id).SplitTable(tabs => tabs.Take(2)).ExecuteCommand();
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
        #endregion
    }
}
