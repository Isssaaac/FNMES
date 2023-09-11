using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.DTO.Parms
{
    public class SearchParms
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public string keyWord { get; set; }
    }
}
