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
    [Area("Sys")]
    public class EquipmentController : BaseController
    {
        private readonly SysEquipmentLogic equipmentlogic;

        public EquipmentController()
        {
            equipmentlogic = new SysEquipmentLogic();
        }


        [Route("system/equipment/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }

        [Route("system/equipment/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string lineId, string keyWord)
        {
            int totalCount = 0;
            var pageData = equipmentlogic.GetList(pageIndex, pageSize, long.Parse(lineId), keyWord, ref totalCount);
            var result = new LayPadding<SysEquipment>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return Content(result.ToJson());
        }

        [Route("system/equipment/form")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Form()
        {
            return View();
        }


        [Route("system/equipment/form")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Form(SysEquipment model)
        {
            Logger.RunningInfo(model.ToJson());
            long modyfyId = long.Parse(OperatorProvider.Instance.Current.UserId);
            if (model.Id == 0)
            {
                int row = equipmentlogic.Insert(model, modyfyId);
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = equipmentlogic.Update(model, modyfyId);
                return row > 0 ? Success() : Error();
            }
        }

        [Route("system/equipment/detail")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Detail()
        {
            return View();
        }
        [Route("system/equipment/delete")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string primaryKey)
        {
            return equipmentlogic.Delete(long.Parse(primaryKey)) > 0 ? Success() : Error();
        }

        [Route("system/equipment/getForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetForm(string primaryKey)
        {
            SysEquipment entity = equipmentlogic.Get(long.Parse(primaryKey));
            return Content(entity.ToJson());
        }

     
    }
}
