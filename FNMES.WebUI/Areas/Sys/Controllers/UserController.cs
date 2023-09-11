using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Logic.Sys;
using FNMES.Entity.Sys;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using System.Collections.Generic;
using FNMES.Utility.Security;
using FNMES.Logic;
using FNMES.Entity.DTO.Parms;

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
            if (model.Id.IsNullOrEmpty())
            {
                DateTime defaultDt = DateTime.Today;
                DateTime.TryParse(model.StrBirthday + " 00:00:00", out defaultDt);
                model.Birthday = defaultDt;
                int row = userLogic.Insert(model, model.password, OperatorProvider.Instance.Current.UserId, model.roleIds.SplitToList().ToArray());
                return row > 0 ? Success() : Error();
            }
            else
            {
                DateTime defaultDt = DateTime.Today;
                DateTime.TryParse(model.StrBirthday + " 00:00:00", out defaultDt);
                model.Birthday = defaultDt;
                //更新用户基本信息。
                int row = userLogic.UpdateAndSetRole(model, OperatorProvider.Instance.Current.UserId, model.roleIds.SplitToList().ToArray());
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
            SysUser entity = userLogic.Get(primaryKey);
            entity.StrBirthday = entity.Birthday.Value.ToString("yyyy-MM-dd");
            entity.RoleId = userRoleRelationLogic.GetList(entity.Id).Select(c => c.RoleId).ToList();

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
            if (userIdList.Contains(OperatorProvider.Instance.Current.UserId))
            {
                return Error("不能删除自己");
            }
            //多用户删除。
            int row = userLogic.Delete(userIdList);
            userRoleRelationLogic.Delete(userIdList);
            userLogOnLogic.Delete(userIdList);
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
            SysUserLogOn sysUserLogOn = userLogOnLogic.GetByAccount(primaryKey);
            sysUserLogOn.Password = "123456".MD5Encrypt().DESEncrypt(sysUserLogOn.SecretKey).MD5Encrypt();
            int row = userLogOnLogic.UpdatePassword(sysUserLogOn);
            return row > 0 ? Success() : Error();
        }


        /// <summary>
        /// 用户管理主界面数据
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/user/index")]
        public ActionResult AppIndex([FromBody] SearchParms parms)
        {
            int totalCount = 0;
            var pageData = userLogic.GetList(parms.pageIndex, parms.pageSize, parms.keyWord, ref totalCount);
            var result = new LayPadding<SysUser>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return AppSuccess<LayPadding<SysUser>>(result);
        }

        /// <summary>
        /// 新增/修改用户数据提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/user/form")]
        public ActionResult AppForm([FromBody] SysUser model)
        {
            if (model.Id.IsNullOrEmpty())
            {
                var userEntity = userLogic.GetByUserName(model.Account);
                if (userEntity != null)
                {
                    return AppError("已存在当前用户名，请重新输入");
                }
                DateTime defaultDt = DateTime.Today;
                DateTime.TryParse(model.StrBirthday + " 00:00:00", out defaultDt);
                model.Birthday = defaultDt;
                int row = userLogic.AppInsert(model, model.password, model.roleIds.SplitToList().ToArray(), model.CreateUserId);
                return row > 0 ? AppSuccess() : AppError();
            }
            else
            {
                DateTime defaultDt = DateTime.Today;
                DateTime.TryParse(model.StrBirthday + " 00:00:00", out defaultDt);
                model.Birthday = defaultDt;
                //更新用户基本信息。
                int row = userLogic.AppUpdateAndSetRole(model, model.roleIds.SplitToList().ToArray(), model.ModifyUserId);
                //更新用户角色信息。
                return row > 0 ? AppSuccess() : AppError();
            }
        }

        /// <summary>
        /// 通过userId获取用户信息
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/user/getForm")]
        public ActionResult AppGetForm([FromBody] StrPrimaryKeyParms parms)
        {
            SysUser entity = userLogic.Get(parms.primaryKey);
            entity.StrBirthday = entity.Birthday.Value.ToString("yyyy-MM-dd");
            entity.RoleId = userRoleRelationLogic.GetList(entity.Id).Select(c => c.RoleId).ToList();
            return AppSuccess<SysUser>(entity);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/user/delete")]
        public ActionResult AppDelete([FromBody]UserDeleteParms parms)
        {
            //过滤系统管理员
            if (userLogic.ContainsUser("admin", parms.userIdList.ToArray()))
            {
                return AppError("系统管理员用户不能删除");
            }
            if (parms.userIdList.Contains(parms.currentUserId))
            {
                return AppError("不能删除自己");
            }
            //多用户删除。
            int row = userLogic.Delete(parms.userIdList);
            userRoleRelationLogic.Delete(parms.userIdList);
            userLogOnLogic.Delete(parms.userIdList);
            Logger.OperateInfo($"用户{parms.operateUser}删除了用户");
            return row > 0 ? AppSuccess() : AppError();
        } 
    }
}
