using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Month)]
    [SugarTable("Record_Cell_Start_{year}{month}{day}"), LineTableInit]
    [SugarIndex("index_cellStart_productCode", nameof(RecordCellStart.ProductCode), OrderByType.Asc)]    //索引
    public class RecordCellStart:RecordBase
    {
        /// <summary>
        /// 电芯条码
        ///</summary>
        [SugarColumn(ColumnName = "ProductCode", ColumnDataType = "varchar(100)")]
        public string ProductCode { get; set; }

        //档位
        [SugarColumn(ColumnName = "Grade", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string Grade { get; set; }

        //O2测试时间
        [SugarColumn(ColumnName = "LastOCVDate", ColumnDataType = "varchar(30)", IsNullable = true)]
        public string LastOCVDate { get; set; }

        //
        [SugarColumn(ColumnName = "O2Voltage", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string O2Voltage { get; set; }

        //标志位
        [SugarColumn(ColumnName = "Flag", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string Flag { get; set; }
    }
}
