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
    public class ProductController : BaseController
    {
        private readonly ParamProductLogic productLogic;
        public ProductController()
        {
            productLogic = new ParamProductLogic();
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
                var result = new LayPadding<ParamProduct>()
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




        [Route("param/product/form")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Form()
        {
            return View();
        }

        [Route("param/product/form")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Form( ParamProduct model)
        {

            Logger.RunningInfo(model.ToJson()+"数据库"+model.ConfigId);
            
            if (model.Id==0)
            {
                int row = productLogic.Insert(model,long.Parse(OperatorProvider.Instance.Current.UserId));
                return row > 0 ? Success() : Error();
            }
            else
            {
                //更新用户基本信息。
                int row = productLogic.Update(model, long.Parse(OperatorProvider.Instance.Current.UserId));
                //更新用户角色信息。
                return row > 0 ? Success() : Error();
            }
        }





        [Route("param/product/detail")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Detail()
        {
            return View();
        }


        [Route("param/product/getForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetForm(string primaryKey, string configId)
        {
            ParamProduct entity = productLogic.Get(long.Parse(primaryKey),configId);
            return Content(entity.ToJson());
        }





        [Route("param/product/delete")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string productId, string configId)
        {
            
            /*//过滤系统管理员
            if (productStepLogic.ContainsUser("admin", userIdList.ToArray()))
            {
                return Error("产品有已设置的配方，不允许删除");
            }*/
            return productLogic.Delete(long.Parse(productId), configId) > 0 ? Success() : Error();
        } 



              
    }
}
