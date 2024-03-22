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

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Record")]
    [HiddenApi]
    public class BindController : BaseController
    {
        private readonly ProcessBindLogic bindLogic;
        public BindController()
        {
            bindLogic = new ProcessBindLogic();
        }


        [Route("record/bind/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("record/bind/data")]
        [HttpGet]
        public ActionResult Index(int page, int limit, string keyWord,string configId,string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = bindLogic.GetList(page, limit, keyWord, configId, ref totalCount, index);
                var result = new LayPadding<ProcessBind>()
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
                return Content(new LayPadding<ProcessBind>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ProcessBind>(),
                    count =0
                }.ToJson()) ;
            }
        }

    }
}
