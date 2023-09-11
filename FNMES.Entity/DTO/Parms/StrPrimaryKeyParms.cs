using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.DTO.Parms
{
    public class StrPrimaryKeyParms
    {
        public string operaterId { get; set; }
        public string primaryKey { get; set; }

        public string operateUser { get; set; }
        public string userIds { get; set; }
        public string roleId { get; set; }
    }
}
