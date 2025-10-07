using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    [SugarTable("Param_BarcodeRule"), LineTableInit]
    public class ParamBarcodeRule : ParamBase
    {
        /// <summary>
        ///  规格代码
        ///</summary>
        [SugarColumn(ColumnName = "StandardCode", IsNullable = true)]
        public string StandardCode { get; set; }
        /// <summary>
        ///  追溯信息码
        ///</summary>
        [SugarColumn(ColumnName = "TraceInfoCode", IsNullable = true)]
        public string TraceInfoCode { get; set; }
        /// <summary>
        ///  厂商地址
        ///</summary>
        [SugarColumn(ColumnName = "VendorAddress", IsNullable = true)]
        public string VendorAddress { get; set; }
        /// <summary>
        ///  流水号
        ///</summary>
        [SugarColumn(ColumnName = "SerialNumber", IsNullable = true)]
        public int SerialNumber { get; set; }
    }
}
