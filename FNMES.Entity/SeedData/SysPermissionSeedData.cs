using FNMES.Entity.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.IO;
using System.Globalization;
using FNMES.Utility;

namespace FNMES.Entity
{
       
    public class SysPermissionSeedData : SeedDataBase<SysPermission>, ISqlSugarEntitySeedData<SysPermission>
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<SysPermission> SeedData()
        {
            List<SysPermission> records = ReadCsvToDictionaries();
            return records;
        }
    }
}
