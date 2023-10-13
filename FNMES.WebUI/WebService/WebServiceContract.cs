using FNMES.Entity.DTO;
using FNMES.Entity.DTO.ApiData;
using FNMES.Entity.DTO.ApiParam;
using FNMES.Entity.DTO.AppData;
using FNMES.Entity.Param;
using FNMES.Entity.Sys;
using FNMES.Utility.Core;
using FNMES.Utility.Network;
using FNMES.WebUI.API;
using FNMES.WebUI.Logic.Param;
using FNMES.WebUI.Logic.Sys;
using SqlSugar;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ServiceModel;

namespace FNMES.Service.WebService
{
    [ServiceContract]
    public class WebServiceContract
    {
        public SysLineLogic lineLogic;
        public SysEquipmentLogic equipmentLogic;
        public SysPermissionLogic permissionLogic;
        public FactoryStatusLogic factoryLogic;
        public ErrorAndStatusLogic errorLogic;


        public WebServiceContract()
        {
            lineLogic = new SysLineLogic();
            equipmentLogic = new SysEquipmentLogic();
            permissionLogic = new SysPermissionLogic();
            factoryLogic = new FactoryStatusLogic();
            errorLogic = new ErrorAndStatusLogic();
        }

        //通过IP查询设备代码，大工站、小工站、configID         通用工位
        [OperationContract]
        public string CheckLink()
        {
            return "success";
        }

