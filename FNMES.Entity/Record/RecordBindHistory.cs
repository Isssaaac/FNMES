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
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="status", ColumnDataType = "char(1)", IsNullable = true)]
         public string Status { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="repairFlag", ColumnDataType = "char(1)", IsNullable = true)]
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
