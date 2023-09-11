using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Sys
{
    [SugarTable("Sys_Organize")]
    public partial class SysOrganize : BaseModelEntity
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public string Id { get; set; }


        [SugarColumn(ColumnName = "ParentId")]
        public string ParentId { get; set; }

        [SugarColumn(ColumnName = "Layer")]
        public int? Layer { get; set; }

        [SugarColumn(ColumnName = "EnCode")]
        public string EnCode { get; set; }

        [SugarColumn(ColumnName = "FullName")]
        public string FullName { get; set; }

        [SugarColumn(ColumnName = "Type")]
        public int? Type { get; set; }

        [SugarColumn(ColumnName = "ManagerId")]
        public string ManagerId { get; set; }

        [SugarColumn(ColumnName = "TelePhone")]
        public string TelePhone { get; set; }

        [SugarColumn(ColumnName = "WeChat")]
        public string WeChat { get; set; }

        [SugarColumn(ColumnName = "Fax")]
        public string Fax { get; set; }

        [SugarColumn(ColumnName = "Email")]
        public string Email { get; set; }

        [SugarColumn(ColumnName = "Address")]
        public string Address { get; set; }

        [SugarColumn(ColumnName = "SortCode")]
        public int? SortCode { get; set; }


        [SugarColumn(ColumnName = "Remark")]
        public string Remark { get; set; }

    }
}
