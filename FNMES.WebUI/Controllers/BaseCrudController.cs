using FNMES.Entity;
using FNMES.Entity.DTO.Parms;
using FNMES.Reposity;
using FNMES.Service;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.Utility.Other;
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


        [Route("form")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Form(T model)
        {
            PropertyInfo primaryKeyProperty = GetPrimaryKeyProperty(model);
            if (primaryKeyProperty.GetValue(model).IsNullOrEmpty())
            {
                primaryKeyProperty.SetValue(model, UUID.StrSnowId);
                model.EnableFlag = "Y";
                model.DeleteFlag = "N";
                model.CreateUserId = OperatorProvider.Instance.Current.UserId;
                model.ModifyUserId = model.CreateUserId;
                model.CreateTime = DateTime.Now;
                model.ModifyTime = model.CreateTime;
                bool flag = _baseService._repository.Insert(model);
                return flag ? Success() : Error();
            }
            else
            {
                model.ModifyUserId = OperatorProvider.Instance.Current.UserId;
                model.ModifyTime = DateTime.Now;
                bool flag = _baseService._repository.Update(model);
                return flag ? Success() : Error();
            }
        }


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

        /// <summary>
        /// 组织机构主界面数据
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/index")]
        public ActionResult AppIndex([FromBody] SearchParms parms)
        {
            int totalCount = 0;
            List<T> pageData = _baseService.GetList(parms.pageIndex, parms.pageSize, null, ref totalCount);
            var result = new LayPadding<T>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount
            };
            return AppSuccess<LayPadding<T>>(result);
        }

        /// <summary>
        /// 新增/修改组织机构数据提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("app/form")]
        public ActionResult AppForm([FromBody] T model)
        {
            PropertyInfo primaryKeyProperty = GetPrimaryKeyProperty(model);
            if (primaryKeyProperty.GetValue(model).IsNullOrEmpty())
            {
                primaryKeyProperty.SetValue(model, UUID.StrSnowId);
                model.EnableFlag = "Y";
                model.DeleteFlag = "N";
                model.CreateTime = DateTime.Now;
                model.ModifyTime = model.CreateTime;
                bool flag = _baseService._repository.Insert(model);
                return flag ? AppSuccess() : AppError();
            }
            else
            {
                model.ModifyTime = DateTime.Now;
                bool flag = _baseService._repository.Update(model);
                return flag ? AppSuccess() : AppError();
            }
        }

        private PropertyInfo GetPrimaryKeyProperty(T model)
        {
            Type type = typeof(T);
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                SugarColumn column = propertyInfo.GetCustomAttribute<SugarColumn>();
                if (column == null)
                    continue;
                if (column.IsPrimaryKey)
                    return propertyInfo;
            }
            return null;
        }

        /// <summary>
        /// 根据主键获取组织机构数据
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/getForm")]
        public ActionResult AppGetForm([FromBody] StrPrimaryKeyParms parms)
        {
            T entity = _baseService.Get(parms.primaryKey);
            if (entity == null)
            {
                return AppError("不存在");
            }
            return AppSuccess<T>(entity);
        }


        /// <summary>
        /// 删除组织数据
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/delete")]
        public ActionResult AppDelete([FromBody] StrPrimaryKeyParms parms)
        {
            bool flag = _baseService.Delete(parms.primaryKey.SplitToList());
            return flag ? AppSuccess() : AppError();
        }
    }
}
