using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Entity.Sys;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using System.Linq;
using FNMES.WebUI.Logic;
using FNMES.WebUI.Logic.Sys;

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
            if (model.Id == 0)
            {
                int row = roleLogic.Insert(model, long.Parse(OperatorProvider.Instance.Current.UserId));
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = roleLogic.Update(model, long.Parse(OperatorProvider.Instance.Current.UserId));
                Logger.RunningInfo($"更新{row}行");
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
            SysRole entity = roleLogic.Get(long.Parse(primaryKey));
            entity.AllowEdit = entity.AllowEdit == "1" ? "true" : "false";
            return Content(entity.ToJson());
        }





        [Route("system/role/delete")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string primaryKey)
        {
            //判断这些权限是不是被用户绑定了，一旦绑定了，就不能删除，提示请先将用户解除绑定
            List<long> ids = primaryKey.SplitToList().Select(it => long.Parse(it)).ToList();
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
                model.id = item.Id.ToString();
                model.text = item.Name;
                listTree.Add(model);
            }
            return Content(listTree.ToJson());
        }
    }
}
