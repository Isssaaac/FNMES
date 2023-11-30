using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 替代料表
    ///</summary>
    ///
    [SugarTable("Param_AlternativePartItem")]
    [SugarIndex("index_ParamAlternativePartItem_pid", nameof(ParamAlternativePartItem.PartItemId), OrderByType.Asc)]    //索引
    public class ParamAlternativePartItem : CopyAble
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// 配方ID 
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "partItemId")]
        public long PartItemId { get; set; }

        // 物料编码
        [SugarColumn(ColumnName = "partNumber", IsNullable = true)]
        public string PartNumber { get; set; }

        // 物料描述
        [SugarColumn(ColumnName = "partDescription", IsNullable = true)]
        public string PartDescription { get; set; }

        // 物料版本
        [SugarColumn(ColumnName = "partVersion", IsNullable = true)]
        public string PartVersion { get; set; }

        // 数量
        [SugarColumn(ColumnName = "partQty", IsNullable = true)]
        public string PartQty { get; set; }

        // 替代料比例（和原物料的比例）
        [SugarColumn(ColumnName = "partRate", IsNullable = true)]
        public string PartRate { get; set; }

        // 单位
        [SugarColumn(ColumnName = "uom", IsNullable = true)]
        public string Uom { get; set; }






    }
}
