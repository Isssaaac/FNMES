using SqlSugar;
using System;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Record;
using System.Collections.Generic;
using FNMES.Utility.Core;
using System.Linq;
using ServiceStack;
using OfficeOpenXml;
using System.Data;
using FNMES.Entity.DTO.AppData;

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
                ISugarQueryable<RecordOutStation> queryable = db.Queryable<RecordOutStation>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.TaskOrderNumber.Contains(keyWord) || it.ProductCode.Contains(keyWord) || it.ProductStatus.Contains(keyWord) || it.StationCode.Contains(keyWord));
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
                //2024.5.9修改去重后分页
                return queryable.SplitTable(tabs => tabs.Take(3))
                   .Select(it => new
                   {
                       index = SqlFunc.RowNumber($"{it.Id} desc",$"{it.ProductCode}, {it.StationCode}"),
                       it.Id,
                       it.ProductCode,
                       it.ProductStatus,
                       it.StationCode,
                       it.SmallStationCode,
                       it.TaskOrderNumber,
                       it.OperatorNo,
                       it.EquipmentID,
                       it.DefectCode,
                       it.DefectDesc,
                       it.CreateTime,
                       it.InstationTime,
                       it.PalletNo
                   })
                   .MergeTable()
                   .Where(it => it.index == 1)
                   .Select(it => new RecordOutStation()
                   {
                       Id = it.Id,
                       ProductCode = it.ProductCode,
                       ProductStatus = it.ProductStatus,
                       StationCode = it.StationCode,
                       SmallStationCode = it.SmallStationCode,
                       TaskOrderNumber = it.TaskOrderNumber,
                       OperatorNo = it.OperatorNo,
                       EquipmentID = it.EquipmentID,
                       DefectCode = it.DefectCode,
                       DefectDesc = it.DefectDesc,
                       CreateTime = it.CreateTime,
                       InstationTime = it.InstationTime,
                       PalletNo = it.PalletNo
                   })
                   .OrderByDescending(it=>it.Id)
                   .ToPageList(pageIndex, pageSize, ref totalCount);
                // return queryable.SplitTable(tabs => tabs.Take(3)).ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordOutStation>();
            }
        }

        public List<RecordOutStation> GetList(int pageIndex, int pageSize,string configId ,string startDate, string endDate, string productCode,string order, ref int totalCount)
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
                //如果工单存在，那就查工单，如果内控码存在，就查内控码，全部要基于时间内，时间间隔最多三个月
                if (!productCode.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.ProductCode.Contains(productCode) || it.StationCode.Contains(productCode));
                }

                if (!order.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.TaskOrderNumber.Contains(order));
                }

                DateTime start = Convert.ToDateTime(startDate);
                DateTime end = Convert.ToDateTime(endDate);
                queryable = queryable.Where(it => it.CreateTime >= start && it.CreateTime < end);

                TimeSpan daysSpan = new TimeSpan(end.Ticks - start.Ticks);
                if(daysSpan.TotalDays>90)
                    return new List<RecordOutStation>();

                //按月分表三个月取3张表
                //2024.5.9修改去重后分页
                return queryable.SplitTable(start,end)
                   .Select(it => new
                   {
                       index = SqlFunc.RowNumber($"{it.Id} desc", $"{it.ProductCode}, {it.StationCode}"),
                       it.Id,
                       it.ProductCode,
                       it.ProductStatus,
                       it.StationCode,
                       it.SmallStationCode,
                       it.TaskOrderNumber,
                       it.OperatorNo,
                       it.EquipmentID,
                       it.DefectCode,
                       it.DefectDesc,
                       it.CreateTime,
                       it.InstationTime,
                       it.PalletNo
                   })
                   .MergeTable()
                   .Where(it => it.index == 1)
                   .Select(it => new RecordOutStation()
                   {
                       Id = it.Id,
                       ProductCode = it.ProductCode,
                       ProductStatus = it.ProductStatus,
                       StationCode = it.StationCode,
                       SmallStationCode = it.SmallStationCode,
                       TaskOrderNumber = it.TaskOrderNumber,
                       OperatorNo = it.OperatorNo,
                       EquipmentID = it.EquipmentID,
                       DefectCode = it.DefectCode,
                       DefectDesc = it.DefectDesc,
                       CreateTime = it.CreateTime,
                       InstationTime = it.InstationTime,
                       PalletNo = it.PalletNo
                   })
                   .OrderByDescending(it => it.Id)
                   .ToPageList(pageIndex, pageSize, ref totalCount);
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
        //这个是过站记录→物料信息查的RecordPartUpload表
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

        //过站记录→物料信息调用的查询，先查RecordPartUpload表，然后用id来查RecordePart表
        public List<RecordPartData> GetPartData(int pageIndex, int pageSize, ref int totalCount, string configId, string productCode, string stationCode)
        {
            try
            {
                var db = GetInstance(configId);

                RecordPartUpload recordPartUpload = db.Queryable<RecordPartUpload>().Where(it => it.ProductCode == productCode && it.StationCode == stationCode)
                    .SplitTable(tabs => tabs.Take(4)).OrderByDescending(it => it.Id).First();
                //250514修改，原本查不到4个月之前的物料数据
                DateTime start = recordPartUpload.CreateTime.AddMonths(-1);
                DateTime end = recordPartUpload.CreateTime.AddMonths(6);

                if (recordPartUpload != null) { 
                    return db.Queryable<RecordPartData>().Where(it => it.PartUploadId == recordPartUpload.Id)
                        .SplitTable(start,end).ToPageList(pageIndex, pageSize, ref totalCount);
                }
                else
                {
                    return new List<RecordPartData>();
                }
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordPartData>();
            }
        }

        public List<RecordProcessData> GetProcessData(int pageIndex,int pageSize, string keyWord, ref int totalCount, string configId, string productCode, string stationCode)
        {
            try
            {
                var db = GetInstance(configId);
                RecordProcessUpload record = db.Queryable<RecordProcessUpload>().Where(it => it.ProductCode == productCode && it.StationCode == stationCode)
                    .SplitTable(tabs => tabs.Take(4)).OrderByDescending(it => it.Id).First();

                DateTime start = record.CreateTime.AddMonths(-1);
                DateTime end = record.CreateTime.AddMonths(6);

                if (record != null)
                {
                    if (!keyWord.IsNullOrEmpty())
                    {
                        return db.Queryable<RecordProcessData>().Where(it => it.ProcessUploadId == record.Id && (it.ParamCode.Contains(keyWord) || it.ItemFlag.Contains(keyWord)))
                        .SplitTable(start, end).ToPageList(pageIndex, pageSize, ref totalCount);
                    }
                    return db.Queryable<RecordProcessData>().Where(it => it.ProcessUploadId == record.Id)
                        .SplitTable(start, end).ToPageList(pageIndex, pageSize, ref totalCount);
                }
                else
                {
                    return new List<RecordProcessData>();
                }
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordProcessData>();
            }
        }

        

        //精准导出
        public List<OutRecord> GetAllRecord(string configId, string startDate, string endDate, string productCode, string order,
            ref List<RecordOutStation> outStationData, ref List<ProcRecord> procRecordData, ref List<ParRecord> partRecordData)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordOutStation> queryable = db.Queryable<RecordOutStation>();
                if (!productCode.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.ProductCode.Contains(productCode) || it.StationCode.Contains(productCode));
                }
                if (!order.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.TaskOrderNumber.Contains(order));
                }

                DateTime start = Convert.ToDateTime(startDate);
                DateTime end = Convert.ToDateTime(endDate);
                queryable = queryable.Where(it => it.CreateTime >= start && it.CreateTime < end);

                //查的是过站表
                var lst_outstation = queryable
                    .Where(it => it.CreateTime >= start && it.CreateTime < end)
                    .SplitTable(start, end)
                    .Select(it => new
                    {
                        index = SqlFunc.RowNumber($"{it.Id} desc", $"{it.ProductCode}, {it.StationCode}"),
                        it.Id,
                        it.ProductCode,
                        it.ProductStatus,
                        it.StationCode,
                        it.SmallStationCode,
                        it.TaskOrderNumber,
                        it.OperatorNo,
                        it.EquipmentID,
                        it.DefectCode,
                        it.DefectDesc,
                        it.CreateTime,
                        it.InstationTime,
                        it.PalletNo
                    })
                       .MergeTable()
                       .Where(it => it.index == 1)
                       .Select(it => new RecordOutStation()
                       {
                           Id = it.Id,
                           ProductCode = it.ProductCode,
                           ProductStatus = it.ProductStatus,
                           StationCode = it.StationCode,
                           SmallStationCode = it.SmallStationCode,
                           TaskOrderNumber = it.TaskOrderNumber,
                           OperatorNo = it.OperatorNo,
                           EquipmentID = it.EquipmentID,
                           DefectCode = it.DefectCode,
                           DefectDesc = it.DefectDesc,
                           CreateTime = it.CreateTime,
                           InstationTime = it.InstationTime,
                           PalletNo = it.PalletNo
                       })
                       .ToList();

                var productCodes = lst_outstation.Select(it => it.ProductCode).Distinct().ToList(); ;

                outStationData = lst_outstation;
                Logger.RunningInfo($"过站记录导出,outStationData数据量:{outStationData.Count}");
                /**********************工艺参数**********************/
                var lst_processup = db.Queryable<RecordProcessUpload>()
                     
                     .Where(it => productCodes.Contains(it.ProductCode))
                     .SplitTable(start,end)
                     .Select(it => new
                     {
                         index = SqlFunc.RowNumber($"{it.Id} desc", $"{it.ProductCode}, {it.StationCode}"),
                         it.Id,
                         it.ProductCode,
                         it.TotalFlag,
                         it.StationCode,
                         it.SmallStationCode,
                         it.RecipeDescription,
                         it.OperatorNo,
                         it.EquipmentID,
                         it.RecipeNo,
                         it.RecipeVersion,
                         it.CreateTime,

                     })
                       .MergeTable()
                       .Where(it => it.index == 1)
                       .Select(it => new RecordProcessUpload()
                       {
                           Id = it.Id,
                           CreateTime = it.CreateTime,
                           EquipmentID = it.EquipmentID,
                           OperatorNo = it.OperatorNo,
                           ProductCode = it.ProductCode,
                           RecipeDescription = it.RecipeDescription,
                           RecipeNo = it.RecipeNo,
                           RecipeVersion = it.RecipeVersion,
                           SmallStationCode = it.SmallStationCode,
                           StationCode = it.StationCode,
                           TotalFlag = it.TotalFlag
                       });
                //工艺记录表
                var lst_process = db.Queryable<RecordProcessData>()
                    .Where(it => it.CreateTime >= start && it.CreateTime < end)
                    .SplitTable(start,end);

                var processdata = lst_processup.LeftJoin(lst_process, (o, c) => o.Id == c.ProcessUploadId)
                     .Select((o, c) => new ProcRecord
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
                         createTime = o.CreateTime.ToString("yyyy-MM-dd HH-mm-ss.fff"),
                     })
                    .ToList();
                procRecordData = processdata;
                Logger.RunningInfo($"过站记录导出,procRecordData:{procRecordData.Count}");
                /*******************物料******************/
                var lst_partup = db.Queryable<RecordPartUpload>()
                    .Where(it => productCodes.Contains(it.ProductCode))
                    .SplitTable(start,end)
                    .Select(it => new
                    {
                        index = SqlFunc.RowNumber($"{it.Id} desc", $"{it.ProductCode}, {it.StationCode}"),
                        it.Id,
                        it.ProductCode,
                        it.StationCode,
                        it.SmallStationCode,
                        it.OperatorNo,
                        it.EquipmentID,
                        it.CreateTime
                    })
                       .MergeTable()
                       .Where(it => it.index == 1)
                       .Select(it => new RecordPartUpload()
                       {
                           Id = it.Id,
                           CreateTime = it.CreateTime,
                           EquipmentID = it.EquipmentID,
                           OperatorNo = it.OperatorNo,
                           ProductCode = it.ProductCode,
                           SmallStationCode = it.SmallStationCode,
                           StationCode = it.StationCode,
                       });



                var lst_part = db.Queryable<RecordPartData>()
                    .Where(it => it.CreateTime >= start && it.CreateTime < end)
                    .SplitTable(start,end);

                //lst_part 是 RecordPartData
                //lst_partup 是 RecordPartUpload

                //241213出现一个没字段的错误，原因是三分分表里面有一些没统一
                var partdata = lst_partup.LeftJoin(lst_part, (o, c) => o.Id == c.PartUploadId)
                    .Select((o, c) => new ParRecord
                    {
                        productCode = o.ProductCode,
                        stationCode = o.StationCode,
                        partNumber = c.PartNumber,
                        partDescription = c.PartDescription,
                        partBarcode = c.PartBarcode,

                        traceType = c.TraceType,
                        usageQty = c.UsageQty,
                        partuom = c.Uom,
                        createTime = o.CreateTime.ToString("yyyy-MM-dd HH-mm-ss.fff"),
                    }).ToList();

                partRecordData = partdata;
                Logger.RunningInfo($"过站记录导出,partRecordData:{partRecordData.Count}");
                //是不用返回值的
                return new List<OutRecord>();
            }
            catch (Exception ex)
            {
                Logger.ErrorInfo("GetAllRecord错误", ex);
                throw ex;
            }
        }
    

    public List<OutRecord> GetAllRecord(string configId, string index, string keyword,
            ref List<RecordOutStation> outStationData,ref List<ProcRecord> procRecordData,ref List<ParRecord> partRecordData)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordOutStation> queryable = db.Queryable<RecordOutStation>();
                if (!keyword.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.TaskOrderNumber.Contains(keyword) || it.ProductCode.Contains(keyword) || it.ProductStatus.Contains(keyword) || it.StationCode.Contains(keyword));
                }
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
                //
                var lst_outstation = queryable
                    .Where(it => it.CreateTime >= startTime && it.CreateTime < endTime)
                    .SplitTable(tabs => tabs.Take(3))
                    .Select(it => new
                    {
                        index = SqlFunc.RowNumber($"{it.Id} desc", $"{it.ProductCode}, {it.StationCode}"),
                        it.Id,
                        it.ProductCode,
                        it.ProductStatus,
                        it.StationCode,
                        it.SmallStationCode,
                        it.TaskOrderNumber,
                        it.OperatorNo,
                        it.EquipmentID,
                        it.DefectCode,
                        it.DefectDesc,
                        it.CreateTime,
                        it.InstationTime,
                        it.PalletNo
                    })
                       .MergeTable()
                       .Where(it => it.index == 1)
                       .Select(it => new RecordOutStation()
                       {
                           Id = it.Id,
                           ProductCode = it.ProductCode,
                           ProductStatus = it.ProductStatus,
                           StationCode = it.StationCode,
                           SmallStationCode = it.SmallStationCode,
                           TaskOrderNumber = it.TaskOrderNumber,
                           OperatorNo = it.OperatorNo,
                           EquipmentID = it.EquipmentID,
                           DefectCode = it.DefectCode,
                           DefectDesc = it.DefectDesc,
                           CreateTime = it.CreateTime,
                           InstationTime = it.InstationTime,
                           PalletNo = it.PalletNo
                       })
                       .ToList();

                outStationData = lst_outstation;
                Logger.RunningInfo($"过站记录导出,outStationData数据量:{outStationData.Count}");
                //上传表ProcessUpload
                var lst_processup = db.Queryable<RecordProcessUpload>()
                     .Where(it => it.CreateTime >= startTime && it.CreateTime < endTime)
                     .SplitTable(tabs => tabs.Take(3))
                     .Select(it => new
                     {
                         index = SqlFunc.RowNumber($"{it.Id} desc", $"{it.ProductCode}, {it.StationCode}"),
                         it.Id,
                         it.ProductCode,
                         it.TotalFlag,
                         it.StationCode,
                         it.SmallStationCode,
                         it.RecipeDescription,
                         it.OperatorNo,
                         it.EquipmentID,
                         it.RecipeNo,
                         it.RecipeVersion,
                         it.CreateTime,

                     })
                       .MergeTable()
                       .Where(it => it.index == 1)
                       .Select(it => new RecordProcessUpload()
                       {
                           Id = it.Id,
                           CreateTime = it.CreateTime,
                           EquipmentID = it.EquipmentID,
                           OperatorNo = it.OperatorNo,
                           ProductCode = it.ProductCode,
                           RecipeDescription = it.RecipeDescription,
                           RecipeNo = it.RecipeNo,
                           RecipeVersion = it.RecipeVersion,
                           SmallStationCode = it.SmallStationCode,
                           StationCode = it.StationCode,
                           TotalFlag = it.TotalFlag
                       });
                //工艺记录表
                var lst_process = db.Queryable<RecordProcessData>()
                    .Where(it => it.CreateTime >= startTime && it.CreateTime < endTime)
                    .SplitTable(tabs => tabs.Take(3));

                var processdata = lst_processup.LeftJoin(lst_process, (o, c) => o.Id == c.ProcessUploadId)
                     .Select((o, c) => new ProcRecord
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
                         createTime = o.CreateTime.ToString("yyyy-MM-dd HH-mm-ss.fff"),
                     })
                    .ToList();
                procRecordData = processdata;
                Logger.RunningInfo($"过站记录导出,procRecordData:{procRecordData.Count}");
                /*******************物料******************/
                var lst_partup = db.Queryable<RecordPartUpload>()
                    .Where(it => it.CreateTime >= startTime && it.CreateTime < endTime)
                    .SplitTable(tabs => tabs.Take(3))
                    .Select(it => new
                    {
                        index = SqlFunc.RowNumber($"{it.Id} desc", $"{it.ProductCode}, {it.StationCode}"),
                        it.Id,
                        it.ProductCode,
                        it.StationCode,
                        it.SmallStationCode,
                        it.OperatorNo,
                        it.EquipmentID,
                        it.CreateTime
                    })
                       .MergeTable()
                       .Where(it => it.index == 1)
                       .Select(it => new RecordPartUpload()
                       {
                           Id = it.Id,
                           CreateTime = it.CreateTime,
                           EquipmentID = it.EquipmentID,
                           OperatorNo = it.OperatorNo,
                           ProductCode = it.ProductCode,
                           SmallStationCode = it.SmallStationCode,
                           StationCode = it.StationCode,
                       });



                var lst_part = db.Queryable<RecordPartData>()
                    .Where(it => it.CreateTime >= startTime && it.CreateTime < endTime)
                    .SplitTable(tabs => tabs.Take(3));

                //lst_part 是 RecordPartData
                //lst_partup 是 RecordPartUpload

                //241213出现一个没字段的错误，原因是三分分表里面有一些没统一
                var partdata = lst_partup.LeftJoin(lst_part, (o, c) => o.Id == c.PartUploadId)
                    .Select((o, c) => new ParRecord
                     {
                         productCode = o.ProductCode,
                         stationCode = o.StationCode,
                         partNumber = c.PartNumber,
                         partDescription = c.PartDescription,
                         partBarcode = c.PartBarcode,

                         traceType = c.TraceType,
                         usageQty = c.UsageQty,
                         partuom = c.Uom,
                         createTime = o.CreateTime.ToString("yyyy-MM-dd HH-mm-ss.fff"),
                     }).ToList();
                
                partRecordData = partdata;
                Logger.RunningInfo($"过站记录导出,partRecordData:{partRecordData.Count}");
                //是不用返回值的
                return new List<OutRecord>();
            }
            catch (Exception ex)
            {
                Logger.ErrorInfo("GetAllRecord错误", ex);
                throw ex;
            }
        }
    }
}


