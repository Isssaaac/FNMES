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
    public class SysLineSeedData : SeedDataBase<SysLine>, ISqlSugarEntitySeedData<SysLine>
    {
        [IgnoreSeedDataUpdate]
        public IEnumerable<SysLine> SeedData()
        {
            var record = base.ReadCsvToDictionaries();
            return record;
        }
    }
}
