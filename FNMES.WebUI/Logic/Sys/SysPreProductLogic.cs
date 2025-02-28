using FNMES.Entity.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Utility.Operator;
using FNMES.Utility.Extension;
using FNMES.Utility.Core;
using FNMES.Utility.Other;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.DTO.AppData;
using FNMES.Entity.Record;

namespace FNMES.WebUI.Logic.Sys
{
    public class SysPreProductLogic : BaseLogic
    {
        //用于页面显示   显示当前各工序对产品的选择情况
        public List<SysPreSelectProduct> GetList(int pageIndex, int pageSize, long lineId, string keyWord, ref int totalCount,string index)
        {
            try
            {
                //为修改后实时查询，走主库
                var db = GetInstance();
                ISugarQueryable<SysPreSelectProduct> queryable = db.MasterQueryable<SysPreSelectProduct>().Where(it => it.LineId == lineId);
                //当index==1时，就是只看当前
                if(index == "1")
                {
                    queryable = queryable.Where(it => it.EnableFlag == "1");
                }
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => (it.ProductPartNo.Contains(keyWord) || it.ProductDescription.Contains(keyWord)));
                }
                return queryable
                    .Includes(it => it.Line)
                    .ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<SysPreSelectProduct> ();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">要查询的对象模型</param>
        /// <param name="configId">当前选择机台的产品所属线体</param>
        /// <returns></returns>
        public int Insert(SysPreSelectProduct model, string configId)
        {
            try
            {
                var db = GetInstance();
                SysLine sysLine = db.MasterQueryable<SysLine>().Where(it => it.ConfigId == configId).First();
                if (sysLine == null)
                {
                    return 0;
                }
                Db.BeginTran();
                //将旧的记录标识未非启用状态，用做历史数据
                db.Updateable<SysPreSelectProduct>().SetColumns(it => it.EnableFlag == "0").
                    Where(it => (it.LineId == sysLine.Id) && (it.Station == model.Station)).ExecuteCommand();
                //再新增一条数据
                model.Id = SnowFlakeSingle.instance.NextId();
                model.LineId = sysLine.Id;
                model.CreateTime = DateTime.Now;
                model.IsEnabled = true;
                int v = db.Insertable<SysPreSelectProduct>(model).ExecuteCommand();
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

        public SysPreSelectProduct Query(string station,string configId)
        {
            try
            {
                var db = GetInstance();
                SysLine sysLine = db.MasterQueryable<SysLine>().Where(it => it.ConfigId == configId).First();
                if (sysLine == null)
                {
                    return null;
                }
                return db.MasterQueryable<SysPreSelectProduct>().Where(it => it.LineId == sysLine.Id
                    && it.Station == station && it.EnableFlag == "1").Includes(it => it.Line).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return null;
            }
        }
        
        //将自动铆接数据临时存储
        public int InsertRivet(RecordHotRivetData model, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.Insertable<RecordHotRivetData>(model).SplitTable().ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }

        public RecordHotRivetData QueryHotRivet(string productCode,string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.MasterQueryable<RecordHotRivetData>().Where(it => it.ProductCode == productCode).
                    SplitTable(tabs => tabs.Take(2)).OrderByDescending(it => it.Id).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return null;
            }
        }

        public List<HotRecord> GetHotList(int pageIndex, int pageSize, string lineId, string keyWord, ref int totalCount, string index)
        {
            try
            {
                var db = GetInstance(lineId);
                DateTime today = DateTime.Today;
                DateTime startTime = today;
                DateTime endTime = today.AddDays(1);
                if (index == "1")
                {
                    startTime = today;
                    endTime = today.AddDays(1);
                }
                else if (index == "2")
                {
                    startTime = today.AddDays(-6);
                    endTime = today.AddDays(1);
                }
                else if (index == "3")
                {
                    startTime = today.AddDays(-29);
                    endTime = today.AddDays(1);
                }
                else if (index == "4")
                {
                    startTime = today.AddDays(-91);
                    endTime = today.AddDays(1);
                }
                // &&(it.StationCode =="M305"|| it.StationCode =="M307")
                var lst_processup = db.Queryable<RecordProcessUpload>()
                 .Where(it => it.CreateTime >= startTime && it.CreateTime < endTime && (it.StationCode == "M305" || it.StationCode == "M307"))
                 .SplitTable(tabs => tabs.Take(3));
                
                var lst_process = db.Queryable<RecordProcessData>()
                    .Where(it => it.CreateTime >= startTime && it.CreateTime < endTime)
                    .SplitTable(tabs => tabs.Take(3));

                var processdata = lst_processup.LeftJoin(lst_process, (o, c) => o.Id == c.ProcessUploadId)
                .Select((o, c) => new HotRecord
                {
                    productCode = o.ProductCode,
                    stationCode = o.StationCode,
                    recipeNo = o.RecipeNo,
                    recipeDescription = o.RecipeDescription,
                    recipeVersion = o.RecipeVersion,
                    totalFlag = o.TotalFlag,
                    paramCode = c.ParamCode,
                    paramName = c.ParamName,
                    paramValue = c.ParamValue,
                    itemFlag = c.ItemFlag,
                    decisionType = c.DecisionType,
                    paramType = c.ParamType,
                    standValue = c.StandValue,
                    maxValue = c.MaxValue,
                    minValue = c.MinValue,
                    setValue = c.SetValue,
                    uom = c.UnitOfMeasure,
                    createTime = o.CreateTime.ToString("yyyy-MM-dd HH-mm-ss.fff")
                })
                .ToPageList(pageIndex, pageSize, ref totalCount);
                return processdata;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<HotRecord>();
            }
        }

        public List<HotAllRecord> GetHotAllList( string lineId, string index)
        {
            try
            {
                var db = GetInstance(lineId);
                DateTime today = DateTime.Today;
                DateTime startTime = today;
                DateTime endTime = today.AddDays(1);
                if (index == "1")
                {
                    startTime = today;
                    endTime = today.AddDays(1);
                }
                else if (index == "2")
                {
                    startTime = today.AddDays(-6);
                    endTime = today.AddDays(1);
                }
                else if (index == "3")
                {
                    startTime = today.AddDays(-29);
                    endTime = today.AddDays(1);
                }
                else if (index == "4")
                {
                    startTime = today.AddDays(-91);
                    endTime = today.AddDays(1);
                }
                // &&(it.StationCode =="M305"|| it.StationCode =="M307")
                var lst_processup = db.Queryable<RecordProcessUpload>()
                 .Where(it => it.CreateTime >= startTime && it.CreateTime < endTime && (it.StationCode == "M305" || it.StationCode == "M307"))
                 .SplitTable(tabs => tabs.Take(3));

                var lst_process = db.Queryable<RecordProcessData>()
                    .Where(it => it.CreateTime >= startTime && it.CreateTime < endTime)
                    .SplitTable(tabs => tabs.Take(3));

                var processdata = lst_processup.LeftJoin(lst_process, (o, c) => o.Id == c.ProcessUploadId)
                .Select((o, c) => new HotAllRecord
                {
                    operatorNo = o.OperatorNo,
                    productCode = o.ProductCode,
                    stationCode = o.StationCode,
                    smallStationCode = o.SmallStationCode,
                    equipmentID = o.EquipmentID,
                    recipeNo = o.RecipeNo,
                    recipeDescription = o.RecipeDescription,
                    recipeVersion = o.RecipeVersion,
                    totalFlag = o.TotalFlag,
                    paramCode = c.ParamCode,
                    paramName = c.ParamName,
                    paramValue = c.ParamValue,
                    itemFlag = c.ItemFlag,
                    decisionType = c.DecisionType,
                    paramType = c.ParamType,
                    standValue = c.StandValue,
                    maxValue = c.MaxValue,
                    minValue = c.MinValue,
                    setValue = c.SetValue,
                    uom = c.UnitOfMeasure,
                    createTime = o.CreateTime.ToString("yyyy-MM-dd HH-mm-ss.fff")
                })
                .ToList();
                return processdata;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<HotAllRecord>();
            }
        }

        public List<HotrevitPartRecord> GetHotPartList(int pageIndex, int pageSize, string lineId, string keyWord, ref int totalCount, string index)
        {
            try
            {
                var db = GetInstance(lineId);
                DateTime today = DateTime.Today;
                DateTime startTime = today;
                DateTime endTime = today.AddDays(1);
                if (index == "1")
                {
                    startTime = today;
                    endTime = today.AddDays(1);
                }
                else if (index == "2")
                {
                    startTime = today.AddDays(-6);
                    endTime = today.AddDays(1);
                }
                else if (index == "3")
                {
                    startTime = today.AddDays(-29);
                    endTime = today.AddDays(1);
                }
                else if (index == "4")
                {
                    startTime = today.AddDays(-91);
                    endTime = today.AddDays(1);
                }
                // &&(it.StationCode =="M305"|| it.StationCode =="M307")
                var lst_processup = db.Queryable<RecordPartUpload>()
                 .Where(it => it.CreateTime >= startTime && it.CreateTime < endTime && (it.StationCode == "M305" || it.StationCode == "M307"))
                 .SplitTable(tabs => tabs.Take(3));

                var lst_process = db.Queryable<RecordPartData>()
                    .Where(it => it.CreateTime >= startTime && it.CreateTime < endTime)
                    .SplitTable(tabs => tabs.Take(3));

                var processdata = lst_processup.LeftJoin(lst_process, (o, c) => o.Id == c.PartUploadId)
                .Select((o, c) => new HotrevitPartRecord
                {
                    operatorNo = o.OperatorNo,
                    productCode = o.ProductCode,
                    stationCode = o.StationCode,
                    smallStationCode = o.SmallStationCode,
                    equipmentID = o.EquipmentID,
                    partNumber = c.PartNumber,
                    partDescription = c.PartDescription,
                    partBarcode = c.PartBarcode,
                    traceType = c.TraceType,
                    usageQty = c.UsageQty,
                    partuom = c.Uom,
                    createTime = o.CreateTime.ToString("yyyy-MM-dd HH-mm-ss.fff")
                })
                .ToPageList(pageIndex, pageSize, ref totalCount);
                return processdata;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<HotrevitPartRecord>();
            }
        }

        public List<HotrevitPartRecord> GetHotPartAllList(string lineId, string index)
        {
            try
            {
                var db = GetInstance(lineId);
                DateTime today = DateTime.Today;
                DateTime startTime = today;
                DateTime endTime = today.AddDays(1);
                if (index == "1")
                {
                    startTime = today;
                    endTime = today.AddDays(1);
                }
                else if (index == "2")
                {
                    startTime = today.AddDays(-6);
                    endTime = today.AddDays(1);
                }
                else if (index == "3")
                {
                    startTime = today.AddDays(-29);
                    endTime = today.AddDays(1);
                }
                else if (index == "4")
                {
                    startTime = today.AddDays(-91);
                    endTime = today.AddDays(1);
                }
                // &&(it.StationCode =="M305"|| it.StationCode =="M307")
                var lst_processup = db.Queryable<RecordPartUpload>()
                 .Where(it => it.CreateTime >= startTime && it.CreateTime < endTime && (it.StationCode == "M305" || it.StationCode == "M307"))
                 .SplitTable(tabs => tabs.Take(3));

                var lst_process = db.Queryable<RecordPartData>()
                    .Where(it => it.CreateTime >= startTime && it.CreateTime < endTime)
                    .SplitTable(tabs => tabs.Take(3));

                var processdata = lst_processup.LeftJoin(lst_process, (o, c) => o.Id == c.PartUploadId)
                .Select((o, c) => new HotrevitPartRecord
                {
                    operatorNo = o.OperatorNo,
                    productCode = o.ProductCode,
                    stationCode = o.StationCode,
                    smallStationCode = o.SmallStationCode,
                    equipmentID = o.EquipmentID,
                    partNumber = c.PartNumber,
                    partDescription = c.PartDescription,
                    partBarcode = c.PartBarcode,
                    traceType = c.TraceType,
                    usageQty = c.UsageQty,
                    partuom = c.Uom,
                    createTime = o.CreateTime.ToString("yyyy-MM-dd HH-mm-ss.fff")
                })
                .ToList();
                return processdata;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<HotrevitPartRecord>();
            }
        }


    }

