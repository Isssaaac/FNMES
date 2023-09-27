using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Sys
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Sys_Log")]
    public class SysLog
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
         [SugarColumn(ColumnName="type"    )]
         public string Type { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="threadId"    )]
         public int? ThreadId { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="message"    )]
         public string Message { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="createTime"    )]
         public DateTime? CreateTime { get; set; }
    }
}