public class OutRecord
{
    public string productCode { get; set; }
    public string taskOrderNumber { get; set; }
    public string productStatus { get; set; }
    public string defectCode { get; set; }
    public string defectDesc { get; set; }
    public string stationCode { get; set; }
    public string smallStationCode { get; set; }
    public string equipmentID { get; set; }
    public string operatorNo { get; set; }
    public string createTime { get; set; }
    public string instationTime { get; set; }
    public string palletNo { get; set; }
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
    public string partNumber { get; set; }
    public string partDescription { get; set; }
    public string partBarcode { get; set; }
    public string traceType { get; set; }
    public string usageQty { get; set; }
    public string partuom { get; set; }
}

public class ProcRecord
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
    public string itemFlag{ get; set; }
    public string decisionType{ get; set; }
    public string paramType{ get; set; }
    public string standValue{ get; set; }
    public string maxValue{ get; set; }
    public string minValue{ get; set; }
    public string setValue{ get; set; }
    public string uom{ get; set; }
    public string createTime { get; set; }
}

public class ParRecord
{
    public string productCode{ get; set; }
    public string stationCode{ get; set; }
    public string partNumber{ get; set; }
    public string partDescription{ get; set; }
    public string partBarcode{ get; set; }
    public string traceType{ get; set; }
    public string usageQty{ get; set; }
    public string partuom{ get; set; }
    public string createTime { get; set; }
}