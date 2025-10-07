using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season), LineTableInit]
    [SugarTable("Record_EquipmentStop_{year}{month}{day}")]
    public class RecordEquipmentStop : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "EquipmentID", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string EquipmentID { get; set; }

        [SugarColumn(ColumnName = "StationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StationCode { get; set; }
        [SugarColumn(ColumnName = "SmallStationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string SmallStationCode { get; set; }

        [SugarColumn(ColumnName = "OperatorNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string OperatorNo { get; set; }

        [SugarColumn(ColumnName = "StopCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StopCode { get; set; }

        [SugarColumn(ColumnName = "StopDesc", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string StopDesc { get; set; }

        [SugarColumn(ColumnName = "StopTime", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StopTime { get; set; }

        [SugarColumn(ColumnName = "StopDurationTime", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StopDurationTime { get; set; }


        [SplitField]
        [SugarColumn(ColumnName = "CreateTime", IsNullable = true)]
        public DateTime CreateTime { get; set; }
    }
}
