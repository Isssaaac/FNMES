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

namespace FNMES.WebUI.Logic.Param
{
    public class ParamProductLogic : BaseLogic
    {
        /// <summary>
        /// 根据账号得到用户信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public SysUser GetByUserName(string account)
        {
            try
            {
                using var db = GetInstance();
                return db.Queryable<SysUser>().Where(it => it.UserNo == account)
                   //  .Includes(it => it.Organize) && it.DeleteFlag == "0"
                   .Includes(it => it.CreateUser)
                   .Includes(it => it.ModifyUser)
                 .First();
            }
            catch {
                return null;
            }
            
            
        }


        public int Insert(ParamProduct model, long account )
        {
            try
            {
                
                using var db = GetInstance(model.ConfigId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateUserId = account;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                return db.Insertable<ParamProduct>(model).ExecuteCommand();
            }
            catch (Exception)
            {

                return 0;
            }
        }



        /// <summary>
        /// 根据主键得到用户信息
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public ParamProduct Get(long primaryKey, string configId)
        {
            try
            {
                using var db = GetInstance(configId);
                ParamProduct product = db.Queryable<ParamProduct>().Where(it => it.Id == primaryKey).First();
                using var sysdb = GetInstance();
                product.CreateUser = sysdb.Queryable<SysUser>().Where(it => it.Id == product.CreateUserId).First();
                product.CreateUser = sysdb.Queryable<SysUser>().Where(it => it.Id == product.ModifyUserId).First();
                return product;
            }
            catch (Exception)
            {

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
        public List<ParamProduct> GetList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount)
        {
            try
            {
                using var db = GetInstance(configId);
                ISugarQueryable<ParamProduct> queryable = db.Queryable<ParamProduct>();
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.Encode.Contains(keyWord) || it.Name.Contains(keyWord));
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {

                throw;
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
                Logger.RunningInfo(primaryKey.ToString()+configId);
                return db.Deleteable<ParamProduct>().Where(it => primaryKey == it.Id).ExecuteCommand();
            }
            catch (Exception)
            {

                return 0;
            }
        }

        /// <summary>
        /// 更新用户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(ParamProduct model, long userId)
        {
            try
            {
                using var db = GetInstance(model.ConfigId);
                model.ModifyUserId = userId;
                model.ModifyTime = DateTime.Now;

                return db.Updateable<ParamProduct>(model).IgnoreColumns(it => new
                {
                    it.CreateUserId,
                    it.CreateTime
                }).ExecuteCommand();
            }
            catch (Exception)
            {

                return 0;
            }
        }
    }
}
