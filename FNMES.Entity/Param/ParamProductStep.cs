using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param { 
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_ProductStep")]
    public class ParamProductStep
    {
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        /// 产品ID 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="productId"    )]
         public long? ProductId { get; set; }
        /// <summary>
        /// 步骤 
        ///</summary>
         [SugarColumn(ColumnName="step"    )]
         public int? Step { get; set; }
        /// <summary>
        /// 工序 
        ///</summary>
         [SugarColumn(ColumnName="unitProcedureId"    )]
         public long? UnitProcedureId { get; set; }
        /// <summary>
        /// 允许重复作业 
        ///</summary>
         [SugarColumn(ColumnName="allowRepeat"    )]
         public string AllowRepeat { get; set; }
        /// <summary>
        /// 开启进站校验 
        ///</summary>
         [SugarColumn(ColumnName="checkInStation"    )]
         public string CheckInStation { get; set; }
        /// <summary>
        /// 进站校验工序 
        ///</summary>
         [SugarColumn(ColumnName="checkListId"    )]
         public string CheckListId { get; set; }
        /// <summary>
        /// 启用工序 
        ///</summary>
         [SugarColumn(ColumnName="enableFlag"    )]
         public string EnableFlag { get; set; }
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
