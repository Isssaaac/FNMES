using System;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Logic.Sys;
using FNMES.Entity.Sys;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.WebUI.Controllers;
using FNMES.Utility.Extension;
using FNMES.Utility.Other;
using FNMES.Utility.Files;
using System.IO;
using FNMES.Entity.DTO.Parms;

namespace FNMES.WebUI.Areas.Sys.Controllers
{
    [HiddenApi]
    [Area("Sys")]
    public class LogController : BaseController
    {
        private SysLogLogic logic;
        public LogController()
        {
            logic = new SysLogLogic();
        }


        /// <summary>
        /// 操作日志主界面
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("system/log/operateIndex"), AuthorizeChecked]
        public ActionResult OperateIndex()
        {
            return View();
        }

        /// <summary>
        /// 运行日志主界面
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("system/log/runningIndex"), AuthorizeChecked]
        public ActionResult RunningIndex()
        {
            return View();
        }

        /// <summary>
        /// 错误日志主界面
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("system/log/errorIndex"), AuthorizeChecked]
        public ActionResult ErrorIndex()
        {
            return View();
        }

        /// <summary>
        /// 根据keyWord获取日志信息
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("system/log/index"), LoginChecked]
        public ActionResult Index(int pageIndex, int pageSize, string type, string index, string keyWord)
        {
            int totalCount = 0;
            var pageData = logic.GetList(pageIndex, pageSize, type, index, keyWord, ref totalCount);
            var result = new LayPadding<SysLog>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return Content(result.ToJson());
        }


        [HttpPost, Route("system/log/export"), AuthorizeChecked]
        public ActionResult Export(string type)
        {
            string filePath = MyEnvironment.RootPath(UUID.StrSnowId + ".zip");
            string basePath = string.Empty;
            string fileDownloadName = string.Empty;
            //读取配置文件，得到三个日志文件夹
            if (type == "Error")
            {
                basePath = "Log\\Error\\";
                fileDownloadName = "ErrorLog.zip";
            }
            else if (type == "Operate")
            {
                basePath = "Log\\Operate\\";
                fileDownloadName = "OperateLog.zip";
            }
            else
            {
                basePath = "Log\\Info\\";
                fileDownloadName = "InfoLog.zip";
            }
            string baseDirPath = MyEnvironment.RootPath(basePath);
            if (!Directory.Exists(baseDirPath))
            {
                return null;
            }
            try
            {
                //压缩
                ZipHelper.PackFiles(filePath, baseDirPath);
                byte[] bytes = FileUtil.FileToBytes(filePath);
                FileUtil.Delete(filePath);
                return File(bytes, "application/zip", fileDownloadName);
            }
            catch
            {
                return null;
            }
        }



        /// <summary>
        /// 删除日志界面
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("system/log/delete"), AuthorizeChecked]
        public ActionResult Delete()
        {
            return View();
        }

        /// <summary>
        /// 根据选择的删除方法执行删除
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("system/log/delete"), AuthorizeChecked]
        public ActionResult Delete(string type, string index)
        {
            logic.Delete(type, index);
            return Success();
        }

        /// <summary>
        /// 根据keyWord获取日志信息
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/log/index")]
        public ActionResult AppIndex([FromBody] LogIndexParms parms)
        {
            int totalCount = 0;
            var pageData = logic.GetList(parms.pageIndex, parms.pageSize, parms.type, parms.index, parms.keyWord, ref totalCount);
            var result = new LayPadding<SysLog>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return AppSuccess<LayPadding<SysLog>>(result);
        }
        /// <summary>
        /// 根据选择的删除方法执行删除
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/log/delete")]
        public ActionResult AppDelete([FromBody] LogDeleteParms parms)
        {
            logic.Delete(parms.type, parms.index);
            return AppSuccess();
        }

        /// <summary>
        /// 导出日志文件
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/log/export")]
        public ActionResult AppExport([FromBody] LogIndexParms parms)
        {
            string filePath = MyEnvironment.RootPath(UUID.StrSnowId + ".zip");
            string basePath = string.Empty;
            string fileDownloadName = string.Empty;
            //读取配置文件，得到三个日志文件夹
            if (parms.type == "Error")
            {
                basePath = "Log/Error/";
                //basePath = GlobalValue.Config.LogBasePath + GlobalValue.Config.ErrorLogPath;
                fileDownloadName = "ErrorLog.zip";
            }
            else if (parms.type == "Operate")
            {
                basePath = "Log/Operate/";
                //basePath = GlobalValue.Config.LogBasePath + GlobalValue.Config.OperateLogPath;
                fileDownloadName = "OperateLog.zip";
            }
            else
            {
                basePath = "Log/Info/";
                //basePath = GlobalValue.Config.LogBasePath + GlobalValue.Config.InfoLogPath;
                fileDownloadName = "InfoLog.zip";
            }
            string baseDirPath = MyEnvironment.RootPath(basePath);
            if (!Directory.Exists(baseDirPath))
            {
                return AppError("日志文件夹不存在");
            }
            try
            {
                //压缩
                ZipHelper.PackFiles(filePath, baseDirPath);
                byte[] bytes = FileUtil.FileToBytes(filePath);
                FileUtil.Delete(filePath);
                //return File(bytes, "application/zip", fileDownloadName);
                return AppSuccess<byte[]>(bytes);
            }
            catch
            {
                return AppError("删除失败");
            }
        }

    }
}
