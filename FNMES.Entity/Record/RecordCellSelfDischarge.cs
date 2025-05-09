using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_CellSelfDischarge_{year}{month}{day}")]
    [SugarIndex("index_CellSelfDischarge", nameof(cellCode), OrderByType.Asc)]
    public class RecordCellSelfDischarge
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "moduleCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string moduleCode { get; set; }

        [SugarColumn(ColumnName = "cellCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        //电芯条码
        public string cellCode { get; set; }

       
        //结果
        public DateTime createTime { get; set; }
    }
}
