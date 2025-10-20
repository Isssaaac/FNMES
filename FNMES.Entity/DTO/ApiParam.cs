using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ServiceStack;
using Newtonsoft.Json;
using System.Security.Policy;
using System.Web;

namespace FNMES.Entity.DTO.ApiParam
{
    //与工厂MES交互时传递的参数数据
    [DataContract]
    public class BaseParam
    {
        [DataMember]
        public string callTime
        {
            get
            { return DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(); }

            set { }
            //get; set;
        }
        [DataMember]
        public string productionLine { get;set; }


        public BaseParam CopyField(params object[] sources)     //把源数据字段复制到本体
        {
            foreach (var source in sources)
            {
                foreach (var sorfield in source.GetType().GetProperties())
                {
                    foreach (var tarfield in this.GetType().GetProperties())
                    {
                        if (sorfield.Name.ToLower() == tarfield.Name.ToLower() && sorfield.PropertyType.Name == tarfield.PropertyType.Name)
                        {
                            if (!sorfield.PropertyType.Name.Contains("List"))
                            {
                                tarfield.SetValue(this, sorfield.GetValue(source));
                            }
                        }
                    }
                }
            }
            return this;
        }
    }
    [DataContract]
    public class SimpleParam
    {
        [DataMember]
        public string productionLine { get; set; }
        public SimpleParam CopyField(params object[] sources)     //把源数据字段复制到本体
        {
            foreach (var source in sources)
            {
                foreach (var sorfield in source.GetType().GetProperties())
                {
                    foreach (var tarfield in this.GetType().GetProperties())
                    {
                        if (sorfield.Name.ToLower() == tarfield.Name.ToLower() && sorfield.PropertyType.Name == tarfield.PropertyType.Name)
                        {
                            if (!sorfield.PropertyType.Name.Contains("List"))
                            {
                                tarfield.SetValue(this, sorfield.GetValue(source));
                            }
                        }
                    }
                }
            }
            return this;
        }
    }


    [DataContract,Serializable]
    public class HeartbeatParam:BaseParam
    {
        //线体MES系统编号
        [DataMember]
        public string sourceSys { get; set; }
        [DataMember]
        public string ipAddress { get; set; }
    }
    [DataContract]
    public class LoginParam:BaseParam
    {
        [DataMember]
        // 员工卡号（用于工位刷卡）
        public string cardNo { get; set; }
        [DataMember]
        // 员工工号（用于无卡刷工位）
        public string operatorNo { get; set; }
        //员工密码
        [DataMember]
        public string password { get; set; }
        [DataMember]
        // 工序（固定值），参考2.4章节
        public string stationCode { get; set; }
        [DataMember]
        // 设备编码
        public string equipmentID { get; set; }
        //读卡器ID
        [DataMember]
        public string cardReaderID { get; set; }

    }
    [DataContract]
    public class GetOrderParam:BaseParam
    {
       
        [DataMember]
        public string stationCode { get; set; }
        [DataMember]
        public string operatorNo { get; set; }
    }
    [DataContract]
    public class SelectOrderParam:SimpleParam
    {
        [DataMember]
        //派工单号
        public List<SelectOrder> taskOrderNumbers { get; set; }
        [DataMember]
        //工序（固定值）
        public string stationCode { get; set; }
        [DataMember]
        //设备编码
        public string equipmentID { get; set; }
        [DataMember]

        //操作员工号
        public string operatorNo { get; set; }
        [DataMember]
        public string actualStartTime { get; set; }
    }
    [DataContract]
    public class SelectOrder
    {
        [DataMember]
        public string taskOrderNumber { get; set; }
        [DataMember]
        // S 开工，P 暂定，C 取消
        public string actionCode { get; set; }
    }

    //工单动作，默认回传的是D
    public static class ActionCode
    {
        public const string Start = "S";
        public const string Pause = "P";
        public const string Cancel = "C";
        public const string Received = "D";
        public const string Finish = "F";
    }






