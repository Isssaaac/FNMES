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


        //[Route("record/bind/data")]
        //[HttpGet]
        //public ActionResult Index(int page, int limit, string keyWord, string configId, string index)
        //{
        //    try
        //    {
        //        int totalCount = 0;
        //        var pageData = bindLogic.GetList(page, limit, keyWord, configId, ref totalCount, index);
        //        var result = new LayPadding<ProcessBind>()
        //        {
        //            result = true,
        //            msg = "success",
        //            list = pageData,
        //            count = totalCount//pageData.Count
        //        };
        //        return Content(result.ToJson());
        //    }
        //    catch (Exception E)
        //    {
        //        return Content(new LayPadding<ProcessBind>()
        //        {
        //            result = false,
        //            msg = E.Message,
        //            list = new List<ProcessBind>(),
        //            count = 0
        //        }.ToJson());
        //    }
        //}


        [Route("record/bind/index")]
        [HttpPost]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId,string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = bindLogic.GetList(pageIndex, pageSize, keyWord, configId, ref totalCount, index);
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

        #region 2024-04-12 添加快速绑定与解绑功能

        [Route("/record/bind/form")]
        [HttpGet]
        public ActionResult Form()
        {
            return View();
        }

        [Route("/record/bind/form")]
        [HttpPost]
        public ActionResult GetBind(int pageSize, string configId)
        {
            try
            {
                int totalCount = 0;
                var pageData = bindLogic.GetList(1, pageSize, "", configId, ref totalCount, "1");
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
                    count = 0
                }.ToJson());
            }
        }

        [Route("/record/bind/binding")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Binding(long id, string palletNo, string configId)
        {
            return bindLogic.FastBinding(id, palletNo, configId) > 0 ? Success() : Error();
        }

        [Route("/record/bind/unbinding")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Unbinding(long id, string configId)
        {
            return bindLogic.FastUnbinding(id , configId) > 0 ? Success() : Error();
        }

        #endregion

    }
}
