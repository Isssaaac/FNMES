using System;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using FNMES.WebUI.Logic.Param;
using FNMES.WebUI.Logic;
using FNMES.Entity.Param;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using FNMES.Utility.Files;
using CCS.WebUI;
using System.Threading.Tasks;
using System.Diagnostics;
using FNMES.Entity.DTO.ApiData;

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Param")]
    [HiddenApi]
    public class EsopController : BaseController
    {
        private readonly RecipeLogic  recipeLogic;
        public EsopController()
        {
            recipeLogic = new RecipeLogic();
        }









        [Route("param/esop/file")]
        [HttpGet]
        public ActionResult file(string filePath)
        {
            FTPparam fTPparam = AppSetting.FTPparam;
            FtpHelper ftpHelper = new FtpHelper(fTPparam.Host, fTPparam.Username, fTPparam.Password);

            try
            {
                var stream = ftpHelper.DownloadFileStream(filePath);
                // 设置响应头，指定响应的内容类型和文件名
                Response.Headers.Add("Content-Disposition", $"attachment; filename=downloaded-file.pdf");
                return File(stream, "application/pdf");
            }
            catch (Exception e )
            {
                Logger.ErrorInfo(e.Message);
                return Error("读取ftp文件流出错");
            }
        }

        [Route("param/esop/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("param/esop/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId,string productId)
        {
            try
            {
                int totalCount = 0;
                var pageData = recipeLogic.GetEsopList(pageIndex, pageSize, keyWord, configId, ref totalCount, long.Parse(productId));
                var result = new LayPadding<ParamEsopItem>()
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
                return Content(new LayPadding<ParamEsopItem>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamEsopItem>(),
                    count =0
                }.ToJson()) ;
            }
        }




        

        







              
    }
}
