using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Entity.Sys;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using System.Collections.Generic;
using FNMES.Utility.Security;
using System.Diagnostics;
using FNMES.WebUI.Logic.Sys;
using FNMES.WebUI.Logic;

namespace MES.WebUI.Areas.Sys.Controllers
{
    [HiddenApi]
    [Area("Sys")]
    public class UserController : BaseController
    {
        private SysUserLogic userLogic;
        private SysUserRoleRelationLogic userRoleRelationLogic;
        private SysUserLogOnLogic userLogOnLogic;
        public UserController()
        {
            userLogic = new SysUserLogic();
            userRoleRelationLogic = new SysUserRoleRelationLogic();
            userLogOnLogic = new SysUserLogOnLogic();
        }


        [Route("system/user/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("system/user/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord)
        {
            int totalCount = 0;
            var pageData = userLogic.GetList(pageIndex, pageSize, keyWord, ref totalCount);
            var result = new LayPadding<SysUser>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return Content(result.ToJson());
        }




        [Route("system/user/form")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Form()
        {
            return View();
        }

        [Route("system/user/form")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Form(SysUser model)
        {
            
            if (model.Id==0)
            {
                var userEntity = userLogic.GetByUserName(model.UserNo);
                if (userEntity != null)
                {
                    return Error("操作失败，已存在该用户！");
                }

                DateTime defaultDt = DateTime.Today;
                int row = userLogic.Insert(model, model.password, long.Parse(OperatorProvider.Instance.Current.UserId), model.roleIds.SplitToList().ToArray());
                return row > 0 ? Success() : Error();
            }
            else
            {
                DateTime defaultDt = DateTime.Today;
                //更新用户基本信息。
                int row = userLogic.UpdateAndSetRole(model, long.Parse(OperatorProvider.Instance.Current.UserId), model.roleIds.SplitToList().Select(it => long.Parse(it)).ToArray());
                //更新用户角色信息。
                return row > 0 ? Success() : Error();
            }
        }





        [Route("system/user/detail")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Detail()
        {
            return View();
        }


        [Route("system/user/getForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetForm(string primaryKey)
        {
            SysUser entity = userLogic.Get(long.Parse(primaryKey));
            entity.RoleId = userRoleRelationLogic.GetList(entity.Id).Select(c => c.RoleId.ToString()).ToList();
            entity.roleIds = string.Join(",", entity.RoleId);
            Logger.RunningInfo("获取用户表单成功");
            return Content(entity.ToJson());
        }





        [Route("system/user/delete")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string userIds)
        {
            List<string> userIdList = userIds.SplitToList();
            //过滤系统管理员
            if (userLogic.ContainsUser("admin", userIdList.ToArray()))
            {
                return Error("系统管理员用户不能删除");
            }
            if (userIdList.Contains(OperatorProvider.Instance.Current.UserId.ToString()))
            {
                return Error("不能删除自己");
            }
            //多用户删除。
            List<long> users = userIdList.Select(it => long.Parse(it)).ToList(); 
            int row = userLogic.Delete(users);
            userRoleRelationLogic.Delete(users);
            userLogOnLogic.Delete(users);
            Logger.RunningInfo("删除用户表单成功");
            return row > 0 ? Success() : Error();
        } 


        [Route("system/user/checkAccount")]
        [HttpPost, LoginChecked]
        public ActionResult CheckAccount(string userName)
        {
            var userEntity = userLogic.GetByUserName(userName);
            if (userEntity != null)
            {
                return Error("已存在当前用户名，请重新输入");
            }
            return Success("恭喜您，该用户名可以注册");
        }

        [HttpPost, Route("system/user/reset"), AuthorizeChecked]
        public ActionResult Reset(string primaryKey)
        {
            //根据用户Id得到用户SecurityKey
            SysUserLogOn sysUserLogOn = userLogOnLogic.GetByAccount(long.Parse(primaryKey));
            sysUserLogOn.Password = "123456".MD5Encrypt().DESEncrypt(sysUserLogOn.SecretKey).MD5Encrypt();
            int row = userLogOnLogic.UpdatePassword(sysUserLogOn);
            return row > 0 ? Success() : Error();
        }
              
    }
}
