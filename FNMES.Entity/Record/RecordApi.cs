using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Month)]
    [SugarTable("Record_Api_{year}{month}{day}")]
    public class RecordApi : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }
        /// <summary>
        /// 编码 
        ///</summary>
        [SugarColumn(ColumnName = "url", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string Url { get; set; }
      
        [SugarColumn(ColumnName = "requestBody", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string RequestBody { get; set; }

        [SugarColumn(ColumnName = "responseBody", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string ResponseBody { get; set; }

        [SugarColumn(ColumnName = "elapsed")]
        public int Elapsed { get; set; }

        [SplitField]
        [SugarColumn(ColumnName = "createTime")]
        public DateTime CreateTime { get; set; }
    }
}
