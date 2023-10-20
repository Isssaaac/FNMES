using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.DTO.ApiData
{

   
    public class LoginData
    {
        public List<string> operatorRoleCode {  get; set; }
    }

    public class GetOrderData
    {
        // 工单信息列表
        public List<WorkOrder> workOrderList { get; set; }

    }

    public class WorkOrder
    {
        // 派工单号
        public string taskOrderNumber { get; set; }

        // 产品物料号
        public string productPartNo { get; set; }

        // 产品物料描述
        public string productDescription { get; set; }

        // 数量
        public int planQty { get; set; }

        // 单位
        public string uom { get; set; }

        // 计划开始时间
        public DateTime planStartTime { get; set; }

        // 计划结束时间
        public DateTime planEndTime { get; set; }
    }

    public class GetRecipeData
    {
        // 产品物料号
        public string productPartNo { get; set; }

        // 产品物料描述
        public string productDescription { get; set; }

        // 配方编号
        public string recipeNo { get; set; }

        // 配方名称
        public string recipeDescription { get; set; }

        // 配方版本
        public string recipeVersion { get; set; }

        // BOM编码
        public string bomNo { get; set; }

        // BOM名称
        public string bomDescription { get; set; }

        // BOM版本
        public string bomVersion { get; set; }

        // 配方项列表
        public List<RecipeItem> recipeItems { get; set; }
    }

    public class RecipeItem
    {
        // 工序
        public string bigStationCode { get; set; }

        // 小工位
        public string stationCode { get; set; }

        // 物料列表
        public List<Part> partList { get; set; }

        // 参数列表
        public List<Parameter> paramList { get; set; }
    }

    public class Part
    {
        // 物料名称
        public string partNumber { get; set; }

        // 物料描述
        public string partDescription { get; set; }

        // 物料版本
        public string partVersion { get; set; }

        // 数量
        public string partQty { get; set; }

        // 单位
        public string uom { get; set; }
    }

    public class Parameter
    {
        // 工艺参数
        public string paramName { get; set; }

        // 工艺标准值
        public string standValue { get; set; }

        // 工艺最大值
        public string maxValue { get; set; }

        // 工艺最小值
        public string minValue { get; set; }
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
    }

    public class GetSopData
    {
        public List<SopInfo> sopList { get; set; }
    }

    public class SopInfo
    {
        // 工位
        public string bigStationCode { get; set; }

        // 小工位
        public string stationCode { get; set; }

        // SOP 文件存储在文件服务器存储路径
        public string filePath { get; set; }

        // 开始页码
        public string startPageNo { get; set; }

        // 结束页码
        public string endPageNo { get; set; }
    }

}
