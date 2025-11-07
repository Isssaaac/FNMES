using CCS.WebUI;
using FNMES.Entity.Param;
using FNMES.Utility.Core;
using FNMES.Utility.ResponseModels;
using FNMES.WebUI.Controllers;
using FNMES.WebUI.Filters;
using FNMES.WebUI.Logic;
using FNMES.WebUI.Logic.Param;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FNMES.WebUI.Areas.Param.Controllers
{
    [Area("Param")]
    [HiddenApi]
    public class RepairController : BaseController
    {
        private readonly ParamRepairLogic paramRepairLogic;
        private readonly ProcessBindLogic bindLogic;
        private readonly RecipeLogic recipeLogic;

        public RepairController() 
        {
            paramRepairLogic=new ParamRepairLogic();
            bindLogic = new ProcessBindLogic();
            recipeLogic = new RecipeLogic();
        }


        [Route("param/repair/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("param/repair/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string productCode, string startDate, string endDate)
        {
            try
            {
                int totalCount = 0;
                var pageData = paramRepairLogic.GetList(pageIndex, pageSize, productCode, startDate, endDate, ref totalCount);
                var result = new LayPadding<RepairItem>()
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
                return Content(new LayPadding<RepairItem>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RepairItem>(),
                    count = 0
                }.ToJson());
            }
        }

        
        //线体mes登记为返修，这里只更新了ProcessBind里的StationCode
        [Route("param/repair/mark")]
        [HttpPost]
        public ActionResult Mark(string productCode,string configId, string stationCode, string startDate, string endDate)
        {
            Logger.RunningInfo($"登记返修:productCode:{productCode},configId:{configId},stationCode:{stationCode}");
            ProcessBind processBind = bindLogic.GetByProductCode(productCode, configId, startDate, endDate);
            if (processBind == null)
            {
                return Error();
            }
            processBind.RepairFlag = "1";
            if (string.IsNullOrEmpty(processBind.RepairStations))
            {
                processBind.RepairStations = stationCode;
            }
            else
            {
                processBind.RepairStations += ","+ stationCode;
            }

            var result = processBind.RepairStations.Split(',').ToList()
                .Distinct() // 去重
                .OrderBy(s => int.Parse(s.Substring(1)));
            processBind.RepairStations = string.Join(",", result);
            
            int v = bindLogic.Update(processBind, configId);
            return v != 0 ? Success() : Error();
        }
        //取消登记
        [Route("param/repair/dismark")]
        [HttpPost]
        public ActionResult DisMark(string productCode, string configId, string stationCode)
        {
            ProcessBind processBind = bindLogic.GetByProductCode(productCode, configId);
            if (processBind == null)
            {
                return Error();
            }
            List<string> strArray = new List<string>(processBind.RepairStations.Split(","));
            strArray.Remove(stationCode);
            
            Logger.RunningInfo($"内控码{productCode}取消登记工站{stationCode},目前待返修工为<{JsonConvert.SerializeObject(strArray)}>");
            if (strArray.Count == 0 || processBind.RepairStations.IsNullOrEmpty())
            {
                processBind.RepairFlag = "0";
                processBind.RepairStations = "";
                
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < strArray.Count; i++)
                {
                    sb.Append(strArray[i]);
                    if (i < strArray.Count - 1)
                    {
                        sb.Append(",");
                    }
                }
                processBind.RepairFlag = "1";
                processBind.RepairStations = sb.ToString();
            }
            int v = bindLogic.Update(processBind, configId);
            return v != 0 ? Success() : Error();
        }

        /* 出站后，未绑定内控码与产品编号的关系，无法查到对应的配方，以及工艺参数
        [Route("param/repair/param")]
        [HttpGet]
        public ActionResult Param()
        {
            return View();
        }

        [Route("param/repair/param")]
        [HttpPost]
        public ActionResult Param(int pageIndex, int pageSize, string keyWord, string configId, string primaryKey)
        {
            try
            {
                int totalCount = 0;
                var pageData = recipeLogic.GetParamList(pageIndex, pageSize, keyWord, configId, ref totalCount, long.Parse(primaryKey));
                var result = new LayPadding<ParamItem>()
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
                return Content(new LayPadding<ParamItem>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamItem>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("param/repair/step")]
        [HttpGet]
        public ActionResult Step()
        {
            return View();
        }
        [Route("param/repair/step")]
        [HttpPost]
        public ActionResult Step(int pageIndex, int pageSize, string keyWord, string configId, string primaryKey)
        {
            try
            {
                int totalCount = 0;
                var pageData = recipeLogic.GetStepList(pageIndex, pageSize, keyWord, configId, ref totalCount, long.Parse(primaryKey));
                var result = new LayPadding<ParamStepItem>()
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
                return Content(new LayPadding<ParamStepItem>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamStepItem>(),
                    count = 0
                }.ToJson());
            }
        }
        */
    }

    public class RepairItem
    {
        public long Id { get;set; }

        public  string ProduceCode { get; set; }

        public string LineId { get; set; }

        public string StationCode { get; set; }

        public string ProductStatus { get; set; }

        public string DefectCode { get; set; }

        public string DefectDesc { get; set; }
        public string RepairFlag { get; set; }           
    }
}
