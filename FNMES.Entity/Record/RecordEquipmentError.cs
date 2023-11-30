using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Month)]
    [SugarTable("Record_EquipmentError_{year}{month}{day}")]
    public class RecordEquipmentError : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }
       
        [SugarColumn(ColumnName = "equipmentID", ColumnDataType = "varchar(100)")]
        public string EquipmentID { get; set; }

        [SugarColumn(ColumnName = "stationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StationCode { get; set; }
        [SugarColumn(ColumnName = "smallStationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string SmallStationCode { get; set; }

        [SugarColumn(ColumnName = "alarmStatus", ColumnDataType = "char(10)", IsNullable = true)]
        public string AlarmStatus { get; set; }

        [SugarColumn(ColumnName = "alarmCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string AlarmCode { get; set; }

        [SugarColumn(ColumnName = "alarmDesc", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string AlarmDesc { get; set; }


        [SplitField]
        [SugarColumn(ColumnName = "createTime")]
        public DateTime CreateTime { get; set; }
    }
}
