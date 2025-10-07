using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Month), LineTableInit]
    [SugarTable("Record_CellBindBlock_{year}{month}{day}")]
    public class RecordCellBindBlock : RecordBase
    {
        /// <summary>
        /// 电芯条码
        ///</summary>
        [SugarColumn(ColumnName = "CellBarcode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string CellBarcode { get; set; }
        /// <summary>
        /// Block条码
        /// </summary>

        [SugarColumn(ColumnName = "BlockBarcode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string BlockBarcode { get; set; }
        /// <summary>
        /// 虚拟码预留
        /// </summary>

        [SugarColumn(ColumnName = "VirtualBarcode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string VirtualBarcode { get; set; }

        [SugarColumn(ColumnName = "Position", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string Position { get; set; }
    }
}
