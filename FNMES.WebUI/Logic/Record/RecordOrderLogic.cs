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
                var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateTime = DateTime.Now;
                //插入这个RecordOrderStart这份表，这份表用来统计完成个数
                db.Insertable<RecordOrderStart>(model).SplitTable().ExecuteCommand();
                //统计RecordOrderStart有多少个产品
                return db.MasterQueryable<RecordOrderStart>().Where(it => it.TaskOrderNumber == model.TaskOrderNumber).SplitTable(tabs => tabs.Take(2)).Select(s => SqlFunc.AggregateDistinctCount(s.ProductCode)).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }
        public List<RecordOrderStart> GetStartList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount,  string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordOrderStart> queryable = db.Queryable<RecordOrderStart>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.TaskOrderNumber.Contains(keyWord));
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
                //按季度分表三个月取2张表
                return queryable.SplitTable(tabs => tabs.Take(2))
                    .MergeTable()
                    .OrderByDescending(it => it.Id)
                    .ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<RecordOrderStart>();
            }
        }

        public int InsertOutLine(RecordOrderPack model, string configId)
        {
            try
            {
                var db = GetInstance(configId);
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
        public List<RecordOrderPack> GetEndList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordOrderPack> queryable = db.Queryable<RecordOrderPack>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.TaskOrderNumber.Contains(keyWord));
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
                //按季度分表三个月取2张表
                return queryable.SplitTable(tabs => tabs.Take(2))
                    .MergeTable()
                    .OrderByDescending(it => it.Id)
                    .ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordOrderPack>();
            }
        }




    }
}
