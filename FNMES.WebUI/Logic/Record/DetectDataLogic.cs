using SqlSugar;
using System;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;
using FNMES.Entity.Record;
using System.Collections.Generic;
using FNMES.Utility.Core;
using System.Linq;
using FNMES.Entity.DTO;

namespace FNMES.WebUI.Logic.Record
{
    public class DetectDataLogic : BaseLogic
    {
        //注意，分表数据需要加SplitTable()
        //业务查询，必须走主库

        public long InsertACR(RecordTestACR model,string configId)
        {
            try
            {
                var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateTime = DateTime.Now;
                db.Insertable<RecordTestACR>(model).SplitTable().ExecuteCommand();
                return model.Id;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0L;
            }
        }
        
        public RecordTestACR GetACRByKey(long primaryKey, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.MasterQueryable<RecordTestACR>().Where(it => it.Id == primaryKey).SplitTable(tabs => tabs.Take(2)).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return null;
            }



        }

        public long InsertEOL(RecordTestEOL model, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateTime = DateTime.Now;
                db.Insertable<RecordTestEOL>(model).SplitTable().ExecuteCommand();
                return model.Id;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0L;
            }
        }

        public RecordTestEOL GetEOLByKey(long primaryKey, string configId)
        {
            try
            {
                
                var db = GetInstance(configId);
                return db.MasterQueryable<RecordTestEOL>().Where(it => it.Id == primaryKey).SplitTable(tabs => tabs.Take(2)).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return null;
            }



        }
        public long InsertOCV(RecordTestOCV model, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateTime = DateTime.Now;
                db.Insertable<RecordTestOCV>(model).SplitTable().ExecuteCommand();
                return model.Id;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0L;
            }
        }

        public RecordTestOCV GetOCVByKey(long primaryKey, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.MasterQueryable<RecordTestOCV>().Where(it => it.Id == primaryKey).SplitTable(tabs => tabs.Take(2)).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return null;
            }



        }

        public long InsertELEC(RecordTestElectric model, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateTime = DateTime.Now;
                db.Insertable<RecordTestElectric>(model).SplitTable().ExecuteCommand();
                return model.Id;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0L;
            }
        }

        public RecordTestElectric GetELECByKey(string productCode, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.MasterQueryable<RecordTestElectric>().Where(it => it.ProductCode == productCode).SplitTable(tabs => tabs.Take(2)).OrderByDescending(it=>it.Id).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return null;
            }



        }

        public List<RecordTestACR> GetACR(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordTestACR> queryable = db.Queryable<RecordTestACR>();
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.ProductCode.Contains(keyWord));
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
                return queryable.SplitTable(tabs => tabs.Take(3))
                    .MergeTable()
                    .OrderByDescending(it => it.Id)
                    .ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<RecordTestACR>();
            }
        }
        public List<RecordTestEOL> GetEOL(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordTestEOL> queryable = db.Queryable<RecordTestEOL>();
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.ProductCode.Contains(keyWord));
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
                return queryable.SplitTable(tabs => tabs.Take(3))
                    .MergeTable()
                    .OrderByDescending(it => it.Id)
                    .ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<RecordTestEOL>();
            }
        }
        public List<RecordTestOCV> GetOCV(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordTestOCV> queryable = db.Queryable<RecordTestOCV>();
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.ProductCode.Contains(keyWord));
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
                return queryable.SplitTable(tabs => tabs.Take(3))
                    .MergeTable()
                    .OrderByDescending(it => it.Id)
                    .ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<RecordTestOCV>();
            }
        }

        public List<RecordTestElectric> GetELEC(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordTestElectric> queryable = db.Queryable<RecordTestElectric>();
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.ProductCode.Contains(keyWord));
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
                return queryable.SplitTable(tabs => tabs.Take(3))
                    .MergeTable()
                    .OrderByDescending(it => it.Id)
                    .ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<RecordTestElectric>();
            }
        }

    }
}
