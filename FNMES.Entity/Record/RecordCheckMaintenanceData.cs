using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Month), LineTableInit]
    [SugarTable("Record_CheckMaintenanceData_{year}{month}{day}")]
    public class RecordCheckMaintenanceData : RecordBase
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Pid")]
        public long Pid { get; set; }

        [SugarColumn(ColumnName = "Function", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string Function { get; set; }

        [SugarColumn(ColumnName = "Actual_value", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string Actual_value { get; set; }

        [SugarColumn(ColumnName = "Specification_value", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string Specification_value { get; set; }

        [SugarColumn(ColumnName = "CheckResult", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string CheckResult { get; set; }

        [SugarColumn(ColumnName = "CheckStatus", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string CheckStatus { get; set; }

        [SugarColumn(ColumnName = "ResponsePlan", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ResponsePlan { get; set; }


        [SugarColumn(ColumnName = "CheckDate", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string CheckDate { get; set; }

        [SugarColumn(ColumnName = "CheckNumber", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string CheckNumber { get; set; }
    }
}
