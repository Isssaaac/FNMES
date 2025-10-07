using Microsoft.AspNetCore.Mvc;
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
using FNMES.Entity.Record;
using FNMES.WebUI.Logic.Sys;
using FNMES.Entity.Sys;
using FNMES.Entity.DTO.ApiData;
using FNMES.WebUI.Logic.Record;
using FNMES.Entity.Record;
using Newtonsoft.Json;

namespace FNMES.WebUI.Areas.Record.Controller
{
    [Area("Record")]
    [HiddenApi]
    public class CellOutStationController : BaseController
    {
        public RecordCellOutStationLogic cellStartLogic;
        public CellOutStationController()
        {
            cellStartLogic = new RecordCellOutStationLogic();
        }

        [Route("record/celloutstation/index")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [Route("record/celloutstation/index")]
        [HttpPost]
        public ActionResult index(int pageIndex, int pageSize, string configId, string startDate, string endDate, string productCode, string order,string conditions)
        {
            try
            {
                
                int totalCount = 0;
                List<RecordCellOutStation> pageData = cellStartLogic.GetSplitPageList<RecordCellOutStation>(pageIndex, pageSize, configId, startDate  , endDate, conditions, ref totalCount);
                LayPadding<RecordCellOutStation> result = new LayPadding<RecordCellOutStation>()
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
                return Content(new LayPadding<RecordCellOutStation>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordCellOutStation>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("record/celloutstation/processexist")]
        [HttpGet]
        public ActionResult processExist(string productCode, string configId)
        {
            try
            {
                //查一下是否存在
                bool exist = cellStartLogic.processExist(productCode, configId);
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

        [Route("record/celloutstation/partexist")]
        [HttpGet]
        public ActionResult partExist(string productCode, string configId)
        {
            try
            {
                bool exist = cellStartLogic.partExist(productCode, configId);
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

        [Route("record/celloutstation/process")]
        [HttpGet]
        public ActionResult Process()
        {
            return View();
        }


        [Route("record/celloutstation/queryfilter")]
        [HttpGet]
        public ActionResult QueryFilter()
        {
            return View();
        }

        [Route("record/celloutstation/process")]
        [HttpPost]
        public ActionResult Process(int pageIndex, int pageSize, string keyWord, string configId, string productCode, string stationCode)
        {
            try
            {
                int totalCount = 0;
                List<RecordCellProcessData> pageData = cellStartLogic.GetProcessData(pageIndex, pageSize, keyWord, ref totalCount, configId, productCode, stationCode);
                LayPadding<RecordCellProcessData> result = new LayPadding<RecordCellProcessData>()
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
                return Content(new LayPadding<RecordCellProcessData>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordCellProcessData>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("record/celloutstation/part")]
        [HttpGet]
        public ActionResult Part()
        {
            return View();
        }

        //查看物料数据
        [Route("record/celloutstation/part")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Part(int pageIndex, int pageSize, string configId, string productCode)
        {
            try
            {
                int totalCount = 0;
                List<RecordCellPartData> pageData = cellStartLogic.GetPartData(pageIndex, pageSize, ref totalCount, configId, productCode);
                LayPadding<RecordCellPartData> result = new LayPadding<RecordCellPartData>()
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
                return Content(new LayPadding<RecordCellPartData>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordCellPartData>(),
                    count = 0
                }.ToJson());
            }
        }
    }
}