    public class HotRecord
    {
        public string productCode { get; set; }
        public string stationCode { get; set; }
        public string recipeNo { get; set; }
        public string recipeDescription { get; set; }
        public string recipeVersion { get; set; }
        public string totalFlag { get; set; }
        public string paramCode { get; set; }
        public string paramName { get; set; }
        public string paramValue { get; set; }
        public string itemFlag { get; set; }
        public string decisionType { get; set; }
        public string paramType { get; set; }
        public string standValue { get; set; }
        public string maxValue { get; set; }
        public string minValue { get; set; }
        public string setValue { get; set; }
        public string uom { get; set; }
        public string createTime { get; set; }
    }

    public class HotAllRecord
    {
        public string operatorNo { get; set; }
        public string productCode { get; set; }
        public string stationCode { get; set; }
        public string smallStationCode { get; set; }
        public string equipmentID { get; set; }
        public string recipeNo { get; set; }
        public string recipeDescription { get; set; }
        public string recipeVersion { get; set; }
        public string totalFlag { get; set; }
        public string paramCode { get; set; }
        public string paramName { get; set; }
        public string paramValue { get; set; }
        public string itemFlag { get; set; }
        public string decisionType { get; set; }
        public string paramType { get; set; }
        public string standValue { get; set; }
        public string maxValue { get; set; }
        public string minValue { get; set; }
        public string setValue { get; set; }
        public string uom { get; set; }
        public string createTime { get; set; }
    }

    public class HotrevitPartRecord
    {
        public string productCode { get; set; }
        public string stationCode { get; set; }
        public string smallStationCode { get; set; }
        public string equipmentID { get; set; }
        public string partNumber { get; set; }
        public string partDescription { get; set; }
        public string partBarcode { get; set; }
        public string traceType { get; set; }
        public string usageQty { get; set; }
        public string partuom { get; set; }

        public string operatorNo { get; set; }
        public string createTime { get; set; }
    }
}
