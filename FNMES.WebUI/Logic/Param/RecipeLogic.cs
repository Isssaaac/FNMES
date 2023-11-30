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
    public class RecipeLogic : BaseLogic
    {



        public List<ParamRecipeItem> GetList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, long productId)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamRecipeItem> queryable = db.Queryable<ParamRecipeItem>().Where(it => it.RecipeId == productId);
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.StationCode.Contains(keyWord) || it.StationName.Contains(keyWord));
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<ParamRecipeItem>();
            }
        }

        
        //只查询路线，不查询子参数
        public ParamRecipeItem QueryRoute(string productPartNo, string stationCode, string configId)
        {
            
            try
            {
                var db = GetInstance(configId);
                ParamRecipe paramRecipe = db.Queryable<ParamRecipe>().First(it => it.ProductPartNo == productPartNo);
                if (paramRecipe == null)
                {
                    return null;
                }
                return db.Queryable<ParamRecipeItem>().First(it => it.RecipeId == paramRecipe.Id && it.StationCode == stationCode);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }

        }
        public List<ParamItem> GetParamList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, long pid)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamItem> queryable = db.Queryable<ParamItem>().Where(it => it.RecipeItemId == pid);
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.StepName.Contains(keyWord) || it.ParamName.Contains(keyWord)).OrderBy(it => it.StepNo);
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<ParamItem>();
            }
        }
        public List<ParamEsopItem> GetEsopList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, long recipeItemId)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamEsopItem> queryable = db.Queryable<ParamEsopItem>().Where(it => it.RecipeItemId == recipeItemId);
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.FilePath.Contains(keyWord) || it.No.Contains(keyWord)).OrderBy(it => it.No);
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<ParamEsopItem>();
            }
        }

        public List<ParamPartItem> GetPartList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, long recipeItemId)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamPartItem> queryable = db.Queryable<ParamPartItem>().Where(it => it.RecipeItemId == recipeItemId);
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.PartNumber.Contains(keyWord) || it.PartDescription.Contains(keyWord)).OrderBy(it => it.StepNo);
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<ParamPartItem>();
            }
        }

        public List<ParamAlternativePartItem> GetAPartList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, long pid)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamAlternativePartItem> queryable = db.Queryable<ParamAlternativePartItem>().Where(it => it.PartItemId == pid);
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.PartNumber.Contains(keyWord) || it.PartDescription.Contains(keyWord));
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<ParamAlternativePartItem>();
            }
        }



        public List<ParamStepItem> GetStepList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, long recipeItemId)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamStepItem> queryable = db.Queryable<ParamStepItem>().Where(it => it.RecipeItemId == recipeItemId);
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.StepName.Contains(keyWord) || it.StepNo.Contains(keyWord)).OrderBy(it => it.StepNo);
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<ParamStepItem>();
            }
        }
        //查询全部子参数      
        public ParamRecipeItem Query(string productPartNo, string stationCode, string smallStationCode, string configId)
        {
            
            try
            {
                var db = GetInstance(configId);
                ParamRecipe paramRecipe = db.Queryable<ParamRecipe>().First(it => it.ProductPartNo == productPartNo);
                if (paramRecipe == null)
                {
                    return null;
                }
                return db.Queryable<ParamRecipeItem>()
                   .Includes(it => it.ParamList.Where( p => p.SmallStationCode == smallStationCode).ToList())
                   .Includes(it => it.EsopList.Where(p => p.SmallStationCode == smallStationCode).ToList())
                   .Includes(it => it.StepList.Where(p => p.SmallStationCode == smallStationCode).ToList())
                   .Includes(it => it.PartList.Where(p => p.SmallStationCode == smallStationCode).ToList(), p => p.AlternativePartList)
                   .First(it => it.RecipeId == paramRecipe.Id && it.StationCode == stationCode);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }

        }
    }
}
