using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Sys
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Sys_Role"), SystemTableInit]
    public class SysRole : BaseModelEntity
    {
        /// <summary>
        /// 主键 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long Id { get; set; }
        /// <summary>
        /// 编号 
        ///</summary>
        [SugarColumn(ColumnName = "EnCode",IsNullable =true)]
        public string EnCode { get; set; }
        /// <summary>
        /// 数字索引 
        ///</summary>
        [SugarColumn(ColumnName = "Type", IsNullable = true)]
        public int? Type { get; set; }
        /// <summary>
        /// 名称 
        ///</summary>
        [SugarColumn(ColumnName = "Name", IsNullable = true)]
        public string Name { get; set; }
        /// <summary>
        /// 是否可编辑 
        ///</summary>
        [SugarColumn(ColumnName = "AllowEdit", IsNullable = true)]
        public string AllowEdit { get; set; }
        /// <summary>
        /// 是否启用 
        ///</summary>

    }
}