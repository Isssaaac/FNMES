using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FNMES.Entity.Record
{
    [SplitTable(SplitType.Season)]
    [SugarTable("Record_SelfDischarge_{year}{month}{day}")]
    [SugarIndex("index_SelfDischarge", nameof(productCode), OrderByType.Asc), LineTableInit]
    public class RecordSelfDischarge
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "productCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string productCode { get; set; } //物料号

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
        
        //电芯条码
        [SugarColumn(ColumnName = "cellCode", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string cellCode { get; set; }

        //压降
        [SugarColumn(ColumnName = "voltageDrop", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string voltageDrop { get; set; }

        [SugarColumn(ColumnName = "a020TestTime", IsNullable = true)]
        public DateTime a020TestTime { get; set; }
        [SugarColumn(ColumnName = "a020TestVoltage", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string a020TestVoltage { get; set; }

        [SugarColumn(ColumnName = "m350TestTime", IsNullable = true)]
        public DateTime m350TestTime { get; set; }
        [SugarColumn(ColumnName = "m350TestVoltage", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string m350TestVoltage { get; set; }
        [SugarColumn(ColumnName = "timeInterval", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string timeInterval { get; set; }



        [SugarColumn(ColumnName = "intervalVoltageDrop", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string intervalVoltageDrop { get; set; }

        


        //判定1上限判定
        [SugarColumn(ColumnName = "judgment1UpResult", ColumnDataType = "varchar(100)", IsNullable = true)]

        public string judgment1UpResult { get; set; }
        [SugarColumn(ColumnName = "judgment1LoResult", ColumnDataType = "varchar(100)", IsNullable = true)]
        //判定1下限判定
        public string judgment1LoResult { get; set; }
        [SugarColumn(ColumnName = "judgment1Result", ColumnDataType = "varchar(100)", IsNullable = true)]
        //判定1判定
        public string judgment1Result { get; set; }
        [SugarColumn(ColumnName = "judgment2Result", ColumnDataType = "varchar(100)", IsNullable = true)]
        //判定2判定
        public string judgment2Result { get; set; }

        [SugarColumn(ColumnName = "result", ColumnDataType = "varchar(100)", IsNullable = true)]
        //结果
        public string result { get; set; }

        [SplitField]
        [SugarColumn(ColumnName = "createTime", IsNullable = true)]
        //ocv测试时间
        public DateTime createTime { get; set; }
    }
}
