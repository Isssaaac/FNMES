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

    public class EquipmentState 
    {
        // 设备编码
        public string equipmentID { get; set; }

        // 工位
        public string bigStationCode { get; set; }

        // 设备状态
        public string equipmentStatus { get; set; }
        // 状态码
        public string statusCode { get; set; }

        // 描述
        public string statusDescription { get; set; }



    }
}
