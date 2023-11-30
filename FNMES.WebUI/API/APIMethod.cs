using CCS.WebUI;
using FNMES.Entity.Record;
using FNMES.Utility.Core;
using FNMES.Utility.Network;
using FNMES.WebUI.Logic.Record;
using System.Diagnostics;

namespace FNMES.WebUI.API
{
    public class Url
    {
        public const string HeartbeatUrl = "/api/pa/healthCheck";
        public const string LoginUrl = "/api/pa/doLogin";
        public const string GetOrderUrl = "/api/pa/syncTaskOrders";
        public const string SelectOrderUrl = "/api/pa/syncTaskOrderStatus";
        public const string GetRecipeUrl = "/api/pa/dispatchRecipe";
        public const string GetLabelUrl = "/api/pa/applySNCode";
        public const string InStationUrl = "/api/pa/doInbound";
        public const string PartUploadUrl = "/api/pa/uploadMaterialConsumption";
        public const string ProcessUploadUrl = "/api/pa/uploadProcessParameters";
        public const string OutStationUrl = "/api/pa/doOutbound";
        public const string EquipmentStateUrl = "/api/pa/syncEuipStatus";
        public const string EquipmentErrorUrl = "/api/pa/syncEquipAlarms";
        public const string EquipmentStopUrl = "/api/pa/shutdownEquip";
        public const string ReworkUrl = "/api/pa/uploadRepairInfo";
        public const string ToolRemainUrl = "/api/pa/uploadWearPartLife";
        public const string QualityStop = "/api/pa/qualityStopTag";
        public const string GetPackInfoUrl = "/api/pa/getPackInfo";
        public const string BindPalletUrl = "/api/pa/bindSNAndAGC";
        public const string UnBindPalletUrl = "/api/pa/unbindSNAndAGC";
        public const string AndonUrl = "/api/pa/getAndonInfo";
        public const string AndonParamUrl = "";
    }
    public class APIMethod
    {
        private static readonly RecordApiLogic logic;
        private static readonly string url;


        static APIMethod() { 
            logic = new RecordApiLogic();
            url = AppSetting.FactoryUrl;

        }
        public static string Call(string method, object param,string configId,bool disableLog = false)
        {
            method =  url + method;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string response = WebApiRequest.DoPostJson(method, param);
            if(response.IsNullOrEmpty()) {
                response = "{\"messageType\":\"E\",\"message\":\"工厂接口超时或无响应\",\"data\":null}";
            }
            stopwatch.Stop();
            if(!disableLog)
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
    }
}

