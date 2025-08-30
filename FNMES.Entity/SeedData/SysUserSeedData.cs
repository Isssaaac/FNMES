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
    [IgnoreSeedDataUpdate]
    public class SysUserSeedData : ISqlSugarEntitySeedData<SysUser>
    {
        public IEnumerable<SysUser> SeedData()
        {
            Type type = typeof(SysUser);
            string projectRoot = PathHelper.GetDataDirectoryPath();
            string csvFilePath = Path.Combine(projectRoot, type.Name + ".csv");
            List<SysUser> records;
            using (var reader = new StreamReader(csvFilePath))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<SysUserMap>();
                    records = csv.GetRecords<SysUser>().ToList();
                }
            }
            return records;
        }
        public class SysUserMap : CsvHelper.Configuration.ClassMap<SysUser>
        {
            public SysUserMap()
            {
                Map(m => m.Id).Name("id");
                Map(m => m.CardNo).Name("cardNo");
                Map(m => m.UserNo).Name("userNo");
                Map(m => m.Name).Name("name");
            }
        }
        
    }
}
