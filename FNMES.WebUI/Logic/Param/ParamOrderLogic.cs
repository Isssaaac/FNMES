using SqlSugar;
using System;
using System.Collections.Generic;
using FNMES.Utility.Core;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;

namespace FNMES.WebUI.Logic.Param
{
    public class ParamOrderLogic : BaseLogic
    {
        /// <summary>
        /// 根据账号得到用户信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public int Insert(ParamOrder model, string configId,long account )
        {
            try
            {
                
                using var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                return db.Insertable<ParamOrder>(model).ExecuteCommand();
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message.ToString());
                return 0;
            }
        }



        /// <summary>
        /// 根据主键得到用户信息
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public ParamOrder Get(long primaryKey, string configId)
        {
            try
            {
                using var db = GetInstance(configId);
                ParamOrder order = db.Queryable<ParamOrder>().Where(it => it.Id == primaryKey).First();
                using var sysdb = GetInstance();
                return order;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message.ToString());
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
                using var db = GetInstance(configId);
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
                    queryable = queryable.Where(it => it.Flag=="1"|| it.Flag == "2"||it.Flag == "3");
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message.ToString());
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
                using var db = GetInstance(configId);
                return db.Deleteable<ParamOrder>().Where(it => primaryKey == it.Id).ExecuteCommand();
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message.ToString());
                return 0;
            }
        }

        /// <summary>
        /// 更新用户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(ParamOrder model,string configId, long userId)
        {
            try
            {
                using var db = GetInstance(configId);

                return db.Updateable<ParamProduct>(model).IgnoreColumns(it => new
                {
                    it.CreateUserId,
                    it.CreateTime
                }).ExecuteCommand();
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message.ToString());
                return 0;
            }
        }

        public List<ParamOrder> GetList(string configId)
        {
            try
            {
                using var db = GetInstance(configId);

                List<ParamOrder> paramProducts = db.Queryable<ParamOrder>().ToList();
                return paramProducts;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message.ToString());
                return null;
            }
        }
    }
}
