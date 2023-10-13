using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [SugarColumn(ColumnName = "bigStationCode ", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string BigStationCode { get; set; }

        [SugarColumn(ColumnName = "equipmentStatus", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string EquipmentStatus { get; set; }

        [SugarColumn(ColumnName = "statusDescription", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StatusDescription { get; set; }


        [SplitField]
        [SugarColumn(ColumnName = "createTime")]
        public DateTime CreateTime { get; set; }
    }
}
