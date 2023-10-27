using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param { 
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("Param_ProductStep")]
    public class ParamProductStep: BaseSimpleModelEntity
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
        /// 步骤 
        ///</summary>
        [SugarColumn(ColumnName = "desc")]
        public string Desc { get; set; }
        /// <summary>
        /// 工序 
        ///</summary>
        [SugarColumn(ColumnName="unitProcedure"    )]
         public string UnitProcedure { get; set; }
        /// <summary>
        /// 允许重复作业 
        ///</summary>
         [SugarColumn(ColumnName="allowRepeat"    )]
         public string AllowRepeat { get; set; }


        [SugarColumn(IsIgnore = true)]
        public bool IsAllowRepeat
        {
            get
            {
                return AllowRepeat == "1";
            }
            set
            {
                AllowRepeat = value ? "1" : "0";
            }
        }
        /// <summary>
        /// 开启进站校验 
        ///</summary>
        [SugarColumn(ColumnName="checkInStation"    )]
         public string CheckInStation { get; set; }
        [SugarColumn(IsIgnore = true)]
        public bool IsCheckInStation
        {
            get
            {
                return CheckInStation == "1";
            }
            set
            {
                CheckInStation = value ? "1" : "0";
            }
        }
        /// <summary>
        /// 开启工厂进站校验 
        ///</summary>
        [SugarColumn(ColumnName = "checkFactoryInStation")]
        public string CheckFactoryInStation { get; set; }
        [SugarColumn(IsIgnore = true)]
        public bool IsCheckFactoryInStation
        {
            get
            {
                return CheckFactoryInStation == "1";
            }
            set
            {
                CheckFactoryInStation = value ? "1" : "0";
            }
        }

        /// <summary>
        /// 进站校验工序 
        ///</summary>
        [SugarColumn(ColumnName="checkList")]
         public string CheckList { get; set; }


        [SugarColumn(IsIgnore = true)]
        public List<string> CheckLists
        {
            get { 
                if (CheckList != null) { return CheckList.Split(',').ToList(); }
                return new List<string>();
                 }
        }
        /// <summary>
        /// 启用工序 
        ///</summary>
        [SugarColumn(ColumnName="enableFlag"    )]
         public string EnableFlag { get; set; }

        [SugarColumn(IsIgnore = true)]
        public bool IsEnableFlag
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
