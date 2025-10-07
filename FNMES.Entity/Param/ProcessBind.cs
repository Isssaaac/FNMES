using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Process_Bind"), LineTableInit]
    public class ProcessBind:ParamBase
    {
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "PalletNo", IsNullable = true  )]
         public string PalletNo { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "ProductCode", IsNullable = true)]
         public string ProductCode { get; set; }


        /// <summary>
        /// 内控码在托盘上的位置，用1-8数字来表达
        ///</summary>
        [SugarColumn(ColumnName = "Position", IsNullable = true)]
        public string Position { get; set; }


        [SugarColumn(ColumnName = "TaskOrderNumber", IsNullable = true)]
         public string TaskOrderNumber { get; set; }
      
         [SugarColumn(ColumnName = "ProductPartNo", IsNullable = true)]
         public string ProductPartNo { get; set; }


        [SugarColumn(ColumnName = "ReessNo", IsNullable = true)]
        public string ReessNo { get; set; }      //国标码

        //分流器
        [SugarColumn(ColumnName = "Diverter", IsNullable = true)]
        public string Diverter { get; set; }    //分流器条码

        [SugarColumn(ColumnName = "GlueTime", IsNullable = true)]
        public string GlueTime { get; set; }    //中段涂胶时间

        [SugarColumn(ColumnName = "ConfigId", IsNullable = true)]
        public string ConfigId { get; set; }

        [SugarColumn(ColumnName = "CurrentStation", IsNullable = true)]
        public string CurrentStation { get; set; }

        [SugarColumn(ColumnName = "Status", IsNullable = true)]
         public string Status { get; set; }
        /// <summary>`
        ///</summary>
         [SugarColumn(ColumnName = "RepairFlag", IsNullable = true)]
         public string RepairFlag { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "RepairStations", IsNullable = true)]
         public string RepairStations { get; set; }
    }
}
