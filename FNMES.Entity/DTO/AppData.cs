using FNMES.Entity.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
}
