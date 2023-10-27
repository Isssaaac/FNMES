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
using FNMES.Entity.Record;
using ServiceStack;

namespace FNMES.WebUI.Logic.Param
{
    public class ProcessBindLogic : BaseLogic
    {


        public int Insert(ProcessBind model,string configId)
        {
            try
            {
                var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                int res;
                List<ProcessBind> oldprocessBind = db.Queryable<ProcessBind>().Where(it => it.PalletNo == model.PalletNo || it.ProductCode == model.ProductCode).ToList(); ;
                if (oldprocessBind.Count!=0)
                {
                    try
                    {
                        Db.BeginTran();
                        db.Insertable<RecordBindHistory>(oldprocessBind).SplitTable().ExecuteCommand();
                        db.Deleteable<ProcessBind>(oldprocessBind).ExecuteCommand();
                        res = db.Insertable<ProcessBind>(model).ExecuteCommand();
                        Db.CommitTran();
                        return res;
                    }
                    catch (Exception)
                    {
                        Db.RollbackTran();
                        throw ;
                    }
                }
                else
                {
                    res = db.Insertable<ProcessBind>(model).ExecuteCommand();
                    return res;
                }
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }



      
        public ProcessBind GetByPalletNo(string palletNo, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.Queryable<ProcessBind>().Where(it => it.PalletNo == palletNo).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return null;
            }
        }
        public ProcessBind GetByProductCode(string productCode, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.Queryable<ProcessBind>().Where(it => it.ProductCode == productCode).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
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
        public List<ProcessBind> GetList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ProcessBind> queryable = db.Queryable<ProcessBind>();
                if (!keyWord.IsNullOrEmpty())   
                {
                    queryable = queryable.Where(it => it.TaskOrderNumber.Contains(keyWord) 
                    || it.ProductPartNo.Contains(keyWord)|| it.ProductCode.Contains(keyWord));
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }

        public int Delete(string productCode, string configId)
        {
            var db = GetInstance(configId);
            try
            {
                return db.Deleteable<ProcessBind>().Where(it=> it.ProductCode == productCode).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
            
        }



        /// <summary>
        /// 更新用户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(ProcessBind model, string configId)
        {
            try
            {
                var db = GetInstance(configId);

                return db.Updateable<ProcessBind>(model).IgnoreColumns(it => new
                {
                    it.CreateTime
                }).ExecuteCommand();
            }
            catch (Exception)
            {

                return 0;
            }
        }
        public int DeletePalletNo(string palletNo, string configId)
        {
            try
            {
                var db = GetInstance(configId);

                return db.Updateable<ProcessBind>().IgnoreColumns(it => new
                {
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
