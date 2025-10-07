using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Sys
{
    //用于记录预处理线的单工位的产品选择    每个工序可以独立选择要作业的产品   
    [SugarTable("Sys_PreSelectProduct"), SystemTableInit]
    public  class SysPreSelectProduct
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long Id { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "LineId",IsNullable= true)]
        public long LineId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(LineId), nameof(SysLine.Id)),SugarColumn(IsIgnore = true)]//一对一
        public SysLine Line { get; set; } //不能赋值只能是null

        [SugarColumn(ColumnName = "Station", ColumnDataType = "varchar(20)", IsNullable = true)]
        public string Station { get; set; }   //工站

        [SugarColumn(ColumnName = "ProductPartNo", ColumnDataType = "varchar(50)", IsNullable = true)]
        public string ProductPartNo { get; set; }   //所选产品编码

        [SugarColumn(ColumnName = "ProductDescription", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string ProductDescription { get; set; }   //所选产品描述
        [SugarColumn(ColumnName = "SapCustomerProjNo",  IsNullable = true)]
        public string SapCustomerProjNo { get; set; }   //客户产品代码，，给PLC用

        [SugarColumn(ColumnName = "ProductLineId", ColumnDataType = "varchar(5)", IsNullable = true)]
        public string ProductLineId { get; set; }    //产品归属线体标识，，，此字段直接存标识，不存线体主键

        [SugarColumn(ColumnName = "CreateUser", ColumnDataType = "varchar(50)", IsNullable = true)] 
        public string CreateUser { get; set; }   //创建用户
        ///</summary>

        [SugarColumn(ColumnName = "CreateTime",IsNullable = true)]
        public DateTime? CreateTime { get; set; }   //创建时间

        [SugarColumn(ColumnName = "EnableFlag", ColumnDataType = "varchar(5)" ,IsNullable = true)]
        public string EnableFlag { get; set; }

        [SugarColumn(IsIgnore = true)]
        public bool IsEnabled
        {
            get
            {
                return EnableFlag == "1";
            }
            set
            {
                EnableFlag = value ? "1" : "0";
            }
        }
    }
}
