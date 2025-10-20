using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using FNMES.Entity.Enum;
using FNMES.Entity.Sys;
using FNMES.Utility.Files;
using FNMES.Utility.Operator;
using FNMES.Utility.ResponseModels;
using FNMES.WebUI.Filters;
using FNMES.Utility;
using FNMES.Utility.Core;
using FNMES.WebUI.Controllers;
using CCS.WebUI;
using FNMES.WebUI.Logic.Sys;
using FNMES.WebUI.Logic;
using Microsoft.Extensions.Localization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace FNMES.WebUI.Controllers
{
    [HiddenApi]
    public class HomeController : BaseController
    {
        private SysUserLogic userLogic;
        private SysUserLogOnLogic userLogOnLogic;
        private SysPermissionLogic permissionLogic;

        //注入本地化
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(IStringLocalizer<HomeController> localizer)
        {
            userLogic = new SysUserLogic();
            userLogOnLogic = new SysUserLogOnLogic();
            permissionLogic = new SysPermissionLogic();

            _localizer = localizer;
        }

        [Route("home/setlanguage")]
        [HttpPost]
        public IActionResult SetLanguage(string culture)
        {
            // 验证语言参数（只允许支持的语言）
            var supportedCultures = new[] { "zh-CN", "en-US","id-ID" };
            if (!supportedCultures.Contains(culture))
            {
                culture = "zh-CN"; // 默认为中文
            }

            // 设置语言Cookie（有效期1年，让浏览器记住偏好）
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            // 返回成功响应（前端可根据需要刷新页面或更新内容）
            return Json(new { success = true });
        }


        /// <summary>
        /// 后台首页视图。
        /// </summary>
        /// <returns></returns>
        [Route("home/index")]
        [HttpGet, LoginChecked]
        public ActionResult Index()
        {
            ViewBag.Welcome = _localizer["Welcome"];
            if (OperatorProvider.Instance.Current != null)
            {
                ViewBag.SoftwareName = AppSetting.WebSoftwareName;
                ViewBag.Copyright = AppSetting.Copyright;
                ViewBag.CurrentUser = OperatorProvider.Instance.Current;
                Logger.RunningInfo("open home index");
                return View();
            }
            else
            {
                return Redirect("/account/login");
            }
        }
        /// <summary>
        /// 默认显示视图。
        /// </summary>
        /// <returns></returns>
        [Route("home/default")]
        [HttpGet]
        public ActionResult Default()
        {
            return View();
        }

        /// <summary>
        /// 获取左侧菜单。这里牵涉到权限
        /// </summary>
        /// <returns></returns>
        [Route("home/getLeftMenu")]
        [HttpGet, LoginChecked]
        public ActionResult GetLeftMenu()
        {
            List<SysPermission> listModules;
            List<LayNavbar> listNavbar = new List<LayNavbar>();

            //如果是系统管理员，就应该具有所有的权限，不应该从角色权限表中获取
            string acccount = OperatorProvider.Instance.Current.Account;
            if (acccount == "admin")
            {
                listModules = permissionLogic.GetList();
            }
            else
            {
                long userId = long.Parse(OperatorProvider.Instance.Current.UserId);
                listModules = permissionLogic.GetList(userId);
            }
            foreach (var e in listModules)
            {
                //多语言切换需要
                e.Name = _localizer[e.EnCode];
            }
            foreach (var item in listModules.Where(c => c.Type == ModuleType.Menu && c.Layer == 0).ToList())
            {
                LayNavbar navbarEntity = new LayNavbar();
                var listChildNav = listModules.Where(c => c.Type == ModuleType.SubMenu && c.Layer == 1 && c.ParentId == item.Id).
                    Select(c => new LayChildNavbar() { href = c.Url, icon = c.Icon, title = c.Name }).ToList();

                navbarEntity.icon = item.Icon;
                navbarEntity.spread = false;
                navbarEntity.title = _localizer[item.Name];
                navbarEntity.children = listChildNav;
                listNavbar.Add(navbarEntity);
            }
            return Content(listNavbar.ToJson());
        }

        /// <summary>
        /// 获取登录用户权限。
        /// </summary>
        /// <returns></returns>
        [Route("home/getPermission")]
        [HttpGet, LoginChecked]
        public ActionResult GetPermission()
        {
            var account = OperatorProvider.Instance.Current.Account;
            if (account == "admin")
            {
                var permissionList = permissionLogic.GetList();
                foreach (var e in permissionList)
                {
                    //多语言切换需要
                    e.Name = _localizer[e.EnCode];
                }
                var permission = permissionList.ToJson();
                return Content(permission);
            }
            return Content(permissionLogic.GetList(long.Parse(OperatorProvider.Instance.Current.UserId)).ToJson());
        }
    }
}
