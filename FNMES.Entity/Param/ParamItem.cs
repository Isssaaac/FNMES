using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_Item")]
    [SugarIndex("index_ParamItem_pid", nameof(ParamItem.RecipeItemId), OrderByType.Asc)]    //索引
    public class ParamItem: CopyAble
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


        // 工步编码（可选）
        [SugarColumn(ColumnName = "stepNo", IsNullable = true)]
        public string StepNo { get; set; }

        // 工步名称（可选）
        [SugarColumn(ColumnName = "stepName", IsNullable = true)]
        public string StepName { get; set; }

        // 顺序号，工步之间的（可选）
        [SugarColumn(ColumnName = "No", IsNullable = true)]
        public string OrderNo { get; set; }

        // 工艺参数编码（操作类型编码）
        [SugarColumn(ColumnName = "paramCode", IsNullable = true)]
        public string ParamCode { get; set; }

        // 工艺参数名称（操作类型名称）
        [SugarColumn(ColumnName = "paramName", IsNullable = true)]
        public string ParamName { get; set; }

        // 工艺描述
        [SugarColumn(ColumnName = "processDesc", IsNullable = true)]
        public string ProcessDescription { get; set; }

        // 参数分类：产品参数、过程参数、设备预设参数（工艺参数）
        [SugarColumn(ColumnName = "paramClassification", IsNullable = true)]
        public string ParamClassification { get; set; }

        // 判定类型：数据收集、相机判定、设备判定、取标准值、取上下限判定、取下限判定、取上限判定、无需判定
        [SugarColumn(ColumnName = "decisionType", IsNullable = true)]
        public string DecisionType { get; set; }

        // 参数类型：定性（字符串）：选择定性，选项1-不需要，选项2-不需要，设定值-需要；定量（数值型）：标准值，上限，下限。
        [SugarColumn(ColumnName = "paramType", IsNullable = true)]
        public string ParamType { get; set; }

        // 工艺参数标准值，针对定量类型的
        [SugarColumn(ColumnName = "standValue", IsNullable = true)]
        public string StandValue { get; set; }

        // 工艺参数最大值，针对定量类型的
        [SugarColumn(ColumnName = "maxValue", IsNullable = true)]
        public string MaxValue { get; set; }

        // 工艺参数最小值，针对定量类型的
        [SugarColumn(ColumnName = "minValue", IsNullable = true)]
        public string MinValue { get; set; }

        // 针对定性的设定值
        [SugarColumn(ColumnName = "setValue", IsNullable = true)]
        public string SetValue { get; set; }

        // 工厂MES是否二次校验
        [SugarColumn(ColumnName = "isDoubleCheck", IsNullable = true)]
        public string IsDoubleCheck { get; set; }

        // 单位
        [SugarColumn(ColumnName = "uom", IsNullable = true)]
        public string UnitOfMeasure { get; set; }
    }
}
