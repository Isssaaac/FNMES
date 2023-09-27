using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Sys
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Sys_UserRoleRelation")]
    public class SysUserRoleRelation
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
        [SugarColumn(ColumnName="userId"    )]
         public long UserId { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="roleId"    )]
         public long RoleId { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="createUser"    )]
         public long CreateUser { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="createTime"    )]
         public DateTime? CreateTime { get; set; }
    }
}
