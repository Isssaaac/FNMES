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
using OfficeOpenXml;
using FNMES.WebUI.Logic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using JinianNet.JNTemplate.Caching;

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Record")]
    [HiddenApi]
    public class OutStationController : BaseController
    {
        private readonly RecordOutStationLogic outStationLogic;
        private readonly IMemoryCache memoryCache;
        public OutStationController(IMemoryCache cache)
        {
            outStationLogic = new RecordOutStationLogic();
            memoryCache = cache;
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

        //查看物料数据
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

        [Route("record/outstation/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId,string index,string startDate)
        {
            try
            {
                int totalCount = 0;

                var pageData = outStationLogic.GetList(pageIndex, pageSize, keyWord, ref totalCount, configId,index);                
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

        [Route("record/outstation/condition")]
        [HttpPost]
        public ActionResult condition(int pageIndex, int pageSize,string configId, string startDate, string endDate, string productCode, string order)
        {
            try
            {
                int totalCount = 0;
                //index是怎么来的
                var pageData = outStationLogic.GetList(pageIndex, pageSize, configId, startDate, endDate, productCode, order, ref totalCount);
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
                    count = 0
                }.ToJson());
            }
        }


        [Route("record/process/exist")]
        [HttpGet]
        public ActionResult processExist(string productCode,string stationCode, string configId)
        {
            try
            {
               //查一下是否存在
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
        public ActionResult Export(string configId, string index, string keyword)
        {
            try
            {
                if (memoryCache.TryGetValue("LastExportTime", out DateTime lastExportTime))
                {
                    var timeSinceLastExport = DateTime.UtcNow - lastExportTime;
                    if (timeSinceLastExport.TotalMinutes < 1)
                    {
                        return BadRequest("导出请求过于频繁，请稍后再试");
                    }
                }
                memoryCache.Set("LastExportTime", DateTime.UtcNow); // 更新导出缓存时间为当前时间 

                Logger.RunningInfo($"导出线体:<{configId}>,索引:<{index}>过站记录");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                
                List<RecordOutStation> outStationData = new List<RecordOutStation>();
                List<ProcRecord> procRecordData = new List<ProcRecord>();
                List<ParRecord> partRecordData = new List<ParRecord>();

                List<OutRecord> outrecords = outStationLogic.GetAllRecord(configId, index, keyword, ref outStationData, ref procRecordData, ref partRecordData);

                // 出站字段
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
                // 工艺字段
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
                // 物料字段
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

                List<string> sheetNames = new List<string> { "OutStation", "Process", "Part" };
                List<DataTable> tables = new List<DataTable>();

                //for (int i = 0; i < 1000; i++)
                //{
                //    outStationData.Add(new RecordOutStation()
                //    {
                //        Id = 1,
                //        ProductCode = "1",
                //        TaskOrderNumber = "1",
                //        ProductStatus = "2",
                //        SmallStationCode = "3"
                //    });
                //}

                //for (int i = 0; i < 101000; i++)
                //{
                //    procRecordData.Add(new ProcRecord()
                //    {
                //        productCode = "1",
                //        stationCode = "1",
                //        recipeDescription = "1",
                //        recipeVersion = "1",
                //        totalFlag = "1",
                //        paramCode = "1",
                //        paramName = "1",
                //        paramValue = "1",
                //    });
                //}

                //for (int i = 0; i < 1001000; i++)
                //{
                //    partRecordData.Add(new ParRecord()
                //    {
                //        productCode = "1",
                //        stationCode = "2"
                //    });
                //}

                //导出的最多能104.7w行，为了避免出现Row out of Range，3个月的数据就会超100w行，需要截断
                int count = outStationData.Count;
                int start = Math.Max(0, count - 1000000); 
                var outStationData100w = outStationData.GetRange(start, count - start);

                count = procRecordData.Count;
                start = Math.Max(0, count - 1000000);
                var procRecordData100w = procRecordData.GetRange(start, count - start);

                count = partRecordData.Count;
                start = Math.Max(0, count - 1000000);
                var partRecordData100w = partRecordData.GetRange(start, count - start);

                DataTable dt2 = outStationData100w.ToDataTable();
                DataTable dt3 = procRecordData100w.ToDataTable();
                DataTable dt4 = partRecordData100w.ToDataTable();

                tables.Add(dt2);
                tables.Add(dt3);
                tables.Add(dt4);

                Logger.RunningInfo($"过站记录导出,outStationData数据量:{outStationData.Count},procRecordData:{procRecordData.Count},partRecordData:{partRecordData.Count}");
                var bytes = ExcelUtils.DtExportExcel(tables, keyValues, sheetNames);

                
                // 创建文件流
                var stream = new MemoryStream(bytes);

                stopwatch.Stop();
                Logger.RunningInfo($"过站记录导出,outStationData数据量:{outStationData.Count},procRecordData:{procRecordData.Count},partRecordData:{partRecordData.Count}，耗时:{stopwatch.Elapsed.TotalSeconds}秒");

                

                // 设置响应头，指定响应的内容类型和文件名
                Response.Headers.Add("Content-Disposition", "attachment; filename=exported-file.xlsx");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                Logger.ErrorInfo($"过站记录导出失败",ex);
                return Error();
            }
        }


        //"/record/outstation/export?configId=3&startDate=2024-12-01&endDate=2025-01-17&productCode=&order="
        [Route("record/outstation/exportCon")]
        [HttpGet]
        public ActionResult Export(string configId, string startDate, string endDate, string productCode, string order)
        {
            try
            {
                if (memoryCache.TryGetValue("LastExportConTime", out DateTime lastExportConTime))
                {
                    var timeSinceLastExport = DateTime.UtcNow - lastExportConTime;
                    if (timeSinceLastExport.TotalMinutes < 1)
                    {
                        return BadRequest("导出请求过于频繁，请稍后再试");
                    }
                }

                memoryCache.Set("LastExportConTime", DateTime.UtcNow); // 更新导出缓存时间为当前时间 

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                List<RecordOutStation> outStationData = new List<RecordOutStation>();
                List<ProcRecord> procRecordData = new List<ProcRecord>();
                List<ParRecord> partRecordData = new List<ParRecord>();

                List<OutRecord> outrecords = outStationLogic.GetAllRecord(configId, startDate, endDate, productCode, order, ref outStationData, ref procRecordData, ref partRecordData);

                // 出站字段
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
                // 工艺字段
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
                // 物料字段
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

                List<string> sheetNames = new List<string> { "OutStation", "Process", "Part" };
                List<DataTable> tables = new List<DataTable>();

                //导出的最多能104.7w行，为了避免出现Row out of Range，3个月的数据就会超100w行，需要截断
                int count = outStationData.Count;
                int start = Math.Max(0, count - 1000000);
                var outStationData100w = outStationData.GetRange(start, count - start);

                count = procRecordData.Count;
                start = Math.Max(0, count - 1000000);
                var procRecordData100w = procRecordData.GetRange(start, count - start);

                count = partRecordData.Count;
                start = Math.Max(0, count - 1000000);
                var partRecordData100w = partRecordData.GetRange(start, count - start);

                DataTable dt2 = outStationData100w.ToDataTable();
                DataTable dt3 = procRecordData100w.ToDataTable();
                DataTable dt4 = partRecordData100w.ToDataTable();

                tables.Add(dt2);
                tables.Add(dt3);
                tables.Add(dt4);

                Logger.RunningInfo($"过站记录导出,outStationData数据量:{outStationData.Count},procRecordData:{procRecordData.Count},partRecordData:{partRecordData.Count}");
                var bytes = ExcelUtils.DtExportExcel(tables, keyValues, sheetNames);


                // 创建文件流
                var stream = new MemoryStream(bytes);

                stopwatch.Stop();
                Logger.RunningInfo($"过站记录导出,outStationData数据量:{outStationData.Count},procRecordData:{procRecordData.Count},partRecordData:{partRecordData.Count}，耗时:{stopwatch.Elapsed.TotalSeconds}秒");

                

                // 设置响应头，指定响应的内容类型和文件名
                Response.Headers.Add("Content-Disposition", "attachment; filename=exported-file.xlsx");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                Logger.ErrorInfo($"过站记录导出失败", ex);
                return Error();
            }
        }


        //仅导出过站数据
        [Route("record/outstation/exportOutstation")]
        [HttpGet]
        public ActionResult ExportOutstation(string configId, string startDate, string endDate, string productCode, string order)
        {
            try
            {
                if (memoryCache.TryGetValue("LastExportConTime", out DateTime lastExportConTime))
                {
                    var timeSinceLastExport = DateTime.UtcNow - lastExportConTime;
                    if (timeSinceLastExport.TotalMinutes < 1)
                    {
                        return BadRequest("导出请求过于频繁，请稍后再试");
                    }
                }

                memoryCache.Set("LastExportConTime", DateTime.UtcNow); // 更新导出缓存时间为当前时间 

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                List<RecordOutStation> outStationData = new List<RecordOutStation>();
                List<ProcRecord> procRecordData = new List<ProcRecord>();
                List<ParRecord> partRecordData = new List<ParRecord>();

                List<OutRecord> outrecords = outStationLogic.GetAllRecord(configId, startDate, endDate, productCode, order, ref outStationData, ref procRecordData, ref partRecordData);

                // 出站字段
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

                // 填充数据到工作表
                List<Dictionary<string, string>> keyValues = new List<Dictionary<string, string>>();
                //keyValues.Add(keyValuePairs);
                keyValues.Add(outkeyValuePairs);

                List<string> sheetNames = new List<string> { "OutStation", "Process", "Part" };
                List<DataTable> tables = new List<DataTable>();

                //导出的最多能104.7w行，为了避免出现Row out of Range，3个月的数据就会超100w行，需要截断
                int count = outStationData.Count;
                int start = Math.Max(0, count - 1000000);
                var outStationData100w = outStationData.GetRange(start, count - start);

                DataTable dt2 = outStationData100w.ToDataTable();

                tables.Add(dt2);

                Logger.RunningInfo($"过站记录导出,outStationData数据量:{outStationData.Count},procRecordData:{procRecordData.Count},partRecordData:{partRecordData.Count}");
                var bytes = ExcelUtils.DtExportExcel(tables, keyValues, sheetNames);


                // 创建文件流
                var stream = new MemoryStream(bytes);

                stopwatch.Stop();
                Logger.RunningInfo($"过站记录导出,outStationData数据量:{outStationData.Count},procRecordData:{procRecordData.Count},partRecordData:{partRecordData.Count}，耗时:{stopwatch.Elapsed.TotalSeconds}秒");

                // 设置响应头，指定响应的内容类型和文件名
                Response.Headers.Add("Content-Disposition", "attachment; filename=exported-file.xlsx");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                Logger.ErrorInfo($"过站记录导出失败", ex);
                return Error();
            }
        }



        //241209导出表格失败
        //[Route("record/outstation/export")]
        //[HttpGet]
        public ActionResult Export_test(string configId, string index,string keyword)
        {
            List<RecordOutStation> outStationData = new List<RecordOutStation>();

            List<ProcRecord> procRecordData = new List<ProcRecord>();
            List<ParRecord> partRecordData = new List<ParRecord>();

            List<OutRecord> outrecords = outStationLogic.GetAllRecord(configId,index,keyword,ref outStationData,ref procRecordData,ref partRecordData);


            // 出站字段
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
            // 工艺字段
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
            // 物料字段
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

            string path = $"D:\\outStation{DateTime.Now.ToString("yyyyMMddHHmmssffff")}.xlsx";
            ExcelUtils.DtExportExcel(tables, keyValues, sheetNames , path);
            

            // 创建文件流
            var stream = new MemoryStream(bytes);

            // 设置响应头，指定响应的内容类型和文件名
            Response.Headers.Add("Content-Disposition", "attachment; filename=exported-file.xlsx");
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

#region
        //[Route("record/outstation/export")]
        //[HttpGet]
        //public ActionResult Export(string configId, string index, string keyword)
        //{
        //    var data = new List<MyDataModel>();
        //    for (int i = 0; i < 1000000; i++)
        //    {
        //        data.Add(new MyDataModel { Id = 1, Name = "Item 1", Value = 10 });
        //    }

        //    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        //    // 创建 Excel 包  
        //    using var package = new ExcelPackage();
        //    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

        //    // 添加表头  
        //    worksheet.Cells[1, 1].Value = "ID";
        //    worksheet.Cells[1, 2].Value = "Name";
        //    worksheet.Cells[1, 3].Value = "Value";

        //    // 添加数据  
        //    for (int i = 0; i < data.Count; i++)
        //    {
        //        worksheet.Cells[i + 2, 1].Value = data[i].Id;
        //        worksheet.Cells[i + 2, 2].Value = data[i].Name;
        //        worksheet.Cells[i + 2, 3].Value = data[i].Value;
        //    }

        //    // 输出流  
        //    var stream = new MemoryStream();
        //    package.SaveAs(stream);
        //    stream.Position = 0;

        //    // 确保返回正确的文件类型  
        //    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export.xlsx");
        //}

        //public class MyDataModel
        //{
        //    public int Id { get; set; }
        //    public string Name { get; set; }
        //    public int Value { get; set; }
        //}
#endregion
    }
}
