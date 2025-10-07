using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_BlockPartUpload_{year}{month}{day}")]
    [SugarIndex("index_block_partUpload_productCode", nameof(RecordBlockPartUpload.ProductCode), OrderByType.Asc), LineTableInit]
    public class RecordBlockPartUpload:RecordBase
    {

        [SugarColumn(ColumnName = "ProductCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 内控码
        public string ProductCode { get; set; }
        [SugarColumn(ColumnName = "StationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StationCode { get; set; }
        [SugarColumn(ColumnName = "SmallStationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string SmallStationCode { get; set; }
        [SugarColumn(ColumnName = "EquipmentID", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 设备代码
        public string EquipmentID { get; set; }
        [SugarColumn(ColumnName = "OperatorNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        // 操作员
        public string OperatorNo { get; set; }
        //[Navigate(NavigateType.OneToMany, nameof(RecordPartData.PartUploadId))]
        // 零件列表
        [SugarColumn(IsIgnore = true)]
        public List<RecordCellPartData> PartList { get; set; }
    }
}
