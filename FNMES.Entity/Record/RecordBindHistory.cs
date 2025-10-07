using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Record
{
    /// <summary>
    /// 
    ///</summary>
    [SplitTable(SplitType.Season), LineTableInit]
    [SugarTable("Record_BindHistory_{year}{month}{day}")]
    public class RecordBindHistory : BaseRecord
    {
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName= "Id", IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "PalletNo", ColumnDataType = "varchar(100)", IsNullable = true)]
         public string PalletNo { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "ProductCode", ColumnDataType = "varchar(100)", IsNullable = true)]
         public string ProductCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "TaskOrderNumber", ColumnDataType = "varchar(100)", IsNullable = true)]
         public string TaskOrderNumber { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "ProductPartNo", ColumnDataType = "varchar(100)", IsNullable = true)]
         public string ProductPartNo { get; set; }

        [SugarColumn(ColumnName = "ReessNo", IsNullable = true)]
        public string ReessNo { get; set; }      //国标码

        [SugarColumn(ColumnName = "Diverter", IsNullable = true)]
        public string Diverter { get; set; }    //分流器条码

        [SugarColumn(ColumnName = "GlueTime", IsNullable = true)]
        public string GlueTime { get; set; }    //中段涂胶时间

        [SugarColumn(ColumnName = "ConfigId")]
        public string ConfigId { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName= "Status", ColumnDataType = "varchar(10)", IsNullable = true)]
         public string Status { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "RepairFlag", ColumnDataType = "varchar(10)", IsNullable = true)]
         public string RepairFlag { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName= "RepairStations", ColumnDataType = "varchar(100)", IsNullable = true)]
         public string RepairStations { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SplitField]
        [SugarColumn(ColumnName= "CreateTime")]
         public DateTime? CreateTime { get; set; }
    }
}
