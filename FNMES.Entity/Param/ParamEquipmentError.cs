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
    [SugarIndex("index_ParamEquipmentError_stationCode", nameof(ParamEquipmentError.StationCode), OrderByType.Asc)]    //索引
    public class ParamEquipmentError : ParamBase
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
         [SugarColumn(ColumnName = "stationCode", IsNullable = true)]
         public string StationCode { get; set; }
        [SugarColumn(ColumnName = "smallStationCode", IsNullable = true)]
        public string SmallStationCode { get; set; }
        /// <summary>
        ///</summary>
        [SugarColumn(ColumnName = "equipmentID", IsNullable = true)]
        public string EquipmentID { get; set; }

        [SugarColumn(ColumnName = "offset", IsNullable = true)]
        public int Offset { get; set; }
        [SugarColumn(ColumnName = "alarmCode", IsNullable = true)]
        public string AlarmCode { get; set; }
        [SugarColumn(ColumnName = "alarmDesc", IsNullable = true)]
        public string AlarmDesc { get; set; }
        [SugarColumn(ColumnName = "plcNo", IsNullable = true)]
        public int PlcNo { get; set; }

        //分库的数据库标识   只用来映射实体传递数据
        [SugarColumn(IsIgnore = true)]
        public string ConfigId
        {
            get; set;
        }

    }
}
