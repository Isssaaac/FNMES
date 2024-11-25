using CCS.WebUI;
using FNMES.Entity.DTO;
using FNMES.Entity.DTO.ApiData;
using FNMES.Entity.DTO.ApiParam;
using FNMES.Entity.DTO.AppData;
using FNMES.Entity.Param;
using FNMES.Entity.Record;
using FNMES.Utility.Core;
using FNMES.Utility.Network;
using FNMES.WebUI.Logic.Base;
using OfficeOpenXml.ConditionalFormatting;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FNMES.WebUI.Logic.Record
{
    //2024-04-11 添加，获取生成过程中的NG数据
    public class ProcessNGLogic : BaseLogic
    {
        public ProcessUploadParam GetProcessNGData(string productCode,string currentStationCode, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                //根据内控码查询绑定信息，获得当前过站信息
                var bindparam =  db.MasterQueryable<ProcessBind>().Where(e=> e.ProductCode == productCode).First();
                //根据工单，获取工艺流程
                var paramLocalRoutes = db.MasterQueryable<ParamLocalRoute>().Where(it => it.ProductPartNo == bindparam.ProductPartNo).OrderBy(it => it.Step).ToList();
                int routStep = paramLocalRoutes.FindIndex(it => it.StationCode == currentStationCode);
                if (routStep < 1)
                {
                    Logger.RunningInfo($"{productCode}在{currentStationCode}工位未找到前一站");
                    return null;
                }
                //上一站
                var route = paramLocalRoutes.ElementAt(routStep-1);

                //根据内控码与工站查询过站数据
                var outStation = db.MasterQueryable<RecordOutStation>()
                    .Where(e => e.ProductCode == productCode && e.StationCode == route.StationCode).SplitTable(e=>e.Take(3)).OrderByDescending(e=>e.Id).First();
                //查询过程数据
                var record = db.MasterQueryable<RecordProcessUpload>()
                    .Where(it => it.ProductCode == productCode && it.StationCode == route.StationCode)
                    .SplitTable(tabs => tabs.Take(4)).OrderByDescending(it => it.Id).First();
                if (record == null) return null;
                //分表不能自动关联，需手动查询过程数据
                var process = db.Queryable<RecordProcessData>().Where(it => it.ProcessUploadId == record.Id)
                        .SplitTable(tabs => tabs.Take(4)).ToList();
                if (process==null) return null;
                ////筛选NG过程数据
                //var NGRecord = process.Where(e => e.ItemFlag.Contains("NG") || e.ItemFlag.Contains("False"));
                //不再使用NG过程数据了，在上位机判断，上位机不仅要上传返修NG数据，还要上传OK之前OK的数据，因为过站查询最后一条过程数据，所以必须补全
                //if (NGRecord == null) return null;
                var data = process.Select(e => new Process { paramValue = e.ParamValue, itemFlag = e.ItemFlag, paramCode = e.ParamCode, paramName = e.ParamName }).ToList() ;
                return new ProcessUploadParam()
                {
                    productCode = record.ProductCode,
                    stationCode = record.StationCode,
                    smallStationCode = record.SmallStationCode,
                    equipmentID = record.EquipmentID,
                    totalFlag = record.TotalFlag,
                    processData = data
                };
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }

        public List<RecordPartData> GetPartData(string productCode, string stationCode, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                RecordPartUpload recordPartUpload = db.Queryable<RecordPartUpload>().Where(it => it.ProductCode == productCode && it.StationCode == stationCode)
                    .SplitTable(tabs => tabs.Take(4)).OrderByDescending(it => it.Id).First();
                if (recordPartUpload != null)
                {
                    return db.Queryable<RecordPartData>().Where(it => it.PartUploadId == recordPartUpload.Id)
                        .SplitTable(tabs => tabs.Take(4)).ToList();
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

        public List<RecordProcessData> GetProcessData(string productCode, string stationCode, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                RecordProcessUpload record = db.Queryable<RecordProcessUpload>().Where(it => it.ProductCode == productCode && it.StationCode == stationCode)
                    .SplitTable(tabs => tabs.Take(4)).OrderByDescending(it => it.Id).First();
                if (record != null)
                {
                    return db.Queryable<RecordProcessData>().Where(it => it.ProcessUploadId == record.Id)
                        .SplitTable(tabs => tabs.Take(4)).ToList();
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

        public FinishedStation GetFinishedStation (string productCode)
        {
            foreach (var item in AppSetting.lineConnections)
            {
                try
                {
                    ISqlSugarClient db = GetInstance(item.ConfigId);
                    //根据内控码查询绑定信息表
                    ProcessBind processBind = db.Queryable<ProcessBind>().Where(e => e.ProductCode == productCode).First();
                    if (processBind != null)
                    {
                        FinishedStation finishedStation = new FinishedStation();
                        finishedStation.configId = item.ConfigId;
                        finishedStation.productPartNo = processBind.ProductPartNo;
                        finishedStation.taskOrderNumber = processBind.TaskOrderNumber;
                        finishedStation.reessNo = processBind.ReessNo;

                        //查工艺路线的工站
                        var route = db.Queryable<ParamLocalRoute>()
                            .Where(it => it.ProductPartNo == processBind.ProductPartNo)
                            .OrderBy(it => it.Step)
                            .ToList();
                        var stations = route.Select(e => e.StationCode).ToList();
                        var index = stations.IndexOf(processBind.CurrentStation);
                        stations = stations.Take(index + 1).ToList();
                        finishedStation.stationCodes = stations;
                        return finishedStation;
                    }
                }
                catch(Exception ex)
                {
                    Logger.ErrorInfo(ex.Message);
                    return null;
                }
            }
            return null;
        }

        public RepairInfoData QueryRepairInfo(string productCode, string stationCode, string configId)
        {
            RepairInfoData infoData = new RepairInfoData();
            infoData.LstRepairPartData = new List<RepairPartData>();
            infoData.LstRepairProcessData = new List<RepairProcessData>();
            infoData.LstRepairStepData = new List<RepairStepData>();
            ISqlSugarClient db = GetInstance(configId);
            var repairStep = db.Queryable<RecordRepairStep>()
                .Where(t => t.ProductCode == productCode && t.StationCode == stationCode)
                .SplitTable(tabs => tabs.Take(4))
                .ToList();
            if (repairStep != null)
            {
                foreach (var item in repairStep)
                {
                    infoData.LstRepairStepData.Add(ConvertHelper.Mapper<RepairStepData, RecordRepairStep>(item));
                }
            }

            var repairPart = db.Queryable<RecordRepairPart>()
                .Where(t => t.ProductCode == productCode && t.StationCode == stationCode)
                .SplitTable(tabs => tabs.Take(4))
                .ToList();
            if (repairPart != null)
            {
                foreach (var item in repairPart)
                {
                    RepairPartData partData = new RepairPartData();
                    partData = ConvertHelper.Mapper<RepairPartData, RecordRepairPart>(item);
                    infoData.LstRepairPartData.Add(partData);
                }
            }

            var repairProcess = db.Queryable<RecordRepairProcess>()
                .Where(t => t.ProductCode == productCode && t.StationCode == stationCode)
                .SplitTable(tabs => tabs.Take(4))
                .ToList();
            if (repairProcess != null)
            {
                foreach (var item in repairProcess)
                {
                    RepairProcessData processData = new RepairProcessData();
                    processData = ConvertHelper.Mapper<RepairProcessData, RecordRepairProcess>(item);
                    infoData.LstRepairProcessData.Add(processData);
                }
            }

            return infoData;
        }

        public string UploadRepairInfo(RepairInfoData param, string configId)
        {
            ISqlSugarClient db = GetInstance(configId);
            int result = 0;
            //返修工步
            List<RecordRepairStep> recordRepairSteps = new List<RecordRepairStep>();
            foreach(var recordRepairStep in param.LstRepairStepData)
            {
                RecordRepairStep repairStep = new RecordRepairStep();
                repairStep.CopyField(recordRepairStep);
                repairStep.Id = SnowFlakeSingle.instance.NextId();
                repairStep.ProductCode = param.ProductCode;
                repairStep.StationCode = param.StationCode;
                repairStep.SmallStationCode = param.SmallStationCode;
                repairStep.CreateTime = DateTime.Now;
                recordRepairSteps.Add(repairStep);
            }

            List<string> productCodes = recordRepairSteps.Select(t => t.ProductCode).ToList();
            List<string> distProductCode = productCodes.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().ToList();
            if (distProductCode == null || distProductCode.Count != 1)
            {
                return "工步内控码异常！";
            }
            List<string> stations = recordRepairSteps.Select(t => t.StationCode).ToList();
            List<string> distStations = stations.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().ToList();
            if (distStations == null || distStations.Count == 0)
            {
                return "工步中工站信息异常！";
            }

            foreach (var item in distStations)
            {
                //先查询该工站是否已经有登记过返修数据，删掉已登记的数据，再上传新的
                var data = db.Queryable<RecordRepairStep>()
                    .Where(t => t.ProductCode == distProductCode[0] && t.StationCode == item)
                    .SplitTable(tabs => tabs.Take(4)).ToList();
                if (data != null && data.Count > 0)
                {
                    db.Deleteable<RecordRepairStep>(data).SplitTable().ExecuteCommand();
                }
            }
            
            result = db.Insertable<RecordRepairStep>(recordRepairSteps).SplitTable().ExecuteCommand();
            if (result == 0) return "插入返修工步数据失败！";

            //返修物料数据
            result = 0;
            List<RecordRepairPart> recordRepairParts = new List<RecordRepairPart>();
            foreach (var item in param.LstRepairPartData)
            {
                RecordRepairPart repairPart = new RecordRepairPart();
                repairPart.CopyField(item);
                repairPart.Id = SnowFlakeSingle.instance.NextId();
                repairPart.ProductCode = param.ProductCode;
                repairPart.StationCode = param.StationCode; 
                repairPart.SmallStationCode = param.SmallStationCode;
                repairPart.CreateTime = DateTime.Now;
                recordRepairParts.Add(repairPart);
            }

            List<string> partProductCodes = recordRepairParts.Select(t => t.ProductCode).ToList();
            List<string> partDistProductCode = partProductCodes.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().ToList();
            if (partDistProductCode == null || partDistProductCode.Count != 1 || partDistProductCode[0] != distProductCode[0])
            {
                return "物料内控码异常！";
            }
            List<string> partStations = recordRepairParts.Select(t => t.StationCode).ToList();
            List<string> partDistStations = partStations.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().ToList();
            if (partDistStations == null || partDistStations.Count == 0)
            {
                return "物料中工站信息异常！";
            }

            foreach (var item in partDistStations)
            {
                //先查询该工站是否已经有登记过返修数据，删掉已登记的数据，再上传新的
                var data = db.Queryable<RecordRepairPart>()
                    .Where(t => t.ProductCode == partDistProductCode[0] && t.StationCode == item)
                    .SplitTable(tabs => tabs.Take(4)).ToList();
                if (data != null && data.Count > 0)
                {
                    db.Deleteable<RecordRepairPart>(data).SplitTable().ExecuteCommand();
                }
            }

            result = db.Insertable<RecordRepairPart>(recordRepairParts).SplitTable().ExecuteCommand();
            if (result == 0) return "插入返修物料数据失败！";

            //返修过程数据
            result = 0;
            List<RecordRepairProcess> recordProcessDatas = new List<RecordRepairProcess>();
            foreach (var item in param.LstRepairProcessData)
            {
                RecordRepairProcess processData = new RecordRepairProcess();
                processData.CopyField(item);
                processData.Id = SnowFlakeSingle.instance.NextId();
                processData.ProductCode = param.ProductCode;
                processData.StationCode = param.StationCode;
                processData.SmallStationCode = param.SmallStationCode;
                processData.CreateTime = DateTime.Now;
                recordProcessDatas.Add(processData);
            }

            List<string> processProductCodes = recordProcessDatas.Select(t => t.ProductCode).ToList();
            List<string> processDistProductCode = processProductCodes.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().ToList();
            if (partDistProductCode == null || partDistProductCode.Count != 1 || processDistProductCode[0] != distProductCode[0])
            {
                return "过程数据内控码异常！";
            }
            List<string> processStations = recordProcessDatas.Select(t => t.StationCode).ToList();
            List<string> processDistStations = processStations.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().ToList();
            if (processDistStations == null || processDistStations.Count == 0)
            {
                return "过程数据中工站信息异常！";
            }

            foreach (var item in processDistStations)
            {
                //先查询该工站是否已经有登记过返修数据，删掉已登记的数据，再上传新的
                var data = db.Queryable<RecordRepairProcess>()
                    .Where(t => t.ProductCode == processDistProductCode[0] && t.StationCode == item)
                    .SplitTable(tabs => tabs.Take(4)).ToList();
                if (data != null && data.Count > 0)
                {
                    db.Deleteable<RecordRepairProcess>(data).SplitTable().ExecuteCommand();
                }
            }

            result = db.Insertable<RecordRepairProcess>(recordProcessDatas).SplitTable().ExecuteCommand();
            if (result == 0) return "插入返修过程数据失败！";

            return "OK";
        }
    }
}
