using FNMES.Entity.Sys;
using System;
using System.Collections.Generic;
using FNMES.Entity.Param;

namespace FNMES.Entity
{
    public class ParamUnitProcedureSeedData : SeedDataBase<ParamUnitProcedure>, ISqlSugarEntitySeedData<ParamUnitProcedure>
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<ParamUnitProcedure> SeedData()
        {
            var record = base.ReadCsvToDictionaries();
            return record;
        }
    }
}
