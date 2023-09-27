using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Sys
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Sys_UserLogOn")]
    public class SysUserLogOn
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
         [SugarColumn(ColumnName="password"    )]
         public string Password { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="secretKey"    )]
         public string SecretKey { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="prevVisitTime"    )]
         public DateTime? PrevVisitTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="lastVisitTime"    )]
         public DateTime? LastVisitTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="changePwdTime"    )]
         public DateTime? ChangePwdTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="loginCount"    )]
         public int? LoginCount { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="allowMultiUserOnline"    )]
         public string AllowMultiUserOnline { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="isOnLine"    )]
         public string IsOnLine { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="theme"    )]
         public string Theme { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "modifyUser")]
        public long ModifyUserId { get; set; }
        /// <summary>
        /// 修改时间 
        ///</summary>
        [SugarColumn(ColumnName = "modifyTime")]
        public DateTime? ModifyTime { get; set; }
    }
}
