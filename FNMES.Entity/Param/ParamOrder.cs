using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 工单表：同步工单
    ///</summary>
    [SugarTable("Order"), LineTableInit]
    public class ParamOrder : ParamBase
    {
        /// <summary>
        ///  派工单号
        ///</summary>
         [SugarColumn(ColumnName= "TaskOrderNumber")]
         public string TaskOrderNumber { get; set; }
        /// <summary>
        ///  物料编号
        ///</summary>
         [SugarColumn(ColumnName= "ProductPartNo")]
         public string ProductPartNo { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "ProductDescription", IsNullable = true)]
         public string ProductDescription { get; set; }
        /// <summary>
        ///  计划产量
        ///</summary>
         [SugarColumn(ColumnName = "PlanQty", IsNullable = true)]
         public int PlanQty { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(IsIgnore = true)]
        public int StartQty { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(IsIgnore = true)]
        public int PackQty { get; set; }

        [SugarColumn(IsIgnore = true)]
        public int ScrappedQty
        {
            get; set;
        }

        [SugarColumn(ColumnName = "Uom", IsNullable = true)]
         public string Uom { get; set; }
        /// <summary>
        ///  计划开始时间
        ///</summary>
         [SugarColumn(ColumnName = "PlanStartTime", IsNullable = true)]
         public DateTime? PlanStartTime { get; set; }
        /// <summary>
        ///  计划结束时间
        ///</summary>
         [SugarColumn(ColumnName = "PlanEndTime", IsNullable = true)]
         public DateTime? PlanEndTime { get; set; }
        /// <summary>
        ///  获取时间
        ///</summary>
         [SugarColumn(ColumnName = "ReceiveTime", IsNullable = true)]
         public DateTime? ReceiveTime { get; set; }
        /// <summary>
        ///  工单状态
        ///</summary>
         [SugarColumn(ColumnName = "Flag", IsNullable = true)]
         public string Flag { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string FlagString {
            get{  
                switch (Flag)
                {
                    case "0": return "未开工";
                    case "1": return "生产中";
                    case "2": return "暂停";
                    case "3": return "取消";
                    case "4": return "完成";
                    case "5": return "人工干预完成";
                    default:return "未开工";
                }
            }
        }
        /// <summary>
        ///  完成标志
        ///</summary>
        [SugarColumn(ColumnName= "FinishFlag", IsNullable = true)]
         public string FinishFlag { get; set; }
        /// <summary>
        ///  开始时间
        ///</summary>
         [SugarColumn(ColumnName = "StartTime", IsNullable = true)]
         public DateTime? StartTime { get; set; }
        /// <summary>
        ///  结束时间
        ///</summary>
         [SugarColumn(ColumnName = "EndTime", IsNullable = true)]
         public DateTime? EndTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "OperatorNo", IsNullable = true)]
         public string OperatorNo { get; set; }
        /// <summary>
        ///  241206新增字段
        ///</summary>
        [SugarColumn(ColumnName = "PackCellGear", IsNullable = true)]
        public string PackCellGear { get; set; }
    }
}
