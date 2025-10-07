using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    //返修记录
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_RepairStep_{year}{month}{day}")]
    [SugarIndex("index_repairStep_productCode", nameof(RecordRepairStep.ProductCode), OrderByType.Asc)]    //索引
    [SugarIndex("index_repairStep_stationCode", nameof(RecordRepairStep.StationCode), OrderByType.Asc), LineTableInit]    //索引
    public class RecordRepairStep : BaseRecord
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
        //小工站
        [SugarColumn(ColumnName = "SmallStationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string SmallStationCode { get; set; }

        // 工步编号
        [SugarColumn(ColumnName = "StepNo", IsNullable = true)]
        public string StepNo { get; set; }

        // 工步名称
        [SugarColumn(ColumnName = "StepName", IsNullable = true)]
        public string StepName { get; set; }

        // 顺序号
        [SugarColumn(ColumnName = "No", IsNullable = true)]
        public string No { get; set; }

        // 工步描述
        [SugarColumn(ColumnName = "StepDesc", IsNullable = true)]
        public string StepDesc { get; set; }

        // 操作
        [SugarColumn(ColumnName = "Operation", IsNullable = true)]
        public string Operation { get; set; }


        [SugarColumn(ColumnName = "CreateTime", IsNullable = true)]
        public DateTime? CreateTime { get; set; }
    }
}
