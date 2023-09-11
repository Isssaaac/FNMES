using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Sys
{
    [SugarTable("Sys_Role")]
    public partial class SysRole : BaseModelEntity
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public string Id { get; set; }

        [SugarColumn(ColumnName = "OrganizeId")]
        public string OrganizeId { get; set; }

        [SugarColumn(ColumnName = "EnCode")]
        public string EnCode { get; set; }

        [SugarColumn(ColumnName = "Type")]
        public int? Type { get; set; }

        [SugarColumn(ColumnName = "Name")]
        public string Name { get; set; }


        [SugarColumn(ColumnName = "AllowEdit")]
        public string AllowEdit { get; set; }


        [SugarColumn(ColumnName = "Remark")]
        public string Remark { get; set; }

        [SugarColumn(ColumnName = "SortCode")]
        public int? SortCode { get; set; }


        [Navigate(NavigateType.OneToOne, nameof(OrganizeId), nameof(SysOrganize.Id))]
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
    }
}
