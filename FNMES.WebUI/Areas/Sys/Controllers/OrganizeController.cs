using System.Collections.Generic;
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

namespace FNMES.WebUI.Areas.Sys.Controllers
{
    [HiddenApi]
    [Area("Sys")]
    public class OrganizeController : BaseController
    {
        private SysOrganizeLogic organizeLogic;

        public OrganizeController()
        {
            organizeLogic = new SysOrganizeLogic();
        }


        [Route("system/organize/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("system/organize/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord)
        {
            int totalCount = 0;
            var pageData = organizeLogic.GetList(pageIndex, pageSize, keyWord, ref totalCount);
            var result = new LayPadding<SysOrganize>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return Content(result.ToJson());
        }


      




        [Route("system/organize/form")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Form()
        {
            return View();
        }


        [Route("system/organize/form")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Form(SysOrganize model)
        {
            if (model.Id.IsNullOrEmpty())
            {
                int row = organizeLogic.Insert(model, OperatorProvider.Instance.Current.UserId);
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = organizeLogic.Update(model, OperatorProvider.Instance.Current.UserId);
                return row > 0 ? Success() : Error();
            }
        }

        


        [Route("system/organize/getForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetForm(string primaryKey)
        {
            var entity = organizeLogic.Get(primaryKey);
            return Content(entity.ToJson());
        }

       

        [Route("system/organize/delete")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string primaryKey)
        {
            int count = organizeLogic.GetChildCount(primaryKey);
            if (count == 0)
            {
                int row = organizeLogic.Delete(primaryKey);
                return row > 0 ? Success() : Error();
            }
            return Error(string.Format("操作失败，请先删除该项的{0}个子级机构。", count));
        }


       


        [Route("system/organize/detail")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Detail()
        {
            return View();
        }


        [Route("system/organize/getListTreeSelect")]
        [HttpGet, LoginChecked]
        public ActionResult GetListTreeSelect()
        {
            var data = organizeLogic.GetList();
            var treeList = new List<TreeSelect>();
            foreach (SysOrganize item in data)
            {
                TreeSelect model = new TreeSelect();
                model.id = item.Id;
                model.text = item.FullName;
                model.parentId = item.ParentId;
                treeList.Add(model);
            }
            return Content(treeList.ToTreeSelectJson());
        }

        /// <summary>
        /// 获取组织机构下拉列表
        /// </summary>
        /// <returns></returns>
        [Route("app/system/organize/getListTreeSelect")]
        [HttpGet]
        public ActionResult AppGetListTreeSelect()
        {
            var data = organizeLogic.GetList();
            var treeList = new List<TreeSelect>();
            foreach (SysOrganize item in data)
            {
                TreeSelect model = new TreeSelect();
                model.id = item.Id;
                model.text = item.FullName;
                model.parentId = item.ParentId;
                treeList.Add(model);
            }
            return AppSuccess<List<TreeSelect>>(treeList.ToTreeSelectJson().ToList<TreeSelect>());
        }
        /// <summary>
        /// 组织机构主界面数据
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/organize/index")]
        public ActionResult AppIndex([FromBody] SearchParms parms)
        {
            int totalCount = 0;
            var pageData = organizeLogic.GetList(parms.pageIndex, parms.pageSize, parms.keyWord, ref totalCount);
            var result = new LayPadding<SysOrganize>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return AppSuccess<LayPadding<SysOrganize>>(result);
        }

        /// <summary>
        /// 新增/修改组织机构数据提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/organize/form")]
        public ActionResult AppForm([FromBody] SysOrganize model)
        {
            if (model.Id.IsNullOrEmpty())
            {
                int row = organizeLogic.AppInsert(model, model.CreateUserId);
                return row > 0 ? AppSuccess() : AppError();
            }
            else
            {
                int row = organizeLogic.AppUpdate(model, model.ModifyUserId);
                return row > 0 ? AppSuccess() : AppError();
            }
        }

        /// <summary>
        /// 根据主键获取组织机构数据
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/organize/getForm")]
        public ActionResult AppGetForm([FromBody] StrPrimaryKeyParms parms)
        {
            SysOrganize entity = organizeLogic.Get(parms.primaryKey);
            if (entity == null)
            {
                return AppError("组织机构信息不存在");
            }
            return AppSuccess<SysOrganize>(entity);
        }


        /// <summary>
        /// 删除组织数据
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/organize/delete")]
        public ActionResult AppDelete([FromBody] StrPrimaryKeyParms parms)
        {
            int count = organizeLogic.GetChildCount(parms.primaryKey);
            if (count == 0)
            {
                int row = organizeLogic.Delete(parms.primaryKey);
                Logger.OperateInfo($"用户{parms.operateUser}删除了组织机构");
                return row > 0 ? AppSuccess() : AppError();
            }
            return AppError(string.Format("操作失败，请先删除该项的{0}个子级机构。", count));
        }
    }
}
