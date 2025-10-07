using FNMES.Entity.Sys;
using System;
using System.Collections.Generic;
using FNMES.Entity.Param;

namespace FNMES.Entity
{
    public class ParamPartItemSeedData : SeedDataBase<ParamPartItem>, ISqlSugarEntitySeedData<ParamPartItem>
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<ParamPartItem> SeedData()
        {
            var record = base.ReadCsvToDictionaries();
            return record;
        }
    }
}
