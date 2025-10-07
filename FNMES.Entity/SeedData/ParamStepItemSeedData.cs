using FNMES.Entity.Sys;
using System;
using System.Collections.Generic;
using FNMES.Entity.Param;

namespace FNMES.Entity
{
    public class ParamStepItemSeedData : SeedDataBase<ParamStepItem>, ISqlSugarEntitySeedData<ParamStepItem>
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<ParamStepItem> SeedData()
        {
            var record = base.ReadCsvToDictionaries();
            return record;
        }
    }
}
