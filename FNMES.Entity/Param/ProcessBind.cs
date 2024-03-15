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
       
         [SugarColumn(ColumnName = "taskOrderNumber", IsNullable = true)]
         public string TaskOrderNumber { get; set; }
      
         [SugarColumn(ColumnName = "productPartNo", IsNullable = true)]
         public string ProductPartNo { get; set; }


        [SugarColumn(ColumnName = "reessNo", IsNullable = true)]
        public string ReessNo { get; set; }      //国标码

        [SugarColumn(ColumnName = "diverter", IsNullable = true)]
        public string Diverter { get; set; }    //分流器条码

        [SugarColumn(ColumnName = "glueTime", IsNullable = true)]
        public string GlueTime { get; set; }    //中段涂胶时间

        [SugarColumn(ColumnName = "configId", IsNullable = true)]
        public string ConfigId { get; set; }

        [SugarColumn(ColumnName = "currentStation", IsNullable = true)]
        public string CurrentStation { get; set; }

        [SugarColumn(ColumnName = "status", IsNullable = true)]
         public string Status { get; set; }
        /// <summary>`
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
