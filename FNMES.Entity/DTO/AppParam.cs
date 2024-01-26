using FNMES.Entity.DTO.ApiParam;
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


}
