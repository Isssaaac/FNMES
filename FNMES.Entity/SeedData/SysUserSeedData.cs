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
    public class SysUserSeedData : SeedDataBase<SysUser>, ISqlSugarEntitySeedData<SysUser> 
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<SysUser> SeedData()
        {
            var record = base.ReadCsvToDictionaries();
            return record;
        }
       
    }
}
