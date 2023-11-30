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

namespace FNMES.WebUI.Logic.Param
{
    public class ParamOrderLogic : BaseLogic
    {

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
                        PackQty = model.planQty.ToInt(),
                        Uom = model.uom,
                        PlanStartTime = model.planStartTime,
                        PlanEndTime = model.planEndTime,
                        ReceiveTime = DateTime.Now,
                        Flag = "0",
                        FinishFlag = "0",
                        OperatorNo = ""
                    });
                }
                Db.BeginTran();
                db.Deleteable<ParamOrder>().Where(it => it.Flag == "0").ExecuteCommand();
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
                ParamOrder order = db.Queryable<ParamOrder>().Where(it => it.Id == primaryKey).First();
                if (order != null)
                {
                    int start = db.Queryable<RecordOrderStart>().Where(it => it.TaskOrderNumber == order.TaskOrderNumber).
                        SplitTable(tabs => tabs.Take(2)).Select(s => SqlFunc.AggregateDistinctCount(s.ProductCode)).First();
                    int pack = db.Queryable<RecordOrderPack>().Where(it => it.TaskOrderNumber == order.TaskOrderNumber).
                        SplitTable(tabs => tabs.Take(2)).Select(s => SqlFunc.AggregateDistinctCount(s.ProductCode)).First();
                    order.StartQty = start;
                    order.PackQty = pack;
                }
                return order;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }

        }

        public ParamOrder Get(long primaryKey, string configId)
        {
            try
            {
                var db = GetInstance(configId);
               // db.CodeFirst.InitTables(typeof(ParamAlternativePartItem));
                ParamOrder order = db.Queryable<ParamOrder>().Where(it => it.Id == primaryKey).First();
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
                var db = GetInstance(configId);
                ParamOrder order = db.Queryable<ParamOrder>().Where(it => it.Flag == "1").First();
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
                ISugarQueryable<ParamOrder> queryable = db.Queryable<ParamOrder>();

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

                List<ParamOrder> paramProducts = db.Queryable<ParamOrder>().ToList();
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

                List<ParamOrder> paramProducts = db.Queryable<ParamOrder>().Where(it => it.Flag =="0").ToList();
                return paramProducts;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }
    }
       
}
