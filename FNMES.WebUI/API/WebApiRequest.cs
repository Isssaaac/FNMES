using FNMES.Utility.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FNMES.Entity.DTO.ApiParam;
using SoapCore.Meta;
using System.Threading.Tasks;
using System.Net;
using Org.BouncyCastle.Ocsp;
using System.Web;

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
            string ret;
            try
            {
                ret = HttpUtils.DoPostData(url, data.ToJson(), "application/json", timeout);
                if (ret.IsNullOrEmpty())
                    return ""; 
                return ret;
            }
            catch (TimeoutException)
            {
                ret = "超时";
                return ret;
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.Timeout)
            {
                ret = "异常";
                return ret;
            }
            catch (Exception ex)
            {
                return $"发生错误: {ex.Message}";
            }
        }

        public static string DoPostJsonData(string url, string jsonData, int? timeout = 10000)
        {
            string ret;
            try
            {
                ret = HttpUtils.DoPostData(url, jsonData, "application/json", timeout);
                if (ret.IsNullOrEmpty())
                    return "";
                return ret;
            }
            catch (TimeoutException)
            {
                ret = "超时";
                return ret;
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.Timeout)
            {
                ret = "异常";
                return ret;
            }
            catch (Exception ex)
            {
                return $"发生错误: {ex.Message}";
            }
        }
        /***************************异步方法*****************************/
        public static async Task<RetMessage<T>> DoGetAsync<T>(string url, Dictionary<string, string> parms, int? timeout = 3000) where T : new()
        {
            try
            {
                string ret = await HttpUtils.DoGetAsync(url, parms, timeout);
                if (ret.IsNullOrEmpty())
                    return null;
                return ret.ToObject<RetMessage<T>>();
            }
            catch
            {
                return null;
            }
        }

        public static async Task<RetMessage<T>> DoPostFormAsync<T>(string url, Dictionary<string, string> parms, int? timeout = 3000) where T : new()
        {
            try
            {
                string ret = await HttpUtils.DoPostAsync(url, parms, timeout);
                if (ret.IsNullOrEmpty())
                    return null;
                return ret.ToObject<RetMessage<T>>();
            }
            catch
            {
                return null;
            }
        }

        public static async Task<RetMessage<T>> DoPostJsonAsync<T>(string url, object data, int? timeout = 3000) where T : new()
        {
            try
            {
                //此处需要增加接口访问记录 TODO
                string ret = await HttpUtils.DoPostDataAsync(url, data.ToJson(), "application/json", timeout);
                if (ret.IsNullOrEmpty())
                    return null;
                return ret.ToObject<RetMessage<T>>();
            }
            catch
            {
                return null;
            }
        }



        public static async Task<string> DoPostJsonAsync(string url, object data, int? timeout = 20000)
        {
            string ret;
            try
            {
                var json_data = data.ToJson();
                ret = await HttpUtils.DoPostDataAsync(url, json_data, "application/json", timeout);
                if (ret.IsNullOrEmpty())
                    return "";
                return ret;
            }
            catch (TimeoutException)
            {
                ret = "超时";
                return ret;
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.Timeout)
            {
                ret = "异常";
                return ret;
            }
            catch (Exception ex)
            {
                return $"发生错误: {ex.Message}";
            }
        }

        public static async Task<string> DoPostJsonDataAsync(string url, string jsonData, int? timeout = 10000)
        {
            string ret;
            try
            {
                ret = await HttpUtils.DoPostDataAsync(url, jsonData, "application/json", timeout);
                if (ret.IsNullOrEmpty())
                    return "";
                return ret;
            }
            catch (TimeoutException)
            {
                ret = "超时";
                return ret;
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.Timeout)
            {
                ret = "异常";
                return ret;
            }
            catch (Exception ex)
            {
                return $"发生错误: {ex.Message}";
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

        public RetMessage(MesRet<T> mesRet)
        {
            if (mesRet != null)
            {
                messageType = mesRet.code == "0" ? RetCode.Success : RetCode.Ng;
                message = mesRet.msg;
                if (mesRet.data != null)
                    data = mesRet.data;
                else
                    data = new T();
            }
            else
            {
                messageType = RetCode.Ng;
                message = $"厂级mes返回信息为空";
                data = new T();
            }
        }

        public RetMessage() { 
            this.data = new T();
        }

        public static RetMessage<T> NewErrorMessage(string message) 
        {

            return new RetMessage<T>()
            {
                messageType = RetCode.Error,
                message = message,
                data = new T()
            };
        }

        public static RetMessage<T> NewNgMessage(string message) 
        {

            return new RetMessage<T>()
            {
                messageType = RetCode.Ng,
                message = message,
                data = new T()
            };
        }
        public static RetMessage<T> NewSuccessMessage(string message) 
        {
            return new RetMessage<T>(default)
            {
                messageType = RetCode.Success,
                message = message,
                data = new T()
            };
        }

        public static RetMessage<T> Convert<TSource>(string json)
           where TSource : ResultRet, new()
        {
            try
            {
                // 1. 解析原始JSON为ApiResponse<TSource>
                var apiResponse = JsonConvert.DeserializeObject<MesRet<TSource>>(json);
                if (apiResponse == null)
                {
                    throw new Exception("接口返回为空");
                }

                // 2. 初始化目标RetMessage
                var retMsg = new RetMessage<T>();
                // 3. 根据code和result设置messageType和message
                if (apiResponse.code == "00000")
                {
                    // code=00000时，data一定存在（根据接口约定）
                    if (apiResponse.data == null)
                    {
                        throw new Exception("code=00000时，data不能为null");
                    }

                    // 根据result设置messageType
                    retMsg.messageType = apiResponse.data.result == "OK" ? RetCode.Success : RetCode.Ng;
                    // message取原始data中的message（OK时可能为空，NG时为错误信息）
                    retMsg.message = apiResponse.data.message;
                }
                else
                {
                    // code非00000时，视为错误
                    retMsg.messageType = RetCode.Error;
                    // message取原始接口的msg（此时data可能为null）
                    retMsg.message = apiResponse.msg;
                }
                return retMsg;
            }
            catch (JsonException ex)
            {
                throw new Exception($"JSON解析失败：{ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"转换失败：{ex.Message}");
            }
        }
    }


    public class RetCode
    {
        public const string Success = "S";
        public const string Error = "E"; //
        public const string Ng = "N";   //产品NG
        public const string Repair = "R";
        //空箱体直接在人工扫码的时候确认，通过交互传递给PLC让其执行动作逻辑

    }
}
