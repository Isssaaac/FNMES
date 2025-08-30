using SqlSugar;
using System;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;
using FNMES.Entity.Record;
using System.Collections.Generic;
using FNMES.Utility.Core;
using System.Linq;
using FNMES.Entity.DTO.ApiParam;
using Org.BouncyCastle.Asn1.Ess;
using FNMES.Entity.DTO.ApiData;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace FNMES.WebUI.Logic.Record
{
    public class RecordSelfDischargeLogic:BaseLogic
    {
        public bool Insert(selfDischargeParamIn model, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                List<RecordSelfDischarge> param_list = new List<RecordSelfDischarge>();
                foreach (var module in model.moduleSelfDischarges)
                {
                    foreach (var cell in module.cellSelfDischarges)
                    {
                        RecordSelfDischarge param = new RecordSelfDischarge();
                        param.Id = SnowFlakeSingle.instance.NextId();
                        param.productCode = model.productCode;
                        param.maxVoltageDrop = module.maxVoltageDrop;
                        param.minVoltageDrop = module.minVoltageDrop;
                        param.averageVoltageDrop = module.averageVoltageDrop;
                        param.stdDeviationVoltageDrop = module.stdDeviationVoltageDrop;
                        param.judgment1Lo = module.judgment1Lo;
                        param.judgment1Up = module.judgment1Up;
                        param.judgment1LoResult = module.judgment1LoResult;
                        param.judgment1UpResult = module.judgment1UpResult;
                        param.judgment1Result = module.judgment1Result;
                        param.judgment2Result = module.judgment2Result;
                        param.result = module.result;
                        param.createTime = DateTime.Now;
                        
                        param.cellCode = cell.cellCode;
                        param.voltageDrop = cell.voltageDrop;
                        param.a020TestTime = cell.a020TestTime;
                        param.a020TestVoltage = cell.a020TestVoltage;
                        param.m350TestTime = cell.m350TestTime;
                        param.m350TestVoltage = cell.m350TestVoltage;
                        param.timeInterval = cell.timeInterval;
                        param.intervalVoltageDrop = cell.intervalVoltageDrop;

                        param_list.Add(param);
                    }
                }
                var ret = db.Insertable<RecordSelfDischarge>(param_list).SplitTable().ExecuteCommand();
                return ret > 0;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("插入自放电数据失败",e);
                return false;
            }
        }


        public List<RecordSelfDischarge> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount, string configId, string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordSelfDischarge> queryable = db.Queryable<RecordSelfDischarge>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.productCode.Contains(keyWord) || it.cellCode.Contains(keyWord));
                }
                //查询当日
                if (index == "1")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today;
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.createTime >= startTime && it.createTime < endTime);
                }
                //近7天
                else if (index == "2")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-6);
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.createTime >= startTime && it.createTime < endTime);
                }
                //近1月
                else if (index == "3")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-29);
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.createTime >= startTime && it.createTime < endTime);
                }
                //近3月
                else if (index == "4")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-91);
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.createTime >= startTime && it.createTime < endTime);
                }
                return queryable.SplitTable(tabs => tabs.Take(3)).OrderByDescending(it => it.Id)
                   .ToPageList(pageIndex, pageSize, ref totalCount); ;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordSelfDischarge>();
            }
        }

        public List<RecordSelfDischarge> GetList(int pageIndex, int pageSize, string configId, string startDate, string endDate, string keyword, ref int totalCount)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordSelfDischarge> queryable = db.Queryable<RecordSelfDischarge>();
                //如果工单存在，那就查工单，如果内控码存在，就查内控码，全部要基于时间内，时间间隔最多三个月
                if (!keyword.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.productCode.Contains(keyword) || it.cellCode.Contains(keyword));
                }

                DateTime start = Convert.ToDateTime(startDate);
                DateTime end = Convert.ToDateTime(endDate);
                queryable = queryable.Where(it => it.createTime >= start && it.createTime < end);

                TimeSpan daysSpan = new TimeSpan(end.Ticks - start.Ticks);
                if (daysSpan.TotalDays > 90)
                    return new List<RecordSelfDischarge>();

                return queryable.SplitTable(start, end)
                   .OrderByDescending(it => it.Id)
                   .ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordSelfDischarge>();
            }
        }


        public List<RecordSelfDischarge> GetAllRecord(string configId, string startDate, string endDate, string keyword)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordSelfDischarge> queryable = db.Queryable<RecordSelfDischarge>();
                //如果工单存在，那就查工单，如果内控码存在，就查内控码，全部要基于时间内，时间间隔最多三个月
                if (!keyword.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.productCode.Contains(keyword) || it.cellCode.Contains(keyword));
                }

                DateTime start = Convert.ToDateTime(startDate);
                DateTime end = Convert.ToDateTime(endDate);
                queryable = queryable.Where(it => it.createTime >= start && it.createTime < end);

                TimeSpan daysSpan = new TimeSpan(end.Ticks - start.Ticks);
                if (daysSpan.TotalDays > 90)
                    return new List<RecordSelfDischarge>();

                return queryable.SplitTable(start, end)
                   .OrderByDescending(it => it.Id)
                   .ToList();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordSelfDischarge>();
            }
        }
    }
}
