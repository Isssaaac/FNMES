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
                            BigStationCode = lastStop.BigStationCode,
                            EquipmentID = lastStop.EquipmentID,
                            OperatorNo = "",
                            StopCode = lastStop.StatusCode,
                            StopDesc = lastStop.StatusDescription,
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
        public List<RecordEquipmentStatus> GetStatusList(int pageIndex, int pageSize, string keyWord, ref int totalCount,string configId)
        {
            var db = GetInstance(configId);
            ISugarQueryable<RecordEquipmentStatus> queryable = db.Queryable<RecordEquipmentStatus>();

            if (!keyWord.IsNullOrEmpty())
            {
                queryable = queryable.Where(it => it.EquipmentID.Contains(keyWord) || it.BigStationCode.Contains(keyWord));
            }
            return queryable.SplitTable(tabs => tabs.Take(2)).ToPageList(pageIndex, pageSize, ref totalCount);
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
                    buf.BigStationCode = model.bigStationCode;
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
