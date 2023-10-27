using SqlSugar;
using System;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;
using FNMES.Entity.Record;
using System.Collections.Generic;
using FNMES.Utility.Core;
using System.Linq;

namespace FNMES.WebUI.Logic.Record
{
    public class RecordOutStationLogic : BaseLogic
    {
        //注意，分表数据需要加SplitTable()

        public int Insert(RecordOutStation model,string configId)
        {
            try
            {
                var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateTime = DateTime.Now;
                return db.Insertable<RecordOutStation>(model).SplitTable().ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }
        public List<RecordOutStation> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordOutStation> queryable = db.Queryable<RecordOutStation>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.TaskOrderNumber.Contains(keyWord) || it.ProductCode.Contains(keyWord));
                }
                return queryable.SplitTable(tabs => tabs.Take(2)).ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordOutStation>();
            }
        }

        public List<RecordOutStation> GetList(string productCode, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.Queryable<RecordOutStation>().Where(it => it.ProductCode == productCode).SplitTable(tabs => tabs.Take(2)).ToList();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordOutStation>();
            }
        }






    }
}
