using FNMES.WebUI.Filters;
using FNMES.Logic.Sys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using FNMES.Entity.Sys;
using FNMES.Utility.Operator;
using FNMES.Utility.Core;
using FNMES.Utility.Extension;
using FNMES.Utility.Files;
using FNMES.Utility.Security;
using FNMES.Utility.Web;
using FNMES.Utility.Other;
using FNMES.Logic;
using FNMES.Entity.DTO.Parms;
using CCS.WebUI;

namespace FNMES.WebUI.Controllers
{
    [HiddenApi]
    public class AccountController : BaseController
    {
        private SysUserLogic userlogic;
        private SysUserLogOnLogic userLogOnLogic;
        private SysLogLogic logLogic;

        public AccountController()
        {
            userlogic = new SysUserLogic();
            userLogOnLogic = new SysUserLogOnLogic();
            logLogic = new SysLogLogic();
        }
        /// <summary>
        /// 登陆页面视图。
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [Route("account/login")]
        [Route("admin")]
        [Route("admin.html")]
        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.SoftwareName = AppSetting.WebSoftwareName;
            return View();
        }

        [Route("account/postTest")]
        [HttpPost]
        public ActionResult PostTest(string str)
        {
            var obj = new { Method = "POST", Data = str };
            return Content(obj.ToJson());
        }


        /// <summary>
        /// 获取验证码图片。
        /// </summary>
        /// <returns></returns>
        [Route("account/verifyCode")]
        [HttpGet]
        public ActionResult VerifyCode()
        {
            VerifyCode verify = new VerifyCode();
            HttpContext.Session.SetString(Keys.SESSION_KEY_VCODE, verify.Text.ToLower());
            return File(verify.Image, "image/jpeg");
        }

        /// <summary>
        /// 提交登陆信息。
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="verifycode">验证码</param>
        /// <returns></returns>
        [Route("account/login")]
        [HttpPost]
        public ActionResult Login(string userName, string password, string verifyCode)
        {
            if (userName.IsNullOrEmpty() || password.IsNullOrEmpty() || verifyCode.IsNullOrEmpty())
            {
                return Error("请求失败，缺少必要参数。");
            }
            if (verifyCode.ToLower() != HttpContext.Session.GetString(Keys.SESSION_KEY_VCODE))
            { 
                return Warning("验证码错误，请重新输入。");
            }
            var userEntity = userlogic.GetByUserName(userName);
            if (userEntity == null)
            {
                return Warning("该账户不存在，请重新输入。");
            }
            if (!userEntity.IsEnabled)
            {
                return Warning("该账户已被禁用，请联系管理员。");
            }
            var userLogOnEntity = userLogOnLogic.GetByAccount(userEntity.Id);
            string inputPassword = password.DESEncrypt(userLogOnEntity.SecretKey).MD5Encrypt();
            if (inputPassword != userLogOnEntity.Password)
            {
                Logger.OperateInfo($"{userName}登录系统，密码错误");
                return Warning("密码错误，请重新输入。");
            }
            else
            {
                Operator operatorModel = new Operator();
                operatorModel.UserId = userEntity.Id;
                operatorModel.Account = userEntity.Account;
                operatorModel.NickName = userEntity.NickName;
                operatorModel.RealName = userEntity.RealName;
                operatorModel.Avatar = userEntity.Avatar;
                //判断头像是否存在，不存在就使用默认头像
                if (!System.IO.File.Exists(MyEnvironment.WebRootPath(userEntity.Avatar)))
                {
                    operatorModel.Avatar = "/Content/framework/images/avatar.png";
                }
                operatorModel.CompanyId = userEntity.CompanyId;
                operatorModel.DepartmentId = userEntity.DepartmentId;
                operatorModel.LoginTime = DateTime.Now;
                operatorModel.Token = UUID.StrSnowId.DESEncrypt();
                operatorModel.Theme = userLogOnEntity.Theme;
                OperatorProvider.Instance.Current = operatorModel;
                userLogOnLogic.UpdateLogin(userLogOnEntity);
                Logger.OperateInfo($"{userName}登录系统成功");
                return Success();
            }
        }





        /// <summary>
        /// 安全退出系统。
        /// </summary>
        /// <returns></returns>
        [Route("account/exit")]
        [HttpGet]
        public ActionResult Exit()
        {
            if (OperatorProvider.Instance.Current != null)
            {
                OperatorProvider.Instance.Remove();
            }
            return Redirect("/account/login");
        }

        /// <summary>
        /// 锁定登陆用户。
        /// </summary>
        /// <returns></returns> 
        [HttpPost, Route("account/lock"), LoginChecked]
        public ActionResult Lock()
        {
            if (OperatorProvider.Instance.Current != null)
            {
                OperatorProvider.Instance.Remove();
            }
            return Success();
        }

