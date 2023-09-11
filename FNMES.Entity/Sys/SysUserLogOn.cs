using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Sys
{
    [SugarTable("Sys_UserLogOn")]
    public partial class SysUserLogOn : BaseModelEntity
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public string Id { get; set; }

        [SugarColumn(ColumnName = "UserId")]
        public string UserId { get; set; }

        [SugarColumn(ColumnName = "Password")]
        public string Password { get; set; }

        [SugarColumn(ColumnName = "SecretKey")]
        public string SecretKey { get; set; }

        [SugarColumn(ColumnName = "PrevVisitTime")]
        public DateTime? PrevVisitTime { get; set; }

        [SugarColumn(ColumnName = "LastVisitTime")]
        public DateTime? LastVisitTime { get; set; }

        [SugarColumn(ColumnName = "ChangePwdTime")]
        public DateTime? ChangePwdTime { get; set; }

        [SugarColumn(ColumnName = "LoginCount")]
        public int LoginCount { get; set; }

        [SugarColumn(ColumnName = "AllowMultiUserOnline")]
        public string AllowMultiUserOnline { get; set; }


        [SugarColumn(ColumnName = "IsOnLine")]
        public string IsOnLine { get; set; }

        [SugarColumn(ColumnName = "Question")]
        public string Question { get; set; }

        [SugarColumn(ColumnName = "AnswerQuestion")]
        public string AnswerQuestion { get; set; }

        [SugarColumn(ColumnName = "CheckIPAddress")]
        public string CheckIPAddress { get; set; }

        [SugarColumn(ColumnName = "Language")]
        public string Language { get; set; }

        [SugarColumn(ColumnName = "Theme")]
        public string Theme { get; set; }

    }
}
