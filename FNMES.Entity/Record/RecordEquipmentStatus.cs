using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Month)]
    [SugarTable("Record_EquipmentStatus_{year}{month}{day}")]
    public class RecordEquipmentStatus : BaseRecord
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

        [SugarColumn(ColumnName = "equipmentStatus", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string EquipmentStatus { get; set; }
        [SugarColumn(ColumnName = "statusDescription", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StatusDescription { get; set; }

        [SugarColumn(ColumnName = "stopCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StopCode { get; set; }
        [SugarColumn(ColumnName = "stopDescription", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StopDescription { get; set; }




        [SugarColumn(ColumnName = "stopFlag", ColumnDataType = "varchar(1)", IsNullable = true)]
        public string StopFlag { get; set; }
        [SugarColumn(IsIgnore = true)]
        public bool HasRecordStop { get {
                return StopFlag == "1";
            } }



        [SugarColumn(ColumnName = "callTime", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string CallTime { get; set; }

        [SplitField]
        [SugarColumn(ColumnName = "createTime")]
        public DateTime CreateTime { get; set; }
    }
}
