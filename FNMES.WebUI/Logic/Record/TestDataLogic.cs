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
    public class TestDataLogic : BaseLogic
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

        public long InsertGasTightness1(RecordGasTightness1 model, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateTime = DateTime.Now;
                db.Insertable<RecordGasTightness1>(model).SplitTable().ExecuteCommand();
                return model.Id;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0L;
            }
        }

        public RecordGasTightness1 GetGas1ByKey(long primaryKey, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.MasterQueryable<RecordGasTightness1>().Where(it => it.Id == primaryKey).SplitTable(tabs => tabs.Take(2)).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return null;
            }



        }

        public long InsertGasTightness2(RecordGasTightness2 model, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateTime = DateTime.Now;
                db.Insertable<RecordGasTightness2>(model).SplitTable().ExecuteCommand();
                return model.Id;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0L;
            }
        }

        public RecordGasTightness2 GetGas2ByKey(long primaryKey, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.MasterQueryable<RecordGasTightness2>().Where(it => it.Id == primaryKey).SplitTable(tabs => tabs.Take(2)).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return null;
            }



        }

    }
}
