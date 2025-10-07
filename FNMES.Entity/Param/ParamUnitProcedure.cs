using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_UnitProcedure"), LineTableInit]
    public class ParamUnitProcedure:BaseSimpleModelEntity
    {
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName= "Id", IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        /// 有pid的表格不能导入导出
        ///</summary>
        ///
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName= "Pid")]
         public long Pid { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "Encode", IsNullable = true)]
         public string Encode { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "Name", IsNullable = true)]
         public string Name { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "Description", IsNullable = true)]
         public string Description { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName = "IsParent", IsNullable = true)]
         public string IsParent { get; set; }

        //进站产品类型
        [SugarColumn(ColumnName = "InStationProcudtType", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string InStationProductType { get; set; }
        //岀站产品类型
        [SugarColumn(ColumnName = "OutStationProcudtType", ColumnDataType = "varchar(10)", IsNullable = true)]
        public string OutStationProductType { get; set; }
        /// <summary>
        ///  
        ///</summary>

    }
}
