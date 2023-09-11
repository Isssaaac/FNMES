using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Sys
{

    [SugarTable("Sys_UserRoleRelation")]
    public partial class SysUserRoleRelation : BaseModelEntity
    {

        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public string Id { get; set; }

        [SugarColumn(ColumnName = "UserId")]
        public string UserId { get; set; }


        [SugarColumn(ColumnName = "RoleId")]
        public string RoleId { get; set; } 

    }
}
