using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Month)]
    [SugarTable("Record_BlockPart_{year}{month}{day}"), LineTableInit]
    [SugarIndex("index_block_part_pid", nameof(RecordCellPartData.PartUploadId), OrderByType.Asc)]    //索引
    [SugarIndex("index_block_part_partBarcode", nameof(RecordCellPartData.PartBarcode), OrderByType.Asc)]    //索引
    public class RecordBlockPartData:RecordBase
    {
        // 主表ID
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "PartUploadId")]
        public long PartUploadId { get; set; }
        // 物料号 
        [SugarColumn(ColumnName = "PartNumber", ColumnDataType = "varchar(20)", IsNullable = true)]
        public string PartNumber { get; set; }
        // 物料描述
        [SugarColumn(ColumnName = "PartDescription", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string PartDescription { get; set; }

        // 物料条码
        [SugarColumn(ColumnName = "PartBarcode", ColumnDataType = "varchar(256)", IsNullable = true)]
        public string PartBarcode { get; set; }

        // 类型
        [SugarColumn(ColumnName = "TraceType", ColumnDataType = "varchar(30)", IsNullable = true)]
        public string TraceType { get; set; }
        // 用量
        [SugarColumn(ColumnName = "UsageQty", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string UsageQty { get; set; }
        // 单位
        [SugarColumn(ColumnName = "Uom", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string Uom { get; set; }
    }
}
