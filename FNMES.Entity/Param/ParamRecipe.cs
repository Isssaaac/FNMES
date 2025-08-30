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
    [SugarTable("Param_Recipe")]
    public class ParamRecipe:ParamBase
    {
        /// <summary>
        ///  
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }

        [SugarColumn(ColumnName = "productPartNo")]
        public string ProductPartNo { get; set; }

        /// <summary>
        /// 名称 
        ///</summary>
        [SugarColumn(ColumnName = "productDescription", IsNullable = true)]
        public string ProductDescription { get; set; }

        /// <summary>
        /// sap客户项目号 
        ///</summary>
        [SugarColumn(ColumnName = "sapCustomerProjNo", IsNullable = true)]
        public string SapCustomerProjNo { get; set; }

        /// <summary>
        /// 编码 
        ///</summary>s
        [SugarColumn(ColumnName = "bomNo", IsNullable = true)]
         public string BomNo { get; set; }

        /// <summary>
        /// 名称 
        ///</summary>
         [SugarColumn(ColumnName = "bomDescription", IsNullable = true)]
         public string BomDescription { get; set; }

        /// <summary>
        /// BOM版本
        /// </summary>
        [SugarColumn(ColumnName = "bomVersion", IsNullable = true)]
        public string BomVersion { get; set; }

        [SugarColumn(ColumnName = "processConfigName", IsNullable = true)]
        public string ProcessConfigName { get; set; }

        [SugarColumn(ColumnName = "routeNo", IsNullable = true)]
        public string RouteNo { get; set; }

        [SugarColumn(ColumnName = "routeName", IsNullable = true)]
        public string RouteName { get; set; }

        [SugarColumn(ColumnName = "routeVersion", IsNullable = true)]
        public string RouteVersion { get; set; }

        [SugarColumn(ColumnName = "createTime", IsNullable = true)]
        public DateTime? CreateTime { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(ParamRecipeItem.RecipeId))]//一对多
        public List<ParamRecipeItem> processParamItems { get; set; }

        //分库的数据库标识
        [SugarColumn(IsIgnore = true)]
        public string ConfigId
        {
            get; set;
        }

        public void CopyFromGetRecipeData(GetRecipeData source)
        {
            this.Id = SnowFlakeSingle.instance.NextId();
            this.ProductPartNo = source.productPartNo;
            this.ProductDescription = source.productDescription;
            this.SapCustomerProjNo = source.sapCustomerProjNo;
            this.BomNo = source.bomNo;
            this.BomDescription = source.bomDescription;
            this.BomVersion = source.bomVersion;
            this.ProcessConfigName = source.processConfigName;
            this.RouteNo = source.routeNo;
            this.RouteName = source.routeName;
            this.RouteVersion = source.routeVersion;
            // Copy processParamItems list
            this.processParamItems = new List<ParamRecipeItem>();
            this.CreateTime = DateTime.Now;
            foreach (ProcessParamItem paramItem in source.processParamItems)
            {
                ParamRecipeItem paramRecipeItem = new ParamRecipeItem() { Id = SnowFlakeSingle.instance.NextId(), RecipeId = Id};
                paramRecipeItem.CopyField(paramItem);
                paramRecipeItem.EsopList = new List<ParamEsopItem>();
                paramRecipeItem.PartList = new List<ParamPartItem>();
                paramRecipeItem.StepList = new List<ParamStepItem>();
                paramRecipeItem.ParamList = new List<ParamItem>();
                foreach (var setp in paramItem.stepList)
                {
                    var item = new ParamStepItem() {
                        Id = SnowFlakeSingle.instance.NextId(),
                        RecipeItemId = paramRecipeItem.Id
                    };
                    item.CopyField(setp);
                    paramRecipeItem.StepList.Add(item);
                }
                foreach (var it in paramItem.esopList)
                {
                    var item = new ParamEsopItem() {
                        Id = SnowFlakeSingle.instance.NextId(),
                        RecipeItemId = paramRecipeItem.Id
                    };
                    item.CopyField(it);
                    paramRecipeItem.EsopList.Add(item);
                }
                foreach (var it in paramItem.paramList)
                {
                    var item = new ParamItem() {
                        Id = SnowFlakeSingle.instance.NextId(),
                        RecipeItemId = paramRecipeItem.Id
                    };
                    item.CopyField(it);
                    paramRecipeItem.ParamList.Add(item);
                }
                foreach (var it in paramItem.partList)
                {
                    var item = new ParamPartItem() { Id = SnowFlakeSingle.instance.NextId(), RecipeItemId = paramRecipeItem.Id };
                    item.CopyField(it);
                    item.AlternativePartList = new List<ParamAlternativePartItem>() { 
                    };
                    foreach (var sit in it.alternativePartList)
                    {
                        var apart = new ParamAlternativePartItem() {
                            Id = SnowFlakeSingle.instance.NextId(),
                            PartItemId = item.Id
                        };
                        apart.CopyField(sit);
                        item.AlternativePartList.Add(apart);
                    }
                    paramRecipeItem.PartList.Add(item);
                }
                this.processParamItems.Add(paramRecipeItem);
            }
        }


    }
}