        //通过IP查询设备代码，大工站、小工站、configID         通用工位
        [OperationContract]
        public RetMessage<EquipmentInfo> GetEquipmentInfo(IpParam param)
        {
            SysEquipment equipment = equipmentLogic.GetByIP(param.Ip);
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
        public RetMessage<UserInfo> GetUserInfo(LoginParam param,string configId)
        {
            FactoryStatus factoryStatus = GetStatus(configId);
            RetMessage<LoginData> retMessage;
            if (factoryStatus.Status == 0) {
                string ret =  APIMethod.Call(Url.LoginUrl, param);
                retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<LoginData>>();
            }
            else   //若不在线，则给默认1级别权限，不校验
            {
                retMessage = new()
                {
                    messageType = RetCode.success,
                    message = "离线",
                    data = new LoginData()
                    {
                        operatorRoleCode = new List<string> {"1"}
                    }
                };
            }
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
        public RetMessage<UserInfo> GetUserRoles(LoginParam param,string configId)
        {
            FactoryStatus factoryStatus = GetStatus(configId);
            RetMessage<LoginData> retMessage;
            if (factoryStatus.Status == 0)
            {
                string ret = APIMethod.Call(Url.LoginUrl, param);
                retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<LoginData>>();
            }
            else   //若不在线，则给默认1级别权限，不校验
            {
                retMessage = new()
                {
                    messageType = RetCode.success,
                    message = "离线",
                    data = new LoginData()
                    {
                        operatorRoleCode = new List<string> { "1" }
                    }
                };
            }
            RetMessage<UserInfo> message = new();
            if (retMessage != null && retMessage.messageType == RetCode.success)
            {
                message.messageType = retMessage.messageType;
                message.message = retMessage.message;
                message.data.Roles = retMessage.data.operatorRoleCode;
            }
            return message;
        }
        //配方参数获取    TODO 
        [OperationContract]
        public RetMessage<GetRecipeData> GetRecipe(GetRecipeParam param)
        {

            //配方待定
            string ret = APIMethod.Call(Url.GetRecipeUrl, param);
            RetMessage<GetRecipeData> apiResponse = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<GetRecipeData>>();
            return apiResponse;
        }
        //SOP文件获取     TODO
        [OperationContract]
        public RetMessage<GetSopData> GetSop(GetSopParam param)
        {
            //配方待定
            string ret = APIMethod.Call(Url.GetSopUrl, param);
            RetMessage<GetSopData> apiResponse = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<GetSopData>>();
            return apiResponse;
        }



        //获取条码                M300\M500
        [OperationContract]
        public RetMessage<GetLabelData> GetLabel(GetLabelParam param, string configId)
        {
            FactoryStatus factoryStatus = GetStatus(configId);
            //访问接口
            RetMessage<GetLabelData> result;
            if (factoryStatus.Status == 1) {
                string ret = APIMethod.Call(Url.GetLabelUrl, param);
                result = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<GetLabelData>>();
            }
            else       //离线时暂定返回一个雪花ID，后续再细化
            {
                result = new RetMessage<GetLabelData>() { 
                    messageType = RetCode.success,
                    message = "离线",
                    data = new GetLabelData() {
                        codeContent = SnowFlakeSingle.instance.NextId().ToString()
                    }
                };
            }
            return result;
        }


        //上线绑定AGV工装与箱体     M300工位使用    绑定信息上传
        [OperationContract]
        public RetMessage<object> BindPallet( BindPalletParam param,  string configId)
        {
            //内部数据绑定 待定 TODO  




            //新建未传内容的表，等后续再人工恢复上传。


            //API接口上传
            string ret = APIMethod.Call(Url.BindPalletUrl, param);
            RetMessage<object> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<object>>();
            return retMessage;
        }

        //获取Pack信息             自动工位使用        非340工位
        [OperationContract]
        public RetMessage<GetPackInfoData> GetPackInfo(GetPackInfoParam param, string configId)
        {
            //内部数据绑定 待定 TODO  



            //API接口上传
            string ret = APIMethod.Call(Url.GetPackInfUrl, param);
            RetMessage<GetPackInfoData> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<GetPackInfoData>>();
            return retMessage;
        }
        //获取Pack信息             自动工位使用        340工位使用
        [OperationContract]
        public RetMessage<GetPackInfoData> GetPackInfo2(GetPackInfoParam param, string configId)
        {
            //内部数据绑定 待定 TODO  



            //API接口上传
            string ret = APIMethod.Call(Url.GetPackInfUrl, param);
            RetMessage<GetPackInfoData> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<GetPackInfoData>>();
            return retMessage;
        }







        //下线解绑AGV工装与箱体               M460工位使用
        [OperationContract]
        public RetMessage<object> UnBindPallet( BindPalletParam param,  string configId)
        {
            //内部数据解绑 待定 TODO 

            //API接口上传
            string ret = APIMethod.Call(Url.UnBindPalletUrl, param);
            RetMessage<object> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<object>>();
            return retMessage;

        }

        //进站        过站查询  本地过站和api过站         通用
        [OperationContract]
        public RetMessage<InStationData> InStation( InStationParam param,  string configId)
        {
            //内部过站、、、、、、
            //API过站   参数配置中需要增加工位是否需要工厂过站字段
            //TODO     判断是否需要过站
            string ret = APIMethod.Call(Url.InStationUrl, param);
            RetMessage<InStationData> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<InStationData>>();
            return retMessage;
        }

        //出站        出站结果绑定       本地和API 
        [OperationContract]
        public RetMessage<OutStationData> OutStation( OutStationParam param,  string configId)
        {
            //内部出站、、、、、、
            //API出站   参数配置中需要增加工位是否需要工厂过站字段
            //TODO     判断是否需要出站
            string ret = APIMethod.Call(Url.OutStationUrl, param);
            RetMessage<OutStationData> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<OutStationData>>();
            return retMessage;

        }

        //物料绑定接口         通用
        [OperationContract]
        public RetMessage<object> PartUpload( PartUploadParam param,  string configId)
        {
            //内部数据上传、、、、、、


            //API数据上传
            string ret = APIMethod.Call(Url.PartUploadUrl, param);
            RetMessage<object> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<object>>();
            return retMessage;
        }

        //过程数据接口     
        [OperationContract]
        public RetMessage<object> ProcessUpload( ProcessUploadParam param, string configId)
        {
            //内部数据绑定、、、、、、



            //API数据上传
            string ret = APIMethod.Call(Url.ProcessUploadUrl, param);
            RetMessage<object> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<object>>();
            return retMessage;

        }

        //Andon     
        [OperationContract]
        public RetMessage<object> Andon( AndonParam param, string configId)
        {
            //内部数据绑定、、、、、、
            //API数据上传
            string ret = APIMethod.Call(Url.AndonUrl, param);
            RetMessage<object> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<object>>();
            return retMessage;

            

        }
        //状态报警信息   根据PLC号获取配置信息
        [OperationContract]
        public RetMessage<PlcParam> GetPlcParam(int plcNo, string configId)
        {
            PlcParam plcParam = errorLogic.getPlcParam(configId, plcNo);
            if (plcParam == null)
            {
                return new RetMessage<PlcParam>() {
                    messageType = RetCode.error,
                    message = "无数据或查询出错",
                    data = null
                };
            }
            else
            {
                return new RetMessage<PlcParam>()
                {
                    messageType = RetCode.success,
                    message = "",
                    data = plcParam
                };
            }
        }

        //设备状态
        [OperationContract]
        public RetMessage<object> EquipmentState( EquipmentStateParam param,  string configId)
        {
            //内部数据绑定、、、、、、
            //API数据上传
            string ret = APIMethod.Call(Url.EquipmentStateUrl, param);
            RetMessage<object> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<object>>();
            return retMessage;
            

        }

        //设备报警
        [OperationContract]
        public RetMessage<object> EquipmentError(EquipmentErrorParam param,  string configId)
        {
            //内部数据绑定、、、、、、
            //API数据上传

            string ret = APIMethod.Call(Url.EquipmentErrorUrl, param);
            RetMessage<object> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<object>>();
            return retMessage;
            

        }

        //设备停机
        [OperationContract]
        public RetMessage<object> EquipmentStop( EquipmentStopParam param,  string configId)
        {
            //内部数据绑定、、、、、、

            string ret = APIMethod.Call(Url.EquipmentStopUrl, param);
            RetMessage<object> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<object>>();
            return retMessage;

            //API数据上传

        }

        //返修
        [OperationContract]
        public RetMessage<object> Rework( ReworkParam param,  string configId)
        {
            //内部数据绑定、、、、、、

            string ret = APIMethod.Call(Url.ReworkUrl, param);
            RetMessage<object> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<object>>();

            return retMessage;

            //API数据上传

        }

        //夹治具寿命
         [OperationContract]
         public RetMessage<object> ToolRemain(ToolRemainParam param, string configId)
         {
            //内部数据绑定、、、、、、
            string ret = APIMethod.Call(Url.ToolRemainUrl, param);
            RetMessage<object> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<object>>();
             return retMessage;
             //API数据上传
         }


        //心跳
        [OperationContract]
        public RetMessage<object> Heartbeat(HeartbeatParam param, string configId)
        {
            FactoryStatus status = GetStatus(configId);
            string ret = APIMethod.Call(Url.HeartbeatUrl, param);
            RetMessage<object> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<object>>();
            //没有响应，根据状态当前状态处理，
            if (retMessage == null || retMessage.messageType != "S")
            {
                if (status.Status == 1 && status.Retry < 3)
                {
                    status.Retry++;
                    factoryLogic.Update(status);
                }
                else if (status.Status == 1 && status.Retry == 3)
                {
                    factoryLogic.Insert(new FactoryStatus()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        Status = 0,
                        ConfigId = configId,
                        CreateTime = DateTime.Now,
                        Retry = 0
                    });
                }
                else if (status.Status == 0)
                {
                    //之前没连上，什么都不用做
                }
            }
            else if (retMessage.messageType == "S")
            {
                if (status.Status == 0 && status.Retry < 3)
                {
                    status.Retry++;
                    factoryLogic.Update(status);
                }
                else if (status.Status == 0 && status.Retry == 3)
                {
                    factoryLogic.Insert(new FactoryStatus()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        Status = 0,
                        ConfigId = configId,
                        CreateTime = DateTime.Now,
                        Retry = 0
                    });
                }
                else if (status.Status == 1)
                {
                    //之前已连上，什么都不用做
                }
            }



            return retMessage;
            //API数据上传
        }

        private FactoryStatus GetStatus(string configId)
        {
            FactoryStatus status = factoryLogic.Get(configId);
            if (status == null)
            {
                status = new FactoryStatus()
                {
                    Id = SnowFlakeSingle.instance.NextId(),
                    Status = 0,
                    ConfigId = configId,
                    CreateTime = DateTime.Now,
                    Retry = 0
                };
                factoryLogic.Insert(status);
            }

            return status;
        }
    }
}