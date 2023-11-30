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
         [SugarColumn(ColumnName="palletNo"  ,IsNullable = true  )]
         public string PalletNo { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "productCode", IsNullable = true)]
         public string ProductCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "taskOrderNumber", IsNullable = true)]
         public string TaskOrderNumber { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "productPartNo", IsNullable = true)]
         public string ProductPartNo { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "configId", IsNullable = true)]
        public string ConfigId { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "status", IsNullable = true)]
         public string Status { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "repairFlag", IsNullable = true)]
         public string RepairFlag { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "repairStations", IsNullable = true)]
         public string RepairStations { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "createTime", IsNullable = true)]
         public DateTime? CreateTime { get; set; }
    }
}
