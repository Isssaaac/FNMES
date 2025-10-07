using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season), LineTableInit]
    [SugarTable("Record_HotRivetData_{year}{month}{day}")]
    [SugarIndex("index_HotRivet_productCode", nameof(ProductCode), OrderByType.Asc)]    //索引
    public class RecordHotRivetData   //热铆自动机台得热铆数据先存到数据库中，后需人工位再查出来进行整合
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long Id { get; set; }
        [SugarColumn(ColumnName = "ProductCode", ColumnDataType = "varchar(40)", IsNullable = true)]
        public string ProductCode { get; set; }    // 内控码

        [SugarColumn(ColumnName = "BatchCode", ColumnDataType = "varchar(40)", IsNullable = true)]
        public string BatchCode { get; set; } // FPC条码

        [SugarColumn(ColumnName = "Station", ColumnDataType = "varchar(20)", IsNullable = true)]
        public string Station { get; set; }      // 工站

        
        [SugarColumn(ColumnName = "Result", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string Result { get; set; }   // 结果
        
        [SugarColumn(ColumnName = "Data", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string Data { get; set; }   // 数据
        [SplitField]
        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }
      
    }
}
