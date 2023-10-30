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
    public class StopController : BaseController
    {
        private readonly ErrorAndStatusLogic errorAndStatusLogic;
        public StopController()
        {
            errorAndStatusLogic = new ErrorAndStatusLogic();
        }


        [Route("param/stop/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("param/stop/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId)
        {
            try
            {
                int totalCount = 0;
                var pageData = errorAndStatusLogic.GetCodeList(pageIndex, pageSize, keyWord, configId, ref totalCount);
                var result = new LayPadding<ParamEquipmentStopCode>()
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
                return Content(new LayPadding<ParamEquipmentStopCode>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamEquipmentStopCode>(),
                    count =0
                }.ToJson()) ;
            }
        }


        [Route("param/stop/import")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Import()
        {
            return View();
        }

        [Route("param/stop/uploadFile")]
        [HttpPost]
        public async Task<ActionResult> UploadFile(IFormFile file,string configId)
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
                    {"停机代码","StopCode" },
                    {"停机描述","StopCodeDesc" },
                };
                List<ParamEquipmentStopCode> codes = ExcelUtils.ImportExcel<ParamEquipmentStopCode>(stream, keyValuePairs);

                int v = errorAndStatusLogic.InsertStopCode(codes, configId);
               if (v == 0)
                {
                    return Error("初始化数据失败");
                }
                return Success("初始化数据成功");
            }

            // 文件为空或上传失败的处理文件
            return Error();
        }


        [Route("param/stop/export")]
        [HttpGet]
        public ActionResult Export(string configId)
        {
            
            List<ParamEquipmentStopCode> statuses = errorAndStatusLogic.GetAllStopCode(configId);

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
                    {"StopCode", "停机代码"},
                    {"StopCodeDesc", "停机描述"},
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
