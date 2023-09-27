using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_SOP_Screw")]
    public class ParamSopScrew
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
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="SOPId"    )]
         public long? SOPId { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="step"    )]
         public int? Step { get; set; }
        /// <summary>
        /// 工具号 
        ///</summary>
         [SugarColumn(ColumnName="toolNo"    )]
         public int? ToolNo { get; set; }
        /// <summary>
        /// 程序号 
        ///</summary>
         [SugarColumn(ColumnName="psetNo"    )]
         public int? PsetNo { get; set; }
        /// <summary>
        /// 套筒号 
        ///</summary>
         [SugarColumn(ColumnName="selectorNo"    )]
         public int? SelectorNo { get; set; }
        /// <summary>
        /// 位置 
        ///</summary>
         [SugarColumn(ColumnName="displayPos"    )]
         public string DisplayPos { get; set; }
        /// <summary>
        /// 显示大小 
        ///</summary>
         [SugarColumn(ColumnName="displaySize"    )]
         public string DisplaySize { get; set; }
        /// <summary>
        /// 显示颜色 
        ///</summary>
         [SugarColumn(ColumnName="displayColor"    )]
         public string DisplayColor { get; set; }
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
