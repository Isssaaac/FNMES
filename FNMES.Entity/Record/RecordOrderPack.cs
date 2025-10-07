using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season)]
    [SugarTable("Order_ack_Pack_{year}{month}{day}")]
    [SugarIndex("index_orderPack_taskOrderNumber", nameof(RecordOrderPack.TaskOrderNumber), OrderByType.Asc), LineTableInit]    //索引
    public class RecordOrderPack : BaseRecord
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long Id { get; set; }
        /// <summary>
        /// 编码 
        ///</summary>
        [SugarColumn(ColumnName = "TaskOrderNumber", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string TaskOrderNumber { get; set; }
        /// <summary>
        /// 名称 
        ///</summary>
        [SugarColumn(ColumnName = "ProductCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ProductCode { get; set; }
        /// <summary>
        /// 备注 
        ///</summary>
        [SugarColumn(ColumnName = "ReessNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ReessNo { get; set; }

        [SplitField]
        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }
    }
}
