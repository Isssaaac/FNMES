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

namespace FNMES.WebUI.Logic.Record
{
    public class RecordToolRemainLogic : BaseLogic
    {
        //注意，分表数据需要加SplitTable()

        public int Insert(ToolRemainParam model, string configId)
        {
            try
            {
                RecordToolRemain toolRemain = new();
                toolRemain.CopyField(model);
                toolRemain.Id = SnowFlakeSingle.instance.NextId();
                toolRemain.CreateTime = DateTime.Now;
                List<RecordToolData> partList = new();
                foreach (var item in model.toolList)
                {
                    RecordToolData buf = new RecordToolData();
                    buf.CopyField(item);
                    buf.Id = SnowFlakeSingle.instance.NextId();
                    buf.ToolRemainId = toolRemain.Id;
                    buf.CreateTime = DateTime.Now;
                    partList.Add(buf);
                }
                Db.BeginTran();
                var db = GetInstance(configId);
                db.Insertable(toolRemain).SplitTable().ExecuteCommand();
                db.Insertable(partList).SplitTable().ExecuteCommand();
                Db.CommitTran();
                return 1;
            }
            catch (Exception e)
            {
                Db.RollbackTran();
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }
        public List<ToolDataList> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount,string configId, string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordToolRemain> queryable = db.Queryable<RecordToolRemain>();
                ISugarQueryable<RecordToolData> queryable1 = db.Queryable<RecordToolData>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.StationCode.Contains(keyWord) || it.ProductCode.Contains(keyWord));
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


                var toolremain = queryable.SplitTable(tabs => tabs.Take(2));

                var tooldata = queryable1.SplitTable(tabs => tabs.Take(2));

                var lst = toolremain.LeftJoin(tooldata, (o, c) => o.Id == c.ToolRemainId) 
                    .Select((o, c) => new ToolDataList
                    {
                        Id = o.Id,
                        ProductCode = o.ProductCode,
                        CreateTime = o.CreateTime,
                        EquipmentID = o.EquipmentID,
                        OperatorNo = o.OperatorNo,
                        StationCode = o.StationCode,
                        ToolName = c.ToolName,
                        ToolNo = c.ToolNo,
                        ToolRemainValue = c.ToolRemainValue,
                        Uom = c.Uom
                    })
                    .MergeTable()
                    .OrderByDescending(e=>e.Id)
                     .ToPageList(pageIndex, pageSize, ref totalCount);

                return lst;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<ToolDataList>();
            }
        }

        public List<ToolDataList> GetList(string keyWord, string configId, string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordToolRemain> queryable = db.Queryable<RecordToolRemain>();
                ISugarQueryable<RecordToolData> queryable1 = db.Queryable<RecordToolData>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.StationCode.Contains(keyWord) || it.ProductCode.Contains(keyWord));
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


                var toolremain = queryable.SplitTable(tabs => tabs.Take(2));

                var tooldata = queryable1.SplitTable(tabs => tabs.Take(2));

                var lst = toolremain.LeftJoin(tooldata, (o, c) => o.Id == c.ToolRemainId)
                    .Select((o, c) => new ToolDataList
                    {
                        Id = o.Id,
                        ProductCode = o.ProductCode,
                        CreateTime = o.CreateTime,
                        EquipmentID = o.EquipmentID,
                        OperatorNo = o.OperatorNo,
                        StationCode = o.StationCode,
                        ToolName = c.ToolName,
                        ToolNo = c.ToolNo,
                        ToolRemainValue = c.ToolRemainValue,
                        Uom = c.Uom
                    })
                    .MergeTable()
                    .OrderByDescending(e => e.Id)
                     .ToList();

                return lst;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<ToolDataList>();
            }
        }

    }

    public class ToolDataList
    {
        public long Id { get; set; }
        public string ProductCode { get; set; }
        public string StationCode { get; set; }
        public string EquipmentID { get; set; }
        public string OperatorNo { get; set; }
        public DateTime CreateTime { get; set; }
        public string ToolNo { get; set; }
        public string ToolName { get; set; }
        public string ToolRemainValue { get; set; }
        public string Uom { get; set; }
    }
}
