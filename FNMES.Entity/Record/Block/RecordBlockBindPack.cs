using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Month), LineTableInit]
    [SugarTable("Record_BlockBindPack_{year}{month}{day}")]
    public class RecordBlockBindPack:RecordBase
    {
        /// <summary>
        /// Block条码
        ///</summary>
        [SugarColumn(ColumnName = "BlockBarcode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string BlockBarcode { get; set; }
        /// <summary>
        /// Pack条码
        /// </summary>
        [SugarColumn(ColumnName = "PackBarcode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string PackBarcode { get; set; }

        [SugarColumn(ColumnName = "Position", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string Position { get; set; }
    }
}
