using System.Collections.Generic;
using FNMES.WebUI.Filters;
using FNMES.Entity.Sys;
using FNMES.Utility.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using FNMES.WebUI.Logic.Base;
using System.Threading.Tasks;

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
        public async Task<ActionResult> Add(SysOperation data)
        {
            int ret = await baseLogic.InsertTableRowAsync(data, "default");
            return ret > 0 ? Success() : Error();
        }

        [Route("system/operation/delete")]
        [HttpPost]
        public async Task<ActionResult> Delete(string primaryKey, string configId)
        {
            return await baseLogic.DeleteTableRowByIDAsync<SysOperation>(primaryKey, "default") > 0 ? Success() : Error();
        }

        [Route("system/operation/getoperaion")]
        [HttpPost]
        public async Task<ActionResult> GetOperaion()
        {
            var operations = await baseLogic.GetTableListAsync<SysOperation>();
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

        [Route("system/operation/form")]
        [HttpGet, LoginChecked]
        public ActionResult Form()
        {
            return View();
        }

        [Route("system/operation/getForm")]
        [HttpPost, LoginChecked]
        public async Task<ActionResult> GetForm(string primaryKey)
        {
            SysOperation entity = await baseLogic.GetTableRowByIDAsync<SysOperation>(primaryKey, "default");
            return Content(entity.ToJson());
        }

        [Route("system/operation/form")]
        [HttpPost, LoginChecked]
        public async Task<ActionResult> Form(SysOperation model)
        {
            int row = await baseLogic.UpdateTableAsync(model, "default");
            return row > 0 ? Success() : Error();
        }
    }
}
