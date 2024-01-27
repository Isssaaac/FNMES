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
    [SugarTable("Param_LocalRoute")]
    [SugarIndex("index_ParamLocalRoute_product", nameof(ParamLocalRoute.ProductPartNo), OrderByType.Asc)]    //索引
    public class ParamLocalRoute : CopyAble
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }
        // 产品
        [SugarColumn(ColumnName = "productPartNo")]
        public string ProductPartNo { get; set; }
        // 工序顺序号   从1开始   1为首工位
        [SugarColumn(ColumnName = "step", IsNullable = true)]
        public int Step { get; set; }
        // 工序编码
        [SugarColumn(ColumnName = "stationCode", IsNullable = true)]
        public string StationCode { get; set; }
        // 过站规则     校验工位结果列表，有多个用,分开   可为空则不校验
        [SugarColumn(ColumnName = "criterion", ColumnDataType = "varchar(100)", IsNullable = true)]
        public string Criterion { get; set; }
        //用于数据显示
        [SugarColumn(IsIgnore = true)]   
        public List<string> CheckStations { get; set; }
        //校验是否允许跳站，不允许则需要上次纪录是上个工位
        [SugarColumn(ColumnName = "allowJump",IsNullable = true)]
        public int AllowJump { get; set; }
        //校验是否允许重复作业
        [SugarColumn(ColumnName = "allowRepeat", IsNullable = true)]
        public int AllowRepeat { get; set; }
        //创建时间
        [SugarColumn(ColumnName = "createTime")]
        public DateTime? CreateTime { get; set; }








    }
}
