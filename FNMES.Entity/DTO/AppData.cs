using SqlSugar;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
        public string BigStationCode { get; set; }
        [DataMember]
        public string StationCode { get; set; }
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
        public List<string> StopCodeDesc { get; set; }
    }
    [DataContract]
    public class ErrorParam
    {

        [DataMember]
        public string BigStationCode { get; set; }
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
        public string BigStationCode { get; set; }
        [DataMember]
        public string EquipmentID { get; set; }
        [DataMember]
        public int Offset { get; set; }
        [DataMember]
        public int StopCodeOffset { get; set; }
    }
    [DataContract]
    public class BindProcessParam 
    {
        [DataMember]
        public string productCode { get; set; } // 内控码
        [DataMember]
        public string palletNo { get; set; } // AGV工装码 
        [DataMember]
        public string bigStationCode { get; set; } // 工位号
        [DataMember]
        public string stationCode { get; set; } // 小工位（选填）
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
        public string Status { get; set; }
        [DataMember]
        public string RepairFlag { get; set; }
        [DataMember]
        public string RepairStations { get; set; }
    }

    [DataContract]
    public class LabelAndOrderData
    {
        // Pack内控码/Reess码
        [DataMember]
        public string CodeContent { get; set; }
        [DataMember]
        public string TaskOrderNumber { get; set; }
        [DataMember]
        public string ProductPartNo { get; set; }
        [DataMember]
        public string ConfigId { get; set; }
    }
}
