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
        public List<RecordOutStation> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount, string configId,string index)
        {
            try
            {
                var db = GetInstance(configId);
                //db.CodeFirst.SplitTables().InitTables<RecordOutStation>();
                //db.CodeFirst.SplitTables().InitTables<RecordPartData>();
                //db.CodeFirst.SplitTables().InitTables<RecordPartUpload>();
                //db.CodeFirst.SplitTables().InitTables<RecordProcessData>();
                //db.CodeFirst.SplitTables().InitTables<RecordProcessUpload>();
                ISugarQueryable<RecordOutStation> queryable = db.Queryable<RecordOutStation>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.TaskOrderNumber.Contains(keyWord) || it.ProductCode.Contains(keyWord));
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
                //按月分表三个月取3张表
                return queryable.SplitTable(tabs => tabs.Take(3)).ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordOutStation>();
            }
        }

        public List<RecordOutStation> GetList(string productCode, string configId)
        {
            //业务逻辑，必须走主库
            try
            {
                var db = GetInstance(configId);
                return db.MasterQueryable<RecordOutStation>().Where(it => it.ProductCode == productCode).SplitTable(tabs => tabs.Take(2)).ToList();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordOutStation>();
            }
        }

        public bool processExist(string productCode, string stationCode, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.Queryable<RecordProcessUpload>().Where(it => it.ProductCode == productCode && it.StationCode == stationCode).SplitTable(tabs => tabs.Take(4)).Any();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return false;
            }
        }

        public bool partExist(string productCode, string stationCode, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.Queryable<RecordPartUpload>().Where(it => it.ProductCode == productCode && it.StationCode == stationCode).SplitTable(tabs => tabs.Take(4)).Any();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return false;
            }
        }
    }
}
