 using System;
using System.Collections.Generic;
using System.Linq;
using FNMES.Entity.DTO.ApiData;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_Plc_Recipe"), LineTableInit]
    public class ParamPlcRecipe : ParamBase
    {
        /// <summary>
        /// 产品编码  例：CPGL008
        /// </summary>
        [SugarColumn(ColumnName = "Product")]
        public string Product { get; set; }
        /// <summary>
        /// plc序号    1、2、3、4、5   属于哪个PLC的配方
        ///</summary>
        [SugarColumn(ColumnName = "Plc")]
        public string Plc { get; set; }
        /// <summary>
        /// 配方内容
        ///</summary>
         [SugarColumn(ColumnName = "Content", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
         public string Content { get; set; }
    }
}
