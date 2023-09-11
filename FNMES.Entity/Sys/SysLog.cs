using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Sys
{

    [SugarTable("Sys_Log")]
    public class SysLog : BaseModelEntity
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn]
        public string Type { get; set; }

        [SugarColumn]
        public int ThreadId { get; set; }

        [SugarColumn]
        public string Message { get; set; }

        [SugarColumn]
        public DateTime CreateTime { get; set; }
    }
}
