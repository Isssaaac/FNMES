using FNMES.Entity.Sys;
using FNMES.Logic.Sys;
using FNMES.Utility.Core;
using FNMES.Utility.Extension;
using FNMES.Utility.Operator;
using FNMES.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.WebUI.Controllers
{
    [HiddenApi]
    public class IndexController : BaseController
    {
        [Route("ueditor.html")]
        [HttpGet]
        public ActionResult UEditor()
        {
            return View();
        }

        [Route("video.html")]
        [HttpGet]
        public ActionResult Video()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PrintTest()
        {
            return View();
        }

        /// <summary>
        /// 主题修改页面显示
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("theme"), LoginChecked]
        public ActionResult Theme()
        {
            string userId = OperatorProvider.Instance.Current.UserId;
            SysUserLogOn userLogOn = new SysUserLogOnLogic().GetByAccount(userId);
            ViewBag.Theme = userLogOn.Theme;
            return View();
        }


        /// <summary>
        /// 主题修改显示
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        [HttpPost, Route("theme"), LoginChecked]
        public ActionResult Theme(string theme)
        {
            Operator user = OperatorProvider.Instance.Current;
            SysUserLogOn userLogOn = new SysUserLogOnLogic().GetByAccount(user.UserId);
            userLogOn.Theme = theme;
            int row = new SysUserLogOnLogic().UpdateTheme(userLogOn);
            if (row > 0)
            {
                user.Theme = theme;
                OperatorProvider.Instance.Current = user;
                return Success();
            }
            return Error();
        }
    }
}
