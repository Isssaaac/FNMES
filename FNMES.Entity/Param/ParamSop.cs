using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param { 
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_SOP")]
    public class ParamSop
    {
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        /// 产品工序 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="productStepId"    )]
         public long? ProductStepId { get; set; }
        /// <summary>
        /// 步骤 
        ///</summary>
         [SugarColumn(ColumnName="step"    )]
         public int? Step { get; set; }
        /// <summary>
        /// 类型  
        ///</summary>
         [SugarColumn(ColumnName="type"    )]
         public int? Type { get; set; }
        /// <summary>
        /// 备注 
        ///</summary>
         [SugarColumn(ColumnName="description"    )]
         public string Description { get; set; }
        /// <summary>
        /// 图片 
        ///</summary>
         [SugarColumn(ColumnName="image"    )]
         public string Image { get; set; }
        /// <summary>
        /// 启用 
        ///</summary>
         [SugarColumn(ColumnName="enableFlag"    )]
         public string EnableFlag { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="createUser"    )]
         public long? CreateUser { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="createTime"    )]
         public DateTime? CreateTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="modifyUser"    )]
         public long? ModifyUser { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="modifyTime"    )]
         public DateTime? ModifyTime { get; set; }
    }
}
