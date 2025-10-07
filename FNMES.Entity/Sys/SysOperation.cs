using FNMES.Entity.Param;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Sys
{
    [SugarTable("Sys_Operation"), SystemTableInit]
    public class SysOperation :ParamBase
    {
        [SugarColumn(ColumnName = "Name",IsNullable = false)]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "Desc",IsNullable = true)]
        public string Desc { get; set; }
    }
}
