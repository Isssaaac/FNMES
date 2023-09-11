using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Utility.ResponseModels
{
    public class LayPaddingDataTable
    {
        public int code { get; set; }

        /// <summary>
        /// 获取结果。
        /// </summary>
        public bool result { get; set; }

        /// <summary>
        /// 备注信息。
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 数据列表。
        /// </summary>
        public DataTable list { get; set; }

        public string backgroundImage { get; set; }
        /// <summary>
        /// 记录条数。
        /// </summary>
        public long count { get; set; }
    }
}
