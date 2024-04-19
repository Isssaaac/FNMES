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
using Org.BouncyCastle.Crypto.Agreement.Kdf;
using FNMES.WebUI.API;
using FNMES.Entity.DTO.ApiParam;
using FNMES.Utility.Network;
using FNMES.Entity.DTO.ApiData;
using FNMES.Entity.Sys;
using FNMES.WebUI.Logic.Sys;

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Param")]
    [HiddenApi]
    public class ProductController : BaseController
    {
        private readonly ParamProductLogic productLogic;
        private readonly SysLineLogic sysLineLogic;
        public ProductController()
        {
            productLogic = new ParamProductLogic();
            sysLineLogic = new SysLineLogic();
        }


        [Route("param/product/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("param/product/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId)
        {
            try
            {
                int totalCount = 0;
                var pageData = productLogic.GetList(pageIndex, pageSize, keyWord, configId, ref totalCount);
                var result = new LayPadding<ParamRecipe>()
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
                return Content(new LayPadding<ParamRecipe>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamRecipe>(),
                    count =0
                }.ToJson()) ;
            }
        }

        
        //强制同步产品配方
        [Route("param/product/getRecipe")]
        [HttpPost , AuthorizeChecked]
        public ActionResult GetRecipe(string configId,string primaryKey,bool force)
        {
            SysLine sysLine = sysLineLogic.GetByConfigId(configId);
            ParamRecipe paramRecipe = productLogic.Get(long.Parse(primaryKey),configId);
            //从工厂同步配方

            GetRecipeParam param = new() { 
                productionLine = sysLine.EnCode,
                productPartNo = paramRecipe.ProductPartNo,
                smallStationCode = "",
                stationCode = "",
                section = "后段",
                equipmentID = "FN-GZ-XTSX-03-M300-A",   //20240409更新设备编码 FN-GZXNY-PACK-024
                operatorNo = OperatorProvider.Instance.Current.UserId,
                actualStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()
            };
            RetMessage<GetRecipeData> retMessage = APIMethod.Call(FNMES.WebUI.API.Url.GetRecipeUrl, param,configId).ToObject<RetMessage<GetRecipeData>>();
            if (retMessage.messageType == "S")
            {
                int v = productLogic.insert(retMessage.data, configId, force);
                if (v > 0)
                {
                    return Success("同步完成");
                }
                return Error("同步失败");
            }
            else
            {
                return Error("工厂接口访问失败");

            }

        }

        [Route("param/product/getListTree")]
        [HttpPost, LoginChecked]
        public ActionResult GetListTree(string configId)
        {
            var listAllProduct = productLogic.GetList(configId);
            List<ZTreeNode> result = new()
            {
                new ZTreeNode
                {
                    id = "1",
                    pId = "0",
                    name = "产品列表",
                    open = true
                }
            };
            if (listAllProduct != null)
            {
                foreach (var product in listAllProduct)
                {
                    ZTreeNode model = new()
                    {
                        id = product.Id.ToString(),
                        pId = "1",
                        name = product.ProductPartNo,
                    };
                    result.Add(model);
                }
            }
           
            return Content(result.ToJson());
        }




    }
}
