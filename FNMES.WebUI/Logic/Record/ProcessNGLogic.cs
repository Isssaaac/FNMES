using FNMES.Entity.DTO.ApiParam;
using FNMES.Entity.DTO.AppData;
using FNMES.Entity.Param;
using FNMES.Entity.Record;
using FNMES.WebUI.Logic.Base;
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
                if (routStep < 0)
                {
                    Logger.RunningInfo($"{productCode}在{currentStationCode}工位未找到前一站");
                    return null;
                }
                var route = paramLocalRoutes.Where(e => e.Step == routStep).First();

                //根据内控码与工站查询过站数据
                var outStation = db.MasterQueryable<RecordOutStation>()
                    .Where(e => e.ProductCode == productCode && e.StationCode == route.StationCode).SplitTable(e=>e.Take(3)).First();
                //查询过程数据
                var record = db.MasterQueryable<RecordProcessUpload>()
                    .Where(it => it.ProductCode == productCode && it.StationCode == route.StationCode)
                    .SplitTable(tabs => tabs.Take(4)).OrderByDescending(it => it.Id).First();
                //分表不能自动关联，需手动查询过程数据
                var process = db.Queryable<RecordProcessData>().Where(it => it.ProcessUploadId == record.Id)
                        .SplitTable(tabs => tabs.Take(4)).ToList();

                //筛选NG过程数据
                var NGRecord = process.Where(e => e.ItemFlag.Contains("NG") || e.ItemFlag.Contains("False"))
                .Select(e => new Process { paramValue = e.ParamValue, itemFlag = e.ItemFlag, paramCode = e.ParamCode, paramName = e.ParamName }).ToList() ;
                return new ProcessUploadParam()
                {
                    productCode = record.ProductCode,
                    stationCode = record.StationCode,
                    smallStationCode = record.SmallStationCode,
                    equipmentID = record.EquipmentID,
                    totalFlag = record.TotalFlag,
                    processData = NGRecord
                };
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }
    }
}
