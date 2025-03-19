using SqlSugar;
using System;
using System.Collections.Generic;
using FNMES.Utility.Core;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;
using FNMES.Entity.Record;
using System.Linq;
using ServiceStack;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using FNMES.Entity.DTO.ApiData;
using FNMES.Utility.ResponseModels;
using FNMES.Utility.Network;
using FNMES.WebUI.API;

namespace FNMES.WebUI.Logic.Param
{
    public class ParamOrderLogic : BaseLogic
    {
        //工单相关全部走主库
        public int Insert(ParamOrder model, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                return db.Insertable<ParamOrder>(model).ExecuteCommand();
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return 0;
            }
        }

        public int Insert(List<WorkOrder> models, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                List<ParamOrder> orders = new List<ParamOrder>();
                foreach (var model in models)
                {
                    orders.Add(new ParamOrder()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        TaskOrderNumber = model.taskOrderNumber,
                        ProductPartNo = model.productPartNo,
                        ProductDescription = model.productDescription,
                        PlanQty = model.planQty.ToInt(),
                        Uom = model.uom,
                        PlanStartTime = model.planStartTime,
                        PlanEndTime = model.planEndTime,
                        ReceiveTime = DateTime.Now,
                        Flag = "0",
                        FinishFlag = "0",
                        OperatorNo = "",
                        PackCellGear = model.packCellGear
                    });
                    Logger.RunningInfo($"插入数据库工单:{model.taskOrderNumber},档位:{model.packCellGear}");
                }
                Db.BeginTran();
                //同步工单的时候不再删除旧工单
                //db.Deleteable<ParamOrder>().Where(it => it.Flag == "0").ExecuteCommand();
                int v = db.Insertable<ParamOrder>(orders).ExecuteCommand();
                Db.CommitTran();
                return v;
            }
            catch (Exception E)
            {
                Db.RollbackTran();
                Logger.ErrorInfo(E.Message);
                return 0;
            }
        }
        public ParamOrder GetWithQty(long primaryKey, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                ParamOrder order = db.MasterQueryable<ParamOrder>().Where(it => it.Id == primaryKey).First();
                if (order != null)
                {
                    int start = db.Queryable<RecordOrderStart>().Where(it => it.TaskOrderNumber == order.TaskOrderNumber).
                        SplitTable(tabs => tabs.Take(2)).Select(s => SqlFunc.AggregateDistinctCount(s.ProductCode)).First();
                    int pack = db.Queryable<RecordOrderPack>().Where(it => it.TaskOrderNumber == order.TaskOrderNumber).
                        SplitTable(tabs => tabs.Take(2)).Select(s => SqlFunc.AggregateDistinctCount(s.ProductCode)).First();
                    int scrapped = db.Queryable<RecordOrderStart>().Where(it => it.TaskOrderNumber == order.TaskOrderNumber && it.Flag == "2").
                        SplitTable(tabs => tabs.Take(2)).Select(s => SqlFunc.AggregateDistinctCount(s.ProductCode)).First();
                    order.StartQty = start;
                    order.PackQty = pack;
                    order.ScrappedQty = scrapped;
                }
                return order;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo($"GetWithQty主键:{primaryKey},查询失败:",E);
                return null;
            }
        }

        //获取当前已上线的数据到表格显示
        public List<RecordOrderStart> GetProductInOrder(int pageIndex, int pageSize , long primaryKey, string configId ,ref int totalCount ,string keyWord)
        {
            try
            {
                var db = GetInstance(configId);
                ParamOrder order = db.MasterQueryable<ParamOrder>().Where(it => it.Id == primaryKey).First();
                
                var start = order.PlanStartTime;
                var end = start.Value.AddDays(60);

                //List<RecordOrderStart> recordOrderStartList = db.MasterQueryable<RecordOrderStart>().SplitTable(start.Value, end).Where(it => it.TaskOrderNumber == order.TaskOrderNumber).OrderBy(it => it.CreateTime).ToPageList(pageIndex , pageSize,ref totalCount);
                var queryAble = db.MasterQueryable<RecordOrderStart>().SplitTable(start.Value, end).Where(it => it.TaskOrderNumber == order.TaskOrderNumber);
                if (!keyWord.IsNullOrEmpty())
                { 
                    queryAble.Where(e=>e.ProductCode.Contains(keyWord) || e.PackNo.Contains(keyWord));
                }
                List<RecordOrderStart> recordOrderStartList = queryAble.OrderBy(it => it.CreateTime).ToPageList(pageIndex, pageSize, ref totalCount);
                foreach (var e in recordOrderStartList)
                {
                    e.PackCellGear = order.PackCellGear;
                }
                return recordOrderStartList;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo($"getProductInOrder失败,主键{primaryKey}",E);
                return null;
            }
        }

        //获取当前已上线的数据
        public List<RecordOrderStart> GetProductInOrder(long primaryKey, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                ParamOrder order = db.MasterQueryable<ParamOrder>().Where(it => it.Id == primaryKey).First();

                var start = order.PlanStartTime;
                var end = DateTime.Today;

                var queryAble = db.MasterQueryable<RecordOrderStart>().SplitTable(start.Value, end).Where(it => it.TaskOrderNumber == order.TaskOrderNumber);

                List<RecordOrderStart> recordOrderStartList = queryAble.OrderBy(it => it.CreateTime).ToList();
                foreach (var e in recordOrderStartList)
                {
                    e.PackCellGear = order.PackCellGear;
                }
                return recordOrderStartList;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo($"getProductInOrder失败,主键{primaryKey}", E);
                return null;
            }
        }

        public ParamOrder Get(long primaryKey, string configId)
        {
            try
            {
                var db = GetInstance(configId);
               // db.CodeFirst.InitTables(typeof(ParamAlternativePartItem));
                ParamOrder order = db.MasterQueryable<ParamOrder>().Where(it => it.Id == primaryKey).First();
                return order;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }

        }

        public ParamOrder GetSelected(string configId)
        {
            try
            {
                //业务逻辑，必须走主库
                var db = GetInstance(configId);
                ParamOrder order = db.MasterQueryable<ParamOrder>().Where(it => it.Flag == "1").First();
                return order;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }

        }

        /// <summary>
        /// 获得列表分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<ParamOrder> GetList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamOrder> queryable = db.MasterQueryable<ParamOrder>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.TaskOrderNumber.Contains(keyWord) || it.ProductPartNo.Contains(keyWord));
                }
                if (index == "1")//正序   
                {
                    queryable = queryable.OrderBy(it => it.Id);
                }
                else if (index == "2")//倒序   
                {
                    queryable = queryable.OrderByDescending(it => it.Id);
                }
                else if (index == "3")//只看未完成   
                {
                    queryable = queryable.Where(it => it.Flag == "1" || it.Flag == "2" || it.Flag == "3");
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }



        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        public int Delete(long primaryKey, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.Deleteable<ParamOrder>().Where(it => primaryKey == it.Id).ExecuteCommand();
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return 0;
            }
        }

        /// <summary>
        /// 更新用户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(ParamOrder model, string configId)
        {
            try
            {
                var db = GetInstance(configId);

                return db.Updateable<ParamOrder>(model).IgnoreColumns(it => it.OperatorNo
                ).ExecuteCommand();
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return 0;
            }
        }

        public List<ParamOrder> GetList(string configId)
        {
            try
            {
                var db = GetInstance(configId);

                List<ParamOrder> paramProducts = db.MasterQueryable<ParamOrder>().ToList();
                return paramProducts;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }

        public List<ParamOrder> GetNew(string configId)
        {
            try
            {
                var db = GetInstance(configId);
                //获取新的，意思是获取Flag等于0的
                List<ParamOrder> paramProducts = db.MasterQueryable<ParamOrder>().Where(it => it.Flag =="0").ToList();
                return paramProducts;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }

        //电芯报废
        public bool Scrapped(string configId, string primaryKey, SynScrapInfoParam param)
        {
            try {
                var db = GetInstance(configId);
                Db.BeginTran();
                var row = db.MasterQueryable<RecordOrderStart>().SplitTable(tab =>tab.Take(3)).Where(it => it.Id == long.Parse(primaryKey)).First();
                row.Flag = "2";
                db.Updateable<RecordOrderStart>(row).SplitTable().ExecuteCommand();
                //调厂级接口
                RetMessage<object> retMessage = APIMethod.Call(FNMES.WebUI.API.Url.SynScrapInfo, param, configId, true).ToObject<RetMessage<object>>();
                Db.CommitTran();
                Logger.RunningInfo($"设置电芯<{row.ProductCode}>报废状态为2,结果:<{retMessage.messageType}>");
                if (retMessage.messageType == "S")
                {
                    Logger.ErrorInfo($"设置电芯报废状态成功，返回{retMessage.message}");
                    return true;
                }
                else
                {
                    Logger.ErrorInfo($"设置电芯报废状态错误，返回{retMessage.message}");
                    return false;
                }
            }
            catch (Exception E) {
                Logger.ErrorInfo($"设置电芯报废状态报错", E);
                Db.RollbackTran();
                return false;
            }
        }
    }
       
}
