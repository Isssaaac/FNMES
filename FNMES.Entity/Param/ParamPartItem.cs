using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_PartItem")]
    [SugarIndex("index_ParamPartItem_pid", nameof(ParamPartItem.RecipeItemId), OrderByType.Asc)]    //索引
    public class ParamPartItem : CopyAble
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// 配方ID 
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "recipeItemId")]
        public long RecipeItemId { get; set; }


        // 小工位
        [SugarColumn(ColumnName = "smallStationCode",IsNullable = true)]
        public string SmallStationCode { get; set; }

        // 工步编码（可选）
        [SugarColumn(ColumnName = "stepNo", IsNullable = true)]
        public string StepNo { get; set; }

        // 工步名称（可选）
        [SugarColumn(ColumnName = "stepName", IsNullable = true)]
        public string StepName { get; set; }

        // 顺序号（可选）
        [SugarColumn(ColumnName = "No", IsNullable = true)]
        public string OrderNo { get; set; }

        // 物料编码
        [SugarColumn(ColumnName = "partNumber")]
        public string PartNumber { get; set; }

        // 物料描述
        [SugarColumn(ColumnName = "partDescription", IsNullable = true)]
        public string PartDescription { get; set; }

        // 物料版本
        [SugarColumn(ColumnName = "partVersion", IsNullable = true)]
        public string PartVersion { get; set; }

        // 数量
        [SugarColumn(ColumnName = "partQty")]
        public string PartQty { get; set; }

        // 单位
        [SugarColumn(ColumnName = "uom", IsNullable = true)]
        public string Uom { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(ParamAlternativePartItem.PartItemId))]//一对多
        public List<ParamAlternativePartItem> AlternativePartList { get; set; }





    }
}
