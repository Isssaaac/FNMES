using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Entity.Sys;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using FNMES.WebUI.Logic.Sys;
using FNMES.WebUI.Logic;

namespace MES.WebUI.Areas.Sys.Controllers
{
    [HiddenApi]
    [Area("Param")]
    public class PreEquipmentController : BaseController
    {
        private readonly SysPreProductLogic sysPreProductLogic;

        public PreEquipmentController()
        {
            sysPreProductLogic = new SysPreProductLogic();
        }


        [Route("param/preEquipment/index")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [Route("param/preEquipment/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string lineId, string keyWord,string index)
        {
            int totalCount = 0;
            var pageData = sysPreProductLogic.GetList(pageIndex, pageSize, long.Parse(lineId), keyWord, ref totalCount, index);
            var result = new LayPadding<SysPreSelectProduct>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return Content(result.ToJson());
        }
     
    }
}
