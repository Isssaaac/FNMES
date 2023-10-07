using FNMES.Entity.DTO.ApiData;
using FNMES.Entity.DTO.ApiParam;
using FNMES.Utility.Network;
using System.Security.Policy;

namespace FNMES.WebUI.API
{
    public class APIMethod
    {
        private const string HeartbeatUrl = "";
        private const string LoginUrl = "";
        private const string GetOrderUrl = "";
        private const string SelectOrderUrl = "";
        private const string GetRecipeUrl = "";
        private const string GetLabelUrl = "";
        private const string InStationUrl = "";
        private const string PartUploadUrl = "";
        private const string ProcessUploadUrl = "";
        private const string OutStationUrl = "";
        private const string DisAssembleUrl = "";
        private const string AssembleUrl = "";
        private const string EquipmentStateUrl = "";
        private const string EquipmentErrorUrl = "";
        private const string EquipmentStopUrl = "";
        private const string ReworkUrl = "";
        private const string ToolRemainUrl = "";
        private const string GetPackInfUrl = "";
        private const string BindPalletUrl = "";
        private const string UnBindPalletUrl = "";
        private const string GetSopUrl = "";
        private const string AndonUrl = "";
        

        public static RetMessage<object> Heartbeat(HeartbeatParam heartbeatParam)
        {
            return WebApiRequest.DoPostJson<object>(HeartbeatUrl, heartbeatParam);
        }

        public static RetMessage<LoginData> Login(LoginParam loginParam)
        {
            return WebApiRequest.DoPostJson<LoginData>(LoginUrl, loginParam);
        }

        public static RetMessage<GetOrderData> GetOrder(GetOrderParam getOrderParam)
        {
            return WebApiRequest.DoPostJson<GetOrderData>(GetOrderUrl, getOrderParam);
        }

        public static RetMessage<object> SelectOrder(SelectOrderParam selectOrderParam)
        {
            return WebApiRequest.DoPostJson<object> (SelectOrderUrl, selectOrderParam);
        }

        public static RetMessage<GetRecipeData> GetRecipe(GetRecipeParam getRecipeParam)
        {
            return WebApiRequest.DoPostJson<GetRecipeData>(GetRecipeUrl, getRecipeParam);
        }

        public static RetMessage<GetLabelData> GetLabel(GetLabelParam getLabelParam)
        {
            return WebApiRequest.DoPostJson<GetLabelData>(GetLabelUrl, getLabelParam);
        }

        public static RetMessage<InStationData> InStation(InStationParam inStationParam)
        {
            return WebApiRequest.DoPostJson<InStationData>(InStationUrl, inStationParam);
        }

        public static RetMessage<object> PartUpload(PartUploadParam partUploadParam)
        {
            return WebApiRequest.DoPostJson<object>(PartUploadUrl, partUploadParam);
        } public static RetMessage<object> ProcessUpload(ProcessUploadParam processUploadParam)
        {
            return WebApiRequest.DoPostJson<object>(ProcessUploadUrl, processUploadParam);
        }

        public static RetMessage<OutStationData> OutStation(OutStationParam outStationParam)
        {
            return WebApiRequest.DoPostJson<OutStationData>(OutStationUrl, outStationParam);
        }

        public static RetMessage<object> DisAssemble(DisAssembleParam disAssembleParam)
        {
            return WebApiRequest.DoPostJson<object>(DisAssembleUrl, disAssembleParam);
        }

        public static RetMessage<object> Assemble(AssembleParam assembleParam)
        {
            return WebApiRequest.DoPostJson<object>(AssembleUrl, assembleParam);
        }

        public static RetMessage<object> EquipmentState(EquipmentStateParam equipmentStateParam)
        {
            return WebApiRequest.DoPostJson<object>(EquipmentStateUrl, equipmentStateParam);
        }

        public static RetMessage<object> EquipmentError(EquipmentErrorParam equipmentErrorParam)
        {
            return WebApiRequest.DoPostJson<object>(EquipmentErrorUrl, equipmentErrorParam);
        }

        public static RetMessage<object> EquipmentStop(EquipmentStopParam equipmentStopParam)
        {
            return WebApiRequest.DoPostJson<object>(EquipmentStopUrl, equipmentStopParam);
        }
        public static RetMessage<object> Rework(ReworkParam reworkParam)
        {
            return WebApiRequest.DoPostJson<object>(ReworkUrl, reworkParam);
        }

        public static RetMessage<object> ToolRemain(ToolRemainParam toolRemainParam)
        {
            return WebApiRequest.DoPostJson<object>(ToolRemainUrl, toolRemainParam);
        }

        public static RetMessage<GetPackInfoData> getPackInfo(GetPackInfoParam getPackInfoParam)
        {
            return WebApiRequest.DoPostJson<GetPackInfoData>(GetPackInfUrl, getPackInfoParam);
        }

        public static RetMessage<object> BindPallet(BindPalletParam bindPalletParam)
        {
            return WebApiRequest.DoPostJson<object>(BindPalletUrl, bindPalletParam);
        }
        public static RetMessage<object> UnBindPallet(BindPalletParam bindPalletParam)
        {
            return WebApiRequest.DoPostJson<object>(UnBindPalletUrl, bindPalletParam);
        }

        public static RetMessage<GetSopData> GetSop(GetSopParam getSopParam)
        {
            return WebApiRequest.DoPostJson<GetSopData>(GetSopUrl,getSopParam);
        }

        public static RetMessage<object> Andon(AndonParam andonParam)
        {
            return WebApiRequest.DoPostJson<object>(AndonUrl, andonParam);
        }
    }
}
