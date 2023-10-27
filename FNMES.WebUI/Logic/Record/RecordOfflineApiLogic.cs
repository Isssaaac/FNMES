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
        public List<RecordOfflineApi> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordOfflineApi> queryable = db.Queryable<RecordOfflineApi>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.Url.Contains(keyWord));
                }
                return queryable.SplitTable(tabs => tabs.Take(2)).ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return null;
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
                return null;
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
