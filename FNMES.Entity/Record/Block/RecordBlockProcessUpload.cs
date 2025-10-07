using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_BlockProcessUpload_{year}{month}{day}")]
    [SugarIndex("index_block_processUpload_productCode", nameof(RecordBlockProcessUpload.ProductCode), OrderByType.Asc), LineTableInit]    //索引
    public class RecordBlockProcessUpload: RecordBase
    {
        // 电芯码
        [SugarColumn(ColumnName = "ProductCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ProductCode { get; set; }
        // 大工站
        [SugarColumn(ColumnName = "StationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string StationCode { get; set; }
        [SugarColumn(ColumnName = "SmallStationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string SmallStationCode { get; set; }
        // 设备代码
        [SugarColumn(ColumnName = "EquipmentID", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string EquipmentID { get; set; }
        // 配方号
        [SugarColumn(ColumnName = "RecipeNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string RecipeNo { get; set; }

        // 程序名称
        [SugarColumn(ColumnName = "RecipeDescription", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string RecipeDescription { get; set; }

        // 程序版本
        [SugarColumn(ColumnName = "RecipeVersion", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string RecipeVersion { get; set; }

        // 检测最终结果
        [SugarColumn(ColumnName = "TotalFlag", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string TotalFlag { get; set; }

        // 操作工
        [SugarColumn(ColumnName = "OperatorNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string OperatorNo { get; set; }

        //[Navigate(NavigateType.OneToMany, nameof(RecordProcessData.ProcessUploadId))]
        // 过程列表,分表后不支持导航
        [SugarColumn(IsIgnore = true)]
        public List<RecordBlockProcessData> ProcessList { get; set; }
    }
}