    [DataContract]
    public class GetRecipeParam : SimpleParam
    {
        [DataMember]
        // 产品型号
        public string productPartNo { get; set; }
        [DataMember]
        // 工位
        public string stationCode { get; set; }
        [DataMember]
        // 小工位
        public string smallStationCode { get; set; }
        [DataMember]
        // 工段
        public string section { get; set; }
        [DataMember]
        // 设备编码
        public string equipmentID { get; set; }
        [DataMember]
        // 操作员工号
        public string operatorNo { get; set; }
        [DataMember]
        // 实际开始时间（时间戳）
        public string actualStartTime { get ; set ;}
    }
    [DataContract]
    public class GetLabelParam:BaseParam
    {
        // 工位号
        [DataMember]
        public string stationCode { get; set; }

        // 小工位（选填）
        [DataMember]
        public string smallStationCode { get; set; }
        //工单号
        [DataMember]
        public string taskOrderNumber { get; set; }

        // 请求码类型，用于区分内控码，REESS码等
        [DataMember]
        public string requestCodeType { get; set; }

        // 请求REESS码的时候需填内控码
        [DataMember]
        public string productCode { get; set; }
        // 厂别代码：用数字表示，01~09，01代表PACK一工厂(内控码使用)
        [DataMember]
        public string plantCode { get; set; }
        // 操作工
        [DataMember]
        public string operatorNo { get; set; }
    }
    [DataContract]
    public class InStationParam:BaseParam
    {
        // 内控码
        [DataMember]
        public string productCode { get; set; }
        [DataMember]
        public string palletNo { get; set; } // AGV工装码

        // 进站电芯状态（合格状态、不合格状态、返修状态）
        [DataMember]
        public string productStatus { get; set; }

        // 工位号
        [DataMember]
        public string stationCode { get; set; }

        // 小工位
        [DataMember]
        public string smallStationCode { get; set; }

        // 设备编码（选填）
        [DataMember]
        public string equipmentID { get; set; }

        // 操作工
        [DataMember]
        public string operatorNo { get; set; }

        //型号
        [DataMember]
        public string ProductPartNo { get; set; }
    }
    [DataContract]
    public class PartUploadParam:BaseParam
    {
        // 内控码
        [DataMember]
        public string productCode { get; set; }
        [DataMember]
        public string GUID { get; set; }
        // 工位号
        [DataMember]
        public string stationCode { get; set; }

        // 小工位
        [DataMember]
        public string smallStationCode { get; set; }

        // 设备编码（选填）
        [DataMember]
        public string equipmentID { get; set; }

        // 操作工
        [DataMember]
        public string operatorNo { get; set; }

        #region 20240408增加
        //预装件条码（M305 FPC预装或M306喷胶贴泡棉预装件必填） 
        [DataMember]
        public string semBarcode { get; set; }

        [DataMember]
        public string usageQty { get; set; }

        [DataMember]
        public string uom { get; set; }

        #endregion
        // 请求时间(时间戳)


        // 零件列表
        [DataMember]
        public List<Part> partList { get; set; }
    }

    [DataContract]
    public class Part
    {
        // 物料号 
        [DataMember]
        public string partNumber { get; set; }

        // 物料描述
        [DataMember]
        public string partDescription { get; set; }

        // 物料条码
        [DataMember]
        public string partBarcode { get; set; }
        // 类型
        [DataMember]
        public string traceType { get; set; }
        [DataMember]
        public string usageQty { get; set; }
        [DataMember]
        public string uom { get; set; }
    }

    [DataContract]
    public class ProcessUploadParam:BaseParam
    {
        // 内控码
        [DataMember]
        public string productCode { get; set; }

        // 工位号
        [DataMember]
        public string stationCode { get; set; }

        // 小工位
        [DataMember]
        public string smallStationCode { get; set; }

        // 设备编码（选填）
        [DataMember]
        public string equipmentID { get; set; }

