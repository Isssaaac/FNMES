using FNMES.Utility.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Utility.Network
{

    /// <summary>
    /// 本框架通信使用
    /// </summary>
    public class WebApiRequest
    {

        public static RetMessage<T> DoGet<T>(string url, Dictionary<string, string> parms, int? timeout = 3000) where T : new()
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

        public static RetMessage<T> DoPostForm<T>(string url, Dictionary<string, string> parms, int? timeout = 3000) where T : new()
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

        public static RetMessage<T> DoPostJson<T>(string url, object data, int? timeout = 3000) where T : new()
        {
            try
            {
                //此处需要增加接口访问记录 TODO
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
        public static string DoPostJson(string url, object data, int? timeout = 20000) 
        {
            try
            {
                string ret = HttpUtils.DoPostData(url, data.ToJson(), "application/json", timeout);
                if (ret.IsNullOrEmpty())
                    return ""; 
                return ret;
            }
            catch
            {
                return "";
            }
        }

        public static string DoPostJsonData(string url, string jsonData, int? timeout = 10000)
        {
            try
            {
                string ret = HttpUtils.DoPostData(url, jsonData, "application/json", timeout);
                if (ret.IsNullOrEmpty())
                    return "";
                return ret;
            }
            catch
            {
                return "";
            }
        }





    }


    [DataContract]
    public class RetMessage<T> where T : new()
    {
        
        [DataMember]
        public string messageType { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public T data { get; set; }

        public RetMessage( T data,string messageType = "", string message = "")
        {
            this.messageType = messageType;
            this.message = message;
            this.data = data;
        }
        public RetMessage() { 
            this.data = new T();
        }

        
    }


    public class RetCode
    {
        public const string Success = "S";
        public const string Error = "E";
        public const string Ng = "N";   //产品NG
        public const string Repair = "R";
        //空箱体直接在人工扫码的时候确认，通过交互传递给PLC让其执行动作逻辑

    }
}
