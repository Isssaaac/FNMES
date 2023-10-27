using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Sys
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Sys_Permission")]
    public class SysPermission:BaseModelEntity
    {
        /// <summary>
        /// 主键 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        /// 父级 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="parentId"    )]
         public long ParentId { get; set; }
        /// <summary>
        /// 层次 
        ///</summary>
         [SugarColumn(ColumnName="layer"    )]
         public int? Layer { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="enCode"    )]
         public string EnCode { get; set; }
        /// <summary>
        /// 名称 
        ///</summary>
         [SugarColumn(ColumnName="name"    )]
         public string Name { get; set; }
        /// <summary>
        /// 事件 
        ///</summary>
         [SugarColumn(ColumnName="jsEvent"    )]
         public string JsEvent { get; set; }
        /// <summary>
        /// 图标 
        ///</summary>
         [SugarColumn(ColumnName="icon"    )]
         public string Icon { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="symbolIndex"    )]
         public int? SymbolIndex { get; set; }
        /// <summary>
        /// 链接 
        ///</summary>
         [SugarColumn(ColumnName="url"    )]
         public string Url { get; set; }
        
        /// <summary>
        /// 2-主菜单，0-子菜单，1-按钮  3-主菜单  4-子菜单  5-按钮   6-作业
        ///</summary>
         [SugarColumn(ColumnName="type"    )]
         public int? Type { get; set; }
        
        /// <summary>
        /// 允许编辑 
        ///</summary>
         [SugarColumn(ColumnName="isEdit"    )]
         public string IsEdit { get; set; }

        
    }
}
