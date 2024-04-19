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
using SqlSugar;

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
        public ActionResult Process(int pageIndex, int pageSize, string keyWord, string configId, string productCode, string stationCode)
        {
            try
            {
                int totalCount = 0;
                List<RecordProcessData> pageData = outStationLogic.GetProcessData(pageIndex, pageSize, keyWord, ref totalCount, configId, productCode, stationCode);
                LayPadding<RecordProcessData> result = new LayPadding<RecordProcessData>()
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
                return Content(new LayPadding<RecordProcessData>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordProcessData>(),
                    count = 0
                }.ToJson());
            }


        }

      



        [Route("record/part/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Part()
        {
            return View();
        }

        [Route("record/part/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Part(int pageIndex, int pageSize, string configId, string productCode, string stationCode)
        {
            try
            {
                int totalCount = 0;
                List<RecordPartData> pageData = outStationLogic.GetPartData(pageIndex, pageSize, ref totalCount, configId, productCode, stationCode);
                LayPadding<RecordPartData> result = new LayPadding<RecordPartData>()
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
                return Content(new LayPadding<RecordPartData>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordPartData>(),
                    count = 0
                }.ToJson());
            }



        }
       
        
        /*
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
                //在分页查询后去重导致页面不足10条
                var pageData1 = pageData.GroupBy(e => new { e.ProductCode, e.StationCode }).Select(e => e.First()).ToList();
                
                var result = new LayPadding<RecordOutStation>()
                {
                    result = true,
                    msg = "success",
                    list = pageData1,
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
