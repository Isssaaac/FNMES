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
using FNMES.Utility.Network;
using FNMES.WebUI.API;
using FNMES.Entity.DTO.ApiParam;
using FNMES.WebUI.Logic.Sys;
using FNMES.Entity.Sys;
using FNMES.Entity.DTO.ApiData;
using FNMES.WebUI.Logic.Record;
using FNMES.Entity.Record;

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Record")]
    [HiddenApi]
    public class DetectController : BaseController
    {
        private readonly DetectDataLogic detectDataLogic;
        public DetectController()
        {
            detectDataLogic = new DetectDataLogic();
        }


        [Route("record/detect/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("record/detect/acr")]
        [HttpGet]
        public ActionResult ACR(int page, int limit, string keyWord,string configId,string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = detectDataLogic.GetACR(page, limit, keyWord, configId, ref totalCount, index);
                var result = new LayPadding<RecordTestACR>()
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
                return Content(new LayPadding<RecordTestACR>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordTestACR>(),
                    count =0
                }.ToJson()) ;
            }
        }

        [Route("record/detect/eol")]
        [HttpGet]
        public ActionResult EOL( int page, int limit, string keyWord, string configId, string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = detectDataLogic.GetEOL(page, limit, keyWord, configId, ref totalCount, index);
                var result = new LayPadding<RecordTestEOL>()
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
                return Content(new LayPadding<RecordTestEOL>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordTestEOL>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("record/detect/ocv")]
        [HttpGet]
        public ActionResult OCV(int page, int limit, string keyWord, string configId, string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = detectDataLogic.GetOCV(page, limit, keyWord, configId, ref totalCount, index);
                var result = new LayPadding<RecordTestOCV>()
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
                return Content(new LayPadding<RecordTestOCV>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordTestOCV>(),
                    count = 0
                }.ToJson());
            }
        }

        
    }
}
