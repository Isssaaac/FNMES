using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Sys
{
    [SugarTable("Sys_Equipment"), SystemTableInit]
    public class SysEquipment : BaseModelEntity
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long Id { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "LineId", IsNullable = true)]
        public long LineId { get; set; }

        [SugarColumn(ColumnName = "IP", IsNullable = true)]
        public string IP { get; set; }

        [SugarColumn(ColumnName = "EnCode",IsNullable= true)]
        public string EnCode { get; set; }

        [SugarColumn(ColumnName = "Name", IsNullable = true)]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "UnitProcedure", IsNullable = true)]
        public string UnitProcedure { get; set; }

        [SugarColumn(ColumnName = "BigProcedure", IsNullable = true)]
        public string BigProcedure { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(LineId), nameof(SysLine.Id)), SugarColumn(IsIgnore = true)]//一对一
        public SysLine Line { get; set; } //不能赋值只能是null
    }

}
