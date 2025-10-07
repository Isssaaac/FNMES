using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Sys
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Sys_Log"), SystemTableInit]
    public class SysLog
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
         [SugarColumn(ColumnName= "Type", IsNullable = true)]
         public string Type { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "ThreadId", IsNullable = true)]
         public int? ThreadId { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "Message", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
         public string Message { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "CreateTime", IsNullable = true)]
         public DateTime? CreateTime { get; set; }
    }
}
