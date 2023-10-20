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
    public class RecordOrderLogic : BaseLogic
    {
        //注意，分表数据需要加SplitTable()


        //返回当前订单的内控码个数
        public int InsertInLine(RecordOrderStart model,string configId)
        {
            try
            {
                using var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateTime = DateTime.Now;
                db.Insertable<RecordOrderStart>(model).SplitTable().ExecuteCommand();
                return db.MasterQueryable<RecordOrderStart>().Where(it => it.TaskOrderNumber == model.TaskOrderNumber).SplitTable(tabs => tabs.Take(3)).Select(s => SqlFunc.AggregateDistinctCount(s.ProductCode)).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }
        public List<RecordOrderStart> GetStartList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            using var db = GetInstance();
            ISugarQueryable<RecordOrderStart> queryable = db.Queryable<RecordOrderStart>();

            if (!keyWord.IsNullOrEmpty())
            {
                queryable = queryable.Where(it => it.TaskOrderNumber.Contains(keyWord));
            }
            return queryable.SplitTable(tabs => tabs.Take(2)).ToPageList(pageIndex, pageSize, ref totalCount);
        }

        public int InsertOutLine(RecordOrderPack model, string configId)
        {
            try
            {
                using var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateTime = DateTime.Now;
                return db.Insertable<RecordOrderPack>(model).SplitTable().ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }
        public List<RecordOrderPack> GetEndList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            using var db = GetInstance();
            ISugarQueryable<RecordOrderPack> queryable = db.Queryable<RecordOrderPack>();

            if (!keyWord.IsNullOrEmpty())
            {
                queryable = queryable.Where(it => it.TaskOrderNumber.Contains(keyWord));
            }
            return queryable.SplitTable(tabs => tabs.Take(2)).ToPageList(pageIndex, pageSize, ref totalCount);
        }




    }
}
