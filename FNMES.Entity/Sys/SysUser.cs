using FNMES.Utility.Extension;
using FNMES.Utility.Security;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using FNMES.Utility.Core;

namespace FNMES.Entity.Sys
{
    [SugarTable("Sys_User")]
    public partial class SysUser : BaseModelEntity
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public string Id { get; set; }

        [SugarColumn(ColumnName = "Account")]
        public string Account { get; set; }

        [SugarColumn(ColumnName = "RealName")]
        public string RealName { get; set; }

        [SugarColumn(ColumnName = "NickName")]
        public string NickName { get; set; }

        [SugarColumn(ColumnName = "Avatar")]
        public string Avatar { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string AvatarBase64
        {
            get
            {
                if (Avatar.IsNullOrEmpty())
                    return "";
                string filePath = MyEnvironment.RootPath("wwwroot" + Avatar);
                if (!File.Exists(filePath))
                {
                    return "";
                }
                return Base64Helper.FileToBase64(filePath);
            }
        }


        [SugarColumn(ColumnName = "Gender")]
        public string Gender { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string GenderStr
        {
            get
            {
                return Gender == "1" ? "男" : "女";
            }
        }

        [SugarColumn(ColumnName = "Birthday")]
        public DateTime? Birthday { get; set; }

        [SugarColumn(ColumnName = "MobilePhone")]
        public string MobilePhone { get; set; }


        [SugarColumn(ColumnName = "Email")]
        public string Email { get; set; }

        [SugarColumn(ColumnName = "Signature")]
        public string Signature { get; set; }


        [SugarColumn(ColumnName = "Address")]
        public string Address { get; set; }


        [SugarColumn(ColumnName = "CompanyId")]
        public string CompanyId { get; set; }


        [SugarColumn(ColumnName = "SortCode")]
        public int? SortCode { get; set; }

        [SugarColumn(ColumnName = "DepartmentId")]
        public string DepartmentId { get; set; }


        [Navigate(NavigateType.OneToOne, nameof(DepartmentId), nameof(SysOrganize.Id))]
        public SysOrganize Organize { get; set; }


        [SugarColumn(IsIgnore = true)]
        public string DeptName
        {
            get
            {
                if (Organize == null)
                    return "";
                return Organize.FullName;
            }
        }

        [SugarColumn(IsIgnore = true)]
        public string StrBirthday { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<string> RoleId { set; get; }


        [SugarColumn(IsIgnore = true)]
        public string password { set; get; }
        [SugarColumn(IsIgnore = true)]
        public string roleIds { set; get; }

    }
}
