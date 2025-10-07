using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_Test_ACR_{year}{month}{day}"), LineTableInit]
    //[SugarIndex("index_ACR_productCode", nameof(ProductCode), OrderByType.Asc)]    //索引
    public class RecordTestACR
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long Id { get; set; }
        [SugarColumn(ColumnName = "ProductCode", ColumnDataType = "varchar(40)", IsNullable = true)]
        // 内控码
        public string ProductCode { get; set; }
        // 结果
        [SugarColumn(ColumnName = "Result", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string Result { get; set; }
        // 数据
        [SugarColumn(ColumnName = "Data", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string Data { get; set; }
        [SplitField]
        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }
      
    }
}
