using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 工厂接口状态表，不需要主键外索引
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
         [SugarColumn(ColumnName= "status", IsNullable = true )]
         public int Status { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "retry" , IsNullable = true)]
        public int Retry { get; set; }
       
        [SugarColumn(IsIgnore = true)]
        public bool isOnline {
            get{ return Status == 1; }
                }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "createTime", IsNullable = true)]
         public DateTime? CreateTime { get; set; }

        //分库的数据库标识   只用来映射实体传递数据
        [SugarColumn(IsIgnore = true)]
        public string ConfigId
        {
            get; set;
        }

    }
}
