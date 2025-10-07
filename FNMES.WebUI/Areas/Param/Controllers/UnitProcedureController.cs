using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Entity.Sys;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using System.Collections.Generic;
using FNMES.WebUI.Logic.Param;
using FNMES.WebUI.Logic;
using FNMES.Entity.Param;
using FNMES.WebUI.Logic.Sys;
using System.Drawing.Drawing2D;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Drawing.Printing;
using FNMES.Entity.DTO;
using FNMES.Utility.Files;
using System.IO;
using Microsoft.AspNetCore.Http;

//Param_LocalRoute和Param_RecipeItem应当同步
namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Param")]
    [HiddenApi]
    public class UnitProcedureController : BaseController
    {
        private readonly UnitProcedureLogic unitProcedureLogic;
        private readonly SysLineLogic sysLineLogic;
        private readonly RecipeLogic productStepLogic;
        private readonly ParamRecipeItemLogic productItemLogic;

        public UnitProcedureController()
        {
            unitProcedureLogic = new UnitProcedureLogic();
            sysLineLogic = new SysLineLogic();
            productStepLogic = new RecipeLogic();
            productItemLogic = new ParamRecipeItemLogic();
        }

        [Route("param/unitProcedure/getParent")]
        [HttpPost]
        public ActionResult GetParent(string configId)
        {
            var data = unitProcedureLogic.GetParentList(configId);
            var treeList = new List<TreeSelect>();
            
            foreach (ParamUnitProcedure item in data)
            {
                TreeSelect model = new()
                {
                    id = item.Id.ToString(),
                    text = item.Encode,
                };
                treeList.Add(model);
            }
            return Content(treeList.ToJson());
        }

        //获取大工站,GetSonList获取小工站，有个IsParent的标志位，等于1是大工站
        [Route("param/unitProcedure/getParentCode")]
        [HttpPost]
        public ActionResult GetParentCode(string configId)
        {
            var data = unitProcedureLogic.GetParentList(configId);
            var treeList = new List<TreeSelect>();

            foreach (ParamUnitProcedure item in data)
            {
                TreeSelect model = new()
                {
                    id = item.Encode,
                    text = item.Encode,
                };
                treeList.Add(model);
            }
            return Content(treeList.ToJson());
        }

        //获取小工站,GetSonList获取小工站，有个IsParent的标志位，等于0是小工站
        [Route("param/unitProcedure/getListTreeUnSelect")]
        [HttpPost]
        public ActionResult GetListTreeUnSelect(string configId)
        {
            var data = unitProcedureLogic.GetSonList(configId);
            var treeList = new List<TreeSelect>();
            if(data.Count > 0)
            {
                foreach (ParamUnitProcedure item in data)
                {
                        TreeSelect model = new()
                        {
                            id = item.Encode.ToString(),
                            text = item.Encode,
                        };
                        treeList.Add(model);
                }
            }
            return Content(treeList.ToJson());
        }

        [Route("param/unitProcedure/getParentByLine")]
        [HttpPost]
        public ActionResult GetParentByLine(string lineId)
        {
            SysLine sysLine = sysLineLogic.Get(long.Parse(lineId));
            var data = unitProcedureLogic.GetParentList(sysLine.ConfigId);
            var treeList = new List<TreeSelect>() {
               new TreeSelect
               {
                    id = "null",
                    text = "--请选择--",
               }
            };

            foreach (ParamUnitProcedure item in data)
            {
                TreeSelect model = new()
                {
                    id = item.Encode,
                    text = item.Encode,
                };
                treeList.Add(model);
            }
            return Content(treeList.ToJson());
        }

        [Route("param/unitProcedure/getProcedureByline")]
        [HttpPost]
        public ActionResult GetProcedureByLine(string lineId, string parent)
        {
            SysLine sysLine = sysLineLogic.Get(long.Parse(lineId));
            var data = unitProcedureLogic.GetProcedureList(sysLine.ConfigId, parent);
            var treeList = new List<TreeSelect>(){
               new TreeSelect
               {
                    id = "null",
                    text = "--请选择--",
               }

            };
            if(data != null)
            {
                foreach (ParamUnitProcedure item in data)
                {
                    TreeSelect model = new()
                    {
                        id = item.Encode,
                        text = item.Encode,
                    };
                    treeList.Add(model);
                }
            }
            return Content(treeList.ToJson());
        }

        [Route("param/unitProcedure/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }

        [Route("param/unitProcedure/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId)
        {
            try
            {
                int totalCount = 0;
                var pageData = unitProcedureLogic.GetList(pageIndex, pageSize, keyWord, configId, ref totalCount);
                var result = new LayPadding<ParamUnitProcedure>()
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
                return Content(new LayPadding<ParamUnitProcedure>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamUnitProcedure>(),
                    count = 0
                }.ToJson());
            }
        }

        [Route("param/unitProcedure/form")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Form()
        {
            return View();
        }

        [Route("param/unitProcedure/form")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Form( ParamUnitProcedure model)
        {

            //Logger.RunningInfo(model.ToJson()+"数据库"+model.ConfigId);
            
            if (model.Id==0)
            {
                if (model.IsParent == "1")
                    model.Pid = 0;
                var entities = unitProcedureLogic.GetList(model.ConfigId);
                if (entities != null)
                {
                    if (entities.Exists(it => it.Encode == model.Encode))
                    {
                        return  Error($"{model.Encode}已存在");
                    }
                }
                    
                int row = unitProcedureLogic.Insert(model,long.Parse(OperatorProvider.Instance.Current.UserId));
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = unitProcedureLogic.Update(model, long.Parse(OperatorProvider.Instance.Current.UserId));
                return row > 0 ? Success() : Error();
            }
        }

        [Route("param/unitProcedure/detail")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Detail()
        {
            return View();
        }

        [Route("param/unitProcedure/getForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetForm(string primaryKey, string configId)
        {
            ParamUnitProcedure entity = unitProcedureLogic.Get(long.Parse(primaryKey),configId);
            return Content(entity.ToJson());
        }

        [Route("param/unitProcedure/getStationName")]
        [HttpPost]
        public ActionResult GetStationName(string id, string configId)
        {
            var entitys = unitProcedureLogic.GetParentList(configId);
            var stationName = entitys.Where(it => it.Id == long.Parse(id)).Select(it => it.Name).First();
            return Content(stationName);
        }

        [Route("param/unitProcedure/getStationNameByCode")]
        [HttpPost]
        public ActionResult GetStationNameByCode(string smallStationCode, string lineId)
        {
            try
            {
                SysLine sysLine = sysLineLogic.Get(long.Parse(lineId));
                var entitys = unitProcedureLogic.GetSonList(sysLine.ConfigId);
                var stationName = entitys.Where(it => it.Encode == smallStationCode).Select(it => it.Name).First();
                return Content(stationName);
            }
            catch (Exception ex)
            {
                return Content("");
            }
        }

        [Route("param/unitProcedure/delete")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string primaryId, string configId)
        {
            
            /*//过滤系统管理员
            if (productStepLogic.ContainsUser("admin", userIdList.ToArray()))
            {
                return Error("产品有已设置的配方，不允许删除");
            }*/
            return unitProcedureLogic.Delete(long.Parse(primaryId), configId) > 0 ? Success() : Error();
        }


        [Route("param/unitProcedure/export")]
        [HttpGet]
        public ActionResult Export(string configId)
        {
            List<ParamUnitProcedure> paramUnitProcedures = unitProcedureLogic.GetList(configId);
            List<UnitProcedure> ups = new List<UnitProcedure>();
            foreach (var unitProcedure in paramUnitProcedures)
            {
                UnitProcedure up = new UnitProcedure();
                if (unitProcedure.Pid == 0)
                    up.Parent = "";
                else
                    up.Parent = paramUnitProcedures.Where(it=>it.Id== unitProcedure.Pid).First().Encode;
                up.Encode = unitProcedure.Encode;
                up.Name = unitProcedure.Name;
                up.Description = unitProcedure.Description;
                up.InStationProductType = unitProcedure.InStationProductType;
                up.OutStationProductType = unitProcedure.OutStationProductType;
                ups.Add(up);
            }

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
                    {"Parent","父工站" },
                    {"Encode","工站编码" },
                    {"Name","工站名称" },
                    {"Description","工步描述" },
                    {"InStationProductType","进站产品类型" },
                    {"OutStationProductType","岀站产品类型" },
                };

            // 将 ExcelPackage 转换为字节数组
            var bytes = ExcelUtils.ExportExcel(ups, keyValuePairs, "设备管理", false);
            // 创建文件流
            var stream = new MemoryStream(bytes);
            // 设置响应头，指定响应的内容类型和文件名
            Response.Headers.Add("Content-Disposition", "attachment; filename=exported-file.xlsx");
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Route("param/unitProcedure/import")]
        [HttpGet]
        public ActionResult Import()
        {
            return View();
        }

        [Route("param/unitProcedure/uploadParamFile")]
        [HttpPost]
        public ActionResult UploaParamFile(IFormFile file, string recipeId, string configId)
        {
            if (file != null && file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
                    {"父工站","Parent" },
                    {"工站编码","Encode" },
                    {"工站名称","Name" },
                    {"工步描述","Description" },
                    {"进站产品类型","InStationProductType" },
                    {"岀站产品类型","OutStationProductType" },
                };
                List<UnitProcedure> models = ExcelUtils.ImportExcel<UnitProcedure>(stream, keyValuePairs);

                bool v = unitProcedureLogic.import(models, configId);
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
