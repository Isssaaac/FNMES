using FNMES.Utility.Network;

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
        public static string Call(string method, object param)
        {
            return WebApiRequest.DoPostJson(method, param);
        }
    }
}

