using System;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.WebUI.Controllers;
using System.Collections.Generic;
using FNMES.WebUI.Logic.Record;
using FNMES.Entity.Record;
using FNMES.Utility.Files;
using System.IO;
using System.Data;

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Record")]
    [HiddenApi]
    public class OutStationController : BaseController
    {
        private readonly RecordOutStationLogic outStationLogic;
        public OutStationController()
        {
            outStationLogic = new RecordOutStationLogic();
           
        }


        [Route("record/outstation/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }

        [Route("record/process/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Process()
        {
            return View();
        }
        
        [Route("record/process/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Process(int pageIndex, int pageSize, string keyWord, string configId, string productCode, string stationCode)
        {
            try
            {
                int totalCount = 0;
                List<RecordProcessData> pageData = outStationLogic.GetProcessData(pageIndex, pageSize, keyWord, ref totalCount, configId, productCode, stationCode);
                LayPadding<RecordProcessData> result = new LayPadding<RecordProcessData>()
                {
                    result = true,
                    msg = "success",
                    list = pageData,
                    count = totalCount//pageData.Count
                };
                return Content(result.ToJson());
            }
            catch (Exception E)
            {
                return Content(new LayPadding<RecordProcessData>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordProcessData>(),
                    count = 0
                }.ToJson());
            }


        }

      



        [Route("record/part/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Part()
        {
            return View();
        }

        [Route("record/part/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Part(int pageIndex, int pageSize, string configId, string productCode, string stationCode)
        {
            try
            {
                int totalCount = 0;
                List<RecordPartData> pageData = outStationLogic.GetPartData(pageIndex, pageSize, ref totalCount, configId, productCode, stationCode);
                LayPadding<RecordPartData> result = new LayPadding<RecordPartData>()
                {
                    result = true,
                    msg = "success",
                    list = pageData,
                    count = totalCount//pageData.Count
                };
                return Content(result.ToJson());
            }
            catch (Exception E)
            {
                return Content(new LayPadding<RecordPartData>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordPartData>(),
                    count = 0
                }.ToJson());
            }



        }
       
        
        /*
        [Route("record/part/data")]
        [HttpPost, AuthorizeChecked]
        public ActionResult PartData(int pageIndex, int pageSize, string configId, string pId)
        {

        }*/





        [Route("record/outstation/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId,string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = outStationLogic.GetList(pageIndex, pageSize, keyWord, ref totalCount, configId,index);
                //在分页查询后去重导致页面不足10条，已在查询时去重，分页后去重影响界面显示，pagesize=10，可能不足10条
                //var pageData1 = pageData.GroupBy(e => new { e.ProductCode, e.StationCode }).Select(e => e.First()).ToList();
                
                var result = new LayPadding<RecordOutStation>()
                {
                    result = true,
                    msg = "success",
                    list = pageData,
                    count = totalCount//pageData.Count
                };
                return Content(result.ToJson());
            }
            catch (Exception E)
            {
                return Content(new LayPadding<RecordOutStation>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordOutStation>(),
                    count =0
                }.ToJson()) ;
            }
        }


        [Route("record/process/exist")]
        [HttpGet]
        public ActionResult processExist(string productCode,string stationCode, string configId)
        {
            try
            {
                bool exist = outStationLogic.processExist(productCode, stationCode, configId);
               if (exist)
                {
                    return Success();
                }
                return Error();
            }
            catch 
            {
                return Error();
            }
        }

        [Route("record/part/exist")]
        [HttpGet]
        public ActionResult partExist(string productCode, string stationCode, string configId)
        {
            try
            {
                bool exist = outStationLogic.partExist(productCode, stationCode, configId);
                if (exist)
                {
                    return Success();
                }
                return Error();
            }
            catch
            {
                return Error();
            }
        }

        [Route("record/outstation/export")]
        [HttpGet]
        public ActionResult Export(string configId, string index,string keyword)
        {
            List<RecordOutStation> outStationData = new List<RecordOutStation>();
            List<ProcRecord> procRecordData = new List<ProcRecord>();
            List<ParRecord> partRecordData = new List<ParRecord>();
            List<OutRecord> outrecords = outStationLogic.GetAllRecord(configId,index,keyword,ref outStationData,ref procRecordData,ref partRecordData);

            //Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
            //        {"productCode", "内控码"},
            //        {"taskOrderNumber", "工单"},
            //        {"productStatus","产品状态" },
            //        {"defectCode","不良代码" },
            //        {"defectDesc","不良描述" },
            //        {"stationCode","工站" },
            //        {"smallStationCode","小工站" },
            //        {"equipmentID","设备编码" },
            //        {"operatorNo","操作员" },
            //        {"createTime","出站时间" },
            //        {"instationTime","进站时间" },
            //        {"palletNo","AGV码" },
            //        {"recipeNo","配方编号" },
            //        {"recipeDescription","配方名称" },
            //        {"recipeVersion","配方版本" },
            //        {"totalFlag","总结果" },
            //        {"paramCode","参数代码" },
            //        {"paramName","参数名称" },
            //        {"paramValue","值" },
            //        {"itemFlag","项结果" },
            //        {"decisionType","判断类型" },
            //        {"paramType","参数类型" },
            //        {"standValue","标准值" },
            //        {"maxValue","最大值" },
            //        {"minValue","最小值" },
            //        {"setValue","设定值" },
            //        {"uom","单位" },
            //        {"partNumber","物料数量" },
            //        {"partDescription","物料描述" },
            //        {"partBarcode","物料条码" },
            //        {"traceType","批次" },
            //        {"usageQty","用量" },
            //        {"partuom","单位" }
            //    };

            Dictionary<string, string> outkeyValuePairs = new Dictionary<string, string>() {
                    {"ProductCode", "内控码"},
                    {"TaskOrderNumber", "工单"},
                    {"ProductStatus","产品状态" },
                    {"DefectCode","不良代码" },
                    {"DefectDesc","不良描述" },
                    {"StationCode","工站" },
                    {"SmallStationCode","小工站" },
                    {"EquipmentID","设备编码" },
                    {"OperatorNo","操作员" },
                    {"CreateTime","出站时间" },
                    {"instationTime","进站时间" },
                    {"palletNo","AGV码" },
            };

            Dictionary<string, string> prockeyValuePairs = new Dictionary<string, string>() {
                    {"productCode", "内控码"},
                    {"stationCode", "工站"},
                    {"recipeNo","配方编号" },
                    {"recipeDescription","配方名称" },
                    {"recipeVersion","配方版本" },
                    {"totalFlag","总结果" },
                    {"paramCode","参数代码" },
                    {"paramName","参数名称" },
                    {"paramValue","值" },
                    {"itemFlag","项结果" },
                    {"decisionType","判断类型" },
                    {"paramType","参数类型" },
                    {"standValue","标准值" },
                    {"maxValue","最大值" },
                    {"minValue","最小值" },
                    {"setValue","设定值" },
                    {"uom","单位" },
                    {"createTime","创建时间" },
            };

            Dictionary<string, string> partkeyValuePairs = new Dictionary<string, string>() {
                    {"productCode", "内控码"},
                    {"stationCode", "工站"},
                    {"partNumber","物料数量" },
                    {"partDescription","物料描述" },
                    {"partBarcode","物料条码" },
                    {"traceType","批次" },
                    {"usageQty","用量" },
                    {"partuom","单位" },
                    {"createTime","创建时间" },
                };
            // 填充数据到工作表
            List<Dictionary<string, string>> keyValues = new List<Dictionary<string, string>>();
            //keyValues.Add(keyValuePairs);
            keyValues.Add(outkeyValuePairs);
            keyValues.Add(prockeyValuePairs);
            keyValues.Add(partkeyValuePairs);
            // 将 ExcelPackage 转换为字节数组
            //var bytes = ExcelUtils.ExportExcel(outrecords, keyValuePairs, "Data", true);
            //var outbytes = ExcelUtils.ExportExcel(outStationData, outkeyValuePairs, "OutStation", true);
            //var procbytes = ExcelUtils.ExportExcel(procRecordData, prockeyValuePairs, "Process", true);
            //var partbytes = ExcelUtils.ExportExcel(partRecordData, partkeyValuePairs, "Part", true);
            List<string> sheetNames = new List<string>{ "OutStation", "Process", "Part" };
            List<DataTable> tables = new List<DataTable>();
           // DataTable dt1 = outrecords.ToDataTable();
            DataTable dt2 = outStationData.ToDataTable();
            DataTable dt3 = procRecordData.ToDataTable();
            DataTable dt4 = partRecordData.ToDataTable();
            //tables.Add(dt1);
            tables.Add(dt2);
            tables.Add(dt3);
            tables.Add(dt4);
            var bytes = ExcelUtils.DtExportExcel(tables, keyValues,sheetNames);

            // 创建文件流
            var stream = new MemoryStream(bytes);

            // 设置响应头，指定响应的内容类型和文件名
            Response.Headers.Add("Content-Disposition", "attachment; filename=exported-file.xlsx");
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
