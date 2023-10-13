using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Status_Factory")]
    public class FactoryStatus
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
         [SugarColumn(ColumnName="status"    )]
         public int Status { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "retry")]
        public int Retry { get; set; }
       
        [SugarColumn(IsIgnore = true)]
        public bool isOnline {
            get{ return Status == 1; }
                }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "createTime")]
         public DateTime? CreateTime { get; set; }

        //分库的数据库标识   只用来映射实体传递数据
        [SugarColumn(IsIgnore = true)]
        public string ConfigId
        {
            get; set;
        }

    }
}
