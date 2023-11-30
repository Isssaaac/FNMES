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
    [SugarIndex("index_process_pid", nameof(RecordProcessData.ProcessUploadId), OrderByType.Asc)]    //索引
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
        [SugarColumn(ColumnName = "paramName", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ParamName { get; set; }

        // 参数值
        [SugarColumn(ColumnName = "paramValue", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ParamValue { get; set; }

        // 检测结果 OK/NG
        [SugarColumn(ColumnName = "itemFlag", ColumnDataType = "char(10)", IsNullable = true)]
        public string ItemFlag { get; set; }
        [SplitField]
        [SugarColumn(ColumnName = "createTime")]
        public DateTime CreateTime { get; set; }




       
    }
}
