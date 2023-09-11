using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.DTO.Parms
{
    public class FileUploadParms
    {
        public byte[] file { get; set; }
        public string fileName { get; set; }
    }
}
