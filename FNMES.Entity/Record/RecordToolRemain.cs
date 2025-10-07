using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_ToolRemain_{year}{month}{day}"), LineTableInit]
    [SugarIndex("index_toolRemain_stationCode", nameof(RecordToolRemain.StationCode), OrderByType.Asc)]    //索引
    public class RecordToolRemain : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long Id { get; set; }
        
        [SugarColumn(ColumnName = "ProductCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 产品码
        public string ProductCode { get; set; }

        [SugarColumn(ColumnName = "StationCode", ColumnDataType = "varchar(10)", IsNullable = true)]
        // 工站
        public string StationCode { get; set; }

        [SugarColumn(ColumnName = "EquipmentID", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 设备代码
        public string EquipmentID { get; set; }

        [SugarColumn(ColumnName = "OperatorNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 操作员
        public string OperatorNo { get; set; }

        //[Navigate(NavigateType.OneToMany, nameof(RecordPartData.PartUploadId))]
        // 零件列表
        [SugarColumn(IsIgnore = true)]
        public List<RecordToolData> ToolList { get; set; }

        [SplitField]
        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }
      
    }
}
