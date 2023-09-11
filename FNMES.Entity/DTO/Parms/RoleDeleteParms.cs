using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.DTO.Parms
{
    public class RoleDeleteParms
    {
        public List<string> roleIdList { get; set; }
        public string operateUser { get; set; }
    }
}
