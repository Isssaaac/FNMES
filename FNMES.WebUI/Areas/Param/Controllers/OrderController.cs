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
using FNMES.Utility.Network;
using FNMES.WebUI.API;
using FNMES.Entity.DTO.ApiParam;
using FNMES.WebUI.Logic.Sys;
using FNMES.Entity.Sys;
using FNMES.Entity.DTO.ApiData;
using System.Linq;
using SqlSugar;


namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Param")]
    [HiddenApi]
    public class OrderController : BaseController
    {
        private readonly ParamOrderLogic orderLogic;
        private readonly SysLineLogic sysLineLogic;
        private readonly ParamProductLogic productLogic;
        public OrderController()
        {
            orderLogic = new ParamOrderLogic();
            sysLineLogic = new SysLineLogic();
            productLogic = new ParamProductLogic();
        }


        [Route("param/order/index")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Index()
        {
            return View();
        }


        [Route("param/order/index")]
        [HttpPost, AuthorizeChecked]
        public ActionResult Index(int pageIndex, int pageSize, string keyWord,string configId,string index)
        {
            try
            {
                int totalCount = 0;
                var pageData = orderLogic.GetList(pageIndex, pageSize, keyWord, configId, ref totalCount, index);
                var result = new LayPadding<ParamOrder>()
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
                return Content(new LayPadding<ParamOrder>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<ParamOrder>(),
                    count =0
                }.ToJson()) ;
            }
        }


        [Route("param/order/detail")]
        [HttpGet, AuthorizeChecked]
        public ActionResult Detail()
        {
            return View();
        }


        [Route("param/order/getForm")]
        [HttpPost, LoginChecked]
        public ActionResult GetForm(string primaryKey, string configId)
        {
            ParamOrder entity = orderLogic.GetWithQty(long.Parse(primaryKey),configId);
            return Content(entity.ToJson());
        }

        [Route("param/order/start")]
        [HttpPost, LoginChecked]
        public ActionResult Start(string primaryKey, string configId)
        {
            SysLine sysLine = sysLineLogic.GetByConfigId(configId);
            //没有激活订单才可以激活，且当前订单状态为0-未开工或2- 暂停
            ParamOrder entity = orderLogic.GetSelected( configId);
            if(entity != null)
            {
                return Error("已存在开工工单，操作不允许");
            }
            ParamOrder order = orderLogic.Get(long.Parse(primaryKey), configId);
            if (order == null)
            {
                return Error("选中工单不存在");
            }
            if (order.Flag == "0" || order.Flag == "2")
            {
                order.Flag = "1";
                order.StartTime = DateTime.Now;
                SelectOrderParam orderParam = new SelectOrderParam() { 
                    taskOrderNumbers = new List<SelectOrder>() { new SelectOrder() { 
                        taskOrderNumber = order.TaskOrderNumber ,
                        actionCode = ActionCode.Start, } } ,
                    stationCode = "M300",    //此处填充内容根据工厂确定
                    equipmentID = "FN-GZ-XTSX-03-M300-A",          //20240409更新设备编码FN-GZXNY-PACK-024 
                    productionLine = sysLine.EnCode,
                    operatorNo = OperatorProvider.Instance.Current.UserId,
                    actualStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                };

                //准备开工，先请求工厂接口，再更新本地数据
                RetMessage<object> retMessage = APIMethod.Call(FNMES.WebUI.API.Url.SelectOrderUrl, orderParam, configId).ToObject<RetMessage<object>>();

                if(retMessage.messageType == "S")
                {
                    int v = orderLogic.Update(order, configId);
                    if (v == 0)
                    {
                        return Error("选中工单开工失败");
                    }
                    return Success("开工成功");
                }
                else
                {
                    return Error("工厂接口同步失败");
                }
            }
            else
            {
                return Error("选中工单不满足开工条件");
            }
            
        }

        [Route("param/order/pause")]
        [HttpPost, LoginChecked]
        public ActionResult Pause(string primaryKey, string configId)
        {
            //激活才能暂停
            SysLine sysLine = sysLineLogic.GetByConfigId(configId);
            ParamOrder order = orderLogic.Get(long.Parse(primaryKey), configId);
            if (order == null)
            {
                return Error("选中工单不存在");
            }
            if (order.Flag == "1" )
            {
                order.Flag = "2";
                SelectOrderParam orderParam = new SelectOrderParam()
                {
                    taskOrderNumbers = new List<SelectOrder>() { new SelectOrder() { 
                        taskOrderNumber = order.TaskOrderNumber, 
                        actionCode = ActionCode.Pause } },
                    stationCode = "M300",    //此处填充内容根据工厂确定
                    equipmentID = "FN-GZ-XTSX-03-M300-A",    //20240409更新设备编码FN-GZXNY-PACK-024
                    productionLine = sysLine.EnCode,
                    operatorNo = OperatorProvider.Instance.Current.UserId,
                    actualStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                };

                //准备开工，先请求工厂接口，再更新本地数据
                RetMessage<object> retMessage = APIMethod.Call(FNMES.WebUI.API.Url.SelectOrderUrl, orderParam, configId).ToObject<RetMessage<object>>();

                if (retMessage.messageType == "S")
                {
                    int v = orderLogic.Update(order, configId);
                    if (v == 0)
                    {
                        return Error("选中工单暂停失败");
                    }
                    return Success("暂停成功");
                }
                else
                {
                    return Error("工厂接口同步失败");
                }
            }
            else
            {
                return Error("选中工单不满足暂停条件");
            }

        }

        [Route("param/order/cancel")]
        [HttpPost, LoginChecked]
        public ActionResult Cancel(string primaryKey, string configId)
        {
            SysLine sysLine = sysLineLogic.GetByConfigId(configId);
            ParamOrder order = orderLogic.Get(long.Parse(primaryKey), configId);
            if (order == null)
            {
                return Error("选中工单不存在");
            }
            if (order.Flag == "1"|| order.Flag == "2")
            {
                order.Flag = "3";
                SelectOrderParam orderParam = new SelectOrderParam()
                {
                    taskOrderNumbers = new List<SelectOrder>() { new SelectOrder() { 
                        taskOrderNumber = order.TaskOrderNumber,
                        actionCode = ActionCode.Cancel} },
                    stationCode = "M300",    //此处填充内容根据工厂确定S
                    equipmentID = "FN-GZ-XTSX-03-M300-A",   //20240409更新设备编码FN-GZXNY-PACK-024        
                    productionLine = sysLine.EnCode,
                    operatorNo = OperatorProvider.Instance.Current.UserId,
                    actualStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                };

                //准备开工，先请求工厂接口，再更新本地数据
                RetMessage<object> retMessage = APIMethod.Call(FNMES.WebUI.API.Url.SelectOrderUrl, orderParam, configId).ToObject<RetMessage<object>>();

                if (retMessage.messageType == "S")
                {
                    int v = orderLogic.Update(order, configId);
                    if (v == 0)
                    {
                        return Error("选中工单取消失败");
                    }
                    return Success("取消成功");
                }
                else
                {
                    return Error("工厂接口同步失败");
                }
            }
            else
            {
                return Error("选中工单不满足取消条件");
            }
        }

        object GetOrderLock=new object();
        [Route("param/order/getOrder")]
        [HttpPost, LoginChecked]
        public ActionResult GetOrder(string configId)
        {
            lock (GetOrderLock)
            {
                SysLine sysLine = sysLineLogic.GetByConfigId(configId);

                GetOrderParam getOrderParam = new GetOrderParam()
                {
                    productionLine = sysLine.EnCode,
                    stationCode = "M300",
                    operatorNo = OperatorProvider.Instance.Current.Name
                };
                //应该是这里同步工单
                RetMessage<GetOrderData> retMessage = APIMethod.Call(FNMES.WebUI.API.Url.GetOrderUrl, getOrderParam, configId).ToObject<RetMessage<GetOrderData>>();
                if (retMessage.messageType == "S")
                {
                    List<ParamOrder> paramOrders = new List<ParamOrder>();
                    foreach (var model in retMessage.data.workOrderList)
                    {
                        paramOrders.Add(new ParamOrder()
                        {
                            Id = SnowFlakeSingle.instance.NextId(),
                            TaskOrderNumber = model.taskOrderNumber,
                            ProductPartNo = model.productPartNo,
                            ProductDescription = model.productDescription,
                            PlanQty = Convert.ToInt16(model.planQty),
                            Uom = model.uom,
                            PlanStartTime = model.planStartTime,
                            PlanEndTime = model.planEndTime,
                            ReceiveTime = DateTime.Now,
                            Flag = "0",
                            FinishFlag = "0",
                            OperatorNo = ""
                        });
                    }

                    //这里插入工单信息到线体mes数据库，后面插入
                    int v = orderLogic.Insert(retMessage.data.workOrderList, configId);

                    //同步后需要再向工厂发送一个已接收到的指令
                    //List<ParamOrder> paramOrders = orderLogic.GetNew(configId);

                    if (!paramOrders.IsNullOrEmpty() && paramOrders.Count > 0)
                    {
                        List<SelectOrder> orders = paramOrders.Select(it => new SelectOrder()
                        {
                            taskOrderNumber = it.TaskOrderNumber,
                            actionCode = ActionCode.Received
                        }).ToList();

                        SelectOrderParam selectOrderParam = new SelectOrderParam()
                        {
                            taskOrderNumbers = orders,
                            productionLine = sysLine.EnCode,
                            stationCode = "M300",
                            equipmentID = "FN-GZ-XTSX-03-M300-A",//20240409FN-GZXNY-PACK-024     更改设备编码20240409
                            operatorNo = OperatorProvider.Instance.Current.Name,
                            actualStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                        };

                        //回传在这里
                        RetMessage<object> retMessage1 = APIMethod.Call(FNMES.WebUI.API.Url.SelectOrderUrl, selectOrderParam, configId).ToObject<RetMessage<object>>();
                        if (retMessage1.messageType == "S")
                        {
                            return Success("同步完成");
                        }
                        else
                        {
                            return Error("同步完成后回传失败");
                        }
                    }

                    return Success("同步完成");
                }
                else
                {
                    return Error("工厂接口访问失败");

                }
            }
        }

        //同步产品配方
        [Route("param/order/getRecipe")]
        [HttpPost]
        public ActionResult GetRecipe(string configId, string primaryKey, bool force)
        {
            SysLine sysLine = sysLineLogic.GetByConfigId(configId);
            ParamOrder order = orderLogic.Get(long.Parse(primaryKey), configId);
            //从工厂同步配方
            if(order == null)
            {
                return Error("查无此产品");
            }

            GetRecipeParam param = new()
            {
                productionLine = sysLine.EnCode,
                productPartNo = order.ProductPartNo,
                smallStationCode = "",
                stationCode = "",
                section = "后段",
                equipmentID = "FN-GZ-XTSX-03-M300-A",   //20240409更新设备编码FN-GZXNY-PACK-024
                operatorNo = OperatorProvider.Instance.Current.UserId,
                actualStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()
            };
            RetMessage<GetRecipeData> retMessage = APIMethod.Call(FNMES.WebUI.API.Url.GetRecipeUrl, param, configId).ToObject<RetMessage<GetRecipeData>>();
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
    }
}
