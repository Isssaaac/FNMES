using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Month)]
    [SugarTable("Record_Process_{year}{month}{day}")]
    [SugarIndex("index_recordProcess_pid", nameof(RecordProcessData.ProcessUploadId), OrderByType.Asc)]    //索引
    public class RecordProcessData : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }
        
        // 主表ID
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "pid")]
        public long ProcessUploadId { get; set; }

        // 参数名称（检验项）
        [SugarColumn(ColumnName = "paramCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ParamCode { get; set; }

        // 参数名称（检验项）
        [SugarColumn(ColumnName = "paramName", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ParamName { get; set; }

        // 参数值
        [SugarColumn(ColumnName = "paramValue", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ParamValue { get; set; }

        // 检测结果 OK/NG
        [SugarColumn(ColumnName = "itemFlag", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string ItemFlag { get; set; }

        [SplitField]
        [SugarColumn(ColumnName = "createTime")]
        public DateTime CreateTime { get; set; }


        //2024.5.13 根据孚能要求，在过程数据中需要包含判断标准、判定方式、单位显示

        // 判定类型
        [SugarColumn(ColumnName = "decisionType", IsNullable = true)]
        public string DecisionType { get; set; }

        // 参数类型：定性（字符串）定量（数值型）：标准值，上限，下限。
        [SugarColumn(ColumnName = "paramType", IsNullable = true)]
        public string ParamType { get; set; }

        // 工艺参数标准值，针对定量类型的
        [SugarColumn(ColumnName = "standValue", IsNullable = true)]
        public string StandValue { get; set; }

        // 工艺参数最大值，针对定量类型的
        [SugarColumn(ColumnName = "maxValue", IsNullable = true)]
        public string MaxValue { get; set; }

        // 工艺参数最小值，针对定量类型的
        [SugarColumn(ColumnName = "minValue", IsNullable = true)]
        public string MinValue { get; set; }

        // 针对定性的设定值
        [SugarColumn(ColumnName = "setValue", IsNullable = true)]
        public string SetValue { get; set; }

        // 单位
        [SugarColumn(ColumnName = "uom", IsNullable = true)]
        public string UnitOfMeasure { get; set; }

    }
}
