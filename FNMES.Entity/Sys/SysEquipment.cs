using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Sys
{
    [SugarTable("Sys_Equipment")]
    public class SysEquipment : BaseModelEntity
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "lineId")]
        public long LineId { get; set; }

        [SugarColumn(ColumnName = "ip")]
        public string IP { get; set; }

        [SugarColumn(ColumnName = "EnCode")]
        public string EnCode { get; set; }

        [SugarColumn(ColumnName = "Name")]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "unitProcedure")]
        public string UnitProcedure { get; set; }

        [SugarColumn(ColumnName = "bigProcedure")]
        public string BigProcedure { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(LineId), nameof(SysLine.Id))]//一对一
        public SysLine Line { get; set; } //不能赋值只能是null
    }

}
