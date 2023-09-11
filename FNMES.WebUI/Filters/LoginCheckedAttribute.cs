using FNMES.Utility.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using FNMES.Utility.Operator;

namespace FNMES.WebUI.Filters
{
    /// <summary>
    /// 表示一个特性，该特性用于标识用户是否需要登陆。
    /// </summary>
    public class LoginCheckedAttribute : ActionFilterAttribute
    {

        public bool Ignore { get; set; }
        public LoginCheckedAttribute(bool ignore = true)
        {
            Ignore = ignore;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!Ignore)
            {
                return;
            }

            if (OperatorProvider.Instance.Current == null)
            {
                //StringBuilder script = new StringBuilder();
                //script.Append("<script>top.location.href = '/account/login';</script>");
                //filterContext.Result = new ContentResult() { Content = script.ToString() };
                //filterContext.HttpContext.Response.//Write("<script>top.location.href = '/account/login'</script>");
                filterContext.HttpContext.Response.Redirect("/account/login");
            }
        }
    }
}