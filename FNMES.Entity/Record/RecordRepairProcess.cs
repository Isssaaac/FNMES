using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    //返修记录表
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_RepairProcess_{year}{month}{day}")]
    [SugarIndex("index_repairProcess_productCode", nameof(RecordRepairProcess.ProductCode), OrderByType.Asc)]    //索引
    [SugarIndex("index_repairProcess_stationCode", nameof(RecordRepairProcess.StationCode), OrderByType.Asc), LineTableInit]    //索引
    public class RecordRepairProcess : BaseRecord
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

        // 工艺参数编码（操作类型编码）
        [SugarColumn(ColumnName = "ParamCode", IsNullable = true)]
        public string ParamCode { get; set; }

        [SugarColumn(ColumnName = "ParamName", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ParamName { get; set; }

        // 工艺描述
        [SugarColumn(ColumnName = "ProcessDescription", IsNullable = true)]
        public string ProcessDescription { get; set; }

        [SugarColumn(ColumnName = "CreateTime", IsNullable = true)]
        public DateTime? CreateTime { get; set; }
    }
}
