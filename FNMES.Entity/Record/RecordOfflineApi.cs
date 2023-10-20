using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SugarIndex("index_offlineApi_reUpload", nameof(RecordOfflineApi.ReUpload), OrderByType.Asc)]    //索引
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_OfflineApi_{year}{month}{day}")]
    public class RecordOfflineApi : BaseRecord
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

        [SugarColumn(ColumnName = "reUpload", IsNullable = true)]
        public int ReUpload { get; set; }

        [SugarColumn(IsIgnore = true)]
        public bool Finished { get { return ReUpload > 0; } }

        [SplitField]
        [SugarColumn(ColumnName = "createTime")]
        public DateTime CreateTime { get; set; }
    }
}
