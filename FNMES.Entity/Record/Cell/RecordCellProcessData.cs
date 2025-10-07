using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Month)]
    [SugarTable("Record_CellProcess_{year}{month}{day}")]
    [SugarIndex("index_cell_recordProcess_pid", nameof(RecordCellProcessData.ProcessUploadId), OrderByType.Asc), LineTableInit]    //索引
    public class RecordCellProcessData:RecordBase
    {
        // 主表ID
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "ProcessUploadId")]
        public long ProcessUploadId { get; set; }

        // 参数名称（检验项）
        [SugarColumn(ColumnName = "ParamCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ParamCode { get; set; }

        // 参数名称（检验项）
        [SugarColumn(ColumnName = "ParamName", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ParamName { get; set; }

        // 参数值
        [SugarColumn(ColumnName = "ParamValue", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ParamValue { get; set; }

        // 检测结果 OK/NG
        [SugarColumn(ColumnName = "ItemFlag", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string ItemFlag { get; set; }

        // 判定类型
        [SugarColumn(ColumnName = "DecisionType", IsNullable = true)]
        public string DecisionType { get; set; }

        // 参数类型：定性（字符串）定量（数值型）：标准值，上限，下限。
        [SugarColumn(ColumnName = "ParamType", IsNullable = true)]
        public string ParamType { get; set; }

        // 工艺参数标准值，针对定量类型的
        [SugarColumn(ColumnName = "StandValue", IsNullable = true)]
        public string StandValue { get; set; }

        // 工艺参数最大值，针对定量类型的
        [SugarColumn(ColumnName = "MaxValue", IsNullable = true)]
        public string MaxValue { get; set; }

        // 工艺参数最小值，针对定量类型的
        [SugarColumn(ColumnName = "MinValue", IsNullable = true)]
        public string MinValue { get; set; }

        // 针对定性的设定值
        [SugarColumn(ColumnName = "SetValue", IsNullable = true)]
        public string SetValue { get; set; }

        // 单位
        [SugarColumn(ColumnName = "UnitOfMeasure", IsNullable = true)]
        public string UnitOfMeasure { get; set; }
    }
}
