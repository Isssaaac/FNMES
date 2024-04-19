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
    public class PlcRecipeLogic : BaseLogic
    {
        public int Insert(ParamPlcRecipe model, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.instance.NextId();
                model.CreateTime = DateTime.Now;
                return db.Insertable<ParamPlcRecipe>(model).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }

        public ParamPlcRecipe Get(string product, string plc, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return  db.MasterQueryable<ParamPlcRecipe>()
                    .OrderBy(it => it.Id,OrderByType.Desc)
                    .Where(it => it.Product == product && it.Plc == plc).First();
                
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }

       




    }
}
