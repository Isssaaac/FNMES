using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.WebUI.Controllers;
using System.Collections.Generic;
using FNMES.Entity.Record;
using FNMES.WebUI.Logic.Record;
using FNMES.Entity.Record;

namespace FNMES.WebUI.Areas.Record.Controller
{
    [Area("Record")]
    [HiddenApi]
    public class BlockOutStationController : BaseController
    {
        public RecordBlockOutStationLogic blockOutStationLogic;
        public BlockOutStationController()
        {
            blockOutStationLogic = new RecordBlockOutStationLogic();
        }

        [Route("record/blockoutstation/index")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [Route("record/blockoutstation/index")]
        [HttpPost]
        public ActionResult index(int pageIndex, int pageSize, string configId, string startDate, string endDate, string productCode, string conditions)
        {
            try
            {
                int totalCount = 0;
                List<RecordBlockOutStation> pageData = blockOutStationLogic.GetSplitPageList<RecordBlockOutStation>(pageIndex, pageSize, configId, startDate, endDate, conditions, ref totalCount);
                LayPadding<RecordBlockOutStation> result = new LayPadding<RecordBlockOutStation>()
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
                return Content(new LayPadding<RecordBlockOutStation>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordBlockOutStation>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("record/blockoutstation/processexist")]
        [HttpGet]
        public ActionResult processExist(string productCode, string configId)
        {
            try
            {
                //查一下是否存在
                bool exist = blockOutStationLogic.processExist(productCode, configId);
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

        [Route("record/blockoutstation/partexist")]
        [HttpGet]
        public ActionResult partExist(string productCode, string configId)
        {
            try
            {
                bool exist = blockOutStationLogic.partExist(productCode, configId);
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

        [Route("record/blockoutstation/process")]
        [HttpGet]
        public ActionResult Process()
        {
            return View();
        }

        [Route("record/blockoutstation/process")]
        [HttpPost]
        public ActionResult Process(int pageIndex, int pageSize, string keyWord, string configId, string productCode, string stationCode)
        {
            try
            {
                int totalCount = 0;
                List<RecordBlockProcessData> pageData = blockOutStationLogic.GetProcessData(pageIndex, pageSize, keyWord, ref totalCount, configId, productCode, stationCode);
                LayPadding<RecordBlockProcessData> result = new LayPadding<RecordBlockProcessData>()
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
                return Content(new LayPadding<RecordBlockProcessData>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordBlockProcessData>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("record/blockoutstation/part")]
        [HttpGet]
        public ActionResult Part()
        {
            return View();
        }

        //查看物料数据
        [Route("record/blockoutstation/part")]
        [HttpPost]
        public ActionResult Part(int pageIndex, int pageSize, string configId, string productCode,string stationCode)
        {
            try
            {
                int totalCount = 0;
                List<RecordBlockPartData> pageData = blockOutStationLogic.GetPartData(pageIndex, pageSize, ref totalCount, configId, productCode, stationCode);
                LayPadding<RecordBlockPartData> result = new LayPadding<RecordBlockPartData>()
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
                return Content(new LayPadding<RecordBlockPartData>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordBlockPartData>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("record/blockoutstation/queryfilter")]
        [HttpGet]
        public ActionResult QueryFilter()
        {
            return View();
        }
    }

    
}
