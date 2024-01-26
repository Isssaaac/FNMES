using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FNMES.WebUI.Filters;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using System.Linq;
using FNMES.WebUI.Logic;
using FNMES.Utility.Network;
using FNMES.WebUI.API;
using FNMES.Entity.DTO.ApiParam;
using FNMES.Entity.DTO.ApiData;
using Newtonsoft.Json.Serialization;
using FNMES.Utility.Other;
using SqlSugar;
using System;
using FNMES.Entity.Param;
using FNMES.WebUI.Logic.Param;
using FNMES.Entity.Sys;
using FNMES.WebUI.Logic.Sys;

namespace FNMES.WebUI.Areas.Param.Controllers
{
    [HiddenApi]
    [Area("Param")]
    public class AndonController : BaseController
    {
        private ParamAndonLogic sysAndonLogic;
        private SysLineLogic sysLineLogic;
        public AndonController()
        {
            sysAndonLogic = new ParamAndonLogic();
            sysLineLogic = new SysLineLogic();  
        }

        [Route("param/andon/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }

        [Route("param/andon/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord, string configId)
        {
            int totalCount = 0;
            var pageData = sysAndonLogic.GetList(pageIndex, pageSize, keyWord, ref totalCount,configId);
            var result = new LayPadding<ParamAndon>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount// pageData.Count
            };
            return Content(result.ToJson());
        }
        [Route("param/andon/get")]
        [HttpPost]
        //获取andon参数并更新
        public ActionResult Get(string configId)
        {
            SysLine sysLine = sysLineLogic.GetByConfigId(configId);
            AndonTypeParam andonParam = new() { 
                productionLine = sysLine.EnCode,
                operatorNo = OperatorProvider.Instance.Current.Name,
            };
            RetMessage<AndonTypeData> retMessage = APIMethod.Call(API.Url.AndonParamUrl, andonParam, configId).ToObject<RetMessage<AndonTypeData>>();
            if (retMessage.messageType == "S" && !retMessage.data.dataList.IsNullOrEmpty())
            {

                List<ParamAndon> list = new List<ParamAndon>();
                foreach (var item in retMessage.data.dataList)
                {
                    ParamAndon paramAndon = new ParamAndon()
                    {
                        Id = SnowFlakeSingle.Instance.NextId(),
                        AndonCode = item.andonCode,
                        AndonType = item.andonType,
                        AndonDesc = item.andonDesc,
                        AndonName = item.andonName,
                        CreateTime = DateTime.Now.ToString()
                    };
                    list.Add(paramAndon);
                }
                int v = sysAndonLogic.Update(list, configId);
                if (v > 0)
                {
                    return Success();
                }
                return Error();

            }
            else { return Error("工厂接口同步失败"); }


        }



    }
}
