using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Sys
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Sys_Role")]
    public class SysRole : BaseModelEntity
    {
        /// <summary>
        /// 主键 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }
        /// <summary>
        /// 编号 
        ///</summary>
        [SugarColumn(ColumnName = "enCode")]
        public string EnCode { get; set; }
        /// <summary>
        /// 数字索引 
        ///</summary>
        [SugarColumn(ColumnName = "type")]
        public int? Type { get; set; }
        /// <summary>
        /// 名称 
        ///</summary>
        [SugarColumn(ColumnName = "name")]
        public string Name { get; set; }
        /// <summary>
        /// 是否可编辑 
        ///</summary>
        [SugarColumn(ColumnName = "allowEdit")]
        public string AllowEdit { get; set; }
        /// <summary>
        /// 是否启用 
        ///</summary>

    }
}