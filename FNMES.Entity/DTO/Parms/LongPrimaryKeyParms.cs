using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.DTO.Parms
{
    public class LongPrimaryKeyParms
    {
        public string operaterId { get; set; }
        public long primaryKey { get; set; }

        public string operateUser { get; set; }
        public string userIds { get; set; }
        public string roleId { get; set; }
    }
}
