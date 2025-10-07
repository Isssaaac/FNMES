using System;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.WebUI.Controllers;
using FNMES.WebUI.Logic.Param;
using FNMES.WebUI.Logic;
using FNMES.Entity.Param;
using System.Collections.Generic;
using CCS.WebUI;
using FNMES.Utility.Files;
using System.IO;
using System.Linq;
using FNMES.WebUI.Logic.Base;
using Microsoft.AspNetCore.Http;
using FNMES.Entity.DTO;

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Param")]
    [HiddenApi]
    public class RecipeController : BaseController
    {
        private readonly BaseLogic baseLogic;
        private readonly RecipeLogic  recipeLogic;
        private readonly ParamItemLogic paramItemLogic;
        private readonly ParamPartItemLogic paramPartItemLogic;
        private readonly ParamStepItemLogic paramStepItemLogic;

        public RecipeController()
        {
            recipeLogic = new RecipeLogic();
            paramItemLogic = new ParamItemLogic();
            paramPartItemLogic = new ParamPartItemLogic();
            baseLogic = new BaseLogic();
            paramStepItemLogic = new ParamStepItemLogic();
        }

        [Route("param/recipe/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }

        //ParamRecipeItem
        [Route("param/recipe/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId,string productId)
        {
            try
            {
                int totalCount = 0;
                var pageData = recipeLogic.GetList(pageIndex, pageSize, keyWord, configId, ref totalCount, long.Parse(productId));
                var result = new LayPadding<ParamRecipeItem>()
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
                return Content(new LayPadding<ParamRecipeItem>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamRecipeItem>(),
                    count =0
                }.ToJson()) ;
            }
        }

        [Route("param/recipe/param")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Param()
        {
            return View();
        }

        //ParamItem
        [Route("param/recipe/param")]
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

        [Route("param/recipe/part")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Part()
        {
            return View();
        }
        //ParamPartItem
        [Route("param/recipe/part")]
        [HttpPost]
        public ActionResult Part(int pageIndex, int pageSize, string keyWord, string configId, string primaryKey)
        {
            try
            {
                int totalCount = 0;
                var pageData = recipeLogic.GetPartList(pageIndex, pageSize, keyWord, configId, ref totalCount, long.Parse(primaryKey));
                var result = new LayPadding<ParamPartItem>()
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
                return Content(new LayPadding<ParamPartItem>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamPartItem>(),
                    count = 0
                }.ToJson());
            }
        }
        //ParamAlternativePartItem
        [Route("param/recipe/apart")]
        [HttpPost]
        public ActionResult APart(int pageIndex, int pageSize, string keyWord, string configId, string primaryKey)
        {
            try
            {
                int totalCount = 0;
                var pageData = recipeLogic.GetAPartList(pageIndex, pageSize, keyWord, configId, ref totalCount, long.Parse(primaryKey));
                var result = new LayPadding<ParamAlternativePartItem>()
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
                return Content(new LayPadding<ParamAlternativePartItem>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamAlternativePartItem>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("param/recipe/esop")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Esop()
        {
            return View();
        }
        [Route("param/recipe/esop")]
        [HttpPost]
        public ActionResult Esop(int pageIndex, int pageSize, string keyWord, string configId, string primaryKey)
        {
            try
            {
                int totalCount = 0;
                var pageData = recipeLogic.GetEsopList(pageIndex, pageSize, keyWord, configId, ref totalCount, long.Parse(primaryKey));
                var result = new LayPadding<ParamEsopItem>()
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
                return Content(new LayPadding<ParamEsopItem>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamEsopItem>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("param/recipe/step")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Step()
        {
            return View();
        }

        [Route("param/recipe/stepadd")]
        [HttpGet]
        public ActionResult StepAdd()
        {
            return View();
        }

        [Route("param/recipe/stepadd")]
        [HttpPost]
        public ActionResult StepAdd(ParamStepItem data, string configId)
        {
            int ret = baseLogic.InsertTableRow(data, configId);
            return ret > 0 ? Success() : Error();
        }

        [Route("param/recipe/getstepForm")]
        [HttpPost]
        public ActionResult GetStepForm(string primaryKey, string configId)
        {
            var entity = baseLogic.GetTableRowByID<ParamStepItem>(primaryKey, configId);
            return Content(entity.ToJson());
        }

        [Route("param/recipe/paramadd")]
        [HttpGet]
        public ActionResult ParamAdd()
        {
            return View();
        }

        [Route("param/recipe/paramadd")]
        [HttpPost]
        public ActionResult ParamAdd(ParamItem data,string configId)
        {
            int ret = paramItemLogic.InsertTableRow(data,configId);
            return ret > 0 ? Success() : Error();
        }

        [Route("param/recipe/partadd")]
        [HttpGet]
        public ActionResult PartAdd()
        {
            return View();
        }

        [Route("param/recipe/partadd")]
        [HttpPost]
        public ActionResult PartAdd(ParamPartItem data, string configId)
        {
            int ret = paramPartItemLogic.InsertTableRow(data, configId);
            return ret > 0 ? Success() : Error();
        }

        [Route("param/recipe/getpartForm")]
        [HttpPost]
        public ActionResult GetPartForm(string primaryKey, string configId)
        {
            var entity = paramPartItemLogic.GetTableRowByID<ParamPartItem>(primaryKey, configId);
            return Content(entity.ToJson());
        }

        [Route("param/recipe/getStepName")]
        [HttpPost]
        public ActionResult GetStepName(string configId, string recipeItemId,string StepNo)
        {
            var entity = paramStepItemLogic.GetStepName(configId, recipeItemId, StepNo);
            return Content(entity);
        }

        [Route("param/recipe/partdelete")]
        [HttpPost]
        public ActionResult PartDelete(string primaryKey, string configId)
        {
            return paramPartItemLogic.DeleteTableRowByID<ParamPartItem>(primaryKey, configId) > 0 ? Success() : Error();
        }
        [Route("param/recipe/esopadd")]
        [HttpGet]
        public ActionResult ESOPAdd()
        {
            return View();
        }

        [Route("param/recipe/esopadd")]
        [HttpPost]
        public ActionResult ESOPAdd(ParamEsopItem data, string configId)
        {
            int ret = baseLogic.InsertTableRow(data, configId);
            return ret > 0 ? Success() : Error();
        }

        [Route("param/recipe/partmodify")]
        [HttpGet]
        public ActionResult PartModify()
        {
            return View();
        }

        [Route("param/recipe/parammodify")]
        [HttpGet]
        public ActionResult ParamModify()
        {
            return View();
        }

        [Route("param/recipe/partmodify")]
        [HttpPost]
        public ActionResult PartModify(ParamPartItem data, string configId)
        {
            int ret = baseLogic.UpdateTable(data, configId);
            return ret > 0 ? Success() : Error();
        }

        [Route("param/recipe/parammodify")]
        [HttpPost]
        public ActionResult ParamModify(ParamItem data, string configId)
        {
            int ret = paramItemLogic.UpdateTable(data, configId);
            return ret > 0 ? Success() : Error();
        }



        [Route("param/recipe/getparamForm")]
        [HttpPost]
        public ActionResult GetParamForm(string primaryKey, string configId)
        {
            var entity = paramItemLogic.GetTableRowByID<ParamItem>(primaryKey,configId);
            return Content(entity.ToJson());
        }

        [Route("param/recipe/getesopForm")]
        [HttpPost]
        public ActionResult GetESOPForm(string primaryKey, string configId)
        {
            var entity = paramItemLogic.GetTableRowByID<ParamEsopItem>(primaryKey, configId);
            return Content(entity.ToJson());
        }

        [Route("param/recipe/paramdelete")]
        [HttpPost]
        public ActionResult ParamDelete(string primaryKey, string configId)
        {
            return paramItemLogic.DeleteTableRowByID<ParamItem>(primaryKey, configId) > 0 ? Success() : Error();
        }

        [Route("param/recipe/stepmodify")]
        [HttpGet]
        public ActionResult StepModify()
        {
            return View();
        }

        [Route("param/recipe/stepmodify")]
        [HttpPost]
        public ActionResult StepModify(ParamStepItem data, string configId)
        {
            int ret = baseLogic.UpdateTable(data, configId);
            return ret > 0 ? Success() : Error();
        }

        [Route("param/recipe/stepdelete")]
        [HttpPost]
        public ActionResult StepDelete(string primaryKey, string configId)
        {
            return paramPartItemLogic.DeleteTableRowByID<ParamStepItem>(primaryKey, configId) > 0 ? Success() : Error();
        }

        [Route("param/recipe/esopmodify")]
        [HttpGet]
        public ActionResult EsopModify()
        {
            return View();
        }

        [Route("param/recipe/esopmodify")]
        [HttpPost]
        public ActionResult EsopModify(ParamEsopItem data, string configId)
        {
            int ret = baseLogic.UpdateTable(data, configId);
            return ret > 0 ? Success() : Error();
        }

        [Route("param/recipe/step")]
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

        [Route("param/recipe/esopFile")]
        [HttpGet]
        public ActionResult File(string filePath)
        {
            FTPparam fTPparam = AppSetting.FTPparam;
            FtpHelper ftpHelper = new FtpHelper(fTPparam.Host, fTPparam.Username, fTPparam.Password);

            try
            {
                var stream = ftpHelper.DownloadFileStream(filePath);
                // 设置响应头，指定响应的内容类型和文件名
                Response.Headers.Add("Content-Disposition", $"attachment; filename=downloaded-file.pdf");
                return File(stream, "application/pdf");
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return Error("读取ftp文件流出错");
            }
        }

        [Route("param/recipe/exportstep")]
        [HttpGet]
        public ActionResult ExportStep(string productId,string configId)
        {
            List<ParamRecipeItem> recipes = recipeLogic.QueryAllItem(productId, configId);
            List<RecipeStep> steps = new List<RecipeStep>();

            foreach (var recipe in recipes)
            {
                foreach (var item in recipe.StepList.OrderBy(u => int.Parse(u.StepNo)))
                {
                    RecipeStep step = new RecipeStep();
                    step.StationName = recipe.StationName;
                    step.StationCode = recipe.StationCode;
                    step.SmallStationCode = item.SmallStationCode;
                    step.StepNo = item.StepNo;
                    step.StepName = item.StepName;
                    step.StepDesc = item.StepDesc;
                    step.No = item.No;
                    step.Operation = item.Operation;
                    step.Group = item.Group;
                    steps.Add(step);
                }
            }

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
                    {"StationName","工站名称" },
                    {"StationCode","工站编码" },
                    //{"SmallStationCode","小工站编码" },
                    {"StepNo","工步编号" },
                    {"StepName","工步名称" },
                    {"No","顺序号" },
                    {"StepDesc","工步描述" },
                    {"Operation","操作" },
                    {"Identity","标识" },
                    {"Group","合并" },
                };

            // 将 ExcelPackage 转换为字节数组
            var bytes = ExcelUtils.ExportExcel(steps, keyValuePairs, "工艺流程", false);
            // 创建文件流
            var stream = new MemoryStream(bytes);
            // 设置响应头，指定响应的内容类型和文件名
            Response.Headers.Add("Content-Disposition", "attachment; filename=exported-file.xlsx");
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Route("param/recipe/stepimport")]
        [HttpGet]
        public ActionResult StepImport()
        {
            return View();
        }

        [Route("param/recipe/uploadStepFile")]
        [HttpPost]
        public ActionResult UploadStepFile(IFormFile file,string recipeId, string configId)
        {
            if (file != null && file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
                    {"工站名称","StationName" },
                    {"工站编码","StationCode" },
                    //{"小工站编码","SmallStationCode" },
                    {"工步编号","StepNo" },
                    {"工步名称","StepName" },
                    {"顺序号","No" },
                    {"工步描述","StepDesc" },
                    {"操作","Operation" },
                    {"标识","Identity" },
                    {"合并","Group" },
                };
                List<RecipeStep> models = ExcelUtils.ImportExcel<RecipeStep>(stream, keyValuePairs);

                bool v = paramStepItemLogic.import(models, recipeId, configId);
                if (v)
                {
                    return Success("初始化数据成功");
                }
                return Error("初始化数据失败");
            }

            // 文件为空或上传失败的处理文件
            return Error();
        }


        [Route("param/recipe/exportparam")]
        [HttpGet]
        public ActionResult ExportParam(string productId, string configId)
        {
            List<ParamRecipeItem> recipes = recipeLogic.QueryAllItem(productId, configId);
            List<RecipeProcessParam> processParams = new List<RecipeProcessParam>();
            //.OrderBy(u => int.Parse(u.StepNo)
            foreach (var recipe in recipes)
            {
                foreach (var item in recipe.ParamList)
                {
                    RecipeProcessParam processParam = new RecipeProcessParam();
                    processParam.StationName = recipe.StationName;
                    processParam.StationCode = recipe.StationCode;
                    //processParam.SmallStationCode = item.SmallStationCode;
                    processParam.StepNo = item.StepNo;
                    processParam.StepName = item.StepName;
                    processParam.OrderNo = item.OrderNo;
                    processParam.ParamCode = item.ParamCode;
                    processParam.ParamName = item.ParamName;
                    processParam.ProcessDescription = item.ProcessDescription;
                    processParam.ParamClassification = item.ParamClassification;
                    processParam.DecisionType = item.DecisionType;
                    processParam.ParamType = item.ParamType;
                    processParam.StandValue = item.StandValue;
                    processParam.MaxValue = item.MaxValue;
                    processParam.MinValue = item.MinValue;
                    processParam.SetValue = item.SetValue;
                    processParam.IsDoubleCheck = item.IsDoubleCheck;
                    processParam.UnitOfMeasure = item.UnitOfMeasure;
                    processParams.Add(processParam);
                }
            }

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
                    {"StationName","工站名称" },
                    {"StationCode","工站编码" },
                    //{"SmallStationCode","小工站编码" },
                    {"StepNo","工步编号" },
                    {"StepName","工步名称" },
                    {"OrderNo","顺序号" },
                    {"ParamCode","工艺参数编码" },
                    {"ParamName","工艺参数名称" },
                    {"ProcessDescription","工艺描述" },
                    {"ParamClassification","参数分类" },
                    {"DecisionType","判定类型" },
                    {"ParamType","参数类型" },
                    {"StandValue","工艺参数标准值" },
                    {"MaxValue","工艺参数最大值" },
                    {"MinValue","工艺参数最小值" },
                    {"SetValue","定性的设定值" },
                    {"IsDoubleCheck","二次校验" },
                    {"UnitOfMeasure","单位" },
                };

            // 将 ExcelPackage 转换为字节数组
            var bytes = ExcelUtils.ExportExcel(processParams, keyValuePairs, "工艺参数", false);

            // 创建文件流
            var stream = new MemoryStream(bytes);

            // 设置响应头，指定响应的内容类型和文件名
            Response.Headers.Add("Content-Disposition", "attachment; filename=exported-file.xlsx");
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Route("param/recipe/paramimport")]
        [HttpGet]
        public ActionResult ParamImport()
        {
            return View();
        }

        [Route("param/recipe/uploadParamFile")]
        [HttpPost]
        public ActionResult UploaParamFile(IFormFile file, string recipeId, string configId)
        {
            if (file != null && file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
                    {"工站名称","StationName" },
                    {"工站编码","StationCode" },
                    //{"小工站编码","SmallStationCode" },
                    {"工步编号","StepNo" },
                    {"工步名称","StepName" },
                    {"顺序号","OrderNo" },
                    {"工艺参数编码","ParamCode" },
                    {"工艺参数名称","ParamName" },
                    {"工艺描述","ProcessDescription" },
                    {"参数分类","ParamClassification" },
                    {"判定类型","DecisionType" },
                    {"参数类型","ParamType" },
                    {"工艺参数标准值","StandValue" },
                    {"工艺参数最大值","MaxValue" },
                    {"工艺参数最小值","MinValue" },
                    {"定性的设定值","SetValue" },
                    {"二次校验","IsDoubleCheck" },
                    {"单位","UnitOfMeasure" },
                };
                List<RecipeProcessParam> models = ExcelUtils.ImportExcel<RecipeProcessParam>(stream, keyValuePairs);

                bool v = paramItemLogic.import(models, recipeId, configId);
                if (v)
                {
                    return Success("导入过程数据成功");
                }
                return Error("导入过程数据失败");
            }

            // 文件为空或上传失败的处理文件
            return Error();
        }

        [Route("param/recipe/exportpart")]
        [HttpGet]
        public ActionResult ExportPart(string productId, string configId)
        {
            List<ParamRecipeItem> recipes = recipeLogic.QueryAllItem(productId, configId);
            List<RecipePart> parts = new List<RecipePart>();
            //.OrderBy(u => int.Parse(u.StepNo)
            foreach (var recipe in recipes)
            {
                foreach (var item in recipe.PartList)
                {
                    RecipePart part = new RecipePart();
                    part.StationName = recipe.StationName;
                    part.StationCode = recipe.StationCode;
                    //part.SmallStationCode = item.SmallStationCode;
                    part.StepNo = item.StepNo;
                    part.StepName = item.StepName;
                    part.OrderNo = item.OrderNo;
                    part.PartNumber = item.PartNumber;
                    part.PartDescription = item.PartDescription;
                    part.PartType = item.PartType;
                    part.PartVersion = item.PartVersion;
                    part.PartQty = item.PartQty;
                    part.Uom = item.Uom;
                    parts.Add(part);
                }
            }

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
                    {"StationName","工站名称" },
                    {"StationCode","工站编码" },
                    //{"SmallStationCode","小工站编码" },
                    {"StepNo","工步编号" },
                    {"StepName","工步名称" },
                    {"OrderNo","顺序号" },
                    {"PartNumber","物料编码" },
                    {"PartDescription","物料描述" },
                    {"PartType","物料类型" },
                    {"PartVersion","物料版本" },
                    {"PartQty","数量" },
                    {"Uom","单位" }
                };

            // 将 ExcelPackage 转换为字节数组
            var bytes = ExcelUtils.ExportExcel(parts, keyValuePairs, "物料参数", false);

            // 创建文件流
            var stream = new MemoryStream(bytes);

            // 设置响应头，指定响应的内容类型和文件名
            Response.Headers.Add("Content-Disposition", "attachment; filename=exported-file.xlsx");
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }


        [Route("param/recipe/partimport")]
        [HttpGet]
        public ActionResult PartImport()
        {
            return View();
        }

        [Route("param/recipe/uploadPartFile")]
        [HttpPost]
        public ActionResult UploaPartFile(IFormFile file, string recipeId, string configId)
        {
            if (file != null && file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
                    {"工站名称","StationName" },
                    {"工站编码","StationCode" },
                    //{"小工站编码","SmallStationCode" },
                    {"工步编号","StepNo" },
                    {"工步名称","StepName" },
                    {"顺序号","OrderNo" },
                    {"物料编码","PartNumber" },
                    {"物料描述","PartDescription" },
                    {"物料类型","PartType" },
                    {"物料版本","PartVersion" },
                    {"数量","PartQty" },
                    {"单位","Uom" }
                };
                List<RecipePart> models = ExcelUtils.ImportExcel<RecipePart>(stream, keyValuePairs);

                bool v = paramPartItemLogic.import(models, recipeId, configId);
                if (v)
                {
                    return Success("导入过程数据成功");
                }
                return Error("导入过程数据失败");
            }

            // 文件为空或上传失败的处理文件
            return Error();
        }
    }
}
