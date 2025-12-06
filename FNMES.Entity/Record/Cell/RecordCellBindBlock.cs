using OfficeOpenXml.Table.PivotTable;
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
    [SugarIndex("index_cellBindBlock_cell_block", nameof(RecordCellBindBlock.CellBarcode), OrderByType.Asc,nameof(RecordCellBindBlock.BlockBarcode), OrderByType.Asc)]
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
