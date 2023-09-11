using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.DTO.Parms
{
    public class UserLoginParms
    {
        public string verifyCode { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
    }
}
