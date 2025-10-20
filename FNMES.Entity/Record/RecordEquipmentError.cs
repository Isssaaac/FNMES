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
        //设备ID
        [SugarColumn(ColumnName = "EquipmentID", ColumnDataType = "varchar(100)")]
        public string EquipmentID { get; set; }
        //大工站
        [SugarColumn(ColumnName = "StationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StationCode { get; set; }
        //小工站
        [SugarColumn(ColumnName = "SmallStationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string SmallStationCode { get; set; }
        //报警状态
        [SugarColumn(ColumnName = "AlarmStatus", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string AlarmStatus { get; set; }
        //报警代码
        [SugarColumn(ColumnName = "AlarmCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string AlarmCode { get; set; }
        //报警描述
        [SugarColumn(ColumnName = "AlarmDesc", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string AlarmDesc { get; set; }
        //创建时间
        [SplitField]
        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }
    }
}
