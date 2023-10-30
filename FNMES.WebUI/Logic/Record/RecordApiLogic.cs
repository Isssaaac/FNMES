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
    public class RecordApiLogic : BaseLogic
    {
        //注意，分表数据需要加SplitTable()

        public int Insert(RecordApi model,string configId)
        {
            try
            {
                var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateTime = DateTime.Now;
                return db.Insertable<RecordApi>(model).SplitTable().ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }
        public List<RecordApi> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount,string configId)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordApi> queryable = db.Queryable<RecordApi>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.Url.Contains(keyWord));
                }
                return queryable.SplitTable(tabs => tabs.Take(2)).ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E )
            {
                Logger.ErrorInfo(E.Message);
                return new List<RecordApi>();
            }
        }






    }
}
