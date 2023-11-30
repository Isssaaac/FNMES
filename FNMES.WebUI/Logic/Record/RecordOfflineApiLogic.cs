using SqlSugar;
using System;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Record;
using System.Collections.Generic;
using FNMES.Utility.Core;
using System.Linq;

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
                return queryable.SplitTable(tabs => tabs.Take(2)).ToPageList(pageIndex, pageSize, ref totalCount);
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


    }
}
