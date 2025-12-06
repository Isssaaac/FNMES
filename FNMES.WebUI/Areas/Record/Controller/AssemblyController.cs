using System;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.WebUI.Controllers;
using System.Collections.Generic;
using FNMES.WebUI.Logic.Record;
using FNMES.Entity.Record;

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Record")]
    [HiddenApi]
    public class AssemblyController : BaseController
    {
        private readonly RecordCellBindBlockLogic cellBindBlockLogic;
        private readonly RecordBlockBindPackLogic blockBindPackLogic;
        public AssemblyController()
        {
            cellBindBlockLogic = new RecordCellBindBlockLogic();
            blockBindPackLogic = new RecordBlockBindPackLogic();
        }


        [Route("record/assembly/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("record/assembly/cellbindblock")]
        [HttpGet]
        public ActionResult CellBindBlock(int page, int limit, string keyWord, string configId, string startDate, string endDate)
        {
            try
            {
                int totalCount = 0;
                var pageData = cellBindBlockLogic.GetSplitPageList(page, limit, configId, startDate, endDate, keyWord, ref totalCount);
                var result = new LayPadding<RecordCellBindBlock>()
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
                return Content(new LayPadding<RecordCellBindBlock>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordCellBindBlock>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("record/assembly/blockbindpack")]
        [HttpGet]
        public ActionResult BlockBindPack(int page, int limit, string keyWord, string configId, string startDate,string endDate)
        {
            try
            {
                int totalCount = 0;
                
                var pageData = blockBindPackLogic.GetSplitPageList(page, limit, configId, startDate, endDate, keyWord, ref totalCount);
                var result = new LayPadding<RecordBlockBindPack>()
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
                return Content(new LayPadding<RecordBlockBindPack>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordBlockBindPack>(),
                    count = 0
                }.ToJson());
            }
        }

    }
}
