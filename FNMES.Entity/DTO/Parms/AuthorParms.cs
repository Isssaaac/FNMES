using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.DTO.Parms
{
    public class AuthorParms
    {
        public string roleId { get; set; }
        public List<string> perIds { get; set; }

        public string operater { get; set; }
    }
}
