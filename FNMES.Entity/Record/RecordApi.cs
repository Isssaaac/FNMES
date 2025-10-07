using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Month), LineTableInit]
    [SugarTable("Record_Api_{year}{month}{day}")]
    public class RecordApi : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long Id { get; set; }
        /// <summary>
        /// 编码 
        ///</summary>
        [SugarColumn(ColumnName = "Url", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string Url { get; set; }
      
        [SugarColumn(ColumnName = "RequestBody", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string RequestBody { get; set; }

        [SugarColumn(ColumnName = "ResponseBody", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
        public string ResponseBody { get; set; }

        [SugarColumn(ColumnName = "Elapsed")]
        public int Elapsed { get; set; }

        [SplitField]
        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }
    }
}
