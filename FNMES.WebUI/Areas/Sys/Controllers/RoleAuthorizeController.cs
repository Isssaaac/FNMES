using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Logic.Sys;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using FNMES.Entity.Sys;
using FNMES.Entity.DTO.Parms;

namespace MES.WebUI.Areas.Sys.Controllers
{
    [HiddenApi]
    [Area("Sys")]
    public class RoleAuthorizeController : BaseController
    {
        private SysRoleAuthorizeLogic roleAuthorizeLogic;
        private SysPermissionLogic permissionLogic;

        public RoleAuthorizeController()
        {
            roleAuthorizeLogic = new SysRoleAuthorizeLogic();
            permissionLogic = new SysPermissionLogic();
        }


        [Route("system/roleAuthorize/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("system/roleAuthorize/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(string roleId)
        {
            var listPerIds = roleAuthorizeLogic.GetList(roleId).Select(c => c.ModuleId).ToList();
            List<SysPermission> listAllPers;
            if (OperatorProvider.Instance.Current.Account == "admin")
            {
                listAllPers = permissionLogic.GetList();
            }
            else
            {
                listAllPers = permissionLogic.GetList(OperatorProvider.Instance.Current.UserId);
            }

            listAllPers = Handle(listAllPers);
            List<ZTreeNode> result = new List<ZTreeNode>();
            foreach (var item in listAllPers)
            {
                ZTreeNode model = new ZTreeNode();
                model.@checked = listPerIds.Contains(item.Id) ? model.@checked = true : model.@checked = false;
                model.id = item.Id;
                model.pId = item.ParentId;
                model.name = item.Name;
                model.open = true;
                result.Add(model);
            }
            return Content(result.ToJson());
        }



        [Route("system/roleAuthorize/form")]
        [HttpPost, LoginChecked]
        public ActionResult Form(string roleId, string perIds)
        {
            roleAuthorizeLogic.Authorize(roleId, OperatorProvider.Instance.Current.UserId, perIds.SplitToList().ToArray());
            return Success("授权成功");
        }


        /// <summary>
        /// 角色授权权限数据
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/roleAuthorize/index")]
        public ActionResult AppRoleAuthorizeData([FromBody] StrPrimaryKeyParms parms)
        {
            var listPerIds = roleAuthorizeLogic.GetList(parms.roleId).Select(c => c.ModuleId).ToList();
            List<SysPermission> listAllPers;
            if (new SysUserLogic().ContainsUser("admin", parms.operaterId))
            {
                listAllPers = permissionLogic.GetList();
            }
            else
            {
                listAllPers = permissionLogic.GetList(parms.operaterId);
            }

            listAllPers = Handle(listAllPers);
            List<ZTreeNode> result = new List<ZTreeNode>();
            foreach (var item in listAllPers)
            {
                ZTreeNode model = new ZTreeNode();
                model.@checked = listPerIds.Where(it => (it + "-view").StartsWith(item.Id)).Count() > 0 ? model.@checked = true : model.@checked = false;
                model.id = item.Id;
                model.pId = item.ParentId;
                model.name = item.Name;
                model.open = true;
                result.Add(model);
            }
            return AppSuccess<List<ZTreeNode>>(result);
        }

        /// <summary>
        /// 角色授权权限数据提交
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/roleAuthorize/form")]
        public ActionResult AppForm([FromBody] AuthorParms parms)
        {
            roleAuthorizeLogic.AppAuthorize(parms.operater, parms.roleId, parms.perIds.Select(it => it.Replace("-view", "")).Distinct().ToArray());
            return AppSuccess("授权成功");
        }

        /// <summary>
        /// 权限结构处理
        /// </summary>
        /// <param name="listAllPers"></param>
        /// <returns></returns>
        private List<SysPermission> Handle(List<SysPermission> listAllPers)
        {
            List<SysPermission> list = new List<SysPermission>();

            List<SysPermission> firstNode = listAllPers.Where(it => it.ParentId == "0").ToList();
            foreach (SysPermission permission in firstNode)
            {
                list.Add(permission);
                List<SysPermission> secondNode = listAllPers.Where(it => it.ParentId == permission.Id).ToList();
                foreach (SysPermission per in secondNode)
                {
                    list.Add(per);
                    List<SysPermission> thirdNode = listAllPers.Where(it => it.ParentId == per.Id).ToList();
                    list.Add(new SysPermission
                    {
                        Id = per.Id + "-view",
                        ParentId = per.Id,
                        Layer = 2,
                        EnCode = per.EnCode,
                        Name = "显示",
                    });
                    foreach (SysPermission per2 in thirdNode)
                    {
                        list.Add(per2);
                    }
                }
            }
            return list;
        }
    }
}
