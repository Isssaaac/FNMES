using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Month)]
    [SugarTable("Record_EquipmentStop_{year}{month}{day}")]
    public class RecordEquipmentStop : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "equipmentID", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string EquipmentID { get; set; }

        [SugarColumn(ColumnName = "stationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StationCode { get; set; }
        [SugarColumn(ColumnName = "smallStationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string SmallStationCode { get; set; }

        [SugarColumn(ColumnName = "operatorNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string OperatorNo { get; set; }

        [SugarColumn(ColumnName = "stopCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StopCode { get; set; }

        [SugarColumn(ColumnName = "stopDesc", ColumnDataType = "char(10)", IsNullable = true)]
        public string StopDesc { get; set; }

        [SugarColumn(ColumnName = "stopTime", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StopTime { get; set; }

        [SugarColumn(ColumnName = "stopDurationTime", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StopDurationTime { get; set; }


        [SplitField]
        [SugarColumn(ColumnName = "createTime", IsNullable = true)]
        public DateTime CreateTime { get; set; }
    }
}
