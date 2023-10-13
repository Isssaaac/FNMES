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

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Param")]
    public class OrderController : BaseController
    {
        private readonly ParamOrderLogic orderLogic;
        public OrderController()
        {
            orderLogic = new ParamOrderLogic();
        }


        [Route("param/order/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("param/order/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId,string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = orderLogic.GetList(pageIndex, pageSize, keyWord, configId, ref totalCount, index);
                var result = new LayPadding<ParamOrder>()
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
                return Content(new LayPadding<ParamOrder>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamOrder>(),
                    count =0
                }.ToJson()) ;
            }
        }


        [Route("param/order/detail")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Detail()
        {
            return View();
        }


        [Route("param/order/getForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetForm(string primaryKey, string configId)
        {
            ParamOrder entity = orderLogic.Get(long.Parse(primaryKey),configId);
            return Content(entity.ToJson());
        }



        

    }
}
