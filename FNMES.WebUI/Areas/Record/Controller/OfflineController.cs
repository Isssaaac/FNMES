using System;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using FNMES.WebUI.Logic.Param;
using FNMES.WebUI.Logic;
using FNMES.Entity.Param;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using FNMES.Utility.Network;
using FNMES.WebUI.API;
using FNMES.Entity.DTO.ApiParam;
using FNMES.WebUI.Logic.Sys;
using FNMES.Entity.Sys;
using FNMES.Entity.DTO.ApiData;
using FNMES.WebUI.Logic.Record;
using FNMES.Entity.Record;
using System.Linq;

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Record")]
    [HiddenApi]
    public class OfflineController : BaseController
    {
        private readonly RecordOfflineApiLogic apiLogic;
        public OfflineController()
        {
            apiLogic = new RecordOfflineApiLogic();
        }


        [Route("record/offline/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("record/offline/index")]
        [HttpPost]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId,string index, string index1)
        {
            try
            {
                int totalCount = 0;
                var pageData = apiLogic.GetList(pageIndex, pageSize, keyWord, ref totalCount, configId, index,index1);
                var result = new LayPadding<RecordOfflineApi>()
                {
                    result = true,
                    msg = "success",
                    list = pageData,
                    count = totalCount//pageData.Count
                };
                return Content(result.ToJson());
            }
            catch (Exception E)
            {
                return Content(new LayPadding<RecordOfflineApi>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordOfflineApi>(),
                    count =0
                }.ToJson()) ;
            }
        }

        #region  20240414更新，页面是否允许传递结构体
        [Route("record/offline/singleupload")]
        [HttpPost, AuthorizeChecked]
        public ActionResult SingleUpload(string primaryId, string configId)
        {
            var models = apiLogic.GetUnload(configId);
            var model = models.Where(e => e.Id == long.Parse(primaryId)).First();
            return apiLogic.Upload(model, configId) > 0 ? Success() : Error();
        }

        [Route("record/offline/allupload")]
        [HttpPost, AuthorizeChecked]
        public ActionResult AllUpload(string configId)
        {
            var models = apiLogic.GetUnload(configId);
            return apiLogic.UploadAll(models, configId) > 0 ? Success() : Error();
        }

        [Route("record/offline/delete")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string primaryId, string configId)
        {
            return apiLogic.Delete(long.Parse(primaryId), configId) > 0 ? Success() : Error();
        }
        #endregion
    }
}
