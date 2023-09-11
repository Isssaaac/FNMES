using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Logic.Sys;
using FNMES.Entity.Sys;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using FNMES.Entity.DTO.Parms;
using FNMES.Logic;

namespace MES.WebUI.Areas.Sys.Controllers
{
    [HiddenApi]
    [Area("Sys")]
    public class ItemsDetailController : BaseController
    {
        private SysItemsDetailLogic itemDetaillogic;

        public ItemsDetailController()
        {
            itemDetaillogic = new SysItemsDetailLogic();
        }


        [Route("system/itemsDetail/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }

        [Route("system/itemsDetail/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string itemId, string keyWord)
        {
            int totalCount = 0;
            var pageData = itemDetaillogic.GetList(pageIndex, pageSize, itemId, keyWord, ref totalCount);
            var result = new LayPadding<SysItemDetail>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return Content(result.ToJson());
        }

        [Route("system/itemsDetail/form")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Form()
        {
            return View();
        }


        [Route("system/itemsDetail/form")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Form(SysItemDetail model)
        {
            if (model.Id.IsNullOrEmpty())
            {
                int row = itemDetaillogic.Insert(model, OperatorProvider.Instance.Current.UserId);
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = itemDetaillogic.Update(model, OperatorProvider.Instance.Current.UserId);
                return row > 0 ? Success() : Error();
            }
        }

        [Route("system/itemsDetail/detail")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Detail()
        {
            return View();
        }
        [Route("system/itemsDetail/delete")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string primaryKey)
        {
            int row = itemDetaillogic.Delete(primaryKey);
            return row > 0 ? Success() : Error();
        }

        [Route("system/itemsDetail/getForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetForm(string primaryKey)
        {
            SysItemDetail entity = itemDetaillogic.Get(primaryKey);
            entity.IsDefault = entity.IsDefault == "1" ? "true" : "false";
            return Content(entity.ToJson());
        }


        /// <summary>
        /// 数据字典明细主界面数据
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/itemsDetail/index")]
        public ActionResult AppIndex([FromBody] ItemDetailIndexParms parms)
        {
            int totalCount = 0;
            var pageData = itemDetaillogic.GetList(parms.pageIndex, parms.pageSize, parms.itemId, "", ref totalCount);
            var result = new LayPadding<SysItemDetail>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return AppSuccess<LayPadding<SysItemDetail>>(result);
        }

        /// <summary>
        /// 新增/修改数据字典明细数据提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/itemsDetail/form")]
        public ActionResult AppForm([FromBody] SysItemDetail model)
        {
            if (model.Id.IsNullOrEmpty())
            {
                int row = itemDetaillogic.AppInsert(model, model.CreateUserId);
                return row > 0 ? AppSuccess() : AppError();
            }
            else
            {
                int row = itemDetaillogic.AppUpdate(model, model.ModifyUserId);
                return row > 0 ? AppSuccess() : AppError();
            }
        }

        /// <summary>
        /// 删除数据字典明细
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/itemsDetail/delete")]
        public ActionResult AppDelete([FromBody] StrPrimaryKeyParms parms)
        {
            int row = itemDetaillogic.Delete(parms.primaryKey);
            Logger.OperateInfo($"用户{parms.operateUser}删除了字典选项");
            return row > 0 ? AppSuccess() : AppError();
        }

        /// <summary>
        /// 根据主键获取数据字典明细
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/itemsDetail/getForm")]
        public ActionResult AppGetForm([FromBody] StrPrimaryKeyParms parm)
        {
            SysItemDetail entity = itemDetaillogic.Get(parm.primaryKey);
            if (entity == null)
            {
                return AppError("数据字典信息不存在");
            }
            entity.IsDefault = entity.IsDefault == "1" ? "true" : "false";
            return AppSuccess<SysItemDetail>(entity);
        }
    }
}
