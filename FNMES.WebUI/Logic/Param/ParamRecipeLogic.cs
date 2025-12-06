using FNMES.Entity.Param;
using FNMES.WebUI.Logic.Base;
using System.Threading.Tasks;
using System;

namespace FNMES.WebUI.Logic.Param
{
    public class ParamRecipeLogic :BaseLogic
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ParamRecipe> GetParamRecipe(string productPartNo, string configId) 
        {
            try
            {
                var db = GetInstance(configId);
                return await db.Queryable<ParamRecipe>().Where(it => it.ProductPartNo == productPartNo).FirstAsync();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("获取", e);
                return null;
            }
        }
    }
}
