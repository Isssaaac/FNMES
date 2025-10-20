using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_OutStation_{year}{month}{day}")]
    [SugarIndex("index_outStation_productCode", nameof(RecordOutStation.ProductCode), OrderByType.Asc), LineTableInit]    //索引
    public class RecordOutStation : RecordBase
    {
        //[Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        //[SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        //public long Id { get; set; }

        [SugarColumn(ColumnName = "ProductCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 内控码
        public string ProductCode { get; set; }
        
        [SugarColumn(ColumnName = "TaskOrderNumber", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 派工单号
        public string TaskOrderNumber { get; set; }
       
        [SugarColumn(ColumnName = "ProductStatus", ColumnDataType = "varchar(10)", IsNullable = true)]
        // 出站电芯状态（合格状态、不合格状态、返修状态）
        public string ProductStatus { get; set; }
        
        [SugarColumn(ColumnName = "DefectCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 不良代码
        public string DefectCode { get; set; }
      
        [SugarColumn(ColumnName = "DefectDesc", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 不良描述
        public string DefectDesc { get; set; }
      
        [SugarColumn(ColumnName = "StationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StationCode { get; set; }
      
        [SugarColumn(ColumnName = "SmallStationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string SmallStationCode { get; set; }
       
        [SugarColumn(ColumnName = "EquipmentID", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 设备编码（选填）
        public string EquipmentID { get; set; }
      
        [SugarColumn(ColumnName = "OperatorNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 操作工
        public string OperatorNo { get; set; }

        //[SplitField]
        //[SugarColumn(ColumnName = "CreateTime")]
        //public DateTime CreateTime { get; set; }


        //2024.5.13添加
        //进站时间
        [SugarColumn(ColumnName = "InstationTime", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string InstationTime { get; set; }

        [SugarColumn(ColumnName = "PalletNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string PalletNo { get; set; }
    }
}
