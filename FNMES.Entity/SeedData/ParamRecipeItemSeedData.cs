using FNMES.Entity.Sys;
using System;
using System.Collections.Generic;
using FNMES.Entity.Param;

namespace FNMES.Entity
{
    public class ParamRecipeItemSeedData : SeedDataBase<ParamRecipeItem>, ISqlSugarEntitySeedData<ParamRecipeItem>
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<ParamRecipeItem> SeedData()
        {
            var record = base.ReadCsvToDictionaries();
            return record;
        }
    }
}
