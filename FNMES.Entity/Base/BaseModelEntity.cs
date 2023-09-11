using FNMES.Entity.Sys;
using SqlSugar;
using System;

namespace FNMES.Entity
{
    public class BaseModelEntity
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        [SugarColumn(ColumnName = "IsEnabled", ColumnDescription = "是否启用", IsNullable = true, Length = 1)]
        public virtual string EnableFlag { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(ColumnName = "DeleteMark", ColumnDescription = "是否删除", IsNullable = true, Length = 1)]
        public virtual string DeleteFlag { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(ColumnName = "CreateUser", ColumnDescription = "创建人", IsNullable = true, Length = 255)]
        public virtual string CreateUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "CreateTime", ColumnDescription = "创建时间", IsNullable = true, Length = 3)]
        public virtual DateTime? CreateTime { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        [SugarColumn(ColumnName = "ModifyUser", ColumnDescription = "更新人", IsNullable = true, Length = 255)]
        public virtual string ModifyUserId { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnName = "ModifyTime", ColumnDescription = "更新时间", IsNullable = true, Length = 3)]
        public virtual DateTime? ModifyTime { get; set; }

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
                return CreateUser == null ? "" : CreateUser.RealName;
            }
        }
        [SugarColumn(IsIgnore = true)]
        public string ModifyUserName
        {
            get
            {
                return ModifyUser == null ? "" : ModifyUser.RealName;
            }
        }

        [SugarColumn(IsIgnore = true)]
        public bool IsDeleted
        {
            get
            {
                return DeleteFlag == "Y" ? true : false;
            }
        }

        [SugarColumn(IsIgnore = true)]
        public bool IsEnabled
        {
            get
            {
                return EnableFlag == "Y" ? true : false;
            }
            set
            {
                EnableFlag = value ? "Y" : "N";
            }
        }
    }
}
