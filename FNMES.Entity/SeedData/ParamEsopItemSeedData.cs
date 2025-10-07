using FNMES.Entity.Sys;
using System;
using System.Collections.Generic;
using FNMES.Entity.Param;

namespace FNMES.Entity
{
    public class ParamEsopItemSeedData : SeedDataBase<ParamEsopItem>, ISqlSugarEntitySeedData<ParamEsopItem>
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<ParamEsopItem> SeedData()
        {
            var record = base.ReadCsvToDictionaries();
            return record;
        }
    }
}
