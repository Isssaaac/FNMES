using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Utility
{
    public class MyConnectionConFig
    {

        public string ConfigId { get; set; }
        public string DBType { get; set; }
        public string DbConnectionString { get; set; }
        public SlaveConnection[] SlaveConnections { get; set; }

        public class SlaveConnection
        {
            public string HitRate { get; set; }
            public string ConnectionString { get; set; }
        }

    }
}
