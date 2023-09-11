using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FNMES.Utility.Files
{
    public class XmlUtils
    {
        public string xml2Json(string strXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strXml);
            return JsonConvert.SerializeXmlNode(doc);
        }
    }
}
