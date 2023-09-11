using FNMES.Utility.Files;
using FNMES.Utility.Web;

#if !NETFRAMEWORK
using Microsoft.AspNetCore.Http;
#else
using System.Configuration;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FNMES.Utility.Extension;
using FNMES.Utility.Security;
using FNMES.Utility.Core;

namespace FNMES.Utility.Operator
{
    /// <summary>
    /// 用户登陆信息提供者。
    /// </summary>
    public class OperatorProvider
    {

        /// <summary>
        /// Session/Cookie键。
        /// </summary>
        private const string LOGIN_USER_KEY = "LoginUser";

        private OperatorProvider() { }

        static OperatorProvider() { }

        //使用内部类+静态构造函数实现延迟初始化。
        class Nested
        {
            static Nested()
            {

            }
            public static readonly OperatorProvider instance = new OperatorProvider();
        }
        /// <summary>
        /// 在大多数情况下，静态初始化是在.NET中实现Singleton的首选方法。
        /// </summary>
        public static OperatorProvider Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        /// <summary>
        /// 从配置文件读取登陆提供者模式(Session/Cookie)。
        /// </summary>
        private string LoginProvider = ConfigurationManager.AppSettings["LoginProvider"];

        /// <summary>
        /// 从配置文件读取登陆用户信息保存时间。
        /// </summary>
        private int LoginTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["LoginTimeout"]);

        /// <summary>
        /// 从Session/Cookie获取或设置用户操作模型。
        /// </summary>
        /// <returns></returns>
        //public Operator GetCurrent(HttpContext context)
        //{
        //    Operator operatorModel = new Operator();
        //    operatorModel = context.Session.GetString(LOGIN_USER_KEY).DESDecrypt().ToObject<Operator>();
        //    return operatorModel;
        //}

        public Operator Current
        {
            get
            {
                Operator operatorModel = new Operator();
#if NETFRAMEWORK
                if (LoginProvider == "Cookie")
                {
                    operatorModel = WebHelper.GetCookie(LOGIN_USER_KEY).DESDecrypt().ToObject<Operator>();
                }
                else
                {
                    operatorModel = WebHelper.GetSession(LOGIN_USER_KEY).DESDecrypt().ToObject<Operator>();
                }
                return operatorModel;
#else
                operatorModel = MyHttpContext.httpContext.Session.GetString(LOGIN_USER_KEY).DESDecrypt().ToObject<Operator>();
                return operatorModel;
#endif
            }
            set
            {
#if NETFRAMEWORK
                if (LoginProvider == "Cookie")
                {
                    WebHelper.SetCookie(LOGIN_USER_KEY, value.ToJson().DESEncrypt(), LoginTimeout);
                }
                else
                {
                    WebHelper.SetSession(LOGIN_USER_KEY, value.ToJson().DESEncrypt(), LoginTimeout);
                }
#else
                MyHttpContext.httpContext.Session.SetString(LOGIN_USER_KEY, value.ToJson().DESEncrypt());
#endif

            }
        }

        //public void SetCurrent(HttpContext context, string value)
        //{
        //    context.Session.SetString(LOGIN_USER_KEY, value.DESEncrypt());
        //}

#if !NETFRAMEWORK
        /// <summary>
        /// 从Session/Cookie删除用户操作模型。
        /// </summary>
        public void Remove(HttpContext context)
        {
            if (LoginProvider == "Cookie")
            {

            }
            else
            {
                context.Session.Remove(LOGIN_USER_KEY);
            }
        }
#endif

        public void Remove()
        {
            if (LoginProvider == "Cookie")
            {
#if NETFRAMEWORK
                WebHelper.RemoveCookie(LOGIN_USER_KEY);
#endif
            }
            else
            {
#if NETFRAMEWORK
                WebHelper.RemoveSession(LOGIN_USER_KEY);
#else
                MyHttpContext.httpContext.Session.Remove(LOGIN_USER_KEY);
#endif
            }
        }
    }
    /// <summary>
    /// 操作模型，保存登陆用户必要信息。
    /// </summary>
    public class Operator
    {
        public string UserId { get; set; }
        public string Account { get; set; }

        public string NickName { get; set; }
        public string RealName { get; set; }
        public string Avatar { get; set; }
        public string Password { get; set; }
        public string CompanyId { get; set; }
        public string DepartmentId { get; set; }
        public List<string> RoleId { get; set; }
        public string Token { get; set; }
        public DateTime LoginTime { get; set; }
        public string ClientUrl { get; set; }

        public string Theme { get; set; }
    }
}

