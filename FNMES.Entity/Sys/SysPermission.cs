using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Sys
{

    [SugarTable("Sys_Permission")]
    public partial class SysPermission : BaseModelEntity
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public string Id { get; set; }


        [SugarColumn(ColumnName = "ParentId")]
        public string ParentId { get; set; }

        [SugarColumn(ColumnName = "Layer")]
        public int? Layer { get; set; }

        [SugarColumn(ColumnName = "EnCode")]
        public string EnCode { get; set; }

        [SugarColumn(ColumnName = "Name")]
        public string Name { get; set; }


        [SugarColumn(ColumnName = "JsEvent")]
        public string JsEvent { get; set; }

        [SugarColumn(ColumnName = "Icon")]
        public string Icon { get; set; }

        [SugarColumn(ColumnName = "SymbolIndex")]
        public int SymbolIndex { get; set; }


        [SugarColumn(ColumnName = "Url")]
        public string Url { get; set; }


        [SugarColumn(ColumnName = "Remark")]
        public string Remark { get; set; }

        [SugarColumn(ColumnName = "Type")]
        public int? Type { get; set; }

        [SugarColumn(ColumnName = "SortCode")]
        public int? SortCode { get; set; }

        [SugarColumn(ColumnName = "IsPublic")]
        public string IsPublic { get; set; }

        [SugarColumn(ColumnName = "IsEdit")]
        public string IsEdit { get; set; }

    }
}