        // 程序编号
        [DataMember]
        public string recipeNo { get; set; }

        // 程序名称
        [DataMember]
        public string recipeDescription { get; set; }

        // 程序版本
        [DataMember]
        public string recipeVersion { get; set; }

        // 检测最终结果
        [DataMember]
        public string totalFlag { get; set; }

        // 操作工
        [DataMember]
        public string operatorNo { get; set; }

        // 请求时间(时间戳)


        // 检测参数列表
        [DataMember]
        public List<Process> processData { get; set; }
    }

    [DataContract]
    public class Process
    {
        [DataMember]
        // 参数名称（检验项）
        public string paramCode { get; set; }
        [DataMember]
        // 参数名称（检验项）
        public string paramName { get; set; }
        [DataMember]
        // 参数值
        public string paramValue { get; set; }
        [DataMember]
        // 检测结果 OK/NG
        public string itemFlag { get; set; }

        //2024.5.13 根据孚能要求，在过程数据中需要包含判断标准、判定方式、单位显示

        // 判定类型
        [DataMember]
        public string DecisionType { get; set; }

        // 参数类型：定性（字符串）定量（数值型）：标准值，上限，下限。
        [DataMember]
        public string ParamType { get; set; }

        // 工艺参数标准值，针对定量类型的
        [DataMember]
        public string StandValue { get; set; }

        // 工艺参数最大值，针对定量类型的
        [DataMember]
        public string MaxValue { get; set; }

        // 工艺参数最小值，针对定量类型的
        [DataMember]
        public string MinValue { get; set; }

        // 针对定性的设定值
        [DataMember]
        public string SetValue { get; set; }

        // 单位
        [DataMember]
        public string UnitOfMeasure { get; set; }
    }

    [DataContract]
    public class ProcessUploadParamA : BaseParam
    {
        // 内控码
        [DataMember]
        public string productCode { get; set; }

        [DataMember]
        public string GUID { get; set; }
        // 工位号
        [DataMember]
        public string stationCode { get; set; }

        // 小工位
        [DataMember]
        public string smallStationCode { get; set; }

        // 设备编码（选填）
        [DataMember]
        public string equipmentID { get; set; }

        // 程序编号
        [DataMember]
        public string recipeNo { get; set; }

        // 程序名称
        [DataMember]
        public string recipeDescription { get; set; }

        // 程序版本
        [DataMember]
        public string recipeVersion { get; set; }

        // 检测最终结果
        [DataMember]
        public string totalFlag { get; set; }

        // 操作工
        [DataMember]
        public string operatorNo { get; set; }

        // 请求时间(时间戳)


        // 检测参数列表
        [DataMember]
        public List<ProcessA> processData { get; set; }
    }

    [DataContract]
    public class ProcessA
    {
        [DataMember]
        // 参数名称（检验项）
        public string paramCode { get; set; }
        [DataMember]
        // 参数名称（检验项）
        public string paramName { get; set; }
        [DataMember]
        // 参数值
        public string paramValue { get; set; }
        [DataMember]
        // 检测结果 OK/NG
        public string itemFlag { get; set; }

        [DataMember]
        public string maxValue {  get; set; }

        [DataMember]
        public string minValue { get; set; }

        [DataMember]
        public string standardValue { get; set; }

        [DataMember]
        public string decisionType { get; set; }


    }

    [DataContract]
    public class OutStationParam:BaseParam
    {
        // 内控码
        [DataMember]
        public string productCode { get; set; }

        // 派工单号
        [DataMember]
        public string taskOrderNumber { get; set; }

        // 出站电芯状态（合格状态、不合格状态、返修状态）
        [DataMember]
        public string productStatus { get; set; }

        [DataMember]
        public List<DefectItem> defectList { get; set; }

        // 工位号
        [DataMember]
        public string stationCode { get; set; }

        // 小工位
        [DataMember]
        public string smallStationCode { get; set; }

