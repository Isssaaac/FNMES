using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Month)]
    [SugarTable("Record_Part_{year}{month}{day}")]
    [SugarIndex("index_part_pid", nameof(RecordPartData.PartUploadId), OrderByType.Asc)]    //索引
    [SugarIndex("index_part_partBarcode", nameof(RecordPartData.PartBarcode), OrderByType.Asc)]    //索引
    public class RecordPartData : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }
        // 主表ID
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "pid")]
        public long PartUploadId { get; set; }
        // 物料号 
        [SugarColumn(ColumnName = "partNumber", ColumnDataType = "varchar(20)", IsNullable = true)]
        public string PartNumber { get; set; }
        // 物料描述
        [SugarColumn(ColumnName = "partDescription", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string PartDescription { get; set; }

        // 物料条码
        [SugarColumn(ColumnName = "partBarcode", ColumnDataType = "varchar(256)", IsNullable = true)]
        public string PartBarcode { get; set; }

        // 类型
        [SugarColumn(ColumnName = "traceType", ColumnDataType = "varchar(30)", IsNullable = true)]
        public string TraceType { get; set; }
        // 用量
        [SugarColumn(ColumnName = "usageQty", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string UsageQty { get; set; }
        // 单位
        [SugarColumn(ColumnName = "uom", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string Uom { get; set; }

        [SplitField]
        [SugarColumn(ColumnName = "createTime")]
        public DateTime CreateTime { get; set; }

       
        



    }
}
