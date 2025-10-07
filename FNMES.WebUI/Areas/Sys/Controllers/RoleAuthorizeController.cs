using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using FNMES.Entity.Sys;
using FNMES.WebUI.Logic.Sys;

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
            var listPerIds = roleAuthorizeLogic.GetList(long.Parse(roleId)).Select(c => c.PermissionId).ToList();
            List<SysPermission> listAllPers;
            if (OperatorProvider.Instance.Current.Account == "admin")
            {
                listAllPers = permissionLogic.GetList();
            }
            else
            {
                listAllPers = permissionLogic.GetList(long.Parse(OperatorProvider.Instance.Current.UserId));
            }

            listAllPers = Handle(listAllPers);
            List<ZTreeNode> result = new List<ZTreeNode>();
            bool temp = false;     //用于临时保持子菜单选中状态，提供3级的显示
            foreach (var item in listAllPers)
            {
                ZTreeNode model = new ZTreeNode();
                if (item.Id <= 10000)
                {
                    model.@checked = temp;
                }
                else {
                    model.@checked = listPerIds.Contains(item.Id) ? model.@checked = true : model.@checked = false;
                    temp = model.@checked;
                }
                model.id = item.Id.ToString();
                model.pId = item.ParentId.ToString();
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
            //1000以下值位显示行，不要绑定
            roleAuthorizeLogic.Authorize(long.Parse(roleId), long.Parse(OperatorProvider.Instance.Current.UserId), perIds.SplitToList().Select(it => long.Parse(it)).Where(it => it>1000).ToArray());
            return Success("授权成功");
        }



        /// <summary>
        /// 权限结构处理
        /// </summary>
        /// <param name="listAllPers"></param>
        /// <returns></returns>
        private List<SysPermission> Handle(List<SysPermission> listAllPers)
        {
            List<SysPermission> list = new List<SysPermission>();

            List<SysPermission> firstNode = listAllPers.Where(it => it.ParentId == 0).ToList();
            int i = 100;
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
                        Id = i++,
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
