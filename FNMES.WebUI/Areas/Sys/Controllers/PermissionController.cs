using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Entity.Sys;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using System.Collections;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using FNMES.WebUI.Logic.Sys;
using FNMES.WebUI.Logic;

namespace MES.WebUI.Areas.Sys.Controllers
{
    [HiddenApi]
    [Area("Sys")]
    public class PermissionController : BaseController
    {
        private readonly UnitProcedureLogic logic;

        /// <summary>
        /// 构造方法
        /// </summary>
        public PermissionController()
        {
            logic = new UnitProcedureLogic();
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
            switch (model.Type)
            {
                case 2:   //主菜单
                    model.ParentId = 0;
                    break;
                case 3:    //程序主菜单
                    {
                        model.ParentId = 0;
                        break;
                    }
                case 0:    //子菜单
                    {
                        SysPermission permission = logic.Get(model.ParentId);
                        if (permission.Type != 2)
                        { return Error("当前类型的父级必须为主菜单"); }
                        break;
                    }
                case 4:    //程序子菜单
                    {
                        SysPermission permission = logic.Get(model.ParentId);
                        if (permission.Type != 3)
                        { return Error("当前类型的父级必须为程序主菜单"); }
                        break;
                    }
                case 1:    //按钮
                    {
                        SysPermission permission = logic.Get(model.ParentId);
                        if (permission.Type != 0)
                        { return Error("当前类型的父级必须为子菜单"); }
                        break;
                    }
                case 5:   //程序按钮
                    {
                        SysPermission permission = logic.Get(model.ParentId);
                        if (permission.Type != 4)
                        { return Error("当前类型的父级必须为程序子菜单"); }
                        break;
                    }
            }
            if (model.Id == 0)
            {
                int row = logic.Insert(model, long.Parse(OperatorProvider.Instance.Current.UserId));
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = logic.Update(model, long.Parse(OperatorProvider.Instance.Current.UserId));
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
            long count = logic.GetChildCount(long.Parse(primaryKey));
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
            SysPermission entity = logic.Get(long.Parse(primaryKey));
            entity.IsEdit = entity.IsEdit == "1" ? "true" : "false";
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
                TreeSelect model = new()
                {
                    id = item.Id.ToString(),
                    text = item.Name,
                    parentId = item.ParentId.ToString()
                };
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

    }

}
