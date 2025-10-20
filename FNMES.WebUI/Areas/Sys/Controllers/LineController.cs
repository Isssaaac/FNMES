using System.Collections.Generic;
using FNMES.WebUI.Filters;
using FNMES.Entity.Sys;
using FNMES.Utility.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using FNMES.Utility.Core;
using FNMES.Utility.Operator;
using FNMES.WebUI.Controllers;
using FNMES.WebUI.Logic.Sys;
using FNMES.WebUI;
using Microsoft.Extensions.Localization;
namespace FNMES.WebUI.Areas.Sys.Controllers
{
    [HiddenApi]
    [Area("Sys")]
    public class LineController : BaseController
    {
        private readonly SysLineLogic lineLogic;
        private readonly SysEquipmentLogic equipmentLogic;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public LineController(IStringLocalizer<SharedResource> localizer)
        {
            lineLogic = new SysLineLogic();
            equipmentLogic = new SysEquipmentLogic();
            _localizer = localizer;
        }


        [Route("system/line/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }

        [Route("system/line/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord)
        {
            int totalCount = 0;
            var pageData = lineLogic.GetList(pageIndex, pageSize, keyWord, ref totalCount);
            var result = new LayPadding<SysLine>()
            {
                result = true,
                msg = "success",
                list = pageData,
                count = totalCount//pageData.Count
            };
            return Content(result.ToJson());
        }


        [Route("system/line/form")]
        [HttpGet, LoginChecked]
        public ActionResult Form()
        {
            return View();
        }


        [Route("system/line/form")]
        [HttpPost, LoginChecked]
        public ActionResult Form(SysLine model)
        {
            if (model.Id == 0)
            {
                int row = lineLogic.Insert(model, long.Parse(OperatorProvider.Instance.Current.UserId));
                return row > 0 ? Success() : Error();
            }
            else
            {
                int row = lineLogic.Update(model, long.Parse(OperatorProvider.Instance.Current.UserId));
                return row > 0 ? Success() : Error();
            }
        }

        [Route("system/line/getForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetForm(string primaryKey)
        {
            SysLine entity = lineLogic.Get(long.Parse(primaryKey));
            return Content(entity.ToJson());
        }





        [Route("system/line/delete")]
        [HttpPost, LoginChecked]
        public ActionResult Delete(string primaryKey)
        {
            return lineLogic.Delete(long.Parse(primaryKey)) > 0 ? Success() : Error();
        }


        [Route("system/line/detail")]
        [HttpGet, LoginChecked]
        public ActionResult Detail()
        {
            return View();
        }

        [Route("system/line/getListTree")]
        [HttpPost, LoginChecked]
        public ActionResult GetListTree()
        {
            var listAllLines = lineLogic.GetList();
            List<ZTreeNode> result = new()
            {
                new ZTreeNode
                {
                    id = "1",
                    pId = "0",
                    name = _localizer["LineList"],
                    open = true
                }
            };
            foreach (var line in listAllLines)
            {
                ZTreeNode model = new()
                {
                    id = line.Id.ToString(),
                    pId = "1",
                    name = line.Name,
                    myAttr = line.ConfigId 
                };
                result.Add(model);
            }
            return Content(result.ToJson());
        }


        [Route("system/line/getListTreeSelect")]
        [HttpPost, LoginChecked]
        public ActionResult GetListTreeSelect()
        {
            List<SysLine> listRole = lineLogic.GetList();
            var listTree = new List<TreeSelect>() { 
                 new TreeSelect{
                      id = "0",
                      text = $"----{_localizer["SelectLine"]}----"
                 }
            };
            foreach (var item in listRole)
            {
                TreeSelect model = new()
                {
                    id = item.ConfigId,
                    text = item.Name
                };
                listTree.Add(model);
            }
            return Content(listTree.ToJson());
        }
    }
}
