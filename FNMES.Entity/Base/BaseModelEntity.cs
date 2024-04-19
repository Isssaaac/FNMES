using FNMES.Entity.Sys;
using SqlSugar;
using System;

namespace FNMES.Entity
{
    public class BaseModelEntity
    {
        [SugarColumn(ColumnName = "enableFlag")]
        public string EnableFlag { get; set; }

        //启用
        [SugarColumn(IsIgnore = true)]
        public bool IsEnabled
        {
            get
            {
                return EnableFlag == "1" ? true : false;
            }
            set
            {
                EnableFlag = value ? "1" : "0";
            }
        }
        /// <summary>
        /// 排序码 
        ///</summary>
        [SugarColumn(ColumnName = "sortCode")]
        public int? SortCode { get; set; }
        /// <summary>
        /// 备注 
        ///</summary>
        [SugarColumn(ColumnName = "description")]
        public string Description { get; set; }
        /// <summary>
        /// 创建人 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "createUser")]
        public long CreateUserId { get; set; }
        /// <summary>
        /// 创建时间 
        ///</summary>
        [SugarColumn(ColumnName = "createTime")]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 修改人 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "modifyUser")]
        public long ModifyUserId { get; set; }
        /// <summary>
        /// 修改时间 
        ///</summary>
        [SugarColumn(ColumnName = "modifyTime")]
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(CreateUserId), nameof(SysUser.Id))]//一对一 SchoolId是StudentA类里面的
        public SysUser CreateUser { get; set; } //不能赋值只能是null
        /// <summary>
        /// 更新人
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(ModifyUserId), nameof(SysUser.Id))]//一对一 SchoolId是StudentA类里面的
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

       

       
    }
}
