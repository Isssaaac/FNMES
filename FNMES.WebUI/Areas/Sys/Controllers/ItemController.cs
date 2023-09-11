using System.Collections.Generic;
using FNMES.WebUI.Filters;
using FNMES.Logic.Sys;
using FNMES.Entity.Sys;
using FNMES.Utility.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using FNMES.Entity.DTO.Parms;
using FNMES.Logic;

namespace FNMES.WebUI.Areas.Sys.Controllers
{
    [HiddenApi]
    [Area("Sys")]
    public class ItemController : BaseController
    {
        private SysItemLogic itemLogic;
        private SysItemsDetailLogic itemsDetailLogic;

        public ItemController()
        {
            itemLogic = new SysItemLogic();
            itemsDetailLogic = new SysItemsDetailLogic();
        }


        [Route("system/item/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }

        [Route("system/item/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord)
        {
            int totalCount = 0;
            var pageData = itemLogic.GetList(pageIndex, pageSize, keyWord, ref totalCount);
            var result = new LayPadding<SysItem>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return Content(result.ToJson());
        }


        [Route("system/item/form")]
        [HttpGet, LoginChecked]
        public ActionResult Form()
        {
            return View();
        }


        [Route("system/item/form")]
        [HttpPost, LoginChecked]
        public ActionResult Form(SysItem model)
        {
            if (model.Id.IsNullOrEmpty())
            {
                int row = itemLogic.Insert(model, OperatorProvider.Instance.Current.UserId);
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = itemLogic.Update(model, OperatorProvider.Instance.Current.UserId);
                return row > 0 ? Success() : Error();
            }
        }

        [Route("system/item/getForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetForm(string primaryKey)
        {
            SysItem entity = itemLogic.Get(primaryKey);
            return Content(entity.ToJson());
        }





        [Route("system/item/delete")]
        [HttpPost, LoginChecked]
        public ActionResult Delete(string primaryKey)
        {
            int count = itemLogic.GetChildCount(primaryKey);
            if (count == 0)
            {
                //删除字典。
                int row = itemLogic.Delete(primaryKey);
                //删除字典选项。
                itemsDetailLogic.Delete(primaryKey);
                return row > 0 ? Success() : Error();
            }
            return Warning(string.Format("操作失败，请先删除该项的{0}个子级字典。", count));
        }


        [Route("system/item/detail")]
        [HttpGet, LoginChecked]
        public ActionResult Detail()
        {
            return View();
        }

        [Route("system/item/getListTree")]
        [HttpPost, LoginChecked]
        public ActionResult GetListTree()
        {
            var listAllItems = itemLogic.GetList();
            List<ZTreeNode> result = new List<ZTreeNode>();
            foreach (var item in listAllItems)
            {
                ZTreeNode model = new ZTreeNode();
                model.id = item.Id;
                model.pId = item.ParentId;
                model.name = item.Name;
                model.open = true;
                result.Add(model);
            }
            return Content(result.ToJson());
        }
        [Route("system/item/getListSelectTree")]
        [HttpPost, LoginChecked]
        public ActionResult GetListSelectTree()
        {
            var data = itemLogic.GetList();
            var treeList = new List<TreeSelect>();
            foreach (var item in data)
            {
                TreeSelect model = new TreeSelect();
                model.id = item.Id;
                model.text = item.Name;
                model.parentId = item.ParentId;
                treeList.Add(model);
            }
            return Content(treeList.ToTreeSelectJson());
        }

        /// <summary>
        /// 字典管理主界面数据
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/item/index")]
        public ActionResult AppIndex([FromBody] SearchParms parms)
        {
            int totalCount = 0;
            var pageData = itemLogic.GetAppList(parms.pageIndex, parms.pageSize, ref totalCount);
            var result = new LayPadding<SysItem>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return AppSuccess<LayPadding<SysItem>>(result);
        }

        /// <summary>
        /// 新增/修改字典管理数据提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/item/form")]
        public ActionResult AppForm([FromBody] SysItem model)
        {
            if (model.Id.IsNullOrEmpty())
            {
                int row = itemLogic.AppInsert(model, model.CreateUserId);
                return row > 0 ? AppSuccess() : AppError();
            }
            else
            {
                int row = itemLogic.AppUpdate(model, model.ModifyUserId);
                return row > 0 ? AppSuccess() : AppError();
            }
        }

        /// <summary>
        /// 根据主键获取字典管理
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/item/getForm")]
        public ActionResult AppGetForm([FromBody] StrPrimaryKeyParms parm)
        {
            SysItem entity = itemLogic.Get(parm.primaryKey);
            if (entity == null)
            {
                return AppError("再点不存在");
            }
            return AppSuccess<SysItem>(entity);
        }

        /// <summary>
        /// 删除字典管理
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost, Route("app/system/item/delete")]
        public ActionResult AppDelete([FromBody] StrPrimaryKeyParms parms)
        {
            int count = itemLogic.GetChildCount(parms.primaryKey);
            if (count == 0)
            {
                //删除字典。
                int row = itemLogic.Delete(parms.primaryKey);
                //删除字典选项。
                itemsDetailLogic.Delete(parms.primaryKey);
                Logger.OperateInfo($"用户{parms.operateUser}删除了字典");
                return row > 0 ? AppSuccess() : AppError();
            }
            return AppError($"操作失败，请先删除该项的{count}个子级字典。");
        }

        /// <summary>
        /// 获取字典管理数据列表
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("app/system/item/getListTree")]
        public ActionResult AppGetListTree()
        {
            var listAllItems = itemLogic.GetList();
            List<ZTreeNode> result = new List<ZTreeNode>();
            foreach (var item in listAllItems)
            {
                ZTreeNode model = new ZTreeNode();
                model.id = item.Id;
                model.pId = item.ParentId;
                model.name = item.Name;
                model.open = true;
                result.Add(model);
            }
            return AppSuccess<List<ZTreeNode>>(result);
        }
    }
}
