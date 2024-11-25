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

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Record")]
    [HiddenApi]
    public class ToolRemainController : BaseController
    {
        private readonly RecordToolRemainLogic toolRemainLogic;
        public ToolRemainController()
        {
            toolRemainLogic = new RecordToolRemainLogic();

        }


        [Route("record/toolremain/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }

        [Route("record/toolremain/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord, string configId, string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = toolRemainLogic.GetList(pageIndex, pageSize, keyWord, ref totalCount, configId, index);

                var result = new LayPadding<ToolDataList>()
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
                return Content(new LayPadding<ToolDataList>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ToolDataList>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("record/toolremain/export")]
        [HttpGet]
        public ActionResult Export(string keyWord, string configId, string index)
        {
            List<ToolDataList> outrecords = toolRemainLogic.GetList(keyWord,configId,index);

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
                    {"ProductCode", "内控码"},
                    {"StationCode","工站" },
                    {"EquipmentID","设备编码" },
                    {"OperatorNo","操作员" },
                    {"CreateTime","创建时间" },
                    {"ToolNo","夹治具编码" },
                    {"ToolName","夹治具名称" },
                    {"ToolRemainValue","剩余寿命" },
                    {"Uom","单位" },
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