using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_toolData_{year}{month}{day}")]
    [SugarIndex("index_toolData_pid", nameof(RecordToolData.ToolRemainId), OrderByType.Asc)]    //索引
    public class RecordToolData : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }
        // 主表ID
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "pid")]
        public long ToolRemainId { get; set; }
        // 参数名称（检验项）
        [SugarColumn(ColumnName = "toolNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ToolNo { get; set; }

        // 参数名
        [SugarColumn(ColumnName = "toolName", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ToolName { get; set; }

        // 次数
        [SugarColumn(ColumnName = "toolRemainValue", ColumnDataType = "char(10)", IsNullable = true)]
        public string ToolRemainValue { get; set; }

        // 单位
        [SugarColumn(ColumnName = "uom", ColumnDataType = "char(10)", IsNullable = true)]
        public string Uom { get; set; }
        [SplitField]
        [SugarColumn(ColumnName = "createTime")]
        public DateTime CreateTime { get; set; }




       
    }
}
