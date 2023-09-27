using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_SOP_Scan")]
    public class ParamSopScan
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
        /// 物料号 
        ///</summary>
         [SugarColumn(ColumnName="partNumber"    )]
         public string PartNumber { get; set; }
        /// <summary>
        /// 物料名称 
        ///</summary>
         [SugarColumn(ColumnName="partName"    )]
         public string PartName { get; set; }
        /// <summary>
        /// 物料类型 
        ///</summary>
         [SugarColumn(ColumnName="partType"    )]
         public string PartType { get; set; }
        /// <summary>
        /// 物料标识码 
        ///</summary>
         [SugarColumn(ColumnName="partIdentify"    )]
         public string PartIdentify { get; set; }
        /// <summary>
        ///  
        ///</summary>
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
         [SugarColumn(ColumnName="modifyUser"    )]
         public long? ModifyUser { get; set; }
        /// <summary>
        ///  
        ///</summary>
         [SugarColumn(ColumnName="modifyTime"    )]
         public DateTime? ModifyTime { get; set; }
    }
}