        // 设备编码（选填）
        [DataMember]
        public string equipmentID { get; set; }

        // 操作工
        [DataMember]
        public string operatorNo { get; set; }

        //2024.5.13添加
        //进站时间
        [DataMember]
        public string instationTime { get; set; }

        [DataMember]
        public string palletNo { get; set; }
    }

    [DataContract]
    public class OutStationParamA : BaseParam
    {
        // 内控码
        [DataMember]
        public string productCode { get; set; }

        // 派工单号
        [DataMember]
        public string taskOrderNumber { get; set; }

        // 出站电芯状态（合格状态、不合格状态、返修状态）
        [DataMember]
        public string productStatus { get; set; }

        [DataMember]
        public List<DefectItem> defectList { get; set; }

        // 工位号
        [DataMember]
        public string stationCode { get; set; }

        // 小工位
        [DataMember]
        public string smallStationCode { get; set; }

        // 设备编码（选填）
        [DataMember]
        public string equipmentID { get; set; }

        // 操作工
        [DataMember]
        public string operatorNo { get; set; }

    }


    [DataContract]
    public class DefectItem
    {
        // 不良代码
        [DataMember]
        public string defectCode { get; set; }

        // 不良描述
        [DataMember]
        public string defectDesc { get; set; }
    }



    [DataContract]
    public class DisAssembleParam:BaseParam
    {

        [DataMember]
        public string GUID { get; set; }
        // 工位号
        [DataMember]
        public string stationCode { get; set; }

        // 小工位
        [DataMember]
        public string smallStationCode { get; set; }

        // 设备编码(选填)
        [DataMember]
        public string equipmentID { get; set; }

        // 内控码
        [DataMember]
        public string productCode { get; set; }

        // 操作员工号
        [DataMember]
        public string operatorNo { get; set; }

        // 请求时间(时间戳)


        // 拆出的模块信息列表
        [DataMember]
        public List<Material> partList { get; set; }
    }

    [DataContract]
    public class Material
    {
        [DataMember]
        public string partNumber { get; set; } //物料号
        [DataMember]
        public string partDescription { get; set; } //物料描述

        [DataMember]
        public string partBarcode { get; set; } //物料条码

        [DataMember]
        public string reason { get; set; } //解绑原因
    }

    [DataContract]
    public class Module
    {
        // 模块码
        [DataMember]
        public string moduleCode { get; set; }

        // 模块位置
        [DataMember]
        public string lineNo { get; set; }
        [DataMember]
        public string columnNo { get; set; }
        [DataMember]
        public string layerNo { get; set; }
    }
    [DataContract]
    public class AssembleParam:BaseParam
    {
        // 工位号
        [DataMember]
        public string stationCode { get; set; }

        // 小工位
        [DataMember]
        public string smallStationCode { get; set; }

        // 设备编码(选填)
        [DataMember]
        public string equipmentID { get; set; }

        // 内控码
        [DataMember]
        public string productCode { get; set; }

        // 操作员工号
        [DataMember]
        public string operatorNo { get; set; }

        // 请求时间(时间戳)


        // 模块组装电芯信息列表
        [DataMember]
        public List<Module> moduleList { get; set; }
    }
    [DataContract]
    public class EquipmentStateParam:BaseParam
    {
        [DataMember]
        // 设备编码
        public string equipmentID { get; set; }
        [DataMember]
        // 工位
        public string stationCode { get; set; }
        [DataMember]
        public string smallStationCode { get; set; }
        [DataMember]
        // 设备状态
        public string equipmentStatus { get; set; }
        [DataMember]
        // 描述
        public string statusDescription { get; set; }

       
    }

    [DataContract]
    public class EquipmentErrorParam : SimpleParam
    {
        // 工位
        [DataMember]
        public string stationCode { get; set; }
        // 工位
        [DataMember]
        public string smallStationCode { get; set; }

