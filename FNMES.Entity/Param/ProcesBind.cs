using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Process_Bind")]
    public class ProcessBind
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
         [SugarColumn(ColumnName="palletNo"    )]
         public string PalletNo { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="productCode"    )]
         public string ProductCode { get; set; }
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
         [SugarColumn(ColumnName="status"    )]
         public string Status { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="repairFlag"    )]
         public string RepairFlag { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="repairStations"    )]
         public string RepairStations { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="createTime"    )]
         public DateTime? CreateTime { get; set; }
    }
}
