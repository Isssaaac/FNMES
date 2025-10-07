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
    [SugarTable("Record_OfflineApi_{year}{month}{day}"), LineTableInit]
    public class RecordOfflineApi : BaseRecord
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

        [SugarColumn(ColumnName = "ReUpload", IsNullable = true)]
        public int ReUpload { get; set; }

        [SugarColumn(IsIgnore = true)]
        public bool Finished { get { return ReUpload > 0; } }

        [SplitField]
        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }
    }
}
