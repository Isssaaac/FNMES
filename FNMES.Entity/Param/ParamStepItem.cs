using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_StepItem"), LineTableInit]
    [SugarIndex("index_ParamStepItem_pid", nameof(ParamStepItem.RecipeItemId), OrderByType.Asc)]    //索引
    public class ParamStepItem : ParamBase
    {
        /// 配方ID 
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "RecipeItemId")]
        public long RecipeItemId { get; set; }
        // 小工位
        [SugarColumn(ColumnName = "SmallStationCode", IsNullable = true)]
        public string SmallStationCode { get; set; }
        // 工步编号
        [SugarColumn(ColumnName = "StepNo", IsNullable = true)]
        public string StepNo { get; set; }
        // 工步名称
        [SugarColumn(ColumnName = "StepName", IsNullable = true)]
        public string StepName { get; set; }

        // 顺序号
        [SugarColumn(ColumnName = "No", IsNullable = true)]
        public string No { get; set; }

        // 工步描述
        [SugarColumn(ColumnName = "StepDesc", IsNullable = true)]
        public string StepDesc { get; set; }

        // 操作
        [SugarColumn(ColumnName = "Operation", IsNullable = true)]
        public string Operation { get; set; }

        // 标识
        [SugarColumn(ColumnName = "Identity", IsNullable = true)]
        public string Identity { get; set; }
        // Group
        [SugarColumn(ColumnName = "Group", IsNullable = true)]
        public string Group { get; set; }

        [SugarColumn(IsIgnore = true)]
        public bool IsGroup
        {
            get
            {
                return Group == "1";
            }
            set
            {
                Group = value ? "1" : "0";
            }
        }
    }
}
