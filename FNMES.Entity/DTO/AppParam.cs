using FNMES.Entity.DTO.ApiParam;
using FNMES.Entity.Record;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.DTO
{
    [DataContract]
    public class IpParam
    {
        [DataMember]
        public string Ip { get; set; }
    }
    [DataContract]
    public class EquipmentState :BaseParam
    {
        [DataMember]
        // 设备编码
        public string equipmentID { get; set; }

        // 工位
        [DataMember]
        public string stationCode { get; set; }
        // 工位
        [DataMember]
        public string smallStationCode { get; set; }

        // 设备状态
        [DataMember]
        public string equipmentStatus { get; set; }
        

        // 描述
        [DataMember]
        public string statusDescription { get; set; }


        // 停机码
        [DataMember]
        public string stopCode { get; set; }
        // 停机描述
        [DataMember]
        public string stopDescription { get; set; }


    }

    [DataContract]
    public class TestData
    {
        [DataMember]
        public string productCode;
        [DataMember]
        public string data;
        [DataMember]
        public string result;  //OK NG 
    }

    [DataContract]
    public class TestELECData
    {
        [DataMember]
        public long id;
        [DataMember]
        public string productCode;
        [DataMember]
        public string data;
        [DataMember]
        public string result;  //OK NG 
    }

    [DataContract]
    public class HotRivetRecord
    {
        [DataMember]
        public string productCode;
        [DataMember]
        public string batchCode;
        [DataMember]
        public string station;
        [DataMember]
        public List<HotRivetData> data;
        [DataMember]
        public string result;  //OK NG 
    }
    [DataContract]
    public class HotRivetData   //热铆数据
    {
        [DataMember]
        public string no;
        [DataMember]
        public string realTimeTemperature;
        [DataMember]
        public string holdingTemperature;
        [DataMember]
        public string heatingTime;
        [DataMember]
        public string holdingTime;
        [DataMember]
        public string holdingPressure;
    }

    [DataContract]
    public class PlcRecipeData
    {
        [DataMember]
        public string product;
        [DataMember]
        public string plc;
        [DataMember]
        public string content;
        
    }

    [DataContract]
    public class RepairStepData 
    {

        [DataMember]
        public string StepNo { get; set; }

        [DataMember]
        public string StepName { get; set; }

        [DataMember]
        public string No { get; set; }

        [DataMember]
        public string StepDesc { get; set; }
    }

    [DataContract]
    public class RepairPartData 
    {

        [DataMember]
        public string StepNo { get; set; }

        [DataMember]
        public string StepName { get; set; }

        [DataMember]
        public string PartNumber { get; set; }

        [DataMember]
        public string PartDescription { get; set; }

    }

    [DataContract]
    public class RepairProcessData 
    {
        [DataMember]
        public string StepNo { get; set; }

        [DataMember]
        public string StepName { get; set; }

        [DataMember]
        public string ParamCode { get; set; }

        [DataMember]
        public string ParamName { get; set; }

        [DataMember]
        public string ProcessDescription { get; set; }
    }

    [DataContract]
    public class RepairInfoData
    {
        //内控码
        [DataMember]
        public string ProductCode { get; set; }
        //大工站
        [DataMember]
        public string StationCode { get; set; }
        //小工站
        [DataMember]
        public string SmallStationCode { get; set; }

        [DataMember]
        public string Operation { get; set; }
        
        [DataMember]
        public List<RepairStepData> LstRepairStepData { get; set; } //返修工步

        [DataMember]
        public List<RepairPartData> LstRepairPartData { get; set; } //返修物料

        [DataMember]
        public  List<RepairProcessData> LstRepairProcessData { get; set; } //返修工艺信息，扭矩，电压等
    }

    /***********************************************导入数据********************************************/
    //要先导入工位数据，才能到入小项数据
    public class RecipeStep
    {
        public string StationName { get; set; }
        public string StationCode { get; set; }
        public string SmallStationCode { get; set; }
        public string StepNo { get; set; }
        public string StepName { get; set; }
        public string StepDesc { get; set; }
        public string No { get; set; }
        public string Operation { get; set; }
        public string Identity { get; set; }
        public string Group { get; set; }
    }

    public class RecipePart
    {
        public string StationName { get; set; }
        public string StationCode { get; set; }
        public string SmallStationCode { get; set; }
        public string StepNo { get; set; }
        public string StepName { get; set; }
        public string OrderNo { get; set; }
        public string PartNumber { get; set; }
        public string PartDescription { get; set; }
        public string PartType { get; set; }
        public string PartVersion { get; set; }
        public string PartQty { get; set; }
        public string Uom { get; set; }
    }


    public class RecipeProcessParam
    {
        public string StationName { get; set; }
        public string StationCode { get; set; }
        public string SmallStationCode { get; set; }
        public string StepNo { get; set; }
        public string StepName { get; set; }
        public string OrderNo { get; set; }
        public string ParamCode { get; set; }
        public string ParamName { get; set; }
        public string ProcessDescription { get; set; }
        public string ParamClassification { get; set; }
        public string DecisionType { get; set; }
        public string ParamType { get; set; }
        public string StandValue { get; set; }
        public string MaxValue { get; set; }
        public string MinValue { get; set; }
        public string SetValue { get; set; }
        public string IsDoubleCheck { get; set; }
        public string UnitOfMeasure { get; set; }
    }

    public class UnitProcedure
    {
        public string Parent { get; set; }
        public string Encode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string InStationProductType { get; set; }
        public string OutStationProductType { get; set; }
    }
}
