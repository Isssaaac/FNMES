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
         public long? Pid { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="encode"    )]
         public string Encode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="name"    )]
         public string Name { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="description"    )]
         public string Description { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="isParent"    )]
         public string IsParent { get; set; }
        /// <summary>
        ///  
        ///</summary>
        
    }
}
