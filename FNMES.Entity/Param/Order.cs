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
    public class Order
    {
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="taskOrderNumber"    )]
         public string TaskOrderNumber { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="productPartNo"    )]
         public string ProductPartNo { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="productDescription"    )]
         public string ProductDescription { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="planQty"    )]
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
        [SugarColumn(ColumnName="uom"    )]
         public string Uom { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="planStartTime"    )]
         public DateTime? PlanStartTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="planEndTime"    )]
         public DateTime? PlanEndTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="receiveTime"    )]
         public DateTime? ReceiveTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="flag"    )]
         public string Flag { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="finishFlag"    )]
         public string FinishFlag { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="startTime"    )]
         public DateTime? StartTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="endTime"    )]
         public DateTime? EndTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="operatorNo"    )]
         public string OperatorNo { get; set; }


    }
}
