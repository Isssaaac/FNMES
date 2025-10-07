using FNMES.Entity.Sys;
using System;
using System.Collections.Generic;
using FNMES.Entity.Param;

namespace FNMES.Entity
{
    public class SysRoleSeedData : SeedDataBase<SysRole>, ISqlSugarEntitySeedData<SysRole>
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<SysRole> SeedData()
        {
            var record = base.ReadCsvToDictionaries();
            return record;
        }
    }
}
