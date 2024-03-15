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

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Record")]
    [HiddenApi]
    public class GeneralController : BaseController
    {
        private readonly ProcessBindLogic bindLogic;
        private readonly RecordApiLogic apiLogic;
        private readonly RecordEquipmentLogic equipmentLogic;
        private readonly RecordOrderLogic orderLogic;
        public GeneralController()
        {
            bindLogic = new ProcessBindLogic();
            apiLogic = new RecordApiLogic();
            equipmentLogic = new RecordEquipmentLogic();
            orderLogic = new RecordOrderLogic();
        }


        [Route("record/general/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("record/general/bindhistory")]
        [HttpGet]
        public ActionResult Bindhistory(int page, int limit, string keyWord,string configId,string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = bindLogic.GetHistoryList(page, limit, keyWord, configId, ref totalCount, index);
                var result = new LayPadding<RecordBindHistory>()
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
                return Content(new LayPadding<RecordBindHistory>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordBindHistory>(),
                    count =0
                }.ToJson()) ;
            }
        }

        [Route("record/general/api")]
        [HttpGet]
        public ActionResult Api( int page, int limit, string keyWord, string configId, string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = apiLogic.GetList(page, limit, keyWord, configId, ref totalCount, index);
                var result = new LayPadding<RecordApi>()
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
                return Content(new LayPadding<RecordApi>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordApi>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("record/general/error")]
        [HttpGet]
        public ActionResult Error(int page, int limit, string keyWord, string configId, string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = equipmentLogic.GetErrorList(page, limit, keyWord, configId, ref totalCount, index);
                var result = new LayPadding<RecordEquipmentError>()
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
                return Content(new LayPadding<RecordEquipmentError>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordEquipmentError>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("record/general/status")]
        [HttpGet]
        public ActionResult Status(int page, int limit, string keyWord, string configId, string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = equipmentLogic.GetStatusList(page, limit, keyWord, configId, ref totalCount, index);
                var result = new LayPadding<RecordEquipmentStatus>()
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
                return Content(new LayPadding<RecordEquipmentStatus>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordEquipmentStatus>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("record/general/stop")]
        [HttpGet]
        public ActionResult Stop(int page, int limit, string keyWord, string configId, string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = equipmentLogic.GetStopList(page, limit, keyWord, configId, ref totalCount, index);
                var result = new LayPadding<RecordEquipmentStop>()
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
                return Content(new LayPadding<RecordEquipmentStop>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordEquipmentStop>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("record/general/start")]
        [HttpGet]
        public ActionResult Start(int page, int limit, string keyWord, string configId, string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = orderLogic.GetStartList(page, limit, keyWord, configId, ref totalCount, index);
                var result = new LayPadding<RecordOrderStart>()
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
                return Content(new LayPadding<RecordOrderStart>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordOrderStart>(),
                    count = 0
                }.ToJson());
            }
        }
        [Route("record/general/pack")]
        [HttpGet]
        public ActionResult Pack(int page, int limit, string keyWord, string configId, string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = orderLogic.GetEndList(page, limit, keyWord, configId, ref totalCount, index);
                var result = new LayPadding<RecordOrderPack>()
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
                return Content(new LayPadding<RecordOrderPack>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordOrderPack>(),
                    count = 0
                }.ToJson());
            }
        }
    }
}
