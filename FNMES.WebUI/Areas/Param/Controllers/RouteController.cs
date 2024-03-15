using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Entity.Sys;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using System.Collections.Generic;
using FNMES.WebUI.Logic.Param;
using FNMES.WebUI.Logic;
using FNMES.Entity.Param;
using FNMES.WebUI.Logic.Sys;
using System.Drawing.Drawing2D;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Drawing.Printing;

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Param")]
    [HiddenApi]
    public class RouteController : BaseController
    {
        private readonly SysLineLogic sysLineLogic;
        private readonly RouteLogic routeLogic;
        private readonly ParamProductLogic productLogic;


        public RouteController()
        {
            sysLineLogic = new SysLineLogic();
            routeLogic = new RouteLogic();
            productLogic = new ParamProductLogic();
        }

        [Route("param/route/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("param/route/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId,string productPartNo)
        {
            try
            {
                int totalCount = 0;
                List<ParamLocalRoute> pageData = routeLogic.GetList(pageIndex, pageSize, keyWord, configId, ref totalCount,productPartNo);
                LayPadding<ParamLocalRoute> result = new()
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
                return Content(new LayPadding<ParamLocalRoute>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamLocalRoute>(),
                    count = 0
                }.ToJson());
            }
        }




        [Route("param/route/form")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Form()
        {
            return View();
        }

        [Route("param/route/form")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Form(ParamLocalRoute model)
        {

            //Logger.RunningInfo(model.ToJson()+"数据库"+model.ConfigId);
            
            if (model.Id==0)
            {
                int row = routeLogic.Insert(model,long.Parse(OperatorProvider.Instance.Current.UserId));
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = routeLogic.Update(model, long.Parse(OperatorProvider.Instance.Current.UserId));
                return row > 0 ? Success() : Error();
            }
        }





        [Route("param/route/detail")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Detail()
        {
            return View();
        }


        [Route("param/route/getForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetForm(string primaryKey, string configId)
        {
            ParamLocalRoute entity = routeLogic.Get(long.Parse(primaryKey),configId);
            return Content(entity.ToJson());
        }





        [Route("param/route/delete")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string primaryId, string configId)
        {
            
            /*//过滤系统管理员
            if (productStepLogic.ContainsUser("admin", userIdList.ToArray()))
            {
                return Error("产品有已设置的配方，不允许删除");
            }*/
            return routeLogic.Delete(long.Parse(primaryId), configId) > 0 ? Success() : Error();
        }


        


    }
}
