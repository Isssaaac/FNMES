using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season), LineTableInit]
    [SugarTable("Record_CheckMaintenance_{year}{month}{day}")]
    public class RecordCheckMaintenance : RecordBase
    {
        //大工站
        [SugarColumn(ColumnName = "StationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StationCode { get; set; }
        //小工站
        [SugarColumn(ColumnName = "SmallStationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string SmallStationCode { get; set; }
        //报警状态
        [SugarColumn(ColumnName = "OperatorNo", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string OperatorNo { get; set; }
        //报警代码
        [SugarColumn(ColumnName = "CheckType", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string CheckType { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<RecordCheckMaintenanceData> MaintenanceList { get; set; }
    }
}
