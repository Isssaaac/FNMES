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
using FNMES.Entity.Record;
using FNMES.Utility.ResponseModels;
using System.Data;
using System.Diagnostics;
using System.IO;
using FNMES.Utility.Files;
using System.Threading.Tasks;

namespace MES.WebUI.Areas.Param.Controllers
{
    [Area("Param")]
    [HiddenApi]
    public class OrderController : BaseController
    {
        private readonly ParamOrderLogic orderLogic;
        private readonly SysLineLogic sysLineLogic;
        private readonly ParamProductLogic productLogic;
        private string orderIsSelected; //缓存已选中工单

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
        public ActionResult Index(int pageIndex, int pageSize, string keyWord, string configId, string index)
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
                    count = 0
                }.ToJson());
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
            ParamOrder entity = orderLogic.GetWithQty(long.Parse(primaryKey), configId);
            return Content(entity.ToJson());
        }

        [Route("param/order/data")]
        [HttpGet]
        public ActionResult Data()
        {
            return View();
        }

        [Route("param/order/getData")]
        [HttpPost]
        public ActionResult getData(int pageIndex, int pageSize,string primaryKey, string configId, string keyWord)
        {
            try {
                int totalCount = 0;
                List<RecordOrderStart> entity = orderLogic.GetProductInOrder(pageIndex, pageSize,long.Parse(primaryKey), configId,ref totalCount, keyWord);
                if (entity.IsNullOrEmpty())
                {
                    return Content(new LayPadding<RecordPartData>()
                    {
                        result = true,
                        msg = "无数据",
                        list = new List<RecordPartData>(),
                        count = 0
                    }.ToJson());
                }
                //这里获取工单
                orderIsSelected = entity[0].TaskOrderNumber;

                LayPadding<RecordOrderStart> result = new LayPadding<RecordOrderStart>()
                {
                    result = true,
                    msg = "success",
                    list = entity,
                    count = totalCount//pageData.Count
                };
                return Content(result.ToJson());
            }
            catch (Exception E)
            {
                return Content(new LayPadding<RecordPartData>()
                {
                    result = false,
                    msg = E.Message,
                    list = new List<RecordPartData>(),
                    count = 0
                }.ToJson());
            }
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
            //case "0": return "未开工";
            //case "1": return "生产中";
            //case "2": return "暂停";
            //case "3": return "取消";
            //case "4": return "完成";
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

        object GetOrderLock = new object();
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
                        //241205同步工单岀档位数据
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
                            OperatorNo = "",
                            //广州和赣州的不确定是否能同步，如果不同步的话，这里会不会报错，241217证实有无都不会报错
                            PackCellGear = model.packCellGear
                        });
                        Logger.RunningInfo($"工单:{model.taskOrderNumber},档位:{model.packCellGear}");
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

        [Route("param/order/scrap")]
        [HttpGet]
        public ActionResult Scrap()
        {
            return View();
        }

        //同步产品配方
        [Route("param/order/scrap")]
        [HttpPost]
        public async Task<ActionResult> Scrap(SynScrapInfoParamForm param)
        {
            SysLine sysLine = sysLineLogic.GetByConfigId(param.configId);
            SynScrapInfoParam model = new SynScrapInfoParam();
            model.CopyMatchingProperties(param);
            model.defectList = new List<defectParam>();
            model.defectList.Add(new defectParam() { defectCode = param.defectCode, defectDesc = param.defectDesc });
            model.callTime = ExtDateTime.GetTimeStamp(DateTime.Now);

            var ret = await orderLogic.Scrapped(param.configId, param.primaryKey, model);
            if (ret)
            {
                //ParamOrder order = orderLogic.Get(long.Parse(primaryKey), configId);

                return Success($"报废成功");
            }
            else
            {
                return Error($"报废失败");
            }
        }
        
        //人工干预开工
        [Route("param/order/manualStart")]
        [HttpPost, LoginChecked]
        public ActionResult ManualStart(string primaryKey, string configId)
        {
            SysLine sysLine = sysLineLogic.GetByConfigId(configId);
            //没有激活订单才可以激活，且当前订单状态为0-未开工或2- 暂停
            ParamOrder entity = orderLogic.GetSelected(configId);
            if (entity != null)
            {
                return Error("已存在开工工单，操作不允许");
            }
            ParamOrder order = orderLogic.Get(long.Parse(primaryKey), configId);
            if (order == null)
            {
                return Error("选中工单不存在");
            }
            if (order.Flag == "5")
            {
                Logger.RunningInfo($"工单:{order.TaskOrderNumber},人工干预开工");
                order.Flag = "1";
                order.StartTime = DateTime.Now;
                SelectOrderParam orderParam = new SelectOrderParam()
                {
                    taskOrderNumbers = new List<SelectOrder>() { new SelectOrder() {
                        taskOrderNumber = order.TaskOrderNumber ,
                        actionCode = ActionCode.Start, } },
                    stationCode = "M300",    //此处填充内容根据工厂确定
                    equipmentID = "FN-GZ-XTSX-03-M300-A",          //20240409更新设备编码FN-GZXNY-PACK-024 
                    productionLine = sysLine.EnCode,
                    operatorNo = OperatorProvider.Instance.Current.UserId,
                    actualStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                };

                //人工干预开工的，不发厂级mes
                //RetMessage<object> retMessage = APIMethod.Call(FNMES.WebUI.API.Url.SelectOrderUrl, orderParam, configId).ToObject<RetMessage<object>>();
                int v = orderLogic.Update(order, configId);
                if (v == 0)
                {
                    return Error("选中工单开工失败");
                }
                else
                {
                    Logger.RunningInfo($"工单:{order.TaskOrderNumber},人工干预开工成功");
                    return Success("开工成功");
                }
            }
            else
            {
                return Error("选中工单状态不为[人工干预完成]");
            }
        }

        //
        [Route("param/order/manualCompleted")]
        [HttpPost, LoginChecked]
        public ActionResult ManualCompleted(string primaryKey, string configId)
        {
            //激活才能暂停
            SysLine sysLine = sysLineLogic.GetByConfigId(configId);
            ParamOrder order = orderLogic.Get(long.Parse(primaryKey), configId);
            if (order == null)
            {
                return Error("选中工单不存在");
            }
            if (order.Flag == "1")
            {
                Logger.RunningInfo($"工单:{order.TaskOrderNumber},人工干预完成");
                order.Flag = "5";
                int v = orderLogic.Update(order, configId);
                if (v == 0)
                {
                    return Error("选中工单人工干预完成失败");
                }
                Logger.RunningInfo($"工单:{order.TaskOrderNumber},人工干预完成成功");
                return Success("人工干预完成成功");
            }
            else
            {
                return Error("选中工单状态非[生产中]");
            }
        }

        [Route("param/order/export")]
        [HttpGet]
        public ActionResult Export(string primaryKey, string configId)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                List<RecordOrderStart> entity = orderLogic.GetProductInOrder(long.Parse(primaryKey), configId);

                // 出站字段
                Dictionary<string, string> outkeyValuePairs = new Dictionary<string, string>() {
                    {"TaskOrderNumber", "工单号"},
                    {"ProductCode", "内控码"},
                    {"PackNo","Pack码" },
                    {"CreateTime","创建时间" },
                    {"PackCellGear","档位" },
                };

                // 填充数据到工作表
                List<Dictionary<string, string>> keyValues = new List<Dictionary<string, string>>();
                keyValues.Add(outkeyValuePairs);

                List<string> sheetNames = new List<string> { "ProductInOrder"};
                List<DataTable> tables = new List<DataTable>();

                int count = entity.Count;
                int start = Math.Max(0, count - 1000000);
                var entity100w = entity.GetRange(start, count - start);

                DataTable dt = entity100w.ToDataTable();

                tables.Add(dt);

                var bytes = ExcelUtils.DtExportExcel(tables, keyValues, sheetNames);


                // 创建文件流
                var stream = new MemoryStream(bytes);

                stopwatch.Stop();
                Logger.RunningInfo($"工单产品记录导出,工单数据量:{entity.Count}耗时:{stopwatch.Elapsed.TotalSeconds}秒");

                // 设置响应头，指定响应的内容类型和文件名
                Response.Headers.Add("Content-Disposition", "attachment; filename=exported-order-file.xlsx");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                Logger.ErrorInfo($"工单记录导出失败", ex);
                return Error();
            }
        }
    }
}
