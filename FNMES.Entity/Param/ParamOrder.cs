using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Order")]
    public class ParamOrder
    {
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        ///  派工单号
        ///</summary>
         [SugarColumn(ColumnName="taskOrderNumber"  )]
         public string TaskOrderNumber { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="productPartNo"    )]
         public string ProductPartNo { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="productDescription", IsNullable = true)]
         public string ProductDescription { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "planQty", IsNullable = true)]
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
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "uom", IsNullable = true)]
         public string Uom { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "planStartTime", IsNullable = true)]
         public DateTime? PlanStartTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "planEndTime", IsNullable = true)]
         public DateTime? PlanEndTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "receiveTime", IsNullable = true)]
         public DateTime? ReceiveTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "flag", IsNullable = true)]
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
                    default:return "未开工";
                }
            }
                }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName="finishFlag", IsNullable = true)]
         public string FinishFlag { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "startTime", IsNullable = true)]
         public DateTime? StartTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "endTime", IsNullable = true)]
         public DateTime? EndTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "operatorNo", IsNullable = true)]
         public string OperatorNo { get; set; }


    }
}
