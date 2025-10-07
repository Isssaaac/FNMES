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
    [SugarIndex("index_orderStart_taskOrderNumber", nameof(RecordOrderStart.TaskOrderNumber), OrderByType.Asc), LineTableInit]    //索引
    public class RecordOrderStart : BaseRecord
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
        [SugarColumn(ColumnName = "PackNo", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string PackNo { get; set; }

        [SplitField]
        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }

        //标志位，用于标注内控码是否报废，报废后，统计数量时候会忽略
        [SugarColumn(ColumnName = "Flag", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string Flag { get; set; }


        //分库的数据库标识   只用来映射实体传递数据
        [SugarColumn(IsIgnore = true)]
        public string PackCellGear
        {
            get; set;
        }
    }
}
