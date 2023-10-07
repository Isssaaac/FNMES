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
}
