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
using SqlSugar;

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Param")]
    [HiddenApi]
    public class RouteController : BaseController
    {
        private readonly SysLineLogic sysLineLogic;
        private readonly RouteLogic routeLogic;
        private readonly ParamProductLogic productLogic;
        private readonly UnitProcedureLogic unitProcedureLogic;  
        private readonly RecipeLogic recipeLogic;
        private readonly ParamRecipeItemLogic recipeItemLogic;

        public RouteController()
        {
            sysLineLogic = new SysLineLogic();
            routeLogic = new RouteLogic();
            productLogic = new ParamProductLogic();
            unitProcedureLogic = new UnitProcedureLogic();
            recipeLogic = new RecipeLogic();
            recipeItemLogic = new ParamRecipeItemLogic();
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
                var equipList = unitProcedureLogic.GetTableList<ParamUnitProcedure>(configId);
                List<ParamLocalRoute> pageData = routeLogic.GetList(pageIndex, pageSize, keyWord, configId, ref totalCount, productPartNo);
                foreach (var item in pageData)
                {
                    try
                    {
                        var stationName = equipList.Where(it => it.Encode == item.StationCode).Select(it => it.Name).FirstOrDefault();
                        item.StationName = stationName;
                    }
                    catch {
                        continue;
                    }
                }
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
           
            var equipList = unitProcedureLogic.GetTableList<ParamUnitProcedure>(model.ConfigId);
            var stationName = equipList.Where(it => it.Encode == model.StationCode).Select(it => it.Name).First();
            var recipId = recipeLogic.GetTableList<ParamRecipe>(model.ConfigId).Where(it => it.ProductPartNo == model.ProductPartNo).Select(e => e.Id).First();

            ParamRecipeItem paramRecipeItem = new ParamRecipeItem();
            paramRecipeItem.StationName = stationName;
            paramRecipeItem.StationCode = model.StationCode;
            paramRecipeItem.RecipeId = recipId;
            paramRecipeItem.Step = model.Step;
            paramRecipeItem.PassStationRestriction = model.Criterion;
            if (model.Id==0)
            {
                model.Id = SnowFlakeSingle.Instance.NextId();
                paramRecipeItem.Id = model.Id;
                recipeItemLogic.InsertTableRow(paramRecipeItem, model.ConfigId);
                int row = routeLogic.Insert(model,long.Parse(OperatorProvider.Instance.Current.UserId));
                return row > 0 ? Success() : Error();
            }
            else
            {
                paramRecipeItem.Id = model.Id;
                int row1 = recipeItemLogic.UpdateTable<ParamRecipeItem>(paramRecipeItem, model.ConfigId);
                int row2 = routeLogic.Update(model, long.Parse(OperatorProvider.Instance.Current.UserId));
                return row1 > 0 && row2 > 0 ? Success() : Error();
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


        [Route("param/route/getExistStation")]
        [HttpPost]
        public ActionResult GetExistStation(string productPartNo, string configId)
        {
            var entitys = unitProcedureLogic.GetParentList(configId);
            List<ParamLocalRoute> routes = new  List<ParamLocalRoute>();

            foreach (var e in entitys)
            {
                ParamLocalRoute route = new ParamLocalRoute();
                route.StationCode = e.Encode;
                route.StationName = e.Name;
                route.ProductPartNo = productPartNo;
                route.ConfigId = configId;
                route.Id = SnowFlakeSingle.Instance.NextId();
                route.CreateTime = DateTime.Now;
                
                routes.Add(route);
            }
            bool ret = routeLogic.Align(routes, productPartNo, configId);
            //ParamLocalRoute entity = routeLogic.Get(long.Parse(primaryKey), configId);
            return ret ? Success():Error() ;
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

            var v1 =  routeLogic.Delete(long.Parse(primaryId), configId);
            var v2 = recipeItemLogic.DeleteTableRowByID<ParamRecipeItem>(primaryId, configId);

            return (v1 > 0 && v2 > 0) ? Success() : Error();
        }
    }
}
