using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_Andon")]
    public class ParamAndon
    {
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "andonType", IsNullable = true)]
        public string AndonType { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "andonCode", IsNullable = true)]
        public string AndonCode { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "andonName", IsNullable = true)]
        public string AndonName { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "andonDesc", IsNullable = true)]
        public string AndonDesc { get; set; }
        [SugarColumn(ColumnName = "createTime", IsNullable = true)]
        public string CreateTime { get; set; }

        /// <summary>
        ///  
        ///</summary>
    }
}
