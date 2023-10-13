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
    public class FactoryStatusLogic : BaseLogic
    {


        public int Insert(FactoryStatus model)
        {
            try
            {
                using var db = GetInstance(model.ConfigId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateTime = DateTime.Now;
                return db.Insertable<FactoryStatus>(model).ExecuteCommand();
            }
            catch (Exception)
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取最新状态
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public FactoryStatus Get(string configId)
        {
            try
            {
                using var db = GetInstance(configId);
                FactoryStatus factoryStatus = db.Queryable<FactoryStatus>().OrderBy(it => it.Id ,OrderByType.Desc).First();
                return factoryStatus;
            }
            catch (Exception)
            {
                return null;
            }

        }
    
       
        /// <summary>
        /// 更新用户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(FactoryStatus model)
        {
            try
            {
                using var db = GetInstance(model.ConfigId);

                return db.Updateable<FactoryStatus>(model).IgnoreColumns(it => new
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
