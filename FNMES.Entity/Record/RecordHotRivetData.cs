using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_HotRivetData_{year}{month}{day}")]
    [SugarIndex("index_HotRivet_productCode", nameof(ProductCode), OrderByType.Asc)]    //索引
    public class RecordHotRivetData   //热铆自动机台得热铆数据先存到数据库中，后需人工位再查出来进行整合
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }
        [SugarColumn(ColumnName = "productCode", ColumnDataType = "varchar(40)", IsNullable = true)]
        public string ProductCode { get; set; }    // FPC条码

        [SugarColumn(ColumnName = "batchCode", ColumnDataType = "varchar(40)", IsNullable = true)]
        public string BatchCode { get; set; } // FPC条码

        [SugarColumn(ColumnName = "station", ColumnDataType = "varchar(20)", IsNullable = true)]
        public string Station { get; set; }      // 工站

        
        [SugarColumn(ColumnName = "result", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string Result { get; set; }   // 结果
        
        [SugarColumn(ColumnName = "testData", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string Data { get; set; }   // 数据
        [SplitField]
        [SugarColumn(ColumnName = "createTime")]
        public DateTime CreateTime { get; set; }
      
    }
}
