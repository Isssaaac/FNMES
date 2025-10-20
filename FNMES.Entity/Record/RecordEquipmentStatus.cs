using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season), LineTableInit]
    [SugarTable("Record_EquipmentStatus_{year}{month}{day}")]
    public class RecordEquipmentStatus : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long Id { get; set; }
       
        //设备ID
        [SugarColumn(ColumnName = "EquipmentID", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string EquipmentID { get; set; }
        //大工站

        [SugarColumn(ColumnName = "StationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StationCode { get; set; }
        //小工站
        [SugarColumn(ColumnName = "SmallStationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string SmallStationCode { get; set; }
        //设备状态
        [SugarColumn(ColumnName = "EquipmentStatus", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string EquipmentStatus { get; set; }
        //状态描述
        [SugarColumn(ColumnName = "StatusDescription", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StatusDescription { get; set; }
        //停机代码
        [SugarColumn(ColumnName = "StopCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StopCode { get; set; }
        //停机描述
        [SugarColumn(ColumnName = "StopDescription", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StopDescription { get; set; }
        //停机标志
        [SugarColumn(ColumnName = "StopFlag", ColumnDataType = "varchar(1)", IsNullable = true)]
        public string StopFlag { get; set; }
        [SugarColumn(IsIgnore = true)]
        public bool HasRecordStop { get {
                return StopFlag == "1";
            } }
        //调用时间
        [SugarColumn(ColumnName = "CallTime", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string CallTime { get; set; }

        [SplitField]
        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }
    }
}
