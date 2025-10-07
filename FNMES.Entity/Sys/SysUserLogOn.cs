using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Sys
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Sys_UserLogOn"), SystemTableInit]
    public class SysUserLogOn
    {
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName= "Id", IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName= "UserId",IsNullable = true)]
         public long UserId { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "Password", IsNullable = true)]
         public string Password { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "SecretKey", IsNullable = true)]
         public string SecretKey { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "PrevVisitTime", IsNullable = true)]
         public DateTime? PrevVisitTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "LastVisitTime", IsNullable = true)]
         public DateTime? LastVisitTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "ChangePwdTime", IsNullable = true)]
         public DateTime? ChangePwdTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "LoginCount", IsNullable = true)]
         public int? LoginCount { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "AllowMultiUserOnline", IsNullable = true)]
         public string AllowMultiUserOnline { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "IsOnLine", IsNullable = true)]
         public string IsOnLine { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "Theme", IsNullable = true)]
         public string Theme { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "ModifyUserId", IsNullable = true)]
        public long ModifyUserId { get; set; }
        /// <summary>
        /// 修改时间 
        ///</summary>
        [SugarColumn(ColumnName = "ModifyTime", IsNullable = true)]
        public DateTime? ModifyTime { get; set; }
    }
}
