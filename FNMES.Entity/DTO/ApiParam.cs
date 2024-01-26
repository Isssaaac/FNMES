using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.DTO.ApiParam
{
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
        // 产品物料号
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

       
    }
    [DataContract]
    public class PartUploadParam:BaseParam
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

        // 操作工
        [DataMember]
        public string operatorNo { get; set; }

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




    public class DisAssembleParam:BaseParam
    {
        // 工位号
        public string stationCode { get; set; }

        // 小工位
        public string smallStationCode { get; set; }

        // 设备编码(选填)
        public string equipmentID { get; set; }

        // 内控码
        public string productCode { get; set; }

        // 拆解原因
        public string disassmblyReason { get; set; }

        // 操作员工号
        public string operatorNo { get; set; }

        // 请求时间(时间戳)
       

        // 拆出的模块信息列表
        public List<Module> moduleList { get; set; }
    }

    public class Module
    {
        // 模块码
        public string moduleCode { get; set; }

        // 模块位置
        public string modulePosition { get; set; }
    }
    public class AssembleParam:BaseParam
    {
        // 工位号
        public string stationCode { get; set; }

        // 小工位
        public string smallStationCode { get; set; }

        // 设备编码(选填)
        public string equipmentID { get; set; }

        // 内控码
        public string productCode { get; set; }

        // 操作员工号
        public string operatorNo { get; set; }

        // 请求时间(时间戳)
        

        // 模块组装电芯信息列表
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

  
    [DataContract]
    public class ReworkParam:BaseParam
    {
        // 返修原因
        [DataMember]
        public string reworkReason { get; set; }

        // 返修工单（非必填）
        [DataMember]
        public string reworkWorkOrder { get; set; }

        // 返修工位号
        [DataMember]
        public string stationCode { get; set; }

        [DataMember]
        // 返修小工位
        public string smallStationCode { get; set; }
        [DataMember]
        // 备注（非必填）
        public string reworkComments { get; set; }

        // 员工号
        [DataMember]
        public string operatorNo { get; set; }

        // 请求时间（时间戳）

        [DataMember]
        // 返修的产品清单
        public List<ProductInfo> productList { get; set; }
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

    }
    [DataContract]
    public class AndonTypeParam : BaseParam
    {
        // 操作员工号
        [DataMember]
        public string operatorNo { get; set; }

    }








    [DataContract]
    public class ToolInfo
    {
        // 夹治具变化
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

}
