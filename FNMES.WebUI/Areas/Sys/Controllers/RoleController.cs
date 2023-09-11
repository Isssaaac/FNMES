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

namespace MES.WebUI.Areas.Sys.Controllers
{
    [HiddenApi]
    [Area("Sys")]
    public class RoleController : BaseController
    {
        private SysRoleLogic roleLogic;
        private SysUserRoleRelationLogic roleRelationLogic;
        public RoleController()
        {
            roleLogic = new SysRoleLogic();
            roleRelationLogic = new SysUserRoleRelationLogic();
        }

        [Route("system/role/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }

        [Route("system/role/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord)
        {
            int totalCount = 0;
            var pageData = roleLogic.GetList(pageIndex, pageSize, keyWord, ref totalCount);
            var result = new LayPadding<SysRole>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount// pageData.Count
            };
            return Content(result.ToJson());
        }




        [Route("system/role/form")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Form()
        {
            return View();
        }

        [Route("system/role/form")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Form(SysRole model)
        {
            if (model.Id.IsNullOrEmpty())
            {
                int row = roleLogic.Insert(model, OperatorProvider.Instance.Current.UserId);
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = roleLogic.Update(model, OperatorProvider.Instance.Current.UserId);
                return row > 0 ? Success() : Error();
            }
        }





        [Route("system/role/detail")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Detail()
        {
            return View();
        }


        [Route("system/role/getForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetForm(string primaryKey)
        {
            SysRole entity = roleLogic.Get(primaryKey);
            entity.AllowEdit = entity.AllowEdit == "1" ? "true" : "false";
            return Content(entity.ToJson());
        }





        [Route("system/role/delete")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string primaryKey)
        {
            //判断这些权限是不是被用户绑定了，一旦绑定了，就不能删除，提示请先将用户解除绑定
            List<string> ids = primaryKey.SplitToList();
            List<SysUserRoleRelation> roleRelationList = roleRelationLogic.GetByRoles(ids);
            if (roleRelationList.Count > 0)
            {
                return Error("请先从用户中解除角色绑定");
            }
            int row = roleLogic.Delete(ids);
            return row > 0 ? Success() : Error();
        }



        [Route("system/role/getListTreeSelect")]
        [HttpPost, LoginChecked]
        public ActionResult GetListTreeSelect()
        {
            List<SysRole> listRole = roleLogic.GetList();
            var listTree = new List<TreeSelect>();
            foreach (var item in listRole)
            {
                TreeSelect model = new TreeSelect();
                model.id = item.Id;
                model.text = item.Name;
                listTree.Add(model);
            }
            return Content(listTree.ToJson());
        }


        /// <summary>
        /// 角色管理主界面
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/role/index")]
        public ActionResult AppIndex([FromBody] SearchParms parms)
        {
            int totalCount = 0;
            var pageData = roleLogic.GetList(parms.pageIndex, parms.pageSize, parms.keyWord, ref totalCount);
            var result = new LayPadding<SysRole>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount// pageData.Count
            };
            return AppSuccess<LayPadding<SysRole>>(result);
        }
        /// <summary>
        /// 增加/修改角色数据提交 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/role/form")]
        public ActionResult AppForm([FromBody] SysRole model)
        {
            if (model.Id.IsNullOrEmpty())
            {
                int row = roleLogic.AppInsert(model, model.CreateUserId);
                return row > 0 ? AppSuccess() : AppError();
            }
            else
            {
                int row = roleLogic.AppUpdate(model, model.ModifyUserId);
                return row > 0 ? AppSuccess() : AppError();
            }
        }
        /// <summary>
        /// 根据id获取角色信息
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/role/getForm")]
        public ActionResult AppGetForm([FromBody] StrPrimaryKeyParms parms)
        {
            SysRole entity = roleLogic.Get(parms.primaryKey);
            entity.AllowEdit = entity.AllowEdit == "1" ? "true" : "false";
            if (entity == null)
            {
                return AppError("角色信息不存在");
            }
            return AppSuccess<SysRole>(entity);
        }

        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/role/delete")]
        public ActionResult AppDelete([FromBody] RoleDeleteParms parms)
        {
            //判断这些权限是不是被用户绑定了，一旦绑定了，就不能删除，提示请先将用户解除绑定
            List<string> ids = parms.roleIdList;
            List<SysUserRoleRelation> roleRelationList = roleRelationLogic.GetByRoles(ids);
            if (roleRelationList.Count > 0)
            {
                return AppError("请先从用户中解除角色绑定");
            }
            int row = roleLogic.Delete(ids);
            return row > 0 ? AppSuccess() : AppError();
        }

        /// <summary>
        /// 获取角色下拉列表
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("app/system/role/getListTreeSelect")]
        public ActionResult AppGetListTreeSelect()
        {

            List<SysRole> listRole = roleLogic.GetList();
            var listTree = new List<TreeSelect>();
            foreach (var item in listRole)
            {
                TreeSelect model = new TreeSelect();
                model.id = item.Id;
                model.text = item.Name;
                listTree.Add(model);
            }
            return AppSuccess<List<TreeSelect>>(listTree);
        }
    }
}
