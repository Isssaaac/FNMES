using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Month)]
    [SugarTable("Record_ProcessUpload_{year}{month}{day}")]
    [SugarIndex("index_processUpload_productCode", nameof(RecordProcessUpload.ProductCode), OrderByType.Asc)]    //索引
    public class RecordProcessUpload : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }
        // 内控码
        [SugarColumn(ColumnName = "productCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ProductCode { get; set; }
          // 大工站
        [SugarColumn(ColumnName = "bigStationCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string BigStationCode { get; set; }
        // 小工站
        [SugarColumn(ColumnName = "stationCode", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string StationCode { get; set; }
        // 设备代码
        [SugarColumn(ColumnName = "equipmentID", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string EquipmentID { get; set; }
        // 配方号
        [SugarColumn(ColumnName = "recipeNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string RecipeNo { get; set; }

        // 程序名称
        [SugarColumn(ColumnName = "recipeDescription", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string RecipeDescription { get; set; }

        // 程序版本
        [SugarColumn(ColumnName = "recipeVersion", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string RecipeVersion { get; set; }

        // 检测最终结果
        [SugarColumn(ColumnName = "totalFlag", ColumnDataType = "char(10)", IsNullable = true)]
        public string TotalFlag { get; set; }

        // 操作工
        [SugarColumn(ColumnName = "recipeNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string OperatorNo { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(RecordProcessData.ProcessUploadId))]
        // 过程列表
        public List<RecordProcessData> ProcessList { get; set; }

        [SplitField]
        [SugarColumn(ColumnName = "createTime")]
        public DateTime CreateTime { get; set; }
      
    }
}



