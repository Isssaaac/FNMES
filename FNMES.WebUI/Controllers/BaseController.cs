using FNMES.Utility.Core;
using FNMES.Utility.Network;
using FNMES.Utility.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace FNMES.WebUI.Controllers
{
    public class BaseController : Controller
    { 
        #region 快捷方法
        protected ActionResult Success(string message = "恭喜您，操作成功。", object data = null)
        {
            return Content(new AjaxResult(ResultType.Success, message, data).ToJson());
        }
        protected ActionResult Error(string message = "对不起，操作失败。", object data = null)
        {
            return Content(new AjaxResult(ResultType.Error, message, data).ToJson());
        }
        protected ActionResult Warning(string message, object data = null)
        {
            return Content(new AjaxResult(ResultType.Warning, message, data).ToJson());
        }
        protected ActionResult Info(string message, object data = null)
        {
            return Content(new AjaxResult(ResultType.Info, message, data).ToJson());
        }




        protected ActionResult AppSuccess()
        {
            return AppResult<string>(RetCode.success, "恭喜你，操作成功", string.Empty);
        }
        protected ActionResult AppSuccess<T>(T data)
        {
            return AppResult<T>(RetCode.success, "恭喜你，操作成功", data);
        }

        protected ActionResult AppSuccess(string message)
        {
            return AppResult<string>(RetCode.success, message, string.Empty);
        }
        protected ActionResult AppSuccess<T>(string message, T data)
        {
            return AppResult<T>(RetCode.success, message, data);
        }


        protected ActionResult AppError()
        {
            return AppResult<string>(RetCode.error, "对不起，操作失败", string.Empty);
        }
        protected ActionResult AppError<T>(T data)
        {
            return AppResult<T>(RetCode.error, "对不起，操作失败", data);
        }

        protected ActionResult AppError(string message)
        {
            return AppResult<string>(RetCode.error, message, string.Empty);
        }
        protected ActionResult AppError<T>(string message, T data)
        {
            return AppResult<T>(RetCode.error, message, data);
        }

        protected ActionResult AppResult<T>(string code, string message, T data)
        {
            return Content(new RetMessage<T> { messageType = code, message = message, data = data }.ToJson());
        }
        #endregion
    }
}