using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Sys
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Sys_Permission"), SystemTableInit]
    public class SysPermission:BaseModelEntity
    {
        /// <summary>
        /// 主键 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName= "Id", IsPrimaryKey = true)]
         public long Id { get; set; }
        /// <summary>
        /// 父级 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName= "ParentId", IsNullable = true)]
         public long ParentId { get; set; }
        /// <summary>
        /// 层次 
        ///</summary>
         [SugarColumn(ColumnName= "Layer", IsNullable = true)]
         public int? Layer { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "EnCode", IsNullable = true)]
         public string EnCode { get; set; }
        /// <summary>
        /// 名称 
        ///</summary>
         [SugarColumn(ColumnName= "Name", IsNullable = true)]
         public string Name { get; set; }
        /// <summary>
        /// 事件 
        ///</summary>
         [SugarColumn(ColumnName= "JsEvent", IsNullable = true)]
         public string JsEvent { get; set; }
        /// <summary>
        /// 图标 
        ///</summary>
         [SugarColumn(ColumnName= "Icon", IsNullable = true)]
         public string Icon { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "SymbolIndex", IsNullable = true)]
         public int? SymbolIndex { get; set; }
        /// <summary>
        /// 链接 
        ///</summary>
         [SugarColumn(ColumnName= "Url", IsNullable = true)]
         public string Url { get; set; }
        
        /// <summary>
        /// 2-主菜单，0-子菜单，1-按钮  3-主菜单  4-子菜单  5-按钮   6-作业
        ///</summary>
         [SugarColumn(ColumnName= "Type", IsNullable = true)]
         public int? Type { get; set; }
        
        /// <summary>
        /// 允许编辑 
        ///</summary>
         [SugarColumn(ColumnName= "IsEdit", IsNullable = true)]
         public string IsEdit { get; set; }

        
    }
}
