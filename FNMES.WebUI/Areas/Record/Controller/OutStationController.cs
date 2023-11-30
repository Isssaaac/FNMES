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
    public class OutStationController : BaseController
    {
        private readonly RecordOutStationLogic outStationLogic;
        public OutStationController()
        {
            outStationLogic = new RecordOutStationLogic();
           
        }


        [Route("record/outstation/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }

        [Route("record/process/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Process()
        {
            return View();
        }
        
        [Route("record/process/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Process(string productCode,string stationCode, string configId)
        {
            return View();
        }

        [Route("record/process/data")]
        [HttpPost, AuthorizeChecked]
        public ActionResult ProcessData(int pageIndex, int pageSize, string configId, string pId)
        {
            return View();
        }



        [Route("record/part/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Part()
        {
            return View();
        }

        /*[Route("record/part/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Part(string productCode, string stationCode, string configId)
        {
            
        }
        [Route("record/part/data")]
        [HttpPost, AuthorizeChecked]
        public ActionResult PartData(int pageIndex, int pageSize, string configId, string pId)
        {

        }*/





        [Route("record/outstation/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId,string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = outStationLogic.GetList(pageIndex, pageSize, keyWord, ref totalCount, configId,index);
                var result = new LayPadding<RecordOutStation>()
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
                return Content(new LayPadding<RecordOutStation>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordOutStation>(),
                    count =0
                }.ToJson()) ;
            }
        }


        [Route("record/process/exist")]
        [HttpGet]
        public ActionResult processExist(string productCode,string stationCode, string configId)
        {
            try
            {
                bool exist = outStationLogic.processExist(productCode, stationCode, configId);
               if (exist)
                {
                    return Success();
                }
                return Error();
            }
            catch 
            {
                return Error();
            }
        }

        [Route("record/part/exist")]
        [HttpGet]
        public ActionResult partExist(string productCode, string stationCode, string configId)
        {
            try
            {
                bool exist = outStationLogic.partExist(productCode, stationCode, configId);
                if (exist)
                {
                    return Success();
                }
                return Error();
            }
            catch
            {
                return Error();
            }
        }

    }
}