        // 设备编码
        [DataMember]
        public string equipmentID { get; set; }

        // 报警信息列表
        [DataMember]
        public List<AlarmInfo> alarmList { get; set; }
    }
    [DataContract]
    public class AlarmInfo:BaseParam
    {
        [DataMember]
        // 开始或复位状态 (1:开始, 0:复位)
        public string status { get; set; }
        [DataMember]
        // 报警代码
        public string alarmCode { get; set; }
        [DataMember]
        // 报警描述
        public string alarmDesc { get; set; }

        // 触发时间（时间戳）
       
    }
    [DataContract]
    public class EquipmentStopParam:SimpleParam
    {
        // 工位
        [DataMember]
        public string stationCode { get; set; }
        [DataMember]
        public string smallStationCode { get; set; }

        [DataMember]
        // 设备编码
        public string equipmentID { get; set; }

        // 员工号
        [DataMember]
        public string operatorNo { get; set; }

        // 停机代码，参考2.6章节
        [DataMember]
        public string stopCode { get; set; }

        // 停机描述
        [DataMember]
        public string stopDesc { get; set; }

        // 停机时间（时间戳）
        [DataMember]
        public string stopTime { get; set; }

        // 停机时长（ms）
        [DataMember]
        public string stopDurationTime { get; set; }
    }


    //[DataContract]
    //public class ReworkParam:BaseParam
    //{
    //    // 返修原因
    //    [DataMember]
    //    public string reworkReason { get; set; }

    //    // 返修工单（非必填）
    //    [DataMember]
    //    public string reworkWorkOrder { get; set; }

    //    // 返修工位号
    //    [DataMember]
    //    public string stationCode { get; set; }

    //    [DataMember]
    //    // 返修小工位
    //    public string smallStationCode { get; set; }
    //    [DataMember]
    //    // 备注（非必填）
    //    public string reworkComments { get; set; }

    //    // 员工号
    //    [DataMember]
    //    public string operatorNo { get; set; }

    //    // 请求时间（时间戳）

    //    [DataMember]
    //    // 返修的产品清单
    //    public List<ProductInfo> productList { get; set; }
    //}

    [DataContract]
    public class ReworkParam : BaseParam
    {
        //返修原因
        [DataMember]
        public string reworkReason { get; set; }

        //返修工单，非必填
        [DataMember]
        public string reworkWorkOrder { get; set; }

        //返修工位号
        [DataMember]
        public string stationCode { get; set; }

        //返修小工位
        [DataMember]
        public string smallStationCode { get; set;}

        //备注，非必填
        [DataMember]
        public string reworkComments { get; set;}

        //员工号
        [DataMember]
        public string operatorNo { get; set;}

        //返修产品清单
        [DataMember]
        public List<ProductList> productList { get; set;}
    }

    [DataContract]
    public class ProductList
    {
        //PACK/Module/Cell
        [DataMember]
        public string productType { get; set; }

        [DataMember]    
        public string productCode { get; set; }

        [DataMember]
        public string productStatus { get; set; }

        //替换进的物料编码，换料返修时，此时不需要调用“追溯件信息上传接口”
        [DataMember]
        public List<PartList> partList { get; set; }
    }

    [DataContract]
    public class PartList
    {
        //旧物料编码 
        [DataMember]
        public string oldPartNumber { get; set; }

        //换入的物料编码
        [DataMember]
        public string newPartNumber { get; set;}

        //物料描述
        [DataMember]
        public string partDescription { get; set;}

        //数量
        [DataMember]
        public string partVersion { get; set;}

        //换入的物料的批次号或SN
        [DataMember]
        public string partQty { get; set;}

        //单位
        [DataMember]
        public string batchOrSN { get; set;}

        [DataMember]    
        public string uom { get; set;}  
    }

