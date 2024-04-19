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
    [SugarTable("Param_Plc_Recipe")]
    public class ParamPlcRecipe : CopyAble
    {
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        /// 产品编码  例：CPGL008
        /// </summary>
        [SugarColumn(ColumnName = "product")]
        public string Product { get; set; }
        /// <summary>
        /// plc序号    1、2、3、4、5   属于哪个PLC的配方
        ///</summary>
        [SugarColumn(ColumnName = "plc")]
        public string Plc { get; set; }
        /// <summary>
        /// 配方内容
        ///</summary>
         [SugarColumn(ColumnName = "content", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
         public string Content { get; set; }
        [SugarColumn(ColumnName = "createTime", IsNullable = true)]
        public DateTime? CreateTime { get; set; }

       
    }
}
