using System.Collections.Generic;
using FNMES.WebUI.Filters;
using FNMES.Entity.Sys;
using FNMES.Utility.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;
using FNMES.WebUI.Logic.Param;

namespace FNMES.WebUI.Logic.Sys
{
    [HiddenApi]
    [Area("Param")]
    public class BarcodeRuleController : BaseController
    {
        private readonly ParamBarcodeRuleLogic barcodeRuleLogic;
        private readonly BaseLogic baseLogic;
        public BarcodeRuleController()
        {
            barcodeRuleLogic = new ParamBarcodeRuleLogic();
            baseLogic = new BaseLogic();
        }

        [Route("param/barcoderule/index")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("param/barcoderule/index")]
        [HttpPost]
        public ActionResult Index(string configId)
        {
            int totalCount = 0;
            var pageData = baseLogic.GetTableList<ParamBarcodeRule>(configId);
            var result = new LayPadding<ParamBarcodeRule>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = pageData.Count
            };
            return Content(result.ToJson());
        }

        [Route("param/barcoderule/modify")]
        [HttpGet]
        public IActionResult Modify()
        {
            return View();
        }


        [Route("/param/barcoderule/getFormModify")]
        [HttpPost]
        public ActionResult GetFormModify(string primaryKey, string configId)
        {
            var ret = baseLogic.GetTableRowByID<ParamBarcodeRule>(primaryKey, configId);
            return Content(ret.ToJson());
        }


        [Route("/param/barcoderule/modify")]
        [HttpPost]
        public ActionResult Modify(ParamBarcodeRule param, string configId)
        {
            var ret = baseLogic.UpdateTable(param, configId);
            return ret == 1 ? Success() : Error();
        }

        [Route("/param/barcoderule/genbarcode")]
        [HttpPost]
        public ActionResult GenBarcode( string configId)
        {
            string barcode = "";
            var ret = barcodeRuleLogic.GenBarcode(configId,out barcode);
            return Content( barcode);
        }
    }
}
