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

namespace MES.WebUI.Areas.Sys.Controllers
{
    [HiddenApi]
    [Area("Sys")]
    public class PermissionController : BaseController
    {
        private SysPermissionLogic logic;

        /// <summary>
        /// 构造方法
        /// </summary>
        public PermissionController()
        {
            logic = new SysPermissionLogic();
        }


        /// <summary>
        /// 主界面
        /// </summary>
        /// <returns></returns> 
        [HttpGet, Route("system/permission/index"), AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }



        /// <summary>
        /// 主界面条件检索数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        [HttpPost, Route("system/permission/index"), AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord)
        {
            int totalCount = 0;
            var pageData = logic.GetList(pageIndex, pageSize, keyWord, ref totalCount);
            var result = new LayPadding<SysPermission>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count,
            };
            return Content(result.ToJson());
        }




        [Route("system/permission/form")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Form()
        {
            return View();
        }



        [HttpPost, Route("system/permission/form"), AuthorizeChecked]
        public ActionResult Form(SysPermission model)
        {
            //当前类型是啥
            if (model.Type == 2)
            {
                model.ParentId = "0";
            }
            else if (model.Type == 0)
            {
                SysPermission permission = logic.Get(model.ParentId);
                if (permission.Type != 2)
                    return Error("当前类型的父级必须为主菜单");
            }
            else
            {
                SysPermission permission = logic.Get(model.ParentId);
                if (permission.Type != 0)
                    return Error("当前类型的父级必须为子菜单");
            }
            if (model.Id.IsNullOrEmpty())
            {
                int row = logic.Insert(model, OperatorProvider.Instance.Current.UserId);
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = logic.Update(model, OperatorProvider.Instance.Current.UserId);
                return row > 0 ? Success() : Error();
            }
        }


        [HttpGet, Route("system/permission/detail"), AuthorizeChecked]
        public ActionResult Detail()
        {
            return View();
        }



        [HttpPost, Route("system/permission/delete"), AuthorizeChecked]
        public ActionResult Delete(string primaryKey)
        {
            long count = logic.GetChildCount(primaryKey);
            if (count == 0)
            {
                int row = logic.Delete(primaryKey.SplitToList().ToArray());
                return row > 0 ? Success() : Error();
            }
            return Error(string.Format("操作失败，请先删除该项的{0}个子级权限。", count));
        }



        [Route("system/permission/getForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetForm(string primaryKey)
        {
            SysPermission entity = logic.Get(primaryKey);
            entity.IsEdit = entity.IsEdit == "1" ? "true" : "false"; 
            entity.IsPublic = entity.IsPublic == "1" ? "true" : "false";
            return Content(entity.ToJson());
        }




        [Route("system/permission/getParent")]
        [HttpPost]
        public ActionResult GetParent()
        {
            var data = logic.GetList();
            var treeList = new List<TreeSelect>();
            foreach (SysPermission item in data)
            {
                TreeSelect model = new TreeSelect();
                model.id = item.Id;
                model.text = item.Name;
                model.parentId = item.ParentId;
                treeList.Add(model);
            }
            return Content(treeList.ToTreeSelectJson());
        }

        [Route("system/permission/icon")]
        [HttpGet]
        public ActionResult Icon()
        {
            return View();
        }


        /// <summary>
        /// 权限管理主界面数据
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/permission/index")]
        public ActionResult AppIndex([FromBody] SearchParms parms)
        {
            int totalCount = 0;
            var pageData = logic.GetList(parms.pageIndex, parms.pageSize, parms.keyWord, ref totalCount);
            var result = new LayPadding<SysPermission>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return AppSuccess<LayPadding<SysPermission>>(result);
        }

        /// <summary>
        /// 新增/修改权限数据提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/permission/form")]
        public ActionResult AppForm([FromBody] SysPermission model)
        {
            SysPermissionLogic pLogic = new SysPermissionLogic();
            //判断类型是啥
            if (model.Type == 2)
            {
                model.ParentId = "0";
            }
            else if (model.Type == 0)
            {
                SysPermission permission = pLogic.Get(model.ParentId);
                if (permission.Type != 2)
                {
                    return AppError("当前类型的父级必须为主菜单");
                }
            }
            else
            {
                SysPermission permission = pLogic.Get(model.ParentId);
                if (permission.Type != 0)
                {
                    return AppError("当前类型的父级必须为子菜单");
                }
            }
            if (model.Id.IsNullOrEmpty())
            {
                int row = logic.AppInsert(model, model.CreateUserId);
                return row > 0 ? AppSuccess() : AppError();
            }
            else
            {
                int row = logic.AppUpdate(model, model.ModifyUserId);
                return row > 0 ? AppSuccess() : AppError();
            }
        }

        /// <summary>
        /// 通过Id获取权限权限
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/permission/getForm")]
        public ActionResult AppGetForm([FromBody] StrPrimaryKeyParms parms)
        {
            SysPermission entity = logic.Get(parms.primaryKey);
            if (entity == null)
            {
                return AppError("权限信息不存在");
            }
            return AppSuccess<SysPermission>(entity);
        }


        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/permission/delete")]
        public ActionResult AppDelete([FromBody] StrPrimaryKeyParms parms)
        {
            long count = logic.GetChildCount(parms.primaryKey);
            if (count > 0)
            {
                return AppError($"操作失败，请先删除该项的{count}个子级权限。");
            }
            int row = logic.Delete(parms.primaryKey.SplitToList().ToArray());
            if (row == 0)
            {
                return AppError($"对不起，操作失败");
            }
            Logger.OperateInfo($"用户{parms.operateUser}删除了用权限");
            return row > 0 ? AppSuccess() : AppError();
        }


        /// <summary>
        /// 获取父权限
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("app/system/permission/getParent")]
        public ActionResult AppGetParent()
        {
            var data = logic.GetList();
            var treeList = new List<TreeSelect>();
            foreach (SysPermission item in data)
            {
                TreeSelect model = new TreeSelect();
                model.id = item.Id;
                model.text = item.Name;
                model.parentId = item.ParentId;
                treeList.Add(model);
            }
            return AppSuccess<List<TreeSelect>>(treeList.ToTreeSelectJson().ToList<TreeSelect>());
        }
    }

}
