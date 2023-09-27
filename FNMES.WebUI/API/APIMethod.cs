using FNMES.Entity.DTO.Data;
using FNMES.Entity.DTO.Param;
using FNMES.Utility.Network;
using ServiceStack;

namespace FNMES.WebUI.API
{
    public class APIMethod
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
        public const string AssembleUrl = "";
        public const string EquipmentStateUrl = "";
        public const string EquipmentErrorUrl = "";
        public const string EquipmentStopUrl = "";
        public const string ReworkUrl = "";
        public const string ToolRemainUrl = "";

        public static RetMessage<object> Heartbeat(HeartbeatParam heartbeatParam)
        {
            return WebApiRequest.DoPostJson<object>(HeartbeatUrl, heartbeatParam);
        }

        public static RetMessage<LoginData> Login(LoginParam loginParam)
        {
            return WebApiRequest.DoPostJson <LoginData>(LoginUrl, loginParam);
        }

        public static RetMessage<GetLabelData> GetOrder(GetOrderParam getOrderParam)
        {
            return WebApiRequest.DoPostJson<GetLabelData>(GetOrderUrl, getOrderParam);
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

        public static RetMessage<InStationData> Instation(InStationParam inStationParam)
        {
            return WebApiRequest.DoPostJson<InStationData>(InStationUrl, inStationParam);
        }

        public static RetMessage<object> PartUpload(PartUploadParam partUploadParam)
        {
            return WebApiRequest.DoPostJson<object>(PartUploadUrl, partUploadParam);
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

    }
}
