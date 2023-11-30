using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_UnitProcedure")]
    public class ParamUnitProcedure:BaseSimpleModelEntity
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
        ///
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="pid"    )]
         public long Pid { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "encode", IsNullable = true)]
         public string Encode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "name", IsNullable = true)]
         public string Name { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "description", IsNullable = true)]
         public string Description { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "isParent", IsNullable = true)]
         public string IsParent { get; set; }
        /// <summary>
        ///  
        ///</summary>
        
    }
}
