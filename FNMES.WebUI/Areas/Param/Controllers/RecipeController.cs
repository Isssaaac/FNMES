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
using CCS.WebUI;
using FNMES.Utility.Files;

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Param")]
    [HiddenApi]
    public class RecipeController : BaseController
    {
        private readonly RecipeLogic  recipeLogic;
        public RecipeController()
        {
            recipeLogic = new RecipeLogic();
        }


        [Route("param/recipe/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("param/recipe/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId,string productId)
        {
            try
            {
                int totalCount = 0;
                var pageData = recipeLogic.GetList(pageIndex, pageSize, keyWord, configId, ref totalCount, long.Parse(productId));
                var result = new LayPadding<ParamRecipeItem>()
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
                return Content(new LayPadding<ParamRecipeItem>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamRecipeItem>(),
                    count =0
                }.ToJson()) ;
            }
        }



        [Route("param/recipe/param")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Param()
        {
            return View();
        }


        [Route("param/recipe/param")]
        [HttpPost]
        public ActionResult Param(int pageIndex, int pageSize, string keyWord, string configId, string primaryKey)
        {
            try
            {
                int totalCount = 0;
                var pageData = recipeLogic.GetParamList(pageIndex, pageSize, keyWord, configId, ref totalCount, long.Parse(primaryKey));
                var result = new LayPadding<ParamItem>()
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
                return Content(new LayPadding<ParamItem>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamItem>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("param/recipe/part")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Part()
        {
            return View();
        }

        [Route("param/recipe/part")]
        [HttpPost]
        public ActionResult Part(int pageIndex, int pageSize, string keyWord, string configId, string primaryKey)
        {
            try
            {
                int totalCount = 0;
                var pageData = recipeLogic.GetPartList(pageIndex, pageSize, keyWord, configId, ref totalCount, long.Parse(primaryKey));
                var result = new LayPadding<ParamPartItem>()
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
                return Content(new LayPadding<ParamPartItem>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamPartItem>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("param/recipe/apart")]
        [HttpPost]
        public ActionResult APart(int pageIndex, int pageSize, string keyWord, string configId, string primaryKey)
        {
            try
            {
                int totalCount = 0;
                var pageData = recipeLogic.GetAPartList(pageIndex, pageSize, keyWord, configId, ref totalCount, long.Parse(primaryKey));
                var result = new LayPadding<ParamAlternativePartItem>()
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
                return Content(new LayPadding<ParamAlternativePartItem>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamAlternativePartItem>(),
                    count = 0
                }.ToJson());
            }
        }



        [Route("param/recipe/esop")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Esop()
        {
            return View();
        }
        [Route("param/recipe/esop")]
        [HttpPost]
        public ActionResult Esop(int pageIndex, int pageSize, string keyWord, string configId, string primaryKey)
        {
            try
            {
                int totalCount = 0;
                var pageData = recipeLogic.GetEsopList(pageIndex, pageSize, keyWord, configId, ref totalCount, long.Parse(primaryKey));
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
                    count = 0
                }.ToJson());
            }
        }

        [Route("param/recipe/step")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Step()
        {
            return View();
        }
        [Route("param/recipe/step")]
        [HttpPost]
        public ActionResult Step(int pageIndex, int pageSize, string keyWord, string configId, string primaryKey)
        {
            try
            {
                int totalCount = 0;
                var pageData = recipeLogic.GetStepList(pageIndex, pageSize, keyWord, configId, ref totalCount, long.Parse(primaryKey));
                var result = new LayPadding<ParamStepItem>()
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
                return Content(new LayPadding<ParamStepItem>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamStepItem>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("param/recipe/esopFile")]
        [HttpGet]
        public ActionResult File(string filePath)
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
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return Error("读取ftp文件流出错");
            }
        }












    }
}
