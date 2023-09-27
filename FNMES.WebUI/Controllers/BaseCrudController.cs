using FNMES.Entity;
using FNMES.Reposity;
using FNMES.Service;
using FNMES.Utility.Core;
using FNMES.Utility.ResponseModels;
using FNMES.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FNMES.WebUI.Controllers
{
    public class BaseCrudController<T, TRepository> : BaseController where T : BaseModelEntity, new() where TRepository : IRepository<T>
    {

        protected IBaseService<T, TRepository> _baseService;
        protected IRepository<T> _repository;
        public BaseCrudController(IBaseService<T, TRepository> iBaseService)
        {
            _baseService = iBaseService;
            _repository = iBaseService._repository;
        }


        [Route("index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord)
        {
            int totalCount = 0;
            List<T> list = _baseService.GetList(pageIndex, pageSize, null, ref totalCount);
            var result = new LayPadding<T>()
            {
                result = true,
                msg = "success",
                list = list,
                count = totalCount//pageData.Count
            };
            return Content(result.ToJson());
        }



        [Route("form")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Form()
        {
            return View();
        }


        /*[Route("form")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Form(T model)
        {
            PropertyInfo primaryKeyProperty = GetPrimaryKeyProperty(model);
            if (primaryKeyProperty.GetValue(model).IsNullOrEmpty())
            {
                primaryKeyProperty.SetValue(model, UUID.StrSnowId);
                model.EnableFlag = "Y";
                model.CreateUserId = long.Parse(OperatorProvider.Instance.Current.UserId);
                model.ModifyUserId = model.CreateUserId;
                model.CreateTime = DateTime.Now;
                model.ModifyTime = model.CreateTime;
                bool flag = _baseService._repository.Insert(model);
                return flag ? Success() : Error();
            }
            else
            {
                model.ModifyUserId = long.Parse(OperatorProvider.Instance.Current.UserId);
                model.ModifyTime = DateTime.Now;
                bool flag = _baseService._repository.Update(model);
                return flag ? Success() : Error();
            }
        }*/


        [Route("form")]
        [HttpPost, LoginChecked]
        public ActionResult GetForm(string primaryKey)
        {
            var entity = _baseService.Get(primaryKey);
            return Content(entity.ToJson());
        }



        [Route("delete")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Delete(string primaryKey)
        {
            bool flag = _baseService.Delete(primaryKey.SplitToList());
            return flag ? Success() : Error();
        }



        [Route("detail")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Detail()
        {
            return View();
        }


    }
}
