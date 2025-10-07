using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_EsopItem"), LineTableInit]
    [SugarIndex("index_ParamEsopItem_pid", nameof(ParamEsopItem.RecipeItemId), OrderByType.Asc)]    //索引
    public class ParamEsopItem : ParamBase
    {
        /// 配方ID 
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "RecipeItemId")]
        public long RecipeItemId { get; set; }
        // 小工位
        [SugarColumn(ColumnName = "SmallStationCode", IsNullable = true)]
        public string SmallStationCode { get; set; }


        // SOP文件存储在文件服务器URL路径
        [SugarColumn(ColumnName = "FilePath", ColumnDataType = "varchar(MAX)", IsNullable = true)]
        public string FilePath { get; set; }

        // 顺序号（有多本时）
        [SugarColumn(ColumnName = "No", IsNullable = true)]
        public string No { get; set; }

        // 开始页码
        [SugarColumn(ColumnName = "StartPageNo", IsNullable = true)]
        public string StartPageNo { get; set; }

        // 结束页码
        [SugarColumn(ColumnName = "EndPageNo", IsNullable = true)]
        public string EndPageNo { get; set; }

    }
}
