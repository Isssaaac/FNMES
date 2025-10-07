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
    public class ParamPartItemLogic : BaseLogic
    {
        //删了再存
        public bool import(List<RecipePart> list, string recipeId, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                List<ParamPartItem> items = new List<ParamPartItem>();
                var recipeItemList = db.Queryable<ParamRecipeItem>().Where(it => it.RecipeId == long.Parse(recipeId)).ToList();

                foreach (var e in list)
                {
                    ParamPartItem item = new ParamPartItem();
                    item.CopyMatchingProperties(e);
                    item.RecipeItemId = recipeItemList.Where(it => it.StationCode == e.StationCode).Select(it => it.Id).First();
                    item.Id = SnowFlakeSingle.Instance.NextId();
                    item.CreateTime = DateTime.Now;
                    items.Add(item);
                }

                var ret = db.Deleteable<ParamPartItem>().Where(it => recipeItemList.Select(it => it.Id).Contains(it.RecipeItemId)).ExecuteCommand();
                var ret1 = db.Insertable(items).ExecuteCommand();
                return true;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return false;
            }
        }
    }
}
