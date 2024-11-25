using FNMES.Entity.DTO.ApiParam;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_UnbindPack_{year}{month}{day}")]
    [SugarIndex("index_unbindPack_stationCode", nameof(StationCode), OrderByType.Asc)]    //索引
    public class RecordUnbindPack : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "productCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 产品码
        public string ProductCode { get; set; }

        [SugarColumn(ColumnName = "stationCode", ColumnDataType = "varchar(10)", IsNullable = true)]
        // 工站
        public string StationCode { get; set; }

        [SugarColumn(ColumnName = "smallStationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 小工站
        public string SmallStationCode { get; set; }

        [SugarColumn(ColumnName = "equipmentID", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 设备代码
        public string EquipmentID { get; set; }

        [SugarColumn(ColumnName = "operatorNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 操作员
        public string OperatorNo { get; set; }

        //[Navigate(NavigateType.OneToMany, nameof(RecordPartData.PartUploadId))]
        // 零件列表
        [SugarColumn(IsIgnore = true)]
        public List<Material> ModelList { get; set; }

        [SplitField]
        [SugarColumn(ColumnName = "createTime")]
        public DateTime CreateTime { get; set; }
    }
}
