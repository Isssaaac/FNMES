
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;
using System.Collections.Generic;
using SqlSugar;
using System;
using System.Linq;
using System.Data;
using FNMES.Entity.DTO;
using FNMES.Entity.DTO.ApiParam;

namespace FNMES.WebUI.Logic.Param
{
    public class ParamItemLogic : BaseLogic
    {
        public bool import(List<RecipeProcessParam> list, string recipeId, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                List<ParamItem> items = new List<ParamItem>();
                var recipeItemList = db.Queryable<ParamRecipeItem>().Where(it => it.RecipeId == long.Parse(recipeId)).ToList();

                foreach (var e in list)
                {
                    ParamItem item = new ParamItem();
                    item.CopyMatchingProperties(e);
                    item.RecipeItemId = recipeItemList.Where(it => it.StationCode == e.StationCode).Select(it => it.Id).First();
                    item.Id = SnowFlakeSingle.Instance.NextId();
                    item.CreateTime = DateTime.Now;
                    items.Add(item);
                }

                var ret = db.Deleteable<ParamItem>().Where(it => recipeItemList.Select(it => it.Id).Contains(it.RecipeItemId)).ExecuteCommand();
                var ret1 = db.Insertable(items).ExecuteCommand();
                return true;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return false;
            }
        }

        public int GetNgCodes(string smallStationCode, List<Process> processes, string configId, out List<string> ngCodes)
        {
            try
            {
                var db = GetInstance(configId);
                var paramItems = db.Queryable<ParamItem>().Where(e => e.SmallStationCode == smallStationCode).ToList();
                ngCodes = new List<string>();
                foreach (var e in processes)
                {
                    int index = paramItems.FindIndex(it => it.ParamCode == e.paramCode);
                    if (index < 0)
                        continue;
                    if (e.itemFlag != "OK")
                        ngCodes.Add(paramItems[index].NgCode);
                }
                return 1;
            }
            catch (Exception e)
            {
                ngCodes = new List<string>();
                Logger.ErrorInfo($"{smallStationCode}本地查询出错", e);
                return 0;
            }
        }
    }
}
