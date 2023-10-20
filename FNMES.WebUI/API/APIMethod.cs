using FNMES.Entity.Record;
using FNMES.Utility.Core;
using FNMES.Utility.Network;
using FNMES.WebUI.Logic.Record;
using System.Diagnostics;

namespace FNMES.WebUI.API
{
    public class Url
    {
        public const string HeartbeatUrl = "";
        public const string LoginUrl = "";
        public const string GetOrderUrl = "";
        public const string SelectOrderUrl = "";
        public const string GetRecipeUrl = "";
        public const string GetLabelUrl = "";
        public const string InStationUrl = "";
        public const string PartUploadUrl = "";
        public const string ProcessUploadUrl = "";
        public const string OutStationUrl = "";
        public const string DisAssembleUrl = "";
        public const string EquipmentStateUrl = "";
        public const string EquipmentErrorUrl = "";
        public const string EquipmentStopUrl = "";
        public const string ReworkUrl = "";
        public const string ToolRemainUrl = "";
        public const string GetPackInfUrl = "";
        public const string BindPalletUrl = "";
        public const string UnBindPalletUrl = "";
        public const string GetSopUrl = "";
        public const string AndonUrl = "";

    }
    public class APIMethod
    {
        private static readonly RecordApiLogic logic;
        static APIMethod() { 
            logic = new RecordApiLogic();
        }
        public static string Call(string method, object param,string configId)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string response = WebApiRequest.DoPostJson(method, param);
            if(response.IsNullOrEmpty()) {
                response = "{\"messageType\":\"E\",\"message\":\"工厂接口超时或无响应\",\"data\":null}";
            }
            stopwatch.Stop();
            logic.Insert(new RecordApi()
            {
                Url = method,
                RequestBody = param.ToJson(),
                ResponseBody = response,
                Elapsed = (int)stopwatch.Elapsed.TotalMilliseconds
            }, configId); 
            return response;
        }
    }
}

