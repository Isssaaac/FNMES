using FNMES.Entity.Sys;
using System;
using System.Collections.Generic;
using FNMES.Entity.Param;

namespace FNMES.Entity
{
    public class ParamOrderSeedData : SeedDataBase<ParamOrder>, ISqlSugarEntitySeedData<ParamOrder>
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<ParamOrder> SeedData()
        {
            var record = base.ReadCsvToDictionaries();
            return record;
        }
    }
}
