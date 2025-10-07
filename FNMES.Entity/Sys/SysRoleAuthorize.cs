using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Sys
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Sys_RoleAuthorize"), SystemTableInit]
    public class SysRoleAuthorize
    {
        /// <summary>
        /// 主键 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName= "Id", IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        /// 角色ID 
        ///</summary>
         [SugarColumn(ColumnName="RoleId", IsNullable = true)]
         public long? RoleId { get; set; }
        /// <summary>
        /// 权限ID 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="PermissionId", IsNullable = true)]
         public long? PermissionId { get; set; }
        /// <summary>
        /// 创建人 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName= "CreateUser", IsNullable = true)]
         public long CreateUser { get; set; }
        /// <summary>
        /// 创建时间 
        ///</summary>
         [SugarColumn(ColumnName= "CreateTime", IsNullable = true)]
         public DateTime? CreateTime { get; set; }
    }
}
