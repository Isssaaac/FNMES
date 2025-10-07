using FNMES.Entity.Sys;
using System;
using System.Collections.Generic;
using FNMES.Entity.Param;

namespace FNMES.Entity
{
    public class SysUserLogOnSeedData : SeedDataBase<SysUserLogOn>, ISqlSugarEntitySeedData<SysUserLogOn>
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<SysUserLogOn> SeedData()
        {
            var record = base.ReadCsvToDictionaries();
            return record;
        }
    }
}
