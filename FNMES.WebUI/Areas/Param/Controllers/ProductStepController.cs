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

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Param")]
    public class ProductStepController : BaseController
    {
        private readonly ParamProductLogic productLogic;
        private readonly ProductStepLogic  productStepLogic;
        public ProductStepController()
        {
            productLogic = new ParamProductLogic();
            productStepLogic = new ProductStepLogic();
        }


        [Route("param/productStep/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("param/productStep/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId,string productId)
        {
            try
            {
                int totalCount = 0;
                var pageData = productStepLogic.GetList(pageIndex, pageSize, keyWord, configId, ref totalCount, long.Parse(productId));
                var result = new LayPadding<ParamProductStep>()
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
                return Content(new LayPadding<ParamProduct>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamProduct>(),
                    count =0
                }.ToJson()) ;
            }
        }




        [Route("param/productStep/form")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Form()
        {
            return View();
        }

        [Route("param/productStep/form")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Form(ParamProductStep model)
        {
            if (model.Id==0)
            {
                int row = productStepLogic.Insert(model,long.Parse(OperatorProvider.Instance.Current.UserId));
                return row > 0 ? Success() : Error();
            }
            else
            {
                //更新用户基本信息。
                int row = productStepLogic.Update(model, long.Parse(OperatorProvider.Instance.Current.UserId));
                //更新用户角色信息。
                return row > 0 ? Success() : Error();
            }
        }

        [Route("param/productStep/detail")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Detail()
        {
            return View();
        }


        [Route("param/productStep/getForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetForm(string primaryKey, string configId)
        {
            ParamProductStep entity = productStepLogic.Get(long.Parse(primaryKey),configId);
            return Content(entity.ToJson());
        }





        [Route("param/productStep/delete")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string primaryKey, string configId)
        {
            
            /*//过滤系统管理员
            if (productStepLogic.ContainsUser("admin", userIdList.ToArray()))
            {
                return Error("产品有已设置的配方，不允许删除");
            }*/
            return productStepLogic.Delete(long.Parse(primaryKey), configId) > 0 ? Success() : Error();
        } 



              
    }
}
