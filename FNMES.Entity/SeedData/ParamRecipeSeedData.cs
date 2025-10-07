using FNMES.Entity.Sys;
using System;
using System.Collections.Generic;
using FNMES.Entity.Param;

namespace FNMES.Entity
{
    public class ParamRecipeSeedData : SeedDataBase<ParamRecipe>, ISqlSugarEntitySeedData<ParamRecipe>
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<ParamRecipe> SeedData()
        {
            var record = base.ReadCsvToDictionaries();
            return record;
        }
    }
}
