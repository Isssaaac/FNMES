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
    [SugarTable("Param_RecipeItem")]
    [SugarIndex("index_ParamRecipeItem_pid", nameof(ParamRecipeItem.RecipeId), OrderByType.Asc)]    //索引
    public class ParamRecipeItem:CopyAble
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// 配方ID 
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "recipeId")]
        public long RecipeId { get; set; }


        // 工序编码
        [SugarColumn(ColumnName = "stationCode",IsNullable = true)]
        public string StationCode { get; set; }

        // 工序名称
        [SugarColumn( ColumnName = "stationName", IsNullable = true)]
        public string StationName { get; set; }


        // 工段：前段，中段，后段
        [SugarColumn(IsIgnore = true, ColumnName = "section", IsNullable = true)]
        public string Section { get; set; }

        // 下一站工序编码
        [SugarColumn(ColumnName = "nextStationCode", IsNullable = true)]
        public string NextStationCode { get; set; }

        // 是否首道工序
        [SugarColumn(ColumnName = "isFirstStation", IsNullable = true)]
        public string IsFirstStation { get; set; }

        // 节拍，后段需要，单位秒
        [SugarColumn(ColumnName = "productionBeat", IsNullable = true)]
        public string ProductionBeat { get; set; }

        // 过站限制：进站校验、出站校验、进出站都校验、都不校验
        [SugarColumn(ColumnName = "passStationRestriction", IsNullable = true)]
        public string PassStationRestriction { get; set; }

       

        // 工艺参数，可以到小工位下的工步
        // 要使用includes，就需要增加这个导航
        [Navigate(NavigateType.OneToMany, nameof(ParamItem.RecipeItemId))]//一对多
        public List<ParamItem> ParamList { get; set; }

        // ESOP到小工位
        [Navigate(NavigateType.OneToMany, nameof(ParamEsopItem.RecipeItemId))]//一对多
        public List<ParamEsopItem> EsopList { get; set; }

        // bom物料可以到小工位下的工步
        [Navigate(NavigateType.OneToMany, nameof(ParamPartItem.RecipeItemId))]//一对多
        public List<ParamPartItem> PartList { get; set; }

        // 工步列表，仅PACK后段(组装段)需要，PACK前段和中段不需要
        [Navigate(NavigateType.OneToMany, nameof(ParamStepItem.RecipeItemId))]//一对多
        public List<ParamStepItem> StepList { get; set; }

        
    }
}
