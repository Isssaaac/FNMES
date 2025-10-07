using FNMES.Entity.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft;
using FNMES.Utility.Core;
using Newtonsoft.Json;

namespace FNMES.Entity
{
    public class SysEquipmentSeedData : SeedDataBase<SysEquipment>, ISqlSugarEntitySeedData<SysEquipment>
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<SysEquipment> SeedData()
        {
            var record = base.ReadCsvToDictionaries();
            return record;
        }

    }
}
