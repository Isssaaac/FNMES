using FNMES.Entity.DTO.ApiData;
using FNMES.Entity.DTO.ApiParam;
using FNMES.Entity.Param;
using FNMES.Utility.Core;
using ServiceStack;
using SqlSugar;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Serialization;
using ParamItem = FNMES.Entity.Param.ParamItem;

namespace FNMES.Entity.DTO.AppData
{
    [DataContract]
    public class EquipmentInfo
    {
        [DataMember]
        //设备代码，大工站、小工站、configID
        public string Name{ get; set; }
        [DataMember]
        public string EquipmentCode{ get; set; }
        [DataMember]
        public string StationCode { get; set; }
        [DataMember]
        public string SmallStationCode { get; set; }
        [DataMember]
        public string ConfigId { get; set; }
    }

    [DataContract]
    public class Permission
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Encode { get; set; }
        [DataMember]
        public string Type { get; set; }
    }

    [DataContract]
    public class UserInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string OperatorNo { get; set; }
        [DataMember]
        public List<string> Roles { get; set; }
        [DataMember]
        public List<Permission> Permissions { get; set; }
    }


    [DataContract]
    public class PlcParam
    {
        
        [DataMember]
        public List<ErrorParam> ErrorParams { get; set; }
        [DataMember]
        public List<StatusParam> StatusParams { get; set; }
        [DataMember]
        public List<string> StopCode { get; set; }
        [DataMember]
        public List<string> StopCodeDesc { get; set; }
    }
    [DataContract]
    public class ErrorParam
    {

        [DataMember]
        public string StationCode { get; set; }
        [DataMember]
        public string SmallStationCode { get; set; }
        [DataMember]
        public string EquipmentID { get; set; }
        [DataMember]
        public List<ErrorAddress> ErrorAddresss { get; set; }
    }

    [DataContract]
    public class ErrorAddress
    {
        [DataMember]
        public int Offset { get; set; }
        [DataMember]
        public string AlarmCode { get; set; }
        [DataMember]
        public string AlarmDesc { get; set; }
    }

    [DataContract]
    public class StatusParam
    {
        [DataMember]
        public string StationCode { get; set; }
        [DataMember]
        public string SmallStationCode { get; set; }
        [DataMember]
        public string EquipmentID { get; set; }
        [DataMember]
        public int Offset { get; set; }
        [DataMember]
        public int StopCodeOffset { get; set; }
    }

    //这个里面有分流器条码
    [DataContract]
    public class TranshipStationBindProcessParam : BaseParam
    {
        [DataMember]
        public string productCode { get; set; } // 内控码
        [DataMember]
        public string palletNo { get; set; } // AGV工装码 
        [DataMember]
        public string stationCode { get; set; }
        [DataMember]
        public string smallStationCode { get; set; }
        [DataMember]
        public string equipmentID { get; set; } // 设备编码（选填）
        [DataMember]
        public string operatorNo { get; set; } // 操作工

        //上面部分为api需要的参数，下面为线体绑定需要的参数
        [DataMember]
        public string TaskOrderNumber { get; set; }
        [DataMember]
        public string ProductPartNo { get; set; }
        [DataMember]
        public string ConfigId { get; set; }
        [DataMember]
        public string Status { get; set; } //
        [DataMember]
        public string RepairFlag { get; set; } //返修标志
        [DataMember]
        public string RepairStations { get; set; } //返修工站
        [DataMember]
        public string ReessNo { get; set; }      //国标码
        [DataMember]
        public string Diverter { get; set; }    //分流器条码
        [DataMember]
        public string GlueTime { get; set; }    //中段涂胶时间
        [DataMember]
        public string CurrentStation { get; set; }  //当前工站
    }
    [DataContract]
    public class BindProcessParam :BaseParam
    {
        //由单机来排序
        [DataMember]
        public string productCode { get; set; } // 内控码
        [DataMember]
        public string position { get; set; } //内控码的位置1-8
        [DataMember]
        public string palletNo { get; set; } // AGV工装码 
        [DataMember]
        public string stationCode { get; set; }
        [DataMember]
        public string smallStationCode { get; set; }
        [DataMember]
        public string equipmentID { get; set; } // 设备编码（选填）
        [DataMember]
        public string operatorNo { get; set; } // 操作工

        //上面部分为api需要的参数，下面为线体绑定需要的参数
        [DataMember]
        public string TaskOrderNumber { get; set; }
        [DataMember]
        public string ProductPartNo { get; set; }
        [DataMember]
        public string ConfigId { get; set; }
        [DataMember]
        public string Status { get; set; } //
        [DataMember]
        public string RepairFlag { get; set; } //返修标志
        [DataMember]
        public string RepairStations { get; set; } //返修工站
    }

    [DataContract]
    public class LabelAndOrderData
    {
        // Pack内控码/Reess码
        [DataMember]
        public string CodeContent { get; set; }
        /// <summary>
        /// 工单
        /// </summary>
        [DataMember]
        public string TaskOrderNumber { get; set; }
        /// <summary>
        /// 配方
        /// </summary>
        [DataMember]
        public string ProductPartNo { get; set; }
        [DataMember]
        public string ConfigId { get; set; }

        [DataMember]
        public string ReessNo { get; set; }      //国标码

        [DataMember]
        public string Diverter { get; set; }    //分流器条码

        [DataMember]
        public string GlueTime { get; set; }    //中段涂胶时间
        [DataMember]
        public string Grade { get; set; }    //档位
    }
    #region 原来的RecipeData
    //public class RecipeData
    //{
    //    public RecipeData()
    //    {
    //        this.paramList = new List<Param>();
    //        this.partList = new List<Part>();
    //        this.stepList = new List<Step>();
    //    }


    //    public RecipeData(ParamRecipeItem paramRecipeItem)
    //    {
    //        this.nextBigStationCode = paramRecipeItem.NextStationCode;
    //        this.isFirstStation = paramRecipeItem.IsFirstStation;
    //        this.productionBeat = paramRecipeItem.ProductionBeat;
    //        this.passStationRestriction = paramRecipeItem.PassStationRestriction;
    //        List<Step> steplist = new List<Step>();
    //        List<Param> paramlist = new List<Param>();
    //        List<Part> partlist = new List<Part>();
    //        foreach (var step in paramRecipeItem.StepList)
    //        {
    //            steplist.Add(new Step()
    //            {
    //                identity = step.Identity,
    //                stepNo = step.StepNo,
    //                stepName = step.StepName,
    //                No = step.No,
    //                stepDesc = step.StepDesc,
    //                operation = step.Operation,
    //                paramList = new List<Param>(),
    //                partList = new List<Part>()
    //            });
    //        }
    //        foreach (var item in paramRecipeItem.ParamList)
    //        {
    //            Param param = ConvertHelper.Mapper<Param, ParamItem>(item);
    //            if (item.StepNo.IsNullOrEmpty() || 0 == int.Parse(item.StepNo))
    //            {
    //                //不存在工步，将参数存入工序参数中
    //                paramlist.Add(param);
    //            }
    //            else
    //            {
    //                //存在工步，将参数添加到对应的工步中
    //                steplist.FirstOrDefault(it => it.stepNo == item.StepNo)?.paramList.Add(param);
    //            }

    //        }
    //        foreach (var item in paramRecipeItem.PartList)
    //        {
    //            Part part = ConvertHelper.Mapper<Part, ParamPartItem>(item);
    //            List<AlternativePartItem> alternative = new List<AlternativePartItem>();
    //            foreach (var item1 in item.AlternativePartList)
    //            {
    //                AlternativePartItem alternativePartItem = ConvertHelper.Mapper<AlternativePartItem, ParamAlternativePartItem>(item1);
    //                alternative.Add(alternativePartItem);
    //            }
    //            part.alternativePartList = alternative;
    //            if (item.StepNo.IsNullOrEmpty() || 0 == int.Parse(item.StepNo))
    //            {
    //                //不存在工步，将参数存入工序参数中
    //                partlist.Add(part);
    //            }
    //            else
    //            {
    //                //存在工步，将参数添加到对应的工步中
    //                steplist.FirstOrDefault(it => it.stepNo == item.StepNo)?.partList.Add(part);
    //            }

    //        }
    //        foreach (var item in paramRecipeItem.EsopList)
    //        {
    //            //存在工步，将参数添加到对应的工步中
    //            var step = steplist.FirstOrDefault(it => it.No == item.No);
    //            if (step != null)
    //            {
    //                step.filePath = item.FilePath;
    //                step.startPageNo = item.StartPageNo;
    //                step.endPageNo = item.EndPageNo;
    //            }
    //        }

    //        this.stepList = steplist;
    //        this.partList = partlist;
    //        this.paramList = paramlist;
    //    }

    //    [DataMember]
    //    // 下一站工序编码
    //    public string nextBigStationCode { get; set; }
    //    [DataMember]
    //    // 是否首道工序
    //    public string isFirstStation { get; set; }
    //    [DataMember]
    //    // 节拍，后段需要，单位秒
    //    public string productionBeat { get; set; }
    //    // 过站限制：进站校验、出站校验、进出站都校验、都不校验
    //    [DataMember]
    //    public string passStationRestriction { get; set; }
    //    [DataMember]
    //    // 工艺参数，工序的参数
    //    public List<Param> paramList { get; set; }
    //    [DataMember]
    //    // bom物料，工序的物料
    //    public List<Part> partList { get; set; }
    //    [DataMember]
    //    //工步
    //    public List<Step> stepList { get; set; }
    //}
    #endregion

    [DataContract]
    public class RecipeData
    {
        public RecipeData()
        {
            this.paramList = new List<Param>();
            this.partList = new List<Part>();
            this.stepList = new List<Step>() ;
            this.reworkstepList = new List<Step>();
        }


        public RecipeData(ParamRecipeItem paramRecipeItem)
        {
            //2025年9月25日 09:19:08
            //nextBigStationCode,isFirstStation,productionBeat,passStationRestriction已作废
            this.nextBigStationCode = paramRecipeItem.NextStationCode; 
            this.isFirstStation = paramRecipeItem.IsFirstStation;
            this.productionBeat = paramRecipeItem.ProductionBeat;
            this.passStationRestriction = paramRecipeItem.PassStationRestriction;
            List<Step> steplist = new List<Step>();
            List<Step> reworksteplist = new List<Step>();
            List<Param> paramlist = new List<Param>();
            List<Part> partlist = new List<Part>();
            foreach (var step in paramRecipeItem.StepList)
            {
                if (step.Identity == "rework")
                {
                    reworksteplist.Add(new Step()
                    {
                        identity = step.Identity,
                        stepNo = step.StepNo,
                        stepName = step.StepName,
                        No = step.No,
                        stepDesc = step.StepDesc,
                        operation = step.Operation,
                        paramList = new List<Param>(),
                        partList = new List<Part>()
                    });
                }
                else
                {
                    steplist.Add(new Step()
                    {
                        identity = step.Identity,
                        stepNo = step.StepNo,
                        stepName = step.StepName,
                        No = step.No,
                        stepDesc = step.StepDesc,
                        operation = step.Operation,
                        paramList = new List<Param>(),
                        partList = new List<Part>()
                    });
                }
               
            }
            foreach (var item in paramRecipeItem.ParamList)
            {
                Param param = ConvertHelper.Mapper<Param, ParamItem>(item);
                if (item.StepNo.IsNullOrEmpty() || 0 == int.Parse(item.StepNo))
                {
                    //不存在工步，将参数存入工序参数中
                    paramlist.Add(param);
                }
                else
                {
                    //存在工步，将参数添加到对应的工步中
                    steplist.FirstOrDefault(it => it.stepNo == item.StepNo)?.paramList.Add(param);
                    reworksteplist?.FirstOrDefault(it => it.stepNo == item.StepNo)?.paramList.Add(param);
                }
                
            }
            foreach (var item in paramRecipeItem.PartList)
            {
                Part part = ConvertHelper.Mapper<Part, ParamPartItem>(item);
                List<AlternativePartItem> alternative = new List<AlternativePartItem>();
                foreach (var item1 in item.AlternativePartList)
                {
                    AlternativePartItem alternativePartItem = ConvertHelper.Mapper<AlternativePartItem,ParamAlternativePartItem>(item1);
                    alternative.Add(alternativePartItem);
                }
                part.alternativePartList = alternative;
                if (item.StepNo.IsNullOrEmpty() || 0 == int.Parse(item.StepNo))
                {
                    //不存在工步，将参数存入工序参数中
                    partlist.Add(part);
                }
                else
                {
                    //存在工步，将参数添加到对应的工步中
                    steplist.FirstOrDefault(it => it.stepNo == item.StepNo)?.partList.Add(part);
                    reworksteplist?.FirstOrDefault(it => it.stepNo == item.StepNo)?.partList.Add(part);
                }
                
            }
            foreach (var item in paramRecipeItem.EsopList)
            {
                //存在工步，将参数添加到对应的工步中
                var step = steplist.FirstOrDefault(it => it.No == item.No);
                if(step != null)
                {
                    step.filePath = item.FilePath;
                    step.startPageNo = item.StartPageNo;
                    step.endPageNo = item.EndPageNo;
                }

                //存在工步，将参数添加到对应的工步中
                var reworkstep = reworksteplist?.FirstOrDefault(it => it.No == item.No);
                if (reworkstep != null)
                {
                    reworkstep.filePath = item.FilePath;
                    reworkstep.startPageNo = item.StartPageNo;
                    reworkstep.endPageNo = item.EndPageNo;
                }
            }

            this.stepList = steplist;
            this.partList = partlist;
            this.paramList  = paramlist;
            this.reworkstepList = reworksteplist;
        }

        [DataMember]
        // 下一站工序编码
        public string nextBigStationCode { get; set; }
        [DataMember]
        // 是否首道工序
        public string isFirstStation { get; set; }
        [DataMember]
        // 节拍，后段需要，单位秒
        public string productionBeat { get; set; }
        // 过站限制：进站校验、出站校验、进出站都校验、都不校验
        [DataMember]
        public string passStationRestriction { get; set; }
        [DataMember]
        // 工艺参数，工序的参数
        public List<Param> paramList { get; set; }
        [DataMember]
        // bom物料，工序的物料
        public List<Part> partList { get; set; }
        [DataMember]
        //工步
        public List<Step> stepList { get; set; }

        [DataMember]
        //返修工步
        public List<Step> reworkstepList { get; set; }

    }

    [DataContract]
    public class Step
    {
        [DataMember]
        // 工步编号
        public string stepNo { get; set; }
        [DataMember]
        // 工步名称
        public string stepName { get; set; }
        [DataMember]
        // 顺序号
        public string No { get; set; }
        [DataMember]
        // 工步描述
        public string stepDesc { get; set; }
        [DataMember]
        // 操作
        public string operation { get; set; }
        [DataMember]
        // 标识
        public string identity { get; set; }
        [DataMember]
        // 工艺参数，工步的参数
        public List<Param> paramList { get; set; }
        [DataMember]
        // bom物料，工步的物料
        public List<Part> partList { get; set; }
        [DataMember]
        // SOP文件存储在文件服务器URL路径
        public string filePath { get; set; }
        [DataMember]
        // 开始页码
        public string startPageNo { get; set; }
        [DataMember]
        // 结束页码
        public string endPageNo { get; set; }


    }




    [DataContract]
    public class Param
    {
        // 工艺参数编码（操作类型编码）
        [DataMember]
        public string paramCode { get; set; }
        [DataMember]
        // 工艺参数名称（操作类型名称）
        public string paramName { get; set; }
        [DataMember]
        // 工艺描述
        public string processDesc { get; set; }
        [DataMember]
        // 参数分类：产品参数、过程参数、设备预设参数（工艺参数）
        public string paramClassification { get; set; }
        [DataMember]
        // 判定类型：数据收集、相机判定、设备判定、取标准值、取上下限判定、取下限判定、取上限判定、无需判定
        public string decisionType { get; set; }
        [DataMember]
        // 参数类型：定性（字符串）：选择定性，选项1-不需要，选项2-不需要，设定值-需要；定量（数值型）：标准值，上限，下限。
        public string paramType { get; set; }
        [DataMember]
        // 工艺参数标准值，针对定量类型的
        public string standValue { get; set; }
        [DataMember]
        // 工艺参数最大值，针对定量类型的
        public string maxValue { get; set; }
        [DataMember]
        // 工艺参数最小值，针对定量类型的
        public string minValue { get; set; }
        [DataMember]
        // 针对定性的设定值
        public string setValue { get; set; }
        [DataMember]
        // 工厂MES是否二次校验
        public string isDoubleCheck { get; set; }
        [DataMember]
        // 单位
        public string uom { get; set; }
    }
    [DataContract]
    public class Part
    {
        [DataMember]
        // 物料编码
        public string partNumber { get; set; }
        [DataMember]
        // 物料描述
        public string partDescription { get; set; }
        [DataMember]
        // 物料版本
        public string partVersion { get; set; }
        [DataMember]
        // 物料类型
        public string partType { get; set; }
        [DataMember]
        // 数量
        public string partQty { get; set; }
        [DataMember]
        // 单位
        public string uom { get; set; }
        [DataMember]
        // 替代物料
        public List<AlternativePartItem> alternativePartList { get; set; }
    }

    //测试仪的响应
    [DataContract]
    public class TestUploadRes {
        [DataMember]
        public long primaryKey;
    }

    [DataContract]
    public class Product{
        [DataMember]
        public string lineId;               //所属线体
        [DataMember]
        public string productPartNo;        //产品编码
        [DataMember]
        public string productDescription;   //产品描述
        [DataMember]
        public string sapCustomerProjNo;    //客户产品代码

    }

    [DataContract]
    public class ProductList
    {
        [DataMember]
        public List<Product> products;   //产品列表

        [DataMember]
        public Product SelectedProduct;
    }

 
}
