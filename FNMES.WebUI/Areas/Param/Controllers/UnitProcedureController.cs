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

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Param")]
    public class UnitProcedureController : BaseController
    {
        private readonly UnitProcedureLogic unitProcedureLogic;
        public UnitProcedureController()
        {
            unitProcedureLogic = new UnitProcedureLogic();
        }

        [Route("param/unitProcedure/getParent")]
        [HttpPost]
        public ActionResult GetParent(string configId)
        {
            var data = unitProcedureLogic.GetParentList(configId);
            var treeList = new List<TreeSelect>();
            
            foreach (ParamUnitProcedure item in data)
            {
                TreeSelect model = new()
                {
                    id = item.Id.ToString(),
                    text = item.Name,
                };
                treeList.Add(model);
            }
            return Content(treeList.ToJson());
        }
        
  




        [Route("param/unitProcedure/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("param/unitProcedure/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId)
        {
            try
            {
                int totalCount = 0;
                var pageData = unitProcedureLogic.GetList(pageIndex, pageSize, keyWord, configId, ref totalCount);
                var result = new LayPadding<ParamUnitProcedure>()
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

                return Error(E.Message);
            }
        }




        [Route("param/unitProcedure/form")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Form()
        {
            return View();
        }

        [Route("param/unitProcedure/form")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Form( ParamUnitProcedure model)
        {

            Logger.RunningInfo(model.ToJson()+"数据库"+model.ConfigId);
            
            if (model.Id==0)
            {
                int row = unitProcedureLogic.Insert(model,long.Parse(OperatorProvider.Instance.Current.UserId));
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = unitProcedureLogic.Update(model, long.Parse(OperatorProvider.Instance.Current.UserId));
                return row > 0 ? Success() : Error();
            }
        }





        [Route("param/unitProcedure/detail")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Detail()
        {
            return View();
        }


        [Route("param/unitProcedure/getForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetForm(string primaryKey, string configId)
        {
            ParamUnitProcedure entity = unitProcedureLogic.Get(long.Parse(primaryKey),configId);
            return Content(entity.ToJson());
        }





        [Route("param/unitProcedure/delete")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string primaryId, string configId)
        {
            
            /*//过滤系统管理员
            if (productStepLogic.ContainsUser("admin", userIdList.ToArray()))
            {
                return Error("产品有已设置的配方，不允许删除");
            }*/
            return unitProcedureLogic.Delete(long.Parse(primaryId), configId) > 0 ? Success() : Error();
        } 



              
    }
}
