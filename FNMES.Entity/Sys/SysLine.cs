using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Sys
{
    [SugarTable("Sys_Line"), SystemTableInit]
    public class SysLine : BaseModelEntity
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "EnCode")]
        public string EnCode { get; set; }

        [SugarColumn(ColumnName = "Name")]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "ConfigId")]
        public string ConfigId { get; set; }

        

    }
}
