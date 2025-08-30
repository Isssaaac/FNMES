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
                ISugarQueryable<ParamRecipeItem> queryable = db.MasterQueryable<ParamRecipeItem>().Where(it => it.RecipeId == productId);
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
                ParamRecipe paramRecipe = db.MasterQueryable<ParamRecipe>().First(it => it.ProductPartNo == productPartNo);
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
                ISugarQueryable<ParamItem> queryable = db.MasterQueryable<ParamItem>().Where(it => it.RecipeItemId == pid);
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.StepName.Contains(keyWord) || it.ParamName.Contains(keyWord));
                }
                //return queryable.OrderBy(it => it.StepNo).ToPageList(pageIndex, pageSize, ref totalCount);
                return queryable.OrderBy("TRY_CAST(stepNo AS INT)").ToPageList(pageIndex, pageSize, ref totalCount);
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
                ISugarQueryable<ParamEsopItem> queryable = db.MasterQueryable<ParamEsopItem>().Where(it => it.RecipeItemId == recipeItemId);
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.FilePath.Contains(keyWord) || it.No.Contains(keyWord));
                }
                //return queryable.OrderBy(it => it.No).ToPageList(pageIndex, pageSize, ref totalCount);
                return queryable.OrderBy("TRY_CAST(No AS INT) ASC").ToPageList(pageIndex, pageSize, ref totalCount);
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
                ISugarQueryable<ParamPartItem> queryable = db.MasterQueryable<ParamPartItem>().Where(it => it.RecipeItemId == recipeItemId);
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.PartNumber.Contains(keyWord) || it.PartDescription.Contains(keyWord));
                }
                //return queryable.OrderBy(it => it.StepNo).ToPageList(pageIndex, pageSize, ref totalCount);
                return queryable.OrderBy("TRY_CAST(stepNo AS INT),TRY_CAST(No AS INT)").ToPageList(pageIndex, pageSize, ref totalCount);
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
                ISugarQueryable<ParamAlternativePartItem> queryable = db.MasterQueryable<ParamAlternativePartItem>().Where(it => it.PartItemId == pid);
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
                ISugarQueryable<ParamStepItem> queryable = db.MasterQueryable<ParamStepItem>().Where(it => it.RecipeItemId == recipeItemId);
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.StepName.Contains(keyWord) || it.StepNo.Contains(keyWord));
                }
                //return queryable.OrderBy(it => it.StepNo).ToPageList(pageIndex, pageSize, ref totalCount);
                return queryable.OrderBy("TRY_CAST(No AS INT) ASC").ToPageList(pageIndex, pageSize, ref totalCount);
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
                //通过productPartNo查
                ParamRecipe paramRecipe = db.MasterQueryable<ParamRecipe>().First(it => it.ProductPartNo == productPartNo);
                if (paramRecipe == null)
                {
                    return null;
                }
                return db.Queryable<ParamRecipeItem>()
                   .Includes(it => it.ParamList)
                   .Includes(it => it.EsopList)
                   .Includes(it => it.StepList)
                   .Includes(it => it.PartList, p => p.AlternativePartList)
                   .First(it => it.RecipeId == paramRecipe.Id && it.StationCode == stationCode);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }
        //导入导出用
        public List<ParamRecipeItem> QueryAllItem(string productId, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                //通过productPartNo查
                ParamRecipe paramRecipe = db.MasterQueryable<ParamRecipe>().First(it => it.Id == long.Parse(productId));
                if (paramRecipe == null)
                {
                    return null;
                }
                return db.Queryable<ParamRecipeItem>()
                   .Includes(it => it.ParamList)
                   .Includes(it => it.EsopList)
                   .Includes(it => it.StepList)
                   .Includes(it => it.PartList, p => p.AlternativePartList)
                   .Where(it => it.RecipeId == paramRecipe.Id).ToList();
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }

        //导入step
        public int ImportRecipeSteps(string productId, string configId,List<ParamStepItem> data)
        {
            try
            {
                var db = GetInstance(configId);
                //通过productPartNo查
                ParamRecipe paramRecipe = db.MasterQueryable<ParamRecipe>().First(it => it.Id == long.Parse(productId));
                db.Deleteable<ParamStepItem>().Where(it=> it.RecipeItemId == paramRecipe.Id);
                var ret = InsertTableList(data, configId);
                return ret;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return 2;
            }
        }
    }
}
