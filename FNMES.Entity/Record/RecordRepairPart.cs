using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_RepairPart_{year}{month}{day}")]
    [SugarIndex("index_repairPart_productCode", nameof(RecordRepairPart.ProductCode), OrderByType.Asc)]    //索引
    [SugarIndex("index_repairPart_stationCode", nameof(RecordRepairPart.StationCode), OrderByType.Asc), LineTableInit]    //索引
    public class RecordRepairPart : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long Id { get; set; }
        // 内控码
        [SugarColumn(ColumnName = "ProductCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ProductCode { get; set; }
        // 大工站
        [SugarColumn(ColumnName = "StationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StationCode { get; set; }
        [SugarColumn(ColumnName = "SmallStationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string SmallStationCode { get; set; }

        // 工步编码（可选）
        [SugarColumn(ColumnName = "StepNo", IsNullable = true)]
        public string StepNo { get; set; }

        // 工步名称（可选）
        [SugarColumn(ColumnName = "StepName", IsNullable = true)]
        public string StepName { get; set; }

        // 物料编码
        [SugarColumn(ColumnName = "PartNumber", IsNullable = true)]
        public string PartNumber { get; set; }

        // 物料描述
        [SugarColumn(ColumnName = "PartDescription", IsNullable = true)]
        public string PartDescription { get; set; }

        [SugarColumn(ColumnName = "CreateTime", IsNullable = true)]
        public DateTime? CreateTime { get; set; }
    }
}
