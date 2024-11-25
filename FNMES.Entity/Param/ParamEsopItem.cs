using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_EsopItem")]
    [SugarIndex("index_ParamEsopItem_pid", nameof(ParamEsopItem.RecipeItemId), OrderByType.Asc)]    //索引
    public class ParamEsopItem : CopyAble
    {
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// 配方ID 
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "recipeItemId")]
        public long RecipeItemId { get; set; }
        // 小工位
        [SugarColumn(ColumnName = "smallStationCode",IsNullable = true)]
        public string SmallStationCode { get; set; }


        // SOP文件存储在文件服务器URL路径
        [SugarColumn(ColumnName = "filePath", ColumnDataType = "varchar(MAX)", IsNullable = true)]
        public string FilePath { get; set; }

        // 顺序号（有多本时）
        [SugarColumn(ColumnName = "No", IsNullable = true)]
        public string No { get; set; }

        // 开始页码
        [SugarColumn(ColumnName = "startPageNo", IsNullable = true)]
        public string StartPageNo { get; set; }

        // 结束页码
        [SugarColumn(ColumnName = "endPageNo", IsNullable = true)]
        public string EndPageNo { get; set; }





    }
}
