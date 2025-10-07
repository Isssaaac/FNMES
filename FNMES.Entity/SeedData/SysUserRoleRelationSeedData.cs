using FNMES.Entity.Sys;
using System;
using System.Collections.Generic;
using FNMES.Entity.Param;

namespace FNMES.Entity
{
    public class SysUserRoleRelationSeedData : SeedDataBase<SysUserRoleRelation>, ISqlSugarEntitySeedData<SysUserRoleRelation>
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<SysUserRoleRelation> SeedData()
        {
            var record = base.ReadCsvToDictionaries();
            return record;
        }
    }
}
