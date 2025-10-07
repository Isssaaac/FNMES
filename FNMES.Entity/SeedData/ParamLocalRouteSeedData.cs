using FNMES.Entity.Sys;
using System;
using System.Collections.Generic;
using FNMES.Entity.Param;

namespace FNMES.Entity
{
    public class ParamLocalRouteSeedData : SeedDataBase<ParamLocalRoute>, ISqlSugarEntitySeedData<ParamLocalRoute>
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<ParamLocalRoute> SeedData()
        {
            var record = base.ReadCsvToDictionaries();
            return record;
        }
    }
}
