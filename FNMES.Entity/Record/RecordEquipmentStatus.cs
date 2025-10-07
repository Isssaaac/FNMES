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
       
        [SugarColumn(ColumnName = "EquipmentID", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string EquipmentID { get; set; }

        [SugarColumn(ColumnName = "StationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StationCode { get; set; }
        [SugarColumn(ColumnName = "SmallStationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string SmallStationCode { get; set; }

        [SugarColumn(ColumnName = "EquipmentStatus", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string EquipmentStatus { get; set; }
        [SugarColumn(ColumnName = "StatusDescription", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StatusDescription { get; set; }

        [SugarColumn(ColumnName = "StopCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StopCode { get; set; }
        [SugarColumn(ColumnName = "StopDescription", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StopDescription { get; set; }




        [SugarColumn(ColumnName = "StopFlag", ColumnDataType = "varchar(1)", IsNullable = true)]
        public string StopFlag { get; set; }
        [SugarColumn(IsIgnore = true)]
        public bool HasRecordStop { get {
                return StopFlag == "1";
            } }



        [SugarColumn(ColumnName = "CallTime", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string CallTime { get; set; }

        [SplitField]
        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }
    }
}
