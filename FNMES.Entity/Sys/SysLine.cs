using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Sys
{
    [SugarTable("Sys_Line")]
    public partial class SysLine : BaseModelEntity
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "enCode")]
        public string EnCode { get; set; }

        [SugarColumn(ColumnName = "name")]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "configId")]
        public string ConfigId { get; set; }

        

    }
}
