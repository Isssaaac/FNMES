using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FNMES.Entity.DTO.ApiData
{

   //与工厂MES交互时获取的数据
    public class LoginData
    {
        public string operatorName { get; set; }
        public string operatorNo {  get; set; }
        public List<string> operatorRoleCode {  get; set; }
    }

    public class GetOrderData
    {
        // 工单信息列表
        public List<WorkOrder> workOrderList { get; set; }

    }
    //工单字段
    public class WorkOrder
    {
        // 派工单号
        public string taskOrderNumber { get; set; }

        // 产品物料号
        public string productPartNo { get; set; }

        // 产品物料描述
        public string productDescription { get; set; }

        // 数量
        public string planQty { get; set; }

        // 单位
        public string uom { get; set; }

        // 计划开始时间
        public DateTime planStartTime { get; set; }

        // 计划结束时间
        public DateTime planEndTime { get; set; }

        public string packCellGear { get; set; }
    }
        

    public class GetRecipeData
    {
        // 产品物料号
        public string productPartNo { get; set; }

        // 产品物料描述
        public string productDescription { get; set; }

        // sap客户项目号
        public string sapCustomerProjNo { get; set; }

        // BOM编码
        public string bomNo { get; set; }

        // BOM名称
        public string bomDescription { get; set; }

        // BOM版本（PLM的BOM版本）
        public string bomVersion { get; set; }

        // 工艺配置名称
        public string processConfigName { get; set; }

        // 工艺路线编码
        public string routeNo { get; set; }

        // 工艺路线名称
        public string routeName { get; set; }

        // 工艺路线版本
        public string routeVersion { get; set; }

        // 工艺配置
        public List<ProcessParamItem> processParamItems { get; set; }

        

        // 配方，工序有配方的情况只有5中情况：电芯匀浆配方、电芯分档配方、电芯抽检分容配方、pack模块配方、PACK堆叠焊接配方
        //public List<RecipeItem> recipeItems { get; set; }
    }

    public class ProcessParamItem
    {
        // 工序编码
        public string stationCode { get; set; }

        // 工序名称
        public string stationName { get; set; }

        // 工段：前段，中段，后段
        public string section { get; set; }

        // 下一站工序编码
        public string nextStationCode { get; set; }

        // 是否首道工序
        public string isFirstStation { get; set; }

        // 节拍，后段需要，单位秒
        public string productionBeat { get; set; }

        // 过站限制：进站校验、出站校验、进出站都校验、都不校验
        public string passStationRestriction { get; set; }

        

        // 工艺参数，可以到小工位下的工步
        public List<ParamItem> paramList { get; set; }

        // ESOP到小工位
        public List<EsopItem> esopList { get; set; }

        // bom物料可以到小工位下的工步
        public List<PartItem> partList { get; set; }

        // 工步列表，仅PACK后段(组装段)需要，PACK前段和中段不需要
        public List<StepItem> stepList { get; set; }
    }
    public class ParamItem
    {
        // 工步编码（可选）
        public string stepNo { get; set; }

        // 小工位
        public string smallStationCode { get; set; }
        // 工步名称（可选）
        public string stepName { get; set; }

        // 顺序号，工步之间的（可选）
        public string No { get; set; }

        // 工艺参数编码（操作类型编码）
        public string paramCode { get; set; }

        // 工艺参数名称（操作类型名称）
        public string paramName { get; set; }

        // 工艺描述
        public string processDesc { get; set; }

        // 参数分类：产品参数、过程参数、设备预设参数（工艺参数）
        public string paramClassification { get; set; }

        // 判定类型：数据收集、相机判定、设备判定、取标准值、取上下限判定、取下限判定、取上限判定、无需判定
        public string decisionType { get; set; }

        // 参数类型：定性（字符串）：选择定性，选项1-不需要，选项2-不需要，设定值-需要；定量（数值型）：标准值，上限，下限。
        public string paramType { get; set; }

        // 工艺参数标准值，针对定量类型的
        public string standValue { get; set; }

        // 工艺参数最大值，针对定量类型的
        public string maxValue { get; set; }

        // 工艺参数最小值，针对定量类型的
        public string minValue { get; set; }

        // 针对定性的设定值
        public string setValue { get; set; }

        // 工厂MES是否二次校验
        public string isDoubleCheck { get; set; }

        // 单位
        public string uom { get; set; }
    }

    public class EsopItem
    {
        // 小工位
        public string smallStationCode { get; set; }
        // SOP文件存储在文件服务器URL路径
        public string filePath { get; set; }

        // 顺序号（有多本时）
        public string No { get; set; }

        // 开始页码
        public string startPageNo { get; set; }

        // 结束页码
        public string endPageNo { get; set; }
    }

    public class PartItem
    {
        // 小工位
        public string smallStationCode { get; set; }
        // 工步编码（可选）
        public string stepNo { get; set; }

        // 工步名称（可选）
        public string stepName { get; set; }

        // 顺序号（可选）
        public string No { get; set; }

        // 物料编码
        public string partNumber { get; set; }

        // 物料描述
        public string partDescription { get; set; }

        // 物料版本
        public string partVersion { get; set; }

        // 数量
        public string partQty { get; set; }

        // 单位
        public string uom { get; set; }

        // 是否判定保质期（是，否）
        public string isShelfLife { get; set; }

        //保质期天数
        public string shelfLifeDays { get; set; }

        // 替代物料
        public List<AlternativePartItem> alternativePartList { get; set; }
    }
    [DataContract]
    public class AlternativePartItem
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
        // 替代料比例（和原物料的比例）
        public string partRate { get; set; }
        [DataMember]
        // 单位
        public string uom { get; set; }

        // 是否判定保质期（是，否）
        public string isShelfLife { get; set; }

        //保质期天数
        public string shelfLifeDays { get; set; }
    }

    public class StepItem
    {
        // 小工位
        public string smallStationCode { get; set; }
        // 工步编号
        public string stepNo { get; set; }

        // 工步名称
        public string stepName { get; set; }

        // 顺序号
        public string No { get; set; }

        // 工步描述
        public string stepDesc { get; set; }

        // 操作
        public string operation { get; set; }

        // 标识
        public string identity { get; set; }
    }

    

   
    //配方相关
    public class RecipeItem
    {
        // 工序
        public string bigStationCode { get; set; }

        // 小工位（可选）
        public string stationCode { get; set; }

        // 电芯分档配方结构体
        public List<Dictionary<string, string>> classificationRecipeList { get; set; }

        // 电芯抽检分容配方结构体
        public List<Dictionary<string, string>> partialVolumeRecipeList { get; set; }

        // pack模块配方结构体
        public List<PackModuleRecipeItem> packModuleRecipeList { get; set; }

        // PACK堆叠焊接配方结构体
        public List<PackStackWeldingRecipeItem> packStackWeldingRecipeList { get; set; }
    }

    public class PackModuleRecipeItem
    {
        // 类型代码：(模块填写：A1,B1,B2,C1,D1,E1,电芯填写：CQ1-不折弯,CQ2-正极折弯,CQ3负极折弯)
        public string moduleTypeCode { get; set; }

        // 是否贴泡棉：A1,B1,C1,D1,E1为Y(需要贴泡棉),B2,D2为N(不需要贴泡棉)
        public string isFoamPatch { get; set; }

        // 折耳极性：CQ1为N(无需折弯),CQ2为Z(正极折弯),CQ3为F(负极折弯)
        public string foldingPolarity { get; set; }

        // 极耳焊接
        public List<PoleEarWeldingItem> poleEarWelding { get; set; }

        // 备注
        public string remarks { get; set; }
    }

    public class PoleEarWeldingItem
    {
        // 电芯类型
        public string batteryType { get; set; }

        // 电芯档位
        public string batteryGear { get; set; }
    }

    public class PackStackWeldingRecipeItem
    {
        // 行号，数字
        public string lineNo { get; set; }

        // 列号，数字
        public string columnNo { get; set; }

        // 层数，数字
        public string layerNo { get; set; }

        // 模块类型
        public string moduleType { get; set; }

        // NTC位置：温感线位置：前、中、后、空
        public string ntcPosition { get; set; }

        // 方向：横向、纵向
        public string direction { get; set; }

        // 极性
        public string polarity { get; set; }

        // 备注
        public string remarks { get; set; }
    }




    public class GetLabelData
    {
        // Pack内控码/Reess码
        public string codeContent { get; set; }
    }
    [DataContract]
    public class InStationData
    {
        [DataMember]
        // 校验结果(OK-通过,NG-失败)
        public string result { get; set; }
        [DataMember]
        // 校验失败原因
        public string errorReason { get; set; }
        [DataMember]
        // 质量参数（A020工序工厂MES反馈电芯质量数据）
        public List<QualityParam> qualityParams { get; set; }
    }
    [DataContract]
    public class QualityParam
    {

        [DataMember]
        public string paramCode { get; set; }
        // 参数名称
        [DataMember]
        public string paramName { get; set; }

        // 参数值
        [DataMember]
        public string paramValue { get; set; }
    }
    [DataContract]
    public class OutStationData
    {
        [DataMember]
        // 校验结果(OK-通过,NG-失败)
        public string result { get; set; }
        [DataMember]
        // 校验失败原因
        public string errorReason { get; set; }
    }

    public class GetPackInfoData
    {
        public string productCode { get; set;}
        public string coatingTime { get; set;}
        public List<Module> moduleList { get; set; }
    }

    public class Module
    {
        //模块码
        public string moduleCode { get; set; }
        //模块位置
        public string lineNo { get; set; }
        public string columnNo { get; set; }
        public string layerNo { get; set; }
    }

    public class AndonData
    {
        //andon消息ID
        public string andonNoticeCode { get; set; }
    }

    [DataContract]
    public class AndonTypeData
    {
        [DataMember]
        //andon前获取的参数，andon界面显示枚举
        public List<AndonType> dataList;
    }
    [DataContract]
    public class AndonType
    {
        [DataMember]
        //P/E/M/Q/S
        public string andonType { get; set; }
        [DataMember]
        public string andonCode { get; set; }
        [DataMember]
        public string andonName { get; set; }
        [DataMember]
        public string andonDesc { get; set; }
        [DataMember]
        public string groupName { get; set; }
    }

    [DataContract]
    public class defectParam
    {
        [DataMember]
        //不良代码
        public string defectCode { get; set; }
        [DataMember]
        //不良描述
        public string defectDesc { get; set; }
    }

    [DataContract]
    public class SynScrapInfoParam
    {
        [DataMember]
        public string productCode { get; set; }
        [DataMember]
        //不良列表
        public List<defectParam> defectList { get; set; }
        [DataMember]
        //工位号
        public string stationCode { get; set; }
        [DataMember]
        public string operatorNo { get; set; }
        [DataMember]
        public string callTime { get; set; }
    }

    [DataContract]
    public class SynScrapInfoParamForm
    {
        [DataMember]
        //主键
        public string primaryKey { get; set; }
        [DataMember]
        //线体
        public string configId { get; set; }

        [DataMember]
        public string productCode { get; set; }
        [DataMember]
        //不良代码
        public string defectCode { get; set; }
        //不良描述
        public string defectDesc { get; set; }
        [DataMember]
        //工位号
        public string stationCode { get; set; }
        [DataMember]
        public string operatorNo { get; set; }
        [DataMember]
        public string callTime { get; set; }
    }


    public class QualityStopData
    {

        public List<QualityStopItem> dataList;
    }

    [DataContract]
    public class nullObject
    {

    }
    public class QualityStopItem
    {
        public string equipmentID;
        public string equShutDownKpi;
        public string equShutDownKpiDesc;
        public string lineStopKpi;
        public string lineStopKpiDesc;
        public string kpiTarget;
        public string kpiActual;
        public string time;
    }

    public class FinishedStation
    {
        public string configId;
        public string equipmentID;
        public string taskOrderNumber;
        public string productPartNo;
        public string reessNo;
        public List<string> stationCodes;
    }

    //marking
    public class GetMarkingParamIn
    { 
        public string productionLine { get; set; }
        public string stationCode { get; set; }
        public string productCode { get; set; }
        public string productCodeType { get; set; }
        public string operatorNo { get; set; } 
        public string callTime { get; set; }
    }
    public class GetMarkingResponse
    {
        public List<MarkingData> markingList;
    }

    public class MarkingData
    {
        public string productCode { get; set; }
        public List<MarkingCode> markingCodeList { get; set; }
    }

    public class MarkingCode
    { 
        public string markingCode { get; set; }
        public string markingName { get; set; }
        public string markingCirculationCode { get; set; }
        public string markingCirculationName { get; set; }    
        public string markingOprationStaion { get; set; }
    }

    //获取A020ocv
    public class GetCellVoltageParamIn
    {
        public string productionLine { get; set; }
        public string stationCode { get; set; }
        public string smallStationCode { get; set; }
        public string equipmentID { get; set; }
        public string productCode { get; set; }
        public string callTime { get; set; }
    }


    public class GetCellVoltageData
    {
        public List<moduleData> moduleList { get; set; } 
    }


    public class moduleData
    { 
        public string moduleCode { get; set; }
        public string moduleType { get; set; }
        public List<cellData> cellList { get; set; }
    }

    public class cellData
    { 
        public string cellCode { get; set; }
        public string cellTestSequence { get; set; }
        public List<cellParam> paramList { get; set; }
    }

    public class cellParam
    { 
        public string paramCode { get; set; }
        public string paramValue { get; set; }
    }

    public class selfDischargeParamIn
    {
        //内控码
        public string productCode { get; set; }
        //模组数据
        public List<moduleSelfDischargeData> moduleSelfDischarges { get; set; }
    }

    public class moduleSelfDischargeData
    {
        //压降K值最大值
        public string maxVoltageDrop { get; set; }
        //压降K值最小值
        public string minVoltageDrop { get; set; }
        //压降K值平均值
        public string averageVoltageDrop { get; set; }
        //压降K值标准差
        public string stdDeviationVoltageDrop { get; set; }
        //判定1上限
        public string judgment1Up { get; set; }
        //判定1下限
        public string judgment1Lo { get; set; }
        //判定1上限判定
        public string judgment1UpResult { get; set; }
        //判定1下限判定
        public string judgment1LoResult { get; set; }
        //判定1判定
        public string judgment1Result { get; set; }
        //判定2判定
        public string judgment2Result { get; set; }
        //总判定
        public string result { get; set; }
        //ocv测试时间
        public DateTime createTime { get; set; }
        public List<cellSelfDischargeData> cellSelfDischarges { get; set; }
    }

    public class cellSelfDischargeData
    { 
        //电芯条码
        public string cellCode { get; set; }
        //A020测试时间
        public DateTime a020TestTime { get; set; }
        //A020测试电压
        public string a020TestVoltage { get; set; }
        //M350测试时间
        public DateTime m350TestTime { get; set; }
        //M350测试电压
        public string m350TestVoltage { get; set; }
        //间隔时间
        public string timeInterval { get; set; }
        //间隔压降
        public string intervalVoltageDrop { get; set; }
        //压降
        public string voltageDrop { get; set; }
    }
}
