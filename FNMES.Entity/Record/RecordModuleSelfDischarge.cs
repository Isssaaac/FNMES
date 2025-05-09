using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_ModuleSelfDischarge_{year}{month}{day}")]
    [SugarIndex("index_ModuleSelfDischarge", nameof(moduleCode), OrderByType.Asc)]
    public  class RecordModuleSelfDischarge
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Pid", IsPrimaryKey = true)]
        public string Pid { get; set; }

        [SugarColumn(ColumnName = "moduleCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string moduleCode { get; set; }

        [SugarColumn(ColumnName = "maxVoltageDrop", ColumnDataType = "varchar(100)", IsNullable = true)]
        //压降最大值
        public string maxVoltageDrop { get; set; }
        [SugarColumn(ColumnName = "minVoltageDrop", ColumnDataType = "varchar(100)", IsNullable = true)]
        //压降最小值
        public string minVoltageDrop { get; set; }
        [SugarColumn(ColumnName = "averageVoltageDrop", ColumnDataType = "varchar(100)", IsNullable = true)]
        //压降平均值
        public string averageVoltageDrop { get; set; }
        [SugarColumn(ColumnName = "stdDeviationVoltageDrop", ColumnDataType = "varchar(100)", IsNullable = true)]
        //压降标准差
        public string stdDeviationVoltageDrop { get; set; }
        [SugarColumn(ColumnName = "judgment1Up", ColumnDataType = "varchar(100)", IsNullable = true)]
        //判定1上限
        public string judgment1Up { get; set; }
        [SugarColumn(ColumnName = "judgment1Lo", ColumnDataType = "varchar(100)", IsNullable = true)]
        //判定1下限
        public string judgment1Lo { get; set; }
        [SugarColumn(ColumnName = "createTime", IsNullable = true)]
        //ocv测试时间
        public DateTime createTime { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(RecordCellSelfDischarge.Pid))]
        public List<RecordCellSelfDischarge> cellSelfDischarges { get; set; }
    }
}
