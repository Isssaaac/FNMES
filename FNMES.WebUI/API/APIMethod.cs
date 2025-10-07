using CCS.WebUI;
using FNMES.Entity.Record;
using FNMES.Utility;
using FNMES.Utility.Core;
using FNMES.Utility.Network;
using FNMES.WebUI.Logic.Record;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FNMES.WebUI.API
{
   
    public class APIMethod
    {
        private static readonly RecordApiLogic logic;
        private static readonly string url;


        static APIMethod() { 
            logic = new RecordApiLogic();
            url = AppSetting.FactoryUrl;

        }
        //不能在这里修改返回的厂级mes信息，因为厂级mes会返回条码
        public static string Call(string method, object param,string configId,bool disableLog = false)
        {
            if (!GlobalContext.SystemConfig.IsDemo)
            {
                method = url + method;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                string response = WebApiRequest.DoPostJson(method, param);

                if (response.IsNullOrEmpty())
                {
                    //此处用F表示，访问接口失败。。用于区分访问接口失败和调用结果的E
                    response = "{\"messageType\":\"F\",\"message\":\"工厂接口超时或无响应\",\"data\":null}";
                }
                stopwatch.Stop();
                if (!disableLog)
                {
                    logic.Insert(new RecordApi()
                    {
                        Url = method,
                        RequestBody = param.ToJson(),
                        ResponseBody = response,
                        Elapsed = (int)stopwatch.Elapsed.TotalMilliseconds
                    }, configId);
                }
                return response;
            }
            return "";
        }

        public static string Call(string method,string jsonData,string configId,bool disableLog=false)
        {
            if (!GlobalContext.SystemConfig.IsDemo)
            {
                method = url + method;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                string response = WebApiRequest.DoPostJsonData(method, jsonData);

                if (response.IsNullOrEmpty())
                {
                    //此处用F表示，访问接口失败。。用于区分访问接口失败和调用结果的E
                    response = "{\"messageType\":\"F\",\"message\":\"工厂接口超时或无响应\",\"data\":null}";
                }
                stopwatch.Stop();
                if (!disableLog)
                {
                    logic.Insert(new RecordApi()
                    {
                        Url = method,
                        RequestBody = jsonData,
                        ResponseBody = response,
                        Elapsed = (int)stopwatch.Elapsed.TotalMilliseconds
                    }, configId);
                }
                return response;
            }
            return "";
        }
    }
}

