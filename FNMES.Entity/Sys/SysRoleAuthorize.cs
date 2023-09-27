using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Sys
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Sys_RoleAuthorize")]
    public class SysRoleAuthorize
    {
        /// <summary>
        /// 主键 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        /// 角色ID 
        ///</summary>
         [SugarColumn(ColumnName="roleId"    )]
         public long? RoleId { get; set; }
        /// <summary>
        /// 权限ID 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="permissionId"    )]
         public long? ModuleId { get; set; }
        /// <summary>
        /// 创建人 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="createUser"    )]
         public long CreateUser { get; set; }
        /// <summary>
        /// 创建时间 
        ///</summary>
         [SugarColumn(ColumnName="createTime"    )]
         public DateTime? CreateTime { get; set; }
    }
}
