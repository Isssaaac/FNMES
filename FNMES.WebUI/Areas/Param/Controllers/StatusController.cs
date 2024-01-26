using System;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.WebUI.Controllers;
using FNMES.WebUI.Logic.Param;
using FNMES.Entity.Param;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using FNMES.Utility.Files;
using FNMES.WebUI.Logic;
using OfficeOpenXml;

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Param")]
    [HiddenApi]
    public class StatusController : BaseController
    {
        private readonly ErrorAndStatusLogic errorAndStatusLogic;
        public StatusController()
        {
            errorAndStatusLogic = new ErrorAndStatusLogic();
        }

        [Route("param/status/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }

        [Route("param/status/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId)
        {
            try
            {
                int totalCount = 0;
                var pageData = errorAndStatusLogic.GetStatusList(pageIndex, pageSize, keyWord, configId, ref totalCount);
                var result = new LayPadding<ParamEquipmentStatus>()
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
                return Content(new LayPadding<ParamEquipmentStatus>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamEquipmentStatus>(),
                    count =0
                }.ToJson()) ;
            }
        }

        [Route("param/status/import")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Import()
        {
            return View();
        }

        [Route("param/status/uploadFile")]
        [HttpPost]
        public ActionResult UploadFile(IFormFile file, string configId)
        {
            if (file != null && file.Length > 0)
            {
                /*var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);*/
                /* using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }*/

                // 在这里可以处理上传成功后的操作，例如返回文件路径或其他信息

                using var stream = file.OpenReadStream();
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
                    {"大工站","StationCode" },
                    {"小工站","SmallStationCode" },
                    {"设备","EquipmentID" },
                    {"偏移","Offset" },
                    {"停机码偏移","StopCodeOffset" },
                    {"plc","PlcNo" },
                };
                List<ParamEquipmentStatus> statuses = ExcelUtils.ImportExcel<ParamEquipmentStatus>(stream, keyValuePairs);

                int v = errorAndStatusLogic.InsertStatus(statuses, configId);
                if (v == 0)
                {
                    return Error("初始化数据失败");
                }
                return Success("初始化数据成功");
            }

            // 文件为空或上传失败的处理文件
            return Error();
        }

        [Route("param/status/export")]
        [HttpGet]
        public ActionResult Export(string configId)
        {
            
            List<ParamEquipmentStatus> statuses = errorAndStatusLogic.GetAllStatus(configId);

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
                    {"BigStationCode", "工位"},
                    {"EquipmentID", "设备"},
                    {"Offset","偏移" },
                    {"StopCodeOffset","停机码偏移" },
                    {"PlcNo","plc" },
                };

            // 填充数据到工作表
            // 将 ExcelPackage 转换为字节数组
            var bytes = ExcelUtils.ExportExcel(statuses, keyValuePairs, "error",true);

            // 创建文件流
            var stream = new MemoryStream(bytes);

            // 设置响应头，指定响应的内容类型和文件名
            Response.Headers.Add("Content-Disposition", "attachment; filename=exported-file.xlsx");
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        


    }
}
