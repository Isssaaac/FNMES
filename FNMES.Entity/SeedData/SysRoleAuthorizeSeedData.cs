using FNMES.Entity.Sys;
using System;
using System.Collections.Generic;
using FNMES.Entity.Param;

namespace FNMES.Entity
{
    public class SysRoleAuthorizeSeedData : SeedDataBase<SysRoleAuthorize>, ISqlSugarEntitySeedData<SysRoleAuthorize>
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<SysRoleAuthorize> SeedData()
        {
            var record = base.ReadCsvToDictionaries();
            return record;
        }
    }
}
