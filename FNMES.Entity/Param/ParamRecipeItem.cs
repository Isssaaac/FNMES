using System;
using System.Collections.Generic;
using System.Linq;
using FNMES.Entity.DTO.ApiData;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// ParamRecipe是配方，一个配方对应多个工站ParamRecipeItem，一个工站对应多个配方参数，多个工步，多个物料参数
    ///</summary>
    [SugarTable("Param_RecipeItem"), LineTableInit]
    [SugarIndex("index_ParamRecipeItem_pid", nameof(ParamRecipeItem.RecipeId), OrderByType.Asc)]    //索引
    public class ParamRecipeItem: ParamBase
    {
        /// 配方ID 
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "RecipeId")]
        public long RecipeId { get; set; }

        // 工序编码,针对工站的
        [SugarColumn(ColumnName = "StationCode", IsNullable = true)]
        public string StationCode { get; set; }

        // 工序顺序号   从1开始   1为首工位
        [SugarColumn(ColumnName = "Step", IsNullable = true)]
        public int Step { get; set; }

        // 工序名称
        [SugarColumn( ColumnName = "StationName", IsNullable = true)]
        public string StationName { get; set; }


        // 工段：前段，中段，后段
        [SugarColumn(IsIgnore = true, ColumnName = "Section", IsNullable = true)]
        public string Section { get; set; }

        // 下一站工序编码
        [SugarColumn(ColumnName = "NextStationCode", IsNullable = true)]
        public string NextStationCode { get; set; }

        // 是否首道工序
        [SugarColumn(ColumnName = "IsFirstStation", IsNullable = true)]
        public string IsFirstStation { get; set; }

        // 节拍，后段需要，单位秒
        [SugarColumn(ColumnName = "ProductionBeat", IsNullable = true)]
        public string ProductionBeat { get; set; }

        // 过站限制：进站校验、出站校验、进出站都校验、都不校验
        [SugarColumn(ColumnName = "PassStationRestriction", IsNullable = true)]
        public string PassStationRestriction { get; set; }

        // 工艺参数，可以到小工位下的工步
        // 要使用includes，就需要增加这个导航
        [Navigate(NavigateType.OneToMany, nameof(ParamItem.RecipeItemId)), SugarColumn(IsIgnore = true)]//一对多
        public List<ParamItem> ParamList { get; set; }

        // ESOP到小工位
        [Navigate(NavigateType.OneToMany, nameof(ParamEsopItem.RecipeItemId)), SugarColumn(IsIgnore = true)]//一对多
        public List<ParamEsopItem> EsopList { get; set; }

        // bom物料可以到小工位下的工步
        [Navigate(NavigateType.OneToMany, nameof(ParamPartItem.RecipeItemId)),SugarColumn(IsIgnore = true)]//一对多
        public List<ParamPartItem> PartList { get; set; }

        // 工步列表，仅PACK后段(组装段)需要，PACK前段和中段不需要
        [Navigate(NavigateType.OneToMany, nameof(ParamStepItem.RecipeItemId)),SugarColumn(IsIgnore = true)]//一对多
        public List<ParamStepItem> StepList { get; set; }
    }
}
