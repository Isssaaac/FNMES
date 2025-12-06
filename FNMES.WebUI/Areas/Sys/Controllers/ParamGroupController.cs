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
    public class ParamGroupController : BaseController
    {

        private readonly BaseLogic baseLogic;
        public ParamGroupController()
        {
            baseLogic = new BaseLogic();
        }

        [Route("system/paramgroup/index")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("system/paramgroup/index")]
        [HttpPost]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord)
        {
            int totalCount = 0;
            var pageData = baseLogic.GetTableList<SysParamGroup>(pageIndex, pageSize, ref totalCount, null);
            var result = new LayPadding<SysParamGroup>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return Content(result.ToJson());
        }

        [Route("system/paramgroup/add")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [Route("system/paramgroup/add")]
        [HttpPost]
        public async Task<ActionResult> Add(SysParamGroup data)
        {
            int ret = await baseLogic.InsertTableRowAsync(data, "default");
            return ret > 0 ? Success() : Error();
        }

        [Route("system/paramgroup/delete")]
        [HttpPost]
        public async Task<ActionResult> Delete(string primaryKey, string configId)
        {
            return await baseLogic.DeleteTableRowByIDAsync<SysParamGroup>(primaryKey, "default") > 0 ? Success() : Error();
        }

        [Route("system/paramgroup/getparamgroup")]
        [HttpPost]
        public async Task<ActionResult> GetParamGroup()
        {
            var operations = await baseLogic.GetTableListAsync<SysParamGroup>();
            var treeList = new List<TreeSelect>()
            {
                new TreeSelect
                {
                    id = "null",
                    text = "--请选择--",
                }
            };
            foreach (var item in operations)
            {
                TreeSelect model = new()
                {
                    id = item.Encode,
                    text = item.Encode,
                };
                treeList.Add(model);
            }
            return Content(treeList.ToJson());
        }
    }
}
