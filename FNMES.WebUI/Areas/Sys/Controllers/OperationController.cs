using System.Collections.Generic;
using FNMES.WebUI.Filters;
using FNMES.Entity.Sys;
using FNMES.Utility.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using FNMES.WebUI.Logic.Base;

namespace FNMES.WebUI.Logic.Sys
{
    [HiddenApi]
    [Area("Sys")]
    public class OperationController : BaseController
    {

        private readonly BaseLogic baseLogic;
        public OperationController()
        {
            baseLogic = new BaseLogic();
        }

        [Route("system/operation/index")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("system/operation/index")]
        [HttpPost]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord)
        { 
            int totalCount = 0;
            var pageData = baseLogic.GetTableList<SysOperation>(pageIndex, pageSize, ref totalCount, null);
            var result = new LayPadding<SysOperation>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return Content(result.ToJson());
        }

        [Route("system/operation/add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [Route("system/operation/add")]
        [HttpPost]
        public ActionResult Add(SysOperation data)
        {
            int ret = baseLogic.InsertTableRow(data, "default");
            return ret > 0 ? Success() : Error();
        }

        [Route("system/operation/delete")]
        [HttpPost]
        public ActionResult Delete(string primaryKey, string configId)
        {
            return baseLogic.DeleteTableRowByID<SysOperation>(primaryKey, "default") > 0 ? Success() : Error();
        }

        [Route("system/operation/getoperaion")]
        [HttpPost]
        public ActionResult GetOperaion()
        {
            var operations = baseLogic.GetTableList<SysOperation>();
            var treeList = new List<TreeSelect>();
            //{
            //   new TreeSelect
            //   {
            //        id = "null",
            //        text = "--请选择--",
            //   }
            //};
            foreach (var item in operations)
            {
                TreeSelect model = new()
                {
                    id = item.Name,
                    text = item.Name,
                };
                treeList.Add(model);
            }
            return Content(treeList.ToJson());
        }
    }
}
