using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_EquipmentStop"), LineTableInit]
    public class ParamEquipmentStopCode
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
         [SugarColumn(ColumnName= "StopCode", IsNullable =true)]
         public string StopCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "StopCodeDesc", IsNullable = true)]
        public string StopCodeDesc { get; set; }

        //分库的数据库标识   只用来映射实体传递数据
        [SugarColumn(IsIgnore = true)]
        public string ConfigId
        {
            get; set;
        }

    }
}
