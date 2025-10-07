using FNMES.Entity.Sys;
using System;
using System.Collections.Generic;
using FNMES.Entity.Param;

namespace FNMES.Entity
{
    public class ProcessBindSeedData : SeedDataBase<ProcessBind>, ISqlSugarEntitySeedData<ProcessBind>
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<ProcessBind> SeedData()
        {
            var record = base.ReadCsvToDictionaries();
            return record;
        }
    }
}