    [DataContract]
    public class ToolRemainParam:BaseParam
    {
        // 工位号
        [DataMember]
        public string smallStationCode { get; set; }
        [DataMember]
        public string stationCode { get; set; }
        [DataMember]
        public string productCode { get; set; }
        [DataMember]
        public string equipmentID { get; set; }
        // 员工号
        [DataMember]
        public string operatorNo { get; set; }
        // 夹治具清单
        [DataMember]
        public List<ToolInfo> toolList { get; set; }
    }
    [DataContract]
    public class GetPackInfoParam:BaseParam
    {
        [DataMember]
        public string palletNo { get; set; } // AGV工装码
        [DataMember]
        public string stationCode { get; set; } // 工位号
        [DataMember]
        public string smallStationCode { get; set; } // 小工位（选填）
        [DataMember]
        public string equipmentID { get; set; } // 设备编码（选填）
        [DataMember]
        public string operatorNo { get; set; } // 操作工
    }

    public class GetSopParam:BaseParam
    {
        public string productPartNo { get; set; } // 产品物料号，从当前生产的工单中获取
        public string operatorNo { get; set; } // 操作工
    }
    [DataContract]
    public class BindPalletParam:BaseParam
    {
        [DataMember]
        public string productCode { get; set; } // 内控码
        [DataMember]
        public string palletNo { get; set; } // AGV工装码 
        [DataMember]
        public string stationCode { get; set; } // 工位号
        [DataMember]
        public string smallStationCode { get; set; } // 小工位（选填）
        [DataMember]
        public string equipmentID { get; set; } // 设备编码（选填）
        [DataMember]
        public string operatorNo { get; set; } // 操作工
    }
    //安灯系统
    [DataContract]
    public class AndonParam:BaseParam
    {
        // 设备编码（选填）
        [DataMember]
        public string equipmentID { get; set; }
        // 工位
        [DataMember]
        public string stationCode { get; set; }

        // 小工位
        [DataMember]
        public string smallStationCode { get; set; }

        // P：班组长呼叫/E：设备呼叫/M：物料呼叫
        [DataMember]
        public string andonType { get; set; }
        //名称
        [DataMember]
        public string andonName { get; set; }
        //编码
        [DataMember]
        public string andonCode { get; set; }
        //优先级  高、低枚举
        [DataMember]
        public string priority { get; set; }
        //物料号   物料呼叫时填
        [DataMember]
        public string partNumber { get; set; }

        // ANDON呼叫描述
        [DataMember]
        public string andonDescription { get; set; }

        // 触发：Tigger/响应：Response/复位Reset；响应状态选填
        [DataMember]
        public string actionType { get; set; }
        //消息ID  复位时必填       刚才触发的消息ID
        [DataMember]
        public string andonNoticeCode { get; set; }
        [DataMember]
        // 操作员工号
        public string operatorNo { get; set; }
        //241205新增
        [DataMember]
        public string materialQty { get; set; }
        [DataMember]
        public string destination { get; set; }

    }
    [DataContract]
    public class AndonTypeParam : BaseParam
    {
        // 操作员工号
        [DataMember]
        public string operatorNo { get; set; }

        [DataMember]
        public string callTime { get; set; }

        //[DataMember]
        //public string groupName { get; set; }

    }








    [DataContract]
    public class ToolInfo
    {
        // 夹治具类型
        [DataMember]
        public string toolType { get; set; } = "1";

        // 夹治具编号
        [DataMember]
        public string toolNo { get; set; }

        // 夹治具名称
        [DataMember]
        public string toolName { get; set; }

        // 夹治具剩余寿命
        [DataMember]
        public string toolRemainValue { get; set; }

