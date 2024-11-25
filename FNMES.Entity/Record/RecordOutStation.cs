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
    [SugarIndex("index_outStation_productCode", nameof(RecordOutStation.ProductCode), OrderByType.Asc)]    //索引
    public class RecordOutStation : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "productCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 内控码
        public string ProductCode { get; set; }
        
        [SugarColumn(ColumnName = "taskOrderNumber", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 派工单号
        public string TaskOrderNumber { get; set; }
       
        [SugarColumn(ColumnName = "productStatus", ColumnDataType = "varchar(10)", IsNullable = true)]
        // 出站电芯状态（合格状态、不合格状态、返修状态）
        public string ProductStatus { get; set; }
        
        [SugarColumn(ColumnName = "defectCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 不良代码
        public string DefectCode { get; set; }
      
        [SugarColumn(ColumnName = "defectDesc", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 不良描述
        public string DefectDesc { get; set; }
      
        [SugarColumn(ColumnName = "stationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StationCode { get; set; }
      
        [SugarColumn(ColumnName = "smallStationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string SmallStationCode { get; set; }
       
        [SugarColumn(ColumnName = "equipmentID", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 设备编码（选填）
        public string EquipmentID { get; set; }
      
        [SugarColumn(ColumnName = "operatorNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 操作工
        public string OperatorNo { get; set; }

        [SplitField]
        [SugarColumn(ColumnName = "createTime")]
        public DateTime CreateTime { get; set; }


        //2024.5.13添加
        //进站时间
        [SugarColumn(ColumnName = "instationTime", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string instationTime { get; set; }

        [SugarColumn(ColumnName = "palletNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string palletNo { get; set; }
    }
}
