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
    public class SelfDischargeController : BaseController
    {
        private readonly RecordSelfDischargeLogic selfDischargLogic;
        private readonly IMemoryCache memoryCache; //内置的缓存
        public SelfDischargeController(IMemoryCache cache)
        {
            selfDischargLogic = new RecordSelfDischargeLogic();
            memoryCache = cache;
        }


        [Route("record/SelfDischarge/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }

        [Route("record/SelfDischarge/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord, string configId, string index, string startDate)
        {
            try
            {
                int totalCount = 0;

                var pageData = selfDischargLogic.GetList(pageIndex, pageSize, keyWord, ref totalCount, configId, index);
                var result = new LayPadding<RecordSelfDischarge>()
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
                return Content(new LayPadding<RecordSelfDischarge>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordSelfDischarge>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("record/SelfDischarge/condition")]
        [HttpPost]
        public ActionResult condition(int pageIndex, int pageSize, string configId, string startDate, string endDate, string keyword)
        {
            try
            {
                int totalCount = 0;
                var pageData = selfDischargLogic.GetList(pageIndex, pageSize, configId, startDate, endDate, keyword, ref totalCount);

                var result = new LayPadding<RecordSelfDischarge>()
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
                return Content(new LayPadding<RecordSelfDischarge>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordSelfDischarge>(),
                    count = 0
                }.ToJson());
            }
        }


        //仅导出过站数据
        [Route("record/SelfDischarge/export")]
        [HttpGet]
        public ActionResult Export(string configId, string startDate, string endDate, string keyword)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                List<RecordSelfDischarge> selfDischargeData = selfDischargLogic.GetAllRecord(configId, startDate, endDate,keyword);

                Dictionary<string, string> outkeyValuePairs = new Dictionary<string, string>() {
                    {"productCode", "内控码"},
                    {"maxVoltageDrop","压降最大值" },
                    {"minVoltageDrop","压降最小值" },
                    {"averageVoltageDrop","压降平均值" },
                    {"stdDeviationVoltageDrop","压降标准差" },
                    {"judgment1Up","判定1上限" },
                    {"judgment1Lo","判定1下限" },
                    {"cellCode","电芯条码" },
                    {"voltageDrop","压降" },

                    {"a020TestTime","a020测试时间" },
                    {"a020TestVoltage","a020测试电压" },
                    {"m350TestTime","m350测试时间" },
                    {"m350TestVoltage","m350测试电压" },
                    {"timeInterval","间隔时间" },
                    {"intervalVoltageDrop","间隔压降" },
                    {"judgment1UpResult","判定1上限结果" },
                    {"judgment1LoResult","判定1下限结果" },
                    {"judgment1Result","判定1结果" },
                    {"judgment2Result","判定2结果" },
                    {"result","结果" },
                    {"createTime","创建时间" },
            };

                // 填充数据到工作表
                List<Dictionary<string, string>> keyValues = new List<Dictionary<string, string>>();
                keyValues.Add(outkeyValuePairs);

                List<string> sheetNames = new List<string> { "SelfDischarge" };
                List<DataTable> tables = new List<DataTable>();

                //导出的最多能104.7w行，为了避免出现Row out of Range，3个月的数据就会超100w行，需要截断
                int count = selfDischargeData.Count;
                int start = Math.Max(0, count - 1000000);
                var outStationData100w = selfDischargeData.GetRange(start, count - start);

                DataTable dt2 = outStationData100w.ToDataTable();

                tables.Add(dt2);
                var bytes = ExcelUtils.DtExportExcel(tables, keyValues, sheetNames);

                //创建文件流
                var stream = new MemoryStream(bytes);
                stopwatch.Stop();
                Logger.RunningInfo($"过站记录导出,outStationData数据量:{outStationData100w.Count}，耗时:{stopwatch.Elapsed.TotalSeconds}秒");

                //设置响应头，指定响应的内容类型和文件名
                Response.Headers.Add("Content-Disposition", "attachment; filename=exported-file.xlsx");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                Logger.ErrorInfo($"过站记录导出失败", ex);
                return Error();
            }
        }
    }
}