        // 寿命单位
        [DataMember]
        public string uom { get; set; }
    }
    [DataContract]
    public class ProductInfo
    {
        [DataMember]
        // 产品类型（PACK/模组/电芯返修时）
        public string productType { get; set; }
        [DataMember]
        // 产品码（PACK码/模组码/电芯码返修时）
        public string productCode { get; set; }
        [DataMember]
        public string productStatus { get; set; }
        [DataMember]
        public List<RePart> partList { get; set; }
    }
    [DataContract]
    public class RePart
    {
        [DataMember]
        public string oldPartNumber { get; set; }
        [DataMember]
        public string newPartNumber { get; set; }
        [DataMember]
        public string partDescription { get; set; }
        [DataMember]
        public string partVersion { get; set; }
        [DataMember]
        public string partQty { get; set; }
        [DataMember]
        public string batchOrSN { get; set; }
        [DataMember]
        public string uom { get; set; }
    }

    public class  QualityStopParam:BaseParam
    {
        public string stationCode { get; set; }
        public string equipmentID { get; set; }
        public string smallStationCode { get; set; }
        public string kpiCode { get; set; }
        public string operatorNo { get; set; }
    }



    public class OrderAction
    {
        public const string START = "S";
        public const string PAUSE = "P";
        public const string CANCEL = "C";
    }

    public class AlarmStatus
    {
        public const string START = "1";
        public const string END = "0";
    }
    public class LabelType
    {
        public const string PACK = "packNo";
        public const string END = "reessNo";
    }

    /**************************************瑞浦****************************************/
    public class MesRet
    {
        public string code;
        public string msg;
        public string data;
    }

    public class ResultRet
    {
        public string result;
        public string message;
    }

    public class GetItemDataParam
    {
        public string resource_no;    //资源编号->小工站
        public string sfc;            //产品条码->ProductCode
        public string operation_no;   //工序->大工站
        public GetItemDataParam(InStationParam param)
        {
            operation_no = param.stationCode;
            sfc = param.productCode;
            resource_no = param.smallStationCode;
        }
    }

    public class GetItemDataRet: ResultRet
    {
        public string sfc;
        public string shoporder;
        public string tech;
        public string qty;
    }

    //上传过程数据
    public class UploadData_FParam
    {
        public string resource_no;    //资源编号
        public string operation_no;   //工序
        public string sfc;            //产品条码
        public string cz_date;        //操作时间（格式：yyyy-MM-dd HH:mm:ss）
        public string cz_user;        //操作人（没有为””）
        public string flag;           //判定结果（OK/NG）
        public string ng_code;        //不良代码（多个不良代码，请用”,”隔开）
        public string json_data;
        public UploadData_FParam(OutStationParam param, List<Process> process, List<string> ngCodes)
        {
            sfc = param.productCode;
            resource_no = param.smallStationCode;
            operation_no = param.stationCode;
            cz_date = DateTime.Now.ToString();
            cz_user = param.operatorNo;
            flag = ngCodes.Count > 0 ? "NG" : "OK";
            ng_code = ngCodes.Join(",");

            var dictionary = new Dictionary<string, string>();
            foreach (var item in process)
                dictionary[item.paramCode] = item.paramValue;
            json_data = JsonConvert.SerializeObject(dictionary);
        }
    }

    public class UploadData_FRet : ResultRet
    {
        public string SFC;
    }

    ////绑定模组或Pack，并且上传过程数据
    public class UploadData_MZParam
    {
        public string sfc;           //模组或PACK条码
        public string resource_no;   //资源编号
        public string operation_no;  //工序
        public string cz_date;       //操作时间（格式：yyyy-MM-dd HH:mm:ss）
        public string cz_user;       //操作人（没有为””）
        public string flag;          //判定结果（OK/NG）
        public string ng_code;       //不良代码（多个不良代码，请用”,”隔开）
        public string item_no;       //电芯或模组条码（多个，请用”,”隔开）首站为空
        public string shop_order;    //工单号
        public string json_data;
        public UploadData_MZParam(OutStationParam param, List<Process> process, List<string> ngCodes, List<BindProduct> bindProducts)
        {
            sfc = param.productCode;
            resource_no = param.smallStationCode;
            operation_no = param.stationCode;
            cz_date = DateTime.Now.ToString();
            cz_user = param.operatorNo;
            flag = ngCodes.Count > 0 ? "NG" : "OK";
            ng_code = ngCodes.Join(",");
            item_no = bindProducts.Select(it => it.productCode).ToList().ToString();
            shop_order = param.taskOrderNumber;

            var dictionary = new Dictionary<string, string>();
            foreach (var item in process)
                dictionary[item.paramCode] = item.paramValue;
            json_data = JsonConvert.SerializeObject(dictionary);
        }
    }

