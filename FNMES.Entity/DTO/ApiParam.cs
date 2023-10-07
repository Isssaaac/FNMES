using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.DTO.ApiParam
{

    public class BaseParam
    {
        public string callTime
        {
            get
            {
                if (string.IsNullOrEmpty(callTime))
                {
                    long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    // 将时间戳转换为字符串
                    callTime = currentTimestamp.ToString();
                }
                return callTime;
            }
            set
            {
            }
        }
    }


    
    public class HeartbeatParam:BaseParam
    {
        //线体MES系统编号
        public string sourceSys { get; set; }
        public string ipAddress { get; set; }
    }
    public class LoginParam:BaseParam
    {
        // 员工卡号（用于工位刷卡）
        public string cardNo { get; set; }

        // 员工工号（用于无卡刷工位）
        public string operatorNo { get; set; }
        //员工密码
        public string operatorPsw { get; set; }

        // 工序（固定值），参考2.4章节
        public string bigStationCode { get; set; }

        // 设备编码
        public string equipmentID { get; set; }

    }

    public class GetOrderParam
    {
        public string productionLine { get; set; }
        public string bigStationCode { get; set; }
    }

    public class SelectOrderParam
    {
        //派工单号
        public string taskOrderNumber { get; set; }
        //工序（固定值）
        public string bigStationCode { get; set; }
        //设备编码
        public string equipmentID { get; set; }
        // S 开工，P 暂定，C 取消
        public string actionCode { get; set; }
        //操作员工号
        public string operatorNo { get; set; }
        public string actualStartTime { get; set; }
    }

    public class GetRecipeParam
    {
        // 产品物料号
        public string productPartNo { get; set; }

        // 工位
        public string bigStationCode { get; set; }

        // 小工位
        public string stationCode { get; set; }

        // 设备编码
        public string equipmentID { get; set; }

        // 操作员工号
        public string operatorNo { get; set; }

        // 实际开始时间（时间戳）
        public string actualStartTime
        {
            get
            {
                if (string.IsNullOrEmpty(actualStartTime))
                {
                    long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    // 将时间戳转换为字符串
                    actualStartTime = currentTimestamp.ToString();
                }
                return actualStartTime;
            }
            set
            {
            }
        }
    }

    public class GetLabelParam:BaseParam
    {
        // 工位号
        public string bigStationCode { get; set; }

        // 小工位（选填）
        public string stationCode { get; set; }

        // 产品物料号，从当前生产的工单中获取
        public string productPartNo { get; set; }

        // 请求码类型，用于区分内控码，REESS码等
        public string requestCodeType { get; set; }

        // 请求REESS码的时候需填内控码
        public string productCode { get; set; }

        // 操作工
        public string operatorNo { get; set; }

        // 请求时间(时间戳)
    }

    public class InStationParam:BaseParam
    {
        // 内控码
        public string productCode { get; set; }

        // 进站电芯状态（合格状态、不合格状态、返修状态）
        public string productStatus { get; set; }

        // 工位号
        public string bigStationCode { get; set; }

        // 小工位
        public string stationCode { get; set; }

        // 设备编码（选填）
        public string equipmentID { get; set; }

        // 操作工
        public string operatorNo { get; set; }

       
    }

    public class PartUploadParam:BaseParam
    {
        // 内控码
        public string productCode { get; set; }

        // 工位号
        public string bigStationCode { get; set; }

        // 小工位
        public string stationCode { get; set; }

        // 设备编码（选填）
        public string equipmentID { get; set; }

        // 操作工
        public string operatorNo { get; set; }

        // 请求时间(时间戳)
      

        // 零件列表
        public List<Part> partList { get; set; }
    }

    public class Part
    {
        // 物料号 
        public string partNumber { get; set; }

        // 物料描述
        public string partDescription { get; set; }

        // 物料条码
        public string partBarcode { get; set; }
    }

    public class ProcessUploadParam:BaseParam
    {
        // 内控码
        public string productCode { get; set; }

        // 工位号
        public string bigStationCode { get; set; }

        // 小工位
        public string stationCode { get; set; }

        // 设备编码（选填）
        public string equipmentID { get; set; }

        // 程序编号
        public string recipeNo { get; set; }

        // 程序名称
        public string recipeDescription { get; set; }

        // 程序版本
        public string recipeVersion { get; set; }

        // 检测最终结果
        public string totalFlag { get; set; }

        // 操作工
        public string operatorNo { get; set; }

        // 请求时间(时间戳)
       

        // 检测参数列表
        public List<Process> processData { get; set; }
    }

    public class Process
    {
        // 参数名称（检验项）
        public string paramName { get; set; }

        // 参数值
        public string paramValue { get; set; }

        // 检测结果 OK/NG
        public string itemFlag { get; set; }
    }

    public class OutStationParam:BaseParam
    {
        // 内控码
        public string productCode { get; set; }

        // 派工单号
        public string taskOrderNumber { get; set; }

        // 出站电芯状态（合格状态、不合格状态、返修状态）
        public string productStatus { get; set; }

        // 不良代码
        public string defectCode { get; set; }

        // 不良描述
        public string defectDesc { get; set; }

        // 工位号
        public string bigStationCode { get; set; }

        // 小工位
        public string stationCode { get; set; }

        // 设备编码（选填）
        public string equipmentID { get; set; }

        // 操作工
        public string operatorNo { get; set; }

        // 请求时间(时间戳)
        
    }


    public class DisAssembleParam:BaseParam
    {
        // 工位号
        public string bigStationCode { get; set; }

        // 小工位
        public string stationCode { get; set; }

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
        public string bigStationCode { get; set; }

        // 小工位
        public string stationCode { get; set; }

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

    public class EquipmentStateParam:BaseParam
    {
        // 设备编码
        public string equipmentID { get; set; }

        // 工位
        public string bigStationCode { get; set; }

        // 设备状态
        public string equipmentStatus { get; set; }

        // 描述
        public string statusDescription { get; set; }

       
    }

    
    public class EquipmentErrorParam
    {
        // 工位
        public string bigStationCode { get; set; }

        // 设备编码
        public string equipmentID { get; set; }

        // 报警信息列表
        public List<AlarmInfo> alarmList { get; set; }
    }

    public class AlarmInfo:BaseParam
    {
        // 开始或复位状态 (1:开始, 0:复位)
        public string status { get; set; }

        // 报警代码
        public string alarmCode { get; set; }

        // 报警描述
        public string alarmDesc { get; set; }

        // 触发时间（时间戳）
       
    }

    public class EquipmentStopParam
    {
        // 工位
        public string bigStationCode { get; set; }

        // 设备编码
        public string equipmentID { get; set; }

        // 员工号
        public string operatorNo { get; set; }

        // 停机代码，参考2.6章节
        public string stopCode { get; set; }

        // 停机描述
        public string stopDesc { get; set; }

        // 停机时间（时间戳）
        public string stopTime { get; set; }

        // 停机时长（ms）
        public string stopDurationTime { get; set; }
    }

  

    public class ReworkParam:BaseParam
    {
        // 返修原因
        public string reworkReason { get; set; }

        // 返修工单（非必填）
        public string reworkWorkOrder { get; set; }

        // 返修工位号
        public string bigStationCode { get; set; }

        // 返修小工位
        public string stationCode { get; set; }

        // 备注（非必填）
        public string reworkComments { get; set; }

        // 员工号
        public string operatorNo { get; set; }

        // 请求时间（时间戳）
        

        // 返修的产品清单
        public List<ProductInfo> productList { get; set; }
    }
    [DataContract]
    public class ToolRemainParam:BaseParam
    {
        // 工位号
        [DataMember]
        public string bigStationCode { get; set; }

        // 小工位
        [DataMember]
        public string stationCode { get; set; }

        // 设备编码（选填）
        [DataMember]
        public string equipmentID { get; set; }

        // 员工号
        [DataMember]
        public string operatorNo { get; set; }



        // 夹治具清单
        [DataMember]
        public List<ToolInfo> toolList { get; set; }
    }

    public class GetPackInfoParam:BaseParam
    {
        public string palletNo { get; set; } // AGV工装码
        public string bigStationCode { get; set; } // 工位号
        public string stationCode { get; set; } // 小工位（选填）
        public string equipmentID { get; set; } // 设备编码（选填）
        public string operatorNo { get; set; } // 操作工
    }

    public class GetSopParam:BaseParam
    {
        public string productionLine { get; set; } // 产线编码（固定值）
        public string productPartNo { get; set; } // 产品物料号，从当前生产的工单中获取
        public string operatorNo { get; set; } // 操作工
    }

    public class BindPalletParam:BaseParam
    {

        public string productCode { get; set; } // 内控码
        public string palletNo { get; set; } // AGV工装码 
        public string bigStationCode { get; set; } // 工位号
        public string stationCode { get; set; } // 小工位（选填）
        public string equipmentID { get; set; } // 设备编码（选填）
        public string operatorNo { get; set; } // 操作工
    }

    public class AndonParam:BaseParam
    {
        // 设备编码（选填）
        public string equipmentID { get; set; }

        // 工位
        public string bigStationCode { get; set; }

        // 小工位
        public string stationCode { get; set; }

        // P：班组长呼叫/E：设备呼叫/M：物料呼叫
        public string andonType { get; set; }

        // ANDON呼叫描述
        public string andonDescription { get; set; }

        // 触发：Tigger/响应：Response/复位Reset；响应状态选填
        public string actionType { get; set; }

        // 操作员工号
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

    public class ProductInfo
    {
        // 产品类型（PACK/模组/电芯返修时）
        public string productType { get; set; }

        // 产品码（PACK码/模组码/电芯码返修时）
        public string productCode { get; set; }
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
