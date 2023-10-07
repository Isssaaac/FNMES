using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_Product")]
    public class ParamProduct:BaseSimpleModelEntity
    {
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        /// 编码 
        ///</summary>
         [SugarColumn(ColumnName="encode"    )]
         public string Encode { get; set; }
        /// <summary>
        /// 名称 
        ///</summary>
         [SugarColumn(ColumnName="name"    )]
         public string Name { get; set; }
        /// <summary>
        /// 备注 
        ///</summary>
         [SugarColumn(ColumnName="description"    )]
         public string Description { get; set; }
        /// <summary>
        ///  
        ///</summary>
    }
}
