using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_Cell_Start_{year}{month}{day}"), LineTableInit]
    public class RecordCellStart:RecordBase
    {
        /// <summary>
        /// 电芯条码
        ///</summary>
        [SugarColumn(ColumnName = "ProductCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ProductCode { get; set; }

        //档位
        [SugarColumn(ColumnName = "Gear", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string Gear { get; set; }

        //标志位
        [SugarColumn(ColumnName = "Flag", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string Flag { get; set; }
    }
}
