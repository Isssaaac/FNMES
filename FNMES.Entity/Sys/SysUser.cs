using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using SqlSugar;
namespace FNMES.Entity.Sys
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Sys_User")]
    public class SysUser:BaseModelEntity
    {
        /// <summary>
        /// 主键 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        /// 卡号 
        ///</summary>
         [SugarColumn(ColumnName="cardNo"    )]
         public string CardNo { get; set; }
        /// <summary>
        /// 用户名 
        ///</summary>
         [SugarColumn(ColumnName="userNo"    )]
         public string UserNo { get; set; }
       
        /// <summary>
        /// 用户名 
        ///</summary>
         [SugarColumn(ColumnName="name"    )]
         public string Name { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<String> RoleId {
            get; set;
        }
        [SugarColumn(IsIgnore = true)]
        public string password { set; get; }
        [SugarColumn(IsIgnore = true)]
        public string roleIds { get; set; }

        

    }
}
