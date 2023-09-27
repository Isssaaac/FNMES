using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_SOP_Test")]
    public class ParamSopTest
    {
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="SOPId"    )]
         public long? SOPId { get; set; }
        /// <summary>
        /// 步骤 
        ///</summary>
         [SugarColumn(ColumnName="step"    )]
         public int? Step { get; set; }
        /// <summary>
        /// 名称 
        ///</summary>
         [SugarColumn(ColumnName="testName"    )]
         public string TestName { get; set; }
        /// <summary>
        /// 位置 
        ///</summary>
         [SugarColumn(ColumnName="testPos"    )]
         public string TestPos { get; set; }
        /// <summary>
        /// 项目 
        ///</summary>
         [SugarColumn(ColumnName="testProgram"    )]
         public string TestProgram { get; set; }
        /// <summary>
        /// 基准 
        ///</summary>
         [SugarColumn(ColumnName="baseValue"    )]
         public string BaseValue { get; set; }
        /// <summary>
        /// 上限 
        ///</summary>
         [SugarColumn(ColumnName="upperValue"    )]
         public string UpperValue { get; set; }
        /// <summary>
        /// 下限 
        ///</summary>
         [SugarColumn(ColumnName="lowerValue"    )]
         public string LowerValue { get; set; }
        /// <summary>
        /// 单位 
        ///</summary>
         [SugarColumn(ColumnName="uom"    )]
         public string Uom { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="createUser"    )]
         public long? CreateUser { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="createTime"    )]
         public DateTime? CreateTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="modifyUser"    )]
         public long? ModifyUser { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="modifyTime"    )]
         public DateTime? ModifyTime { get; set; }
    }
}
