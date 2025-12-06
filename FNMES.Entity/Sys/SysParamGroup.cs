using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using FNMES.Entity.Param;

namespace FNMES.Entity.Sys
{
    [SugarTable("Sys_ParamGroup"), SystemTableInit]
    public class SysParamGroup : ParamBase
    {
        /// <summary>
        ///  编码
        ///</summary>
        [SugarColumn(ColumnName = "Encode", IsNullable = true)]
        public string Encode { get; set; }
        /// <summary>
        ///  描述
        ///</summary>
        [SugarColumn(ColumnName = "Desc", IsNullable = true)]
        public string Desc { get; set; }
    }
}
