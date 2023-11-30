using SqlSugar;
using System;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;
using FNMES.Entity.Record;
using System.Collections.Generic;
using FNMES.Utility.Core;
using System.Linq;
using FNMES.Entity.DTO.ApiParam;

namespace FNMES.WebUI.Logic.Record
{
    public class RecordEquipmentLogic : BaseLogic
    {
        //注意，分表数据需要加SplitTable()

        public int InsertStatus(RecordEquipmentStatus model,string configId,ref RecordEquipmentStop stopParam)
        {
            try
            {
                var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CallTime =  DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                model.StopFlag = "";
                model.CreateTime = DateTime.Now;
                int v = 0;
                Db.BeginTran();
                if (model.EquipmentStatus == "自动中")
                {
                    //查询上一次未记录的停机状态
                    RecordEquipmentStatus lastStop = db.Queryable<RecordEquipmentStatus>().Where(it => it.EquipmentStatus == "停机").
                    OrderBy(it => it.Id, OrderByType.Desc).SplitTable(tabs => tabs.Take(2)).First(); 
                    if (lastStop != null && !lastStop.HasRecordStop)
                    {
                        RecordEquipmentStop stop = new()
                        {
                            Id = SnowFlakeSingle.Instance.NextId(),
                            StationCode = lastStop.StationCode,
                            SmallStationCode = lastStop.SmallStationCode,
                            EquipmentID = lastStop.EquipmentID,
                            OperatorNo = "",
                            StopCode = lastStop.StopCode,
                            StopDesc = lastStop.StopDescription,
                            StopTime = lastStop.CallTime,
                            CreateTime = DateTime.Now,
                            StopDurationTime = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - long.Parse(lastStop.CallTime)).ToString(),
                        };
                        //传给引用，用于外部访问接口
                        stopParam = stop;
                        db.Insertable<RecordEquipmentStop>(stop).SplitTable().ExecuteCommand();
                        lastStop.StopFlag = "1";
                        db.Updateable<RecordEquipmentStatus>(lastStop).SplitTable().ExecuteCommand();
                    }
                }
                v = db.Insertable<RecordEquipmentStatus>(model).SplitTable().ExecuteCommand();
                Db.CommitTran();
                return v;
            }
            catch (Exception e)
            {
                Db.RollbackTran();
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }
        public List<RecordEquipmentStatus> GetStatusList(int pageIndex, int pageSize, string keyWord, string configId,ref int totalCount,string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordEquipmentStatus> queryable = db.Queryable<RecordEquipmentStatus>();
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.EquipmentID.Contains(keyWord) || it.StationCode.Contains(keyWord) );
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
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<RecordEquipmentStatus>();
            }
        }
        public List<RecordEquipmentError> GetErrorList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordEquipmentError> queryable = db.Queryable<RecordEquipmentError>();
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.EquipmentID.Contains(keyWord) || it.StationCode.Contains(keyWord)||it.AlarmCode.Contains(keyWord));
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
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<RecordEquipmentError>();
            }
        }
        public List<RecordEquipmentStop> GetStopList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordEquipmentStop> queryable = db.Queryable<RecordEquipmentStop>();
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.EquipmentID.Contains(keyWord) || it.StationCode.Contains(keyWord) || it.StopCode.Contains(keyWord));
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
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<RecordEquipmentStop>();
            }
        }
        public int InsertError(EquipmentErrorParam model, string configId)
        {
            try
            {
                List<RecordEquipmentError> errors = new();
                foreach (var item in model.alarmList)
                {
                    RecordEquipmentError buf = new();
                    buf.CopyField(item);
                    buf.StationCode = model.stationCode;
                    buf.SmallStationCode = model.smallStationCode;
                    buf.EquipmentID = model.equipmentID;
                    buf.Id = SnowFlakeSingle.instance.NextId();
                    buf.CreateTime = DateTime.Now;
                    errors.Add(buf);
                }
                var db = GetInstance(configId);
                return db.Insertable<RecordEquipmentError>(errors).SplitTable().ExecuteCommand();
                
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }
    }
}
