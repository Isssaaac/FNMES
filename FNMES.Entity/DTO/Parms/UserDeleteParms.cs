using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.DTO.Parms
{
    public class UserDeleteParms
    {
        public List<string> userIdList { get; set; }

        public string currentUserId { get; set; }
        public string operateUser { get; set; }
    }
}
