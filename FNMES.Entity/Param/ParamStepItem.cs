using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_StepItem")]
    [SugarIndex("index_ParamStepItem_pid", nameof(ParamStepItem.RecipeItemId), OrderByType.Asc)]    //索引
    public class ParamStepItem : CopyAble
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// 配方ID 
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "recipeItemId")]
        public long RecipeItemId { get; set; }


        // 小工位
        [SugarColumn(ColumnName = "smallStationCode", IsNullable = true)]
        public string SmallStationCode { get; set; }


        // 工步编号
        [SugarColumn(ColumnName = "stepNo", IsNullable = true)]
        public string StepNo { get; set; }

        // 工步名称
        [SugarColumn(ColumnName = "stepName", IsNullable = true)]
        public string StepName { get; set; }

        // 顺序号
        [SugarColumn(ColumnName = "No", IsNullable = true)]
        public string No { get; set; }

        // 工步描述
        [SugarColumn(ColumnName = "stepDesc", IsNullable = true)]
        public string StepDesc { get; set; }

        // 操作
        [SugarColumn(ColumnName = "operation", IsNullable = true)]
        public string Operation { get; set; }

        // 标识
        [SugarColumn(ColumnName = "identity", IsNullable = true)]
        public string Identity { get; set; }
    }
}
