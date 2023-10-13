using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season)]
    [SugarTable("Order_Start_Record_{year}{month}{day}")]
    [SugarIndex("index_orderStart_taskOrderNumber", nameof(RecordOrderStart.TaskOrderNumber), OrderByType.Asc)]    //索引
    public class RecordOrderStart : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }
        /// <summary>
        /// 编码 
        ///</summary>
        [SugarColumn(ColumnName = "taskOrderNumber", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string TaskOrderNumber { get; set; }
        /// <summary>
        /// 名称 
        ///</summary>
        [SugarColumn(ColumnName = "productCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ProductCode { get; set; }
        /// <summary>
        /// 备注 
        ///</summary>
        [SugarColumn(ColumnName = "packNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string PackNo { get; set; }

        [SplitField]
        [SugarColumn(ColumnName = "createTime")]
        public DateTime CreateTime { get; set; }

    }
}
