using System;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.WebUI.Controllers;
using System.Collections.Generic;
using FNMES.WebUI.Logic.Record;
using FNMES.Utility.Files;
using System.IO;
using FNMES.WebUI.Logic.Param;
using System.Drawing;

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Record")]
    [HiddenApi]
    public class UnbindPackController : BaseController
    {
        private readonly ParamRepairLogic unbindLogic;
        public UnbindPackController()
        {
            unbindLogic = new ParamRepairLogic();
        }


        [Route("record/unbindpack/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }

        [Route("record/unbindpack/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord, string configId, string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = unbindLogic.GetUnbindPack(pageIndex, pageSize, keyWord, ref totalCount, configId, index);

                var result = new LayPadding<UnbingPackData>()
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
                return Content(new LayPadding<UnbingPackData>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<UnbingPackData>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("record/unbindpack/export")]
        [HttpGet]
        public ActionResult Export(string keyWord, string configId, string index)
        {
            List<UnbingPackData> outrecords = unbindLogic.GetUnbindPack2(keyWord, configId, index);

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
                    {"ProductCode", "内控码"},
                    {"StationCode","工站" },
                    {"SmallStationCode","小工站" },
                    {"EquipmentID","设备编码" },
                    {"OperatorNo","操作员" },
                    {"CreateTime","创建时间" },
                    {"PartNumber","物料编码" },
                    {"PartDescription","物料描述" },
                    {"PartBarcode","物料条码" },
                    {"Reason","拆解原因" },
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