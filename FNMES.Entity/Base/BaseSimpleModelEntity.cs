using FNMES.Entity.Sys;
using SqlSugar;
using System;

namespace FNMES.Entity
{
    public class BaseSimpleModelEntity
    {
        
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "CreateUserId", IsNullable = true)]
        public long CreateUserId { get; set; }
        /// <summary>
        /// 创建时间 
        ///</summary>
        [SugarColumn(ColumnName = "CreateTime", IsNullable = true)]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 修改人 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "ModifyUserId", IsNullable = true)]
        public long ModifyUserId { get; set; }
        /// <summary>
        /// 修改时间 
        ///</summary>
        [SugarColumn(ColumnName = "ModifyTime", IsNullable = true)]
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        /// 
        //不在系统库，不能通过导航查询
        [SugarColumn(IsIgnore = true)]
        public SysUser CreateUser { get; set; } //不能赋值只能是null
        /// <summary>
        /// 更新人
        /// </summary>
        //不在系统库，不能通过导航查询
        [SugarColumn(IsIgnore = true)]
        public SysUser ModifyUser { get; set; } //不能赋值只能是null


        [SugarColumn(IsIgnore = true)]
        public string CreateUserName
        {
            get
            {
                return CreateUser == null ? "" : CreateUser.Name;
            }
        }
        [SugarColumn(IsIgnore = true)]
        public string ModifyUserName
        {
            get
            {
                return ModifyUser == null ? "" : ModifyUser.Name;
            }
        }
        //分库的数据库标识
        [SugarColumn(IsIgnore = true)]
        public string ConfigId
        {
            get; set;
        }
    }
}
