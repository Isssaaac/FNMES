using FNMES.Utility.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Utility.Network
{

    /// <summary>
    /// 本框架通信使用
    /// </summary>
    public class WebApiRequest
    {

        public static RetMessage<T> DoGet<T>(string url, Dictionary<string, string> parms, int? timeout = 3000)
        {
            try
            {
                string ret = HttpUtils.DoGet(url, parms, timeout);
                if (ret.IsNullOrEmpty())
                    return null;
                return ret.ToObject<RetMessage<T>>();
            }
            catch
            {
                return null;
            }
        }

        public static RetMessage<T> DoPostForm<T>(string url, Dictionary<string, string> parms, int? timeout = 3000)
        {
            try
            {
                string ret = HttpUtils.DoPost(url, parms, timeout);
                if (ret.IsNullOrEmpty())
                    return null;
                return ret.ToObject<RetMessage<T>>();
            }
            catch
            {
                return null;
            }
        }

        public static RetMessage<T> DoPostJson<T>(string url, object data, int? timeout = 3000)
        {
            try
            {
                string ret = HttpUtils.DoPostData(url, data.ToJson(), "application/json", timeout);
                if (ret.IsNullOrEmpty())
                    return null;
                return ret.ToObject<RetMessage<T>>();
            }
            catch
            {
                return null;
            }
        }
    }



    public class RetMessage<T>
    {
        public string messageType { get; set; }
        public string message { get; set; }
        public T data { get; set; }

    }


    public class RetCode
    {
        public const string success = "S";
        public const string error = "E";
    }
}
