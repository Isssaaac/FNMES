using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_PartItem"), LineTableInit]
    [SugarIndex("index_ParamPartItem_pid", nameof(ParamPartItem.RecipeItemId), OrderByType.Asc)]    //索引
    public class ParamPartItem : ParamBase
    {
        /// 配方ID 
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "RecipeItemId")]
        public long RecipeItemId { get; set; }


        // 小工位
        [SugarColumn(ColumnName = "SmallStationCode", IsNullable = true)]
        public string SmallStationCode { get; set; }

        // 工步编码（可选）
        [SugarColumn(ColumnName = "StepNo", IsNullable = true)]
        public string StepNo { get; set; }

        // 工步名称（可选）
        [SugarColumn(ColumnName = "StepName", IsNullable = true)]
        public string StepName { get; set; }

        // 顺序号（可选）
        [SugarColumn(ColumnName = "OrderNo", IsNullable = true)]
        public string OrderNo { get; set; }

        // 物料编码
        [SugarColumn(ColumnName = "PartNumber", IsNullable = true)]
        public string PartNumber { get; set; }

        // 物料描述
        [SugarColumn(ColumnName = "PartDescription", IsNullable = true)]
        public string PartDescription { get; set; }

        // 物料类型：精准追溯/批次追溯 2024.5.10增加
        [SugarColumn(ColumnName = "PartType", IsNullable = true)]
        public string PartType { get; set; }

        // 物料版本
        [SugarColumn(ColumnName = "PartVersion", IsNullable = true)]
        public string PartVersion { get; set; }

        // 数量
        [SugarColumn(ColumnName = "PartQty", IsNullable = true)]
        public string PartQty { get; set; }

        // 单位
        [SugarColumn(ColumnName = "Uom", IsNullable = true)]
        public string Uom { get; set; }


        [Navigate(NavigateType.OneToMany, nameof(ParamAlternativePartItem.PartItemId)), SugarColumn(IsIgnore = true)]//一对多
        public List<ParamAlternativePartItem> AlternativePartList { get; set; }

    }
}
