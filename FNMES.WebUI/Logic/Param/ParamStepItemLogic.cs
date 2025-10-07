
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;
using System.Collections.Generic;
using SqlSugar;
using System;
using System.Linq;
using System.Data;
using FNMES.Entity.DTO;


namespace FNMES.WebUI.Logic.Param
{
    public class ParamStepItemLogic : BaseLogic
    {
        public bool import(List<RecipeStep> list, string recipeId, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                List<ParamStepItem> stepItems = new List<ParamStepItem>();
                var recipeItemList = db.Queryable<ParamRecipeItem>().Where(it => it.RecipeId == long.Parse(recipeId)).ToList();

                foreach (var e in list)
                {
                    ParamStepItem stepItem = new ParamStepItem();
                    stepItem.CopyMatchingProperties(e);
                    stepItem.RecipeItemId = recipeItemList.Where(it => it.StationCode == e.StationCode).Select(it=>it.Id).First();
                    stepItem.Id = SnowFlakeSingle.Instance.NextId();
                    stepItem.CreateTime = DateTime.Now;
                    stepItems.Add(stepItem);
                }

                var ret = db.Deleteable<ParamStepItem>().Where(it => recipeItemList.Select(it => it.Id).Contains(it.RecipeItemId)).ExecuteCommand();
                var ret1 = db.Insertable(stepItems).ExecuteCommand();
                return true;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return false;
            }
        }

        public string GetStepName(string configId, string recipeItemId, string StepNo)
        {
            try
            {
                var db = GetInstance(configId);
                var stepName = db.Queryable<ParamStepItem>().Where(it => it.RecipeItemId == long.Parse(recipeItemId) && it.StepNo==StepNo).Select(it=>it.StepName).First();
                return stepName;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return "";
            }
        }
    }
}