    public class UploadData_MZRet: ResultRet
    {
    }

    public class BindProduct
    {
        public string productCode;
        public string position;
    }

    //上传物料
    public class UpAssembleDataParam
    {
        public string sfc;    //产品条码
        public string barcode; //材料批次号
        public string resource_no; //资源编号
        public string operation_no;//工序
        public string cz_date; //操作时间（格式：yyyy-MM-dd HH:mm:ss）
        public string cz_user; //操作人
        public string shop_order;  //工单号
        public string qty; //数量
        public UpAssembleDataParam(OutStationParam param, Part part)
        {
            sfc = param.productCode;
            barcode = part.partBarcode;
            resource_no = param.smallStationCode;
            operation_no = param.stationCode;
            cz_date = DateTime.Now.ToString();
            cz_user = param.operatorNo;
            shop_order = param.taskOrderNumber;
            qty = part.usageQty;
        }
    }

    public class UpAssembleDataRet : ResultRet
    {

    }

    
    //用户登陆
    public class UserLoginInfoParam
    {
        public string resource_no;
        public string password;
        public string user_id;
    }

    //设备状态
    public class UploadData_SParam
    {
        public string resource_no;  //资源编号
        public string cz_date;      //时间（格式：yyyy-MM-dd HH:mm:ss）
        public string status;       //设备状态（A:工作，B:待机，C:故障，D:关机）
        public string operation_no; //工序
        public string json_data;    //参数值（按数据字典要求进行参数数据上传，注意区分大小写）
        public UploadData_SParam(EquipmentState param)
        {
            resource_no = param.smallStationCode;
            cz_date = DateTime.Now.ToString();
            operation_no = param.stationCode;
            status = param.equipmentStatus;
        }
    }

    //设备报警
    public class UploadData_WParam
    {
        public string resource_no;  //资源编号
        public string cz_date;      //时间（格式：yyyy-MM-dd HH:mm:ss）
        public string operation_no; //工序
        public string json_data;
        public UploadData_WParam(EquipmentErrorParam param)
        {
            resource_no = param.smallStationCode;
            cz_date= DateTime.Now.ToString();
            operation_no= param.stationCode;
        }
    }

    //获取电芯信息
    public class GetSfcInfoParam
    {
        public string resource_no;  //资源编号
        public string operation_no; //工序
        public string sfc;          //电芯条码
        public string json_data;
    }

    public class GetSfcInfoRet:ResultRet
    {
        public GetSfcInfoData Data;
    }

    public class GetSfcInfoData
    {
        public string Resistance;   //电阻
        public string Capacity;     //电容
        public string LastOCVDate;  //O2测试时间
        public string Voltage;      //电压
        public string grade;        //档位
    }

    public class cellInfoParam
    {
        public string productCode;  //电芯条码
        public string lastOCVDate;  //O2测试时间
        public string voltage;      //电压
        public string grade;        //档位
    }


    //一键点检
    public class GetCheckMaitenanceParam
    {
        public string operation_no;
        public string resource_no;
        public string cz_date;
        public string cz_user;
        public string cz_class;
        public List<CheckMaitenanceItems> json_data;
    }

    public class CheckMaitenanceItems
    {
        public string function;
        public string actual_value;
        public string specification_value;
        public string check_result;
        public string check_status;
        public string response_plan;
        public string check_date;
        public string check_number;
    }
    public class GetCheckMaitenanceRet : ResultRet
    {

    }
}
