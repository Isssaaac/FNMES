using FNMES.Entity.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Utility.Operator;
using FNMES.Utility.Security;
using FNMES.Utility.Extension;
using FNMES.Utility.Core;
using FNMES.Utility.Other;
using System.Drawing.Printing;
using Microsoft.VisualBasic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace FNMES.WebUI.Logic.Param
{
    public class ProductStepLogic : BaseLogic
    {


        public int Insert(ParamProductStep model, long account )
        {
            try
            {
                
                var db = GetInstance(model.ConfigId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateUserId = account;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                return db.Insertable<ParamProductStep>(model).ExecuteCommand();
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return 0;
            }
        }



        /// <summary>
        /// 根据主键得到用户信息
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public ParamProductStep Get(long primaryKey, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                ParamProductStep product = db.Queryable<ParamProductStep>().Where(it => it.Id == primaryKey).First();
                using var sysdb = GetInstance();
                product.CreateUser = sysdb.Queryable<SysUser>().Where(it => it.Id == product.CreateUserId).First();
                product.CreateUser = sysdb.Queryable<SysUser>().Where(it => it.Id == product.ModifyUserId).First();
                return product;
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
        public List<ParamProductStep> GetList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, long productId)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamProductStep> queryable = db.Queryable<ParamProductStep>().Where(it => it.ProductId == productId);
                if (!keyWord.IsNullOrEmpty())   
                {
                    queryable = queryable.Where(it => it.Desc.Contains(keyWord) || it.UnitProcedure.Contains(keyWord));
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<ParamProductStep>();
            }
        }

        public List<string> getSelectedProcedure(string configId, string productId)
        {
            try
            {
                var db = GetInstance(configId);
                List<string> procedures = db.Queryable<ParamProductStep>().Where(it => it.ProductId == long.Parse(productId)).Select(it=> it.UnitProcedure).ToList();
                return procedures;
            }
            catch (Exception E)
            {
               Logger.ErrorInfo(E.Message);
                return new List<string>();
            }

        }


        public ParamProductStep Query(string productPartNo,string stationCode, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.Queryable<ParamProductStep>().LeftJoin<ParamProduct>((s, p) => s.ProductId == p.Id)
                    .Where((s, p) => s.UnitProcedure == stationCode && p.Encode == productPartNo).Select((s, p) => s).First();
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
                Logger.RunningInfo(primaryKey.ToString()+configId);
                return db.Deleteable<ParamProductStep>().Where(it => primaryKey == it.Id).ExecuteCommand();
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
        public int Update(ParamProductStep model, long userId)
        {
            try
            {
                var db = GetInstance(model.ConfigId);
                model.ModifyUserId = userId;
                model.ModifyTime = DateTime.Now;

                return db.Updateable<ParamProductStep>(model).IgnoreColumns(it => new
                {
                    it.CreateUserId,
                    it.CreateTime
                }).ExecuteCommand();
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return 0;
            }
        }
    }
}
