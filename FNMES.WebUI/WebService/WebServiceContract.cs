using FNMES.Entity.DTO;
using FNMES.Entity.DTO.ApiData;
using FNMES.Entity.DTO.ApiParam;
using FNMES.Entity.DTO.AppData;
using FNMES.Entity.Sys;
using FNMES.Utility.Core;
using FNMES.Utility.Network;
using FNMES.WebUI.API;
using FNMES.WebUI.Logic.Sys;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace FNMES.Service.WebService
{
    [ServiceContract]
    public class WebServiceContract
    {

        public SysLineLogic lineLogic;
        public SysEquipmentLogic equipmentLogic;
        public SysPermissionLogic permissionLogic;


        public WebServiceContract()
        {
            lineLogic = new SysLineLogic();
            equipmentLogic = new SysEquipmentLogic();
            permissionLogic = new SysPermissionLogic();
        }


        //通过IP查询设备代码，大工站、小工站、configID         通用工位
        [OperationContract]
        public RetMessage<EquipmentInfo> GetEquipmentInfo(IpParam ipParam)
        {
            SysEquipment equipment = equipmentLogic.GetByIP(ipParam.Ip);
            RetMessage<EquipmentInfo> retMessage = new RetMessage<EquipmentInfo>();
            if (equipment == null)
            {
                retMessage.messageType = RetCode.error;
                retMessage.message = "通过IP查无此设备";
            }
            else
            {
                EquipmentInfo info = new EquipmentInfo()
                {
                    BigStationCode = equipment.BigProcedure,
                    Name = equipment.Name,
                    StationCode = equipment.UnitProcedure,
                    ConfigId = equipment.Line.ConfigId,
                    EquipmentCode = equipment.EnCode
                };
                retMessage.message = "";
                retMessage.messageType = RetCode.success;
                retMessage.data = info;
            }
            return retMessage;
        }


        public string GetTypeStr(int? type)
        {
            return type switch
            {
                3 => "主菜单",
                4 => "子菜单",
                5 => "按钮",
                6 => "作业",
                _ => "null",
            };
        }

        //登录接口，返回角色、权限         通用
        [OperationContract]
        public RetMessage<UserInfo> GetUserInfo(LoginParam loginParam)
        {
            RetMessage<LoginData> retMessage = APIMethod.Login(loginParam);
            RetMessage<UserInfo> message = new();
            if (retMessage != null && retMessage.messageType == RetCode.success)
            {
                message.messageType = retMessage.messageType;
                message.message = retMessage.message;
                message.data.Roles = retMessage.data.operatorRoleCode;
                List<SysPermission> sysPermissions = permissionLogic.GetPermissions(message.data.Roles);
                List<Permission> permissions = new List<Permission>();
                foreach (SysPermission item in sysPermissions)
                {
                    permissions.Add(new Permission
                    {
                        Encode = item.EnCode,
                        Name = item.Name,
                        Type = GetTypeStr(item.Type)
                    });
                }
            }
            return message;
        }

        //登录接口，仅返回角色,给PLC登录使用              PLC使用
        [OperationContract]
        public RetMessage<UserInfo> GetUserRoles(LoginParam loginParam)
        {
            RetMessage<LoginData> apiResponse = APIMethod.Login(loginParam);
            RetMessage<UserInfo> message = new RetMessage<UserInfo>();
            if (apiResponse != null && apiResponse.messageType == RetCode.success)
            {
                message.messageType = apiResponse.messageType;
                message.message = apiResponse.message;
                message.data.Roles = apiResponse.data.operatorRoleCode;
            }
            return message;
        }
        //配方参数获取    TODO 
        [OperationContract]
        public RetMessage<GetRecipeData> GetRecipe(GetRecipeParam getRecipeParam)
        {

            //配方待定
            RetMessage<GetRecipeData> apiResponse = APIMethod.GetRecipe(getRecipeParam);
            return apiResponse;
        }
        //SOP文件获取  TODO
        [OperationContract]
        public RetMessage<GetSopData> GetSop(GetSopParam getSopParam)
        {
            //配方待定
            RetMessage<GetSopData> apiResponse = APIMethod.GetSop(getSopParam);
            return apiResponse;
        }



        //获取条码                M300\M500
        [OperationContract]
        public RetMessage<GetLabelData> GetLabel(GetLabelParam getLabelParam, string configId)
        {
            //访问接口
            RetMessage<GetLabelData> retMessage = APIMethod.GetLabel(getLabelParam);
            //是否需要在本地保持  待定

            return retMessage;
        }


        //上线绑定AGV工装与箱体                M300工位使用
        [OperationContract]
        public RetMessage<object> BindPallet( BindPalletParam bindPalletParam,  string configId)
        {
            //内部数据绑定 待定 TODO  



            //API接口上传
            RetMessage<object> retMessage = APIMethod.BindPallet(bindPalletParam);
            return retMessage;
        }

        //下线解绑AGV工装与箱体               M460工位使用
        [OperationContract]
        public RetMessage<object> UnBindPallet( BindPalletParam bindPalletParam,  string configId)
        {
            //内部数据解绑 待定 TODO 

            //API接口上传
            RetMessage<object> retMessage = APIMethod.UnBindPallet(bindPalletParam);
            return retMessage;

        }

        //进站        过站查询  本地过站和api过站         通用
        [OperationContract]
        public RetMessage<InStationData> InStation( InStationParam inStationParam,  string configId)
        {
            //内部过站、、、、、、
            //API过站   参数配置中需要增加工位是否需要工厂过站字段
            //TODO     判断是否需要过站
            RetMessage<InStationData> retMessage = APIMethod.InStation(inStationParam);
            return retMessage;
        }

        //出站        出站结果绑定       本地和API 
        [OperationContract]
        public RetMessage<OutStationData> OutStation( OutStationParam outStationParam,  string configId)
        {
            //内部出站、、、、、、
            //API出站   参数配置中需要增加工位是否需要工厂过站字段
            //TODO     判断是否需要出站
            RetMessage<OutStationData> retMessage = APIMethod.OutStation(outStationParam);
            return retMessage;

        }

        //物料绑定接口         通用
        [OperationContract]
        public RetMessage<object> PartUpload( PartUploadParam partUploadParam,  string configId)
        {
            //内部数据上传、、、、、、


            //API数据上传
            RetMessage<object> retMessage = APIMethod.PartUpload(partUploadParam);
            return retMessage;
        }

        //过程数据接口     
        [OperationContract]
        public RetMessage<object> ProcessUpload( ProcessUploadParam processUploadParam, string configId)
        {
            //内部数据绑定、、、、、、



            //API数据上传
            RetMessage<object> retMessage = APIMethod.ProcessUpload(processUploadParam);
            return retMessage;

        }

        //Andon     
        [OperationContract]
        public RetMessage<object> Andon( AndonParam andonParam, string configId)
        {
            //内部数据绑定、、、、、、
            //API数据上传
            RetMessage<object> retMessage = APIMethod.Andon(andonParam);
            return retMessage;

            

        }

        //设备状态
        [OperationContract]
        public RetMessage<object> EquipmentState( EquipmentStateParam equipmentStateParam,  string configId)
        {
            //内部数据绑定、、、、、、
            //API数据上传

            RetMessage<object> retMessage = APIMethod.EquipmentState(equipmentStateParam);
            return retMessage;
            

        }

        //设备报警
        [OperationContract]
        public RetMessage<object> EquipmentError(EquipmentErrorParam equipmentErrorParam,  string configId)
        {
            //内部数据绑定、、、、、、
            //API数据上传

            RetMessage<object> retMessage = APIMethod.EquipmentError(equipmentErrorParam);
            return retMessage;
            

        }

        //设备停机
        [OperationContract]
        public RetMessage<object> EquipmentStop( EquipmentStopParam equipmentStopParam,  string configId)
        {
            //内部数据绑定、、、、、、

            RetMessage<object> retMessage = APIMethod.EquipmentStop(equipmentStopParam);
            return retMessage;

            //API数据上传

        }

        //返修
        [OperationContract]
        public RetMessage<object> Rework( ReworkParam reworkParam,  string configId)
        {
            //内部数据绑定、、、、、、

            RetMessage<object> retMessage = APIMethod.Rework(reworkParam);
            return retMessage;

            //API数据上传

        }

        //夹治具寿命
         [OperationContract]
         public RetMessage<object> ToolRemain(ToolRemainParam toolRemainParam, string configId)
         {
             //内部数据绑定、、、、、、

             RetMessage<object> retMessage = APIMethod.ToolRemain(toolRemainParam);
             return retMessage;
             //API数据上传
         }


        //心跳
        [OperationContract]
        public RetMessage<object> Heartbeat(HeartbeatParam heartbeatParam, string configId)
        {
            //内部数据绑定、、、、、、

            RetMessage<object> retMessage = APIMethod.Heartbeat(heartbeatParam);
            return retMessage;
            //API数据上传
        }





    }
}