using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_EquipmentError")]
    public class ParamEquipmentError
    {
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "bigStationCode")]
         public string BigStationCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "equipmentID")]
        public string EquipmentID { get; set; }

        [SugarColumn(ColumnName = "offset")]
        public int Offset { get; set; }
        [SugarColumn(ColumnName = "alarmCode")]
        public string AlarmCode { get; set; }
        [SugarColumn(ColumnName = "alarmDesc")]
        public string AlarmDesc { get; set; }
        [SugarColumn(ColumnName = "plcNo")]
        public int PlcNo { get; set; }

        //分库的数据库标识   只用来映射实体传递数据
        [SugarColumn(IsIgnore = true)]
        public string ConfigId
        {
            get; set;
        }

    }
}
