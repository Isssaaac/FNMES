using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Month), LineTableInit]
    [SugarTable("Record_EquipmentError_{year}{month}{day}")]
    public class RecordEquipmentError : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long Id { get; set; }
       
        [SugarColumn(ColumnName = "EquipmentID", ColumnDataType = "varchar(100)")]
        public string EquipmentID { get; set; }

        [SugarColumn(ColumnName = "StationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StationCode { get; set; }
        [SugarColumn(ColumnName = "SmallStationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string SmallStationCode { get; set; }

        [SugarColumn(ColumnName = "AlarmStatus", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string AlarmStatus { get; set; }

        [SugarColumn(ColumnName = "AlarmCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string AlarmCode { get; set; }

        [SugarColumn(ColumnName = "AlarmDesc", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string AlarmDesc { get; set; }


        [SplitField]
        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }
    }
}
