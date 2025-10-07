using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_EquipmentStatus"), LineTableInit]
    [SugarIndex("index_ParamEquipmentStatus_stationCode", nameof(ParamEquipmentStatus.StationCode), OrderByType.Asc)]    //索引
    public class ParamEquipmentStatus
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
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "EquipmentID", IsNullable = true)]
        public string EquipmentID { get; set; }

        [SugarColumn(ColumnName = "Offset", IsNullable = true)]
        public int Offset { get; set; }
        [SugarColumn(ColumnName = "StopCodeOffset", IsNullable = true)]
        public int StopCodeOffset { get; set; }
        [SugarColumn(ColumnName = "PlcNo", IsNullable = true)]
        public int PlcNo { get; set; }

        //分库的数据库标识   只用来映射实体传递数据
        [SugarColumn(IsIgnore = true)]
        public string ConfigId
        {
            get; set;
        }

    }
}
