using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Base
{
    public class CodeGenerator
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string TypeName { get; set; }
        public string TypeLength { get; set; }
        public string IsPrimaryKey { get; set; }
        public string ClassName { get; set; }
        public string PropertyName { get; set; }
        public string CSType { get; set; }

        public string Other { get; set; }
    }
}
