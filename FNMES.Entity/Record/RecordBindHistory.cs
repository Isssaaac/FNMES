using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Record
{
    /// <summary>
    /// 
    ///</summary>
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_BindHistory_{year}{month}{day}")]
    public class RecordBindHistory : BaseRecord
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
         [SugarColumn(ColumnName="palletNo", ColumnDataType = "varchar(100)", IsNullable = true)]
         public string PalletNo { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="productCode", ColumnDataType = "varchar(100)", IsNullable = true)]
         public string ProductCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="taskOrderNumber", ColumnDataType = "varchar(100)", IsNullable = true)]
         public string TaskOrderNumber { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="productPartNo", ColumnDataType = "varchar(100)", IsNullable = true)]
         public string ProductPartNo { get; set; }

        [SugarColumn(ColumnName = "reessNo", IsNullable = true)]
        public string ReessNo { get; set; }      //国标码

        [SugarColumn(ColumnName = "diverter", IsNullable = true)]
        public string Diverter { get; set; }    //分流器条码

        [SugarColumn(ColumnName = "glueTime", IsNullable = true)]
        public string GlueTime { get; set; }    //中段涂胶时间

        [SugarColumn(ColumnName = "configId")]
        public string ConfigId { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName="status", ColumnDataType = "varchar(10)", IsNullable = true)]
         public string Status { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="repairFlag", ColumnDataType = "varchar(10)", IsNullable = true)]
         public string RepairFlag { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="repairStations", ColumnDataType = "varchar(100)", IsNullable = true)]
         public string RepairStations { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SplitField]
        [SugarColumn(ColumnName="createTime"    )]
         public DateTime? CreateTime { get; set; }
    }
}
