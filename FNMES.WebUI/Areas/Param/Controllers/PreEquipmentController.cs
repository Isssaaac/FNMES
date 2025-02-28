using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Entity.Sys;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using FNMES.WebUI.Logic.Sys;
using FNMES.WebUI.Logic;
using System.Collections.Generic;
using FNMES.Utility.Files;
using System.IO;

namespace MES.WebUI.Areas.Sys.Controllers
{
    [HiddenApi]
    [Area("Param")]
    public class PreEquipmentController : BaseController
    {
        private readonly SysPreProductLogic sysPreProductLogic;

        public PreEquipmentController()
        {
            sysPreProductLogic = new SysPreProductLogic();
        }


        [Route("param/preEquipment/index")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [Route("param/preEquipment/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string lineId, string keyWord, string index)
        {
            int totalCount = 0;
            //预处理
            var pageData = sysPreProductLogic.GetList(pageIndex, pageSize, long.Parse(lineId), keyWord, ref totalCount, index);
            var result = new LayPadding<SysPreSelectProduct>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return Content(result.ToJson());
        }


        [Route("param/preEquipment/form")]
        [HttpGet]
        public ActionResult Form()
        {
            return View();
        }

        [Route("param/preEquipment/form")]
        [HttpPost]
        public ActionResult Form(int pageIndex, int pageSize, string configId, string keyWord, string index)
        {
            int totalCount = 0;
            var pageData = sysPreProductLogic.GetHotList(pageIndex, pageSize, configId, keyWord, ref totalCount, index);
            var result = new LayPadding<HotRecord>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return Content(result.ToJson());
        }


        [Route("param/preEquipment/export")]
        [HttpGet]
        public ActionResult Export(string configId, string index)
        {
            List<HotAllRecord> outrecords = sysPreProductLogic.GetHotAllList(configId, index);

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
                    {"productCode", "条码"},
                    {"stationCode","工站" },
                    {"smallStationCode","小工站" },
                    {"equipmentID","设备编码" },
                    {"operatorNo","操作员" },
                    {"createTime","创建时间" },
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
                };

            // 填充数据到工作表

            // 将 ExcelPackage 转换为字节数组
            var bytes = ExcelUtils.ExportExcel(outrecords, keyValuePairs, "Data", true);

            // 创建文件流
            var stream = new MemoryStream(bytes);

            // 设置响应头，指定响应的内容类型和文件名
            Response.Headers.Add("Content-Disposition", "attachment; filename=exported-file.xlsx");
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }


        [Route("param/preEquipment/part")]
        [HttpGet]
        public ActionResult Part()
        {
            return View();
        }

        [Route("param/preEquipment/part")]
        [HttpPost]
        public ActionResult Part(int pageIndex, int pageSize, string configId, string keyWord, string index)
        {
            int totalCount = 0;
            var pageData = sysPreProductLogic.GetHotPartList(pageIndex, pageSize, configId, keyWord, ref totalCount, index);
            var result = new LayPadding<HotrevitPartRecord>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return Content(result.ToJson());
        }


        [Route("param/preEquipment/exportpart")]
        [HttpGet]
        public ActionResult ExportPart(string configId, string index)
        {
            List<HotrevitPartRecord> outrecords = sysPreProductLogic.GetHotPartAllList(configId, index);

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
                    {"productCode", "条码"},
                    {"stationCode","工站" },
                    {"smallStationCode","小工站" },
                    {"equipmentID","设备编码" },
                    {"operatorNo","操作员" },
                    {"createTime","创建时间" },
                    {"partNumber","物料号" },
                    {"partDescription","物料描述" },
                    {"partBarcode","物料条码" },
                    {"traceType","批次物料" },
                    {"usageQty","用量" },
                    {"partuom","单位" },
                };

            // 填充数据到工作表

            // 将 ExcelPackage 转换为字节数组
            var bytes = ExcelUtils.ExportExcel(outrecords, keyValuePairs, "Data", true);

            // 创建文件流
            var stream = new MemoryStream(bytes);

            // 设置响应头，指定响应的内容类型和文件名
            Response.Headers.Add("Content-Disposition", "attachment; filename=exported-file.xlsx");
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
