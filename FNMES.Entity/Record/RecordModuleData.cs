using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season), LineTableInit]
    [SugarTable("Record_moduleData_{year}{month}{day}")]
    [SugarIndex("index_moduleData_pid", nameof(UnbindPackId), OrderByType.Asc)]    //索引
    public class RecordModuleData : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long Id { get; set; }

        // 主表ID
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "UnbindPackId")]
        public long UnbindPackId { get; set; }

        [SugarColumn(ColumnName = "PartNumber", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string PartNumber { get; set; } //物料号

        [SugarColumn(ColumnName = "PartDescription", ColumnDataType = "nvarchar(MAX)", IsNullable = true)]
        public string PartDescription { get; set; } //物料描述

        [SugarColumn(ColumnName = "PartBarcode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string PartBarcode { get; set; } //物料条码

        [SugarColumn(ColumnName = "Reason", ColumnDataType = "nvarchar(MAX)", IsNullable = true)]
        public string Reason { get; set; } //解绑原因

        [SplitField]
        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }


    }
}