        /// <summary>
        /// 解锁登陆用户。
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPost, Route("account/unlock")]
        public ActionResult Unlock(string username, string password)
        {
            var userEntity = userlogic.GetByUserName(username);
            var userLogOnEntity = userLogOnLogic.GetByAccount(userEntity.Id);
            string inputPassword = password.DESEncrypt(userLogOnEntity.SecretKey).MD5Encrypt();
            if (inputPassword != userLogOnEntity.Password)
            {
                return Warning("密码错误，请重新输入。");
            }
            else
            {
                //重新保存用户信息。
                Operator operatorModel = new Operator();
                operatorModel.UserId = userEntity.Id;
                operatorModel.Account = userEntity.Account;
                operatorModel.NickName = userEntity.NickName;
                operatorModel.RealName = userEntity.RealName;
                operatorModel.Avatar = userEntity.Avatar;
                operatorModel.CompanyId = userEntity.CompanyId;
                operatorModel.DepartmentId = operatorModel.DepartmentId;
                operatorModel.LoginTime = DateTime.Now;
                operatorModel.Token = UUID.StrSnowId.DESEncrypt();
                operatorModel.Theme = userLogOnEntity.Theme;
                OperatorProvider.Instance.Current = operatorModel;
            }
            return Success();
        }

        /// <summary>
        /// 账户管理视图。
        /// </summary>
        /// <returns></returns>
        [Route("account/infoCard")]
        [HttpGet, LoginChecked]
        public ActionResult InfoCard()
        {
            return View();
        }

        /// <summary>
        /// 更新用户基础资料。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("account/infoCard")]
        [HttpPost, LoginChecked]
        public ActionResult InfoCard(SysUser model)
        {
            DateTime defaultDt = DateTime.Today;
            DateTime.TryParse(model.StrBirthday, out defaultDt);
            model.Birthday = defaultDt;
            int row = userlogic.UpdateBasicInfo(model, OperatorProvider.Instance.Current.UserId);
            if (row > 0)
            {
                //保存完头像，立刻生效
                Operator current = OperatorProvider.Instance.Current;
                current.Avatar = model.Avatar;
                OperatorProvider.Instance.Current = current;
                return Success();
            }
            return Error();
        }


        [Route("account/getInfoCardForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetInfoCardForm()
        {
            string userId = OperatorProvider.Instance.Current.UserId;
            SysUser userEntity = userlogic.Get(userId);
            userEntity.StrBirthday = userEntity.Birthday.Value.ToString("yyyy-MM-dd");
            var userLogOnEntity = userLogOnLogic.GetByAccount(userId);
            return Content(new { User = userEntity, UserLogOn = userLogOnEntity }.ToJson());
        }

        /// <summary>
        /// 上传头像。
        /// </summary>
        /// <returns></returns>
        [Route("account/uploadAvatar")]
        [HttpPost, LoginChecked]
        public ActionResult UploadAvatar(IFormFile file)
        {
            if (file == null)
            {
                return Error();
            }
            string path = MyHttpContext.WebRootPath("/Uploads/Avatar/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string virtualPath = "/Uploads/Avatar/" + UUID.StrSnowId + Path.GetExtension(file.FileName);
            string filePath = MyHttpContext.WebRootPath(virtualPath);
            if (FileUtil.Exists(filePath))
            {
                FileUtil.Delete(filePath);
            }
            FileUtil.Save(file, filePath);
            return Success("上传成功。", virtualPath);
        }


        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [Route("account/uploadImage")]
        [HttpPost]
        public ActionResult UploadImage(IFormFile file)
        {
            //var file = Request.Files[0];
            if (file == null)
            {
                return Error();
            }
            string path = MyHttpContext.WebRootPath("/Uploads/Avatar/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string virtualPath = "/Uploads/Avatar/" + UUID.StrSnowId + Path.GetExtension(file.FileName);
            string filePath = MyHttpContext.WebRootPath(virtualPath);
            if (FileUtil.Exists(filePath))
            {
                FileUtil.Delete(filePath);
            }
            FileUtil.Save(file, filePath);
            return Success("上传成功。", virtualPath);
        }

        /// <summary>
        /// 加载修改密码界面视图。
        /// </summary>
        /// <returns></returns>
        [Route("account/modifyPwd")]
        [HttpGet, LoginChecked]
        public ActionResult ModifyPwd()
        {
            return View();
        }

        /// <summary>
        /// 修改密码。
        /// </summary>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="confirmPassword">确认密码</param>
        /// <returns></returns>
        [Route("account/modifyPwd")]
        [HttpPost, LoginChecked]
        public ActionResult ModifyPwd(string oldPassword, string newPassword, string confirmPassword)
        {
            if (oldPassword.IsNullOrEmpty() || newPassword.IsNullOrEmpty() || confirmPassword.IsNullOrEmpty())
            {
                return Error("请求失败，缺少必要参数。");
            }
            if (!newPassword.Equals(confirmPassword))
            {
                return Warning("两次密码输入不一致，请重新确认。");
            }
            string userId = OperatorProvider.Instance.Current.UserId;
            var userLoginEntity = userLogOnLogic.GetByAccount(userId);
            if (oldPassword.DESEncrypt(userLoginEntity.SecretKey).MD5Encrypt() != userLoginEntity.Password)
            {
                return Warning("旧密码验证失败。");
            }
            userLoginEntity.Password = newPassword.DESEncrypt(userLoginEntity.SecretKey).MD5Encrypt();
            int isSuccess = userLogOnLogic.ModifyPwd(userLoginEntity);
            return isSuccess > 0 ? Success() : Error();
        }



        /// <summary>
        /// 系统登录
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost, Route("app/account/login")]
        public ActionResult AppLogin([FromBody] UserLoginParms user)// string userName, string password)
        {
            if (user.userName.IsNullOrEmpty() || user.password.IsNullOrEmpty())
            {
                return AppError("请求失败，缺少必要参数。");
            }
            user.password = user.password.MD5Encrypt();
            var userEntity = userlogic.GetByUserName(user.userName);
            if (userEntity == null)
            {
                return AppError("该账户不存在，请重新输入。");
            }
            if (!userEntity.IsEnabled)
            {
                return AppError("该账户已被禁用，请联系管理员。");
            }
            var userLogOnEntity = userLogOnLogic.GetByAccount(userEntity.Id);
            string inputPassword = user.password.DESEncrypt(userLogOnEntity.SecretKey).MD5Encrypt();
            if (inputPassword != userLogOnEntity.Password)
            {
                return AppError("密码错误，请重新输入。");
            }
            else
            {
                userLogOnLogic.UpdateLogin(userLogOnEntity);
                return AppSuccess("恭喜你，操作成功", userEntity);
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/account/getUserInfo")]
        public ActionResult GetUserInfo([FromBody] StrPrimaryKeyParms parms)
        {
            try
            {
                SysUser userEntity = userlogic.Get(parms.primaryKey);
                return AppSuccess<SysUser>(userEntity);
            }
            catch (Exception ex)
            {
                return AppError(ex.Message);
            }
        }


        /// <summary>
        /// 更新用户基础资料。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("app/account/infocard")]
        public ActionResult AppInfoCard([FromBody] SysUser model)
        {
            DateTime defaultDt = DateTime.Today;
            DateTime.TryParse(model.StrBirthday, out defaultDt);
            model.Birthday = defaultDt;
            int row = userlogic.AppUpdateBasicInfo(model);
            return row > 0 ? AppSuccess() : AppError();
        }

        ///// <summary>
        ///// 上传图片
        ///// </summary>
        ///// <returns></returns>
        //[Route("account/uploadImage")]
        //[HttpPost]
        //public ActionResult UploadImage()
        //{
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }
        //    string path = MyEnvironment.WebRootPath("/Uploads/Avatar/");
        //    var provider = new WithExtensionMultipartFormDataStreamProvider(path, UUID.StrSnowId);
        //    var fileData = Request.Content.ReadAsMultipartAsync(provider).Result;
        //    if (fileData.FileData.Count == 0)
        //    {
        //        return Error();
        //    }
        //    var file = fileData.FileData[0];
        //    string virtualPath = "/Uploads/Avatar/" + Path.GetFileName(file.LocalFileName);
        //    return Success("上传成功", virtualPath);
        //}


        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns> 
        [HttpPost, Route("account/uploadImage")]
        public ActionResult UploadImage([FromBody] FileUploadParms parms)
        {
            try
            {
                string ext = Path.GetExtension(parms.fileName);
                string virtualPath = $"/Uploads/Avatar/{UUID.StrSnowId}{ext}";
                string fileName = MyEnvironment.WebRootPath(virtualPath);
                using (FileStream stream = new FileStream(fileName, FileMode.CreateNew))
                {
                    stream.Write(parms.file, 0, parms.file.Length);
                }
                return AppSuccess<string>(virtualPath);
            }
            catch (Exception ex)
            {
                return AppError(ex.Message);
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/account/modifyPwd")]
        public ActionResult AppModifyPwd([FromBody] ModifyPasswordParms parms)
        {
            if (parms.oldPassword.IsNullOrEmpty() || parms.newPassword.IsNullOrEmpty() || parms.confirmPassword.IsNullOrEmpty())
            {
                return AppError("请求失败，缺少必要参数。");
            }
            if (!parms.newPassword.Equals(parms.confirmPassword))
            {
                return AppError("两次密码输入不一致，请重新确认。");
            }
            parms.oldPassword = parms.oldPassword.MD5Encrypt();
            parms.newPassword = parms.newPassword.MD5Encrypt();
            parms.confirmPassword = parms.confirmPassword.MD5Encrypt();

            var userLoginEntity = userLogOnLogic.GetByAccount(parms.userId);
            if (parms.oldPassword.DESEncrypt(userLoginEntity.SecretKey).MD5Encrypt() != userLoginEntity.Password)
            {
                return AppError("旧密码验证失败。");
            }
            userLoginEntity.Password = parms.newPassword.DESEncrypt(userLoginEntity.SecretKey).MD5Encrypt();
            int isSuccess = userLogOnLogic.ModifyPwd(userLoginEntity);
            return isSuccess > 0 ? AppSuccess() : AppError();
        }
    }
}