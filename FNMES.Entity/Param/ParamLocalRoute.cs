using System;
using System.Collections.Generic;
using System.Linq;
using FNMES.Entity.DTO.ApiData;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_LocalRoute"), LineTableInit]
    [SugarIndex("index_ParamLocalRoute_product", nameof(ParamLocalRoute.ProductPartNo), OrderByType.Asc)]    //索引
    public class ParamLocalRoute : BaseSimpleModelEntity
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long Id { get; set; }
        // 产品
        [SugarColumn(ColumnName = "ProductPartNo")]
        public string ProductPartNo { get; set; }
        // 工序顺序号   从1开始   1为首工位
        [SugarColumn(ColumnName = "Step", IsNullable = true)]
        public int Step { get; set; }

        // 工序编码
        [SugarColumn(ColumnName = "StationCode", IsNullable = true)]
        public string StationCode { get; set; }

        // 工序名称,显示在工艺路线界面
        [SugarColumn(IsIgnore = true)]
        public string StationName { get; set; }

        // 过站规则     校验工位结果列表，有多个用,分开   可为空则不校验
        [SugarColumn(ColumnName = "Criterion", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string Criterion { get; set; }
        //用于数据显示  
        [SugarColumn(IsIgnore = true)]   
        public List<string> CheckStations {
            get; set;
            //get { 
            //     if (Criterion!="")
            //    {
            //        return Criterion.Split(',').ToList();
            //    }
            //     return null;
            //    }
            //set {
            //    Criterion = String.Join(",", value);
            //    }
        }
        //校验是否允许跳站，不允许则需要上次记录是上个工位
        [SugarColumn(ColumnName = "AllowJump", ColumnDataType = "varchar(10)",IsNullable = true)]
        public string AllowJump { get; set; }
        //校验是否允许重复作业
        [SugarColumn(ColumnName = "AllowRepeat", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string AllowRepeat { get; set; }
        //是否中转工位  从中段流转下来的首工序   用于在进站的时候从工厂进行校验
        [SugarColumn(ColumnName = "TranshipStation", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string TranshipStation { get; set; }
        //是否中转工位  从中段流转下来的首工序   用于在进站的时候从工厂进行校验
        [SugarColumn(ColumnName = "Entrance", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string Entrance { get; set; }

        //生成国标码工位
        [SugarColumn(ColumnName = "GenerateCodeStation", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string GenerateCodeStation { get; set; }




        //用于数据显示
        [SugarColumn(IsIgnore = true)]
        public bool IsTranshipStation
        {
            get
            {
                return TranshipStation == "1";
            }
            set
            {
                TranshipStation = value ? "1" : "0";
            }
        }
        [SugarColumn(IsIgnore = true)]
        public bool IsEntrance
        {
            get
            {
                return Entrance == "1";
            }
            set
            {
                Entrance = value ? "1" : "0";
            }
        }

        [SugarColumn(IsIgnore = true)]
        public bool IsGenerateCodeStation
        {
            get
            {
                return GenerateCodeStation == "1";
            }
            set
            {
                GenerateCodeStation = value ? "1" : "0";
            }
        }


        [SugarColumn(IsIgnore = true)]
        public bool IsAllowJump
        {
            get
            {
                return AllowJump == "1";
            }
            set
            {
                AllowJump = value ? "1" : "0";
            }
        }




        //用于数据显示
        [SugarColumn(IsIgnore = true)]
        public bool IsAllowRepeat {
            get
            {
                return AllowRepeat == "1";
            }
            set
            {
                AllowRepeat = value ? "1" : "0";
            } 
        }

        
    }
}
