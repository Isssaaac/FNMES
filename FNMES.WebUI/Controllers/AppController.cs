/*using FNMES.Entity.DTO;
using FNMES.Entity.DTO.ApiData;
using FNMES.Entity.DTO.ApiParam;
using FNMES.Entity.DTO.AppData;
using FNMES.Entity.Sys;
using FNMES.Utility.Network;
using FNMES.WebUI.API;
using FNMES.WebUI.Logic.Sys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FNMES.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppController : ControllerBase
    {
        public SysLineLogic lineLogic;
        public SysEquipmentLogic equipmentLogic;
        public SysPermissionLogic permissionLogic;


        public AppController()
        {
            lineLogic = new SysLineLogic();
            equipmentLogic = new SysEquipmentLogic();
            permissionLogic = new SysPermissionLogic();
        }


        //通过IP查询设备代码，大工站、小工站、configID      通用工位
        [HttpPost]
        public RetMessage<EquipmentInfo> GetEquipmentInfo([FromBody]IpParam ipParam)
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
                EquipmentInfo info = new EquipmentInfo() {
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
        [HttpPost]
        public  RetMessage<UserInfo> GetUserInfo([FromBody]LoginParam loginParam) {
            RetMessage<LoginData> retMessage = APIMethod.Login(loginParam);
            RetMessage <UserInfo> message = new RetMessage<UserInfo>();
            if (retMessage != null && retMessage.messageType == RetCode.success ) {
                message.messageType = retMessage.messageType;
                message.message = retMessage.message;
                message.data.Roles = retMessage.data.operatorRoleCode;
                List<SysPermission> sysPermissions = permissionLogic.GetPermissions(message.data.Roles);
                List<Permission> permissions = new  List<Permission>();
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
        [HttpPost]
        public RetMessage<UserInfo> GetUserRoles([FromBody] LoginParam loginParam)
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

        //获取条码                M300\M500
        [HttpPost]
        public RetMessage<object> GetLabel([FromBody] BindPalletParam bindPalletParam, [FromBody] string configId)
        {
            //内部数据绑定


            //API接口上传




        }









        //上线绑定AGV工装与箱体                M300工位使用
        [HttpPost]
        public RetMessage<object> BindPallet([FromBody] BindPalletParam bindPalletParam, [FromBody] string configId)
        {
            //内部数据绑定



            //API接口上传




        }

        //下线解绑AGV工装与箱体               M460工位使用
        [HttpPost]
        public RetMessage<object> UnBindPallet([FromBody] BindPalletParam bindPalletParam, [FromBody] string configId)
        {
            //内部数据解绑、、、、、、



            //API接口上传



        }

        //进站        过站查询  本地过站和api过站         通用
        [HttpPost]
        public RetMessage<object> InStation([FromBody] BindPalletParam bindPalletParam, [FromBody] string configId)
        {
            //内部过站、、、、、、





            //API过站   参数配置中需要增加工位是否需要工厂过站字段

        }

        //出站        出站结果绑定       本地和API 
        [HttpPost]
        public RetMessage<object> OutStation([FromBody] BindPalletParam bindPalletParam, [FromBody] string configId)
        {
            //内部数据绑定、、、、、、



            //API数据上传

        }

        //物料绑定接口         通用
        [HttpPost]
        public RetMessage<object> PartUpload([FromBody] BindPalletParam bindPalletParam, [FromBody] string configId)
        {
            //内部数据绑定、、、、、、


            //API数据上传

        }

        //过程数据接口     
        [HttpPost]
        public RetMessage<object> ProcessUpload([FromBody] BindPalletParam bindPalletParam, [FromBody] string configId)
        {
            //内部数据绑定、、、、、、



            //API数据上传

        }

        //Andon     
        [HttpPost]
        public RetMessage<object> Andon([FromBody] BindPalletParam bindPalletParam, [FromBody] string configId)
        {
            //内部数据绑定、、、、、、

            APIMethod.Andon

            //API数据上传

        }

        //设备状态
        [HttpPost]
        public RetMessage<object> EquipmentState([FromBody] BindPalletParam bindPalletParam, [FromBody] string configId)
        {
            //内部数据绑定、、、、、、

            APIMethod.EquipmentState

            //API数据上传

        }

        //设备报警
        [HttpPost]
        public RetMessage<object> EquipmentError([FromBody] BindPalletParam bindPalletParam, [FromBody] string configId)
        {
            //内部数据绑定、、、、、、

            APIMethod.EquipmentError

            //API数据上传

        }

        //设备停机
        [HttpPost]
        public RetMessage<object> EquipmentStop([FromBody] BindPalletParam bindPalletParam, [FromBody] string configId)
        {
            //内部数据绑定、、、、、、

            APIMethod.EquipmentStop

            //API数据上传

        }

        //返修
        [HttpPost]
        public RetMessage<object> Rework([FromBody] BindPalletParam bindPalletParam, [FromBody] string configId)
        {
            //内部数据绑定、、、、、、

            APIMethod.Rework

            //API数据上传

        }

        //夹治具寿命
        [HttpPost]
        public RetMessage<object> ToolRemain([FromBody] BindPalletParam bindPalletParam, [FromBody] string configId)
        {
            //内部数据绑定、、、、、、

            APIMethod.ToolRemain

            //API数据上传

        }








    }
}
*/