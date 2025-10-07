using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_EquipmentError"), LineTableInit]
    [SugarIndex("index_ParamEquipmentError_stationCode", nameof(ParamEquipmentError.StationCode), OrderByType.Asc)]    //索引
    public class ParamEquipmentError 
    {
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName= "Id", IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "StationCode", IsNullable = true)]
         public string StationCode { get; set; }
        [SugarColumn(ColumnName = "SmallStationCode", IsNullable = true)]
        public string SmallStationCode { get; set; }
        /// <summary>
        ///</summary>
        [SugarColumn(ColumnName = "EquipmentID", IsNullable = true)]
        public string EquipmentID { get; set; }

        [SugarColumn(ColumnName = "Offset", IsNullable = true)]
        public int Offset { get; set; }
        [SugarColumn(ColumnName = "AlarmCode", IsNullable = true)]
        public string AlarmCode { get; set; }
        [SugarColumn(ColumnName = "AlarmDesc", IsNullable = true)]
        public string AlarmDesc { get; set; }
        [SugarColumn(ColumnName = "PlcNo", IsNullable = true)]
        public int PlcNo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "CreateTime", IsNullable = true)]
        public DateTime CreateTime { get; set; }

        //分库的数据库标识   只用来映射实体传递数据
        [SugarColumn(IsIgnore = true)]
        public string ConfigId
        {
            get; set;
        }

    }
}
