using FNMES.Entity.DTO;
using FNMES.Entity.DTO.ApiData;
using FNMES.Entity.DTO.ApiParam;
using FNMES.Entity.DTO.AppData;
using FNMES.Entity.Param;
using FNMES.Entity.Record;
using FNMES.Entity.Sys;
using FNMES.Utility.Core;
using FNMES.Utility.Network;
using FNMES.WebUI.API;
using FNMES.WebUI.Logic.Param;
using FNMES.WebUI.Logic.Record;
using FNMES.WebUI.Logic.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace FNMES.Service.WebService
{
    [ServiceContract]
    public class WebServiceContract
    {
        private readonly SysLineLogic lineLogic;
        private readonly SysEquipmentLogic equipmentLogic;
        private readonly SysPermissionLogic permissionLogic;
        private readonly FactoryStatusLogic factoryLogic;
        private readonly ErrorAndStatusLogic errorLogic;
        private readonly ProcessBindLogic processBindLogic;
        private readonly ProductStepLogic productStepLogic;
        private readonly ParamOrderLogic paramOrderLogic;
        private readonly RecordOrderLogic recordOrderLogic;
        private readonly RecordOfflineApiLogic offlineApiLogic;
        private readonly RecordOutStationLogic outStationLogic;
        private readonly RecordPartUploadLogic partUploadLogic;
        private readonly RecordProcessUploadLogic processUploadLogic;
        private readonly RecordEquipmentLogic recordEquipmentLogic;
        private readonly RecordToolRemainLogic toolRemainLogic;



         public WebServiceContract()
        {
            lineLogic = new SysLineLogic();
            equipmentLogic = new SysEquipmentLogic();
            permissionLogic = new SysPermissionLogic();
            factoryLogic = new FactoryStatusLogic();
            errorLogic = new ErrorAndStatusLogic();
            processBindLogic = new ProcessBindLogic();
            paramOrderLogic = new ParamOrderLogic();
            recordOrderLogic = new RecordOrderLogic();
            offlineApiLogic = new RecordOfflineApiLogic();
            productStepLogic = new ProductStepLogic();
            outStationLogic = new RecordOutStationLogic();
            partUploadLogic = new RecordPartUploadLogic();
            processUploadLogic = new RecordProcessUploadLogic();
            recordEquipmentLogic = new RecordEquipmentLogic();
            toolRemainLogic = new RecordToolRemainLogic();
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
            if(configId.IsNullOrEmpty())
            {
                return NewErrorMessage<UserInfo>("没有给configId参数赋值");
            }
            FactoryStatus factoryStatus = GetStatus(configId);
            RetMessage<LoginData> retMessage;
            if (factoryStatus.isOnline) {
                string ret =  APIMethod.Call(Url.LoginUrl, param, configId);
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
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<UserInfo>("没有给configId参数赋值");
            }
            FactoryStatus factoryStatus = GetStatus(configId);
            RetMessage<LoginData> retMessage;
            if (factoryStatus.isOnline)
            {
                string ret = APIMethod.Call(Url.LoginUrl, param,configId);
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
        public RetMessage<GetRecipeData> GetRecipe(GetRecipeParam param,string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<GetRecipeData>("没有给configId参数赋值");
            }
            //配方待定
            string ret = APIMethod.Call(Url.GetRecipeUrl, param, configId);
            RetMessage<GetRecipeData> apiResponse = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<GetRecipeData>>();
            return apiResponse;
        }
        //SOP文件获取     TODO
        [OperationContract]
        public RetMessage<GetSopData> GetSop(GetSopParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<GetSopData>("没有给configId参数赋值");
            }
            //配方待定
            string ret = APIMethod.Call(Url.GetSopUrl, param, configId);
            RetMessage<GetSopData> apiResponse = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<GetSopData>>();
            return apiResponse;
        }



        //获取条码                M300\M500
        [OperationContract]
        public RetMessage<LabelAndOrderData> GetLabel(GetLabelParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<LabelAndOrderData>("没有给configId参数赋值");
            }
            //校验请求类型
            if (param.requestCodeType != "packNo" && param.requestCodeType != "reessNo")
            {
                return  new RetMessage<LabelAndOrderData>()
                {
                    messageType = RetCode.error,
                    message = "requestCodeType请求类型只能为packNo或reessNo",
                    data = null
                };
            }
            FactoryStatus factoryStatus = GetStatus(configId);
            //访问接口
            RetMessage<LabelAndOrderData> retMessage;
            //上线时候使用当前激活订单的产品
            if (param.requestCodeType == "packNo")
            {
                //查询当前工单
                ParamOrder selectedOrder = paramOrderLogic.GetSelected(configId);
                if (selectedOrder == null)
                {
                    return NewErrorMessage<LabelAndOrderData>("无激活的工单！");
                }
                param.productPartNo = selectedOrder.ProductPartNo;
                if (factoryStatus.Status == 1)
                {
                    string ret = APIMethod.Call(Url.GetLabelUrl, param, configId);
                    RetMessage<GetLabelData> apiRet = ret.ToObject<RetMessage<GetLabelData>>();
                    retMessage = new RetMessage<LabelAndOrderData>()
                    {
                        messageType = apiRet.messageType,
                        message = apiRet.message,
                        data = new LabelAndOrderData()
                        {
                            CodeContent = apiRet.data.codeContent,
                            ConfigId = configId,
                            ProductPartNo = selectedOrder.ProductPartNo,
                            TaskOrderNumber = selectedOrder.TaskOrderNumber
                        }
                    };

                }
                else       //离线时暂定返回一个雪花ID，后续再细化
                {
                    retMessage = new RetMessage<LabelAndOrderData>()
                    {
                        messageType = RetCode.success,
                        message = "离线",
                        data = new LabelAndOrderData()
                        {
                            CodeContent = SnowFlakeSingle.instance.NextId().ToString(),
                            ConfigId = configId,
                            ProductPartNo = selectedOrder.ProductPartNo,
                            TaskOrderNumber = selectedOrder.TaskOrderNumber
                        }
                    };
                }
                //插入上线记录
                if (retMessage.messageType == RetCode.success)
                {
                    int v = recordOrderLogic.InsertInLine(new RecordOrderStart()
                    {
                        TaskOrderNumber = selectedOrder.TaskOrderNumber,
                        PackNo = param.productCode,
                        ProductCode = retMessage.data.CodeContent
                    }, configId);
                    //判断订单是否已完成
                    if (selectedOrder.PlanQty <= v)
                    {
                        selectedOrder.OperatorNo = param.operatorNo;
                        selectedOrder.Flag = "4";
                        paramOrderLogic.Update(selectedOrder,configId);
                    }
                }
                return retMessage;
            }
            //下线时候使用绑定的记录
            {
                //查询内控码绑定的工单
                ProcessBind processBind = processBindLogic.GetByProductCode(param.productCode, configId);
                if (processBind == null)
                {
                    return new RetMessage<LabelAndOrderData>()
                    {
                        messageType = RetCode.error,
                        message = "未查询到内控码绑定的记录",
                        data = null
                    };
                }
                param.productPartNo = processBind.ProductPartNo;
                if (factoryStatus.isOnline)
                {
                    //访问工厂获取条码
                    string ret = APIMethod.Call(Url.GetLabelUrl, param, configId);
                    RetMessage<GetLabelData> apiRet = ret.ToObject<RetMessage<GetLabelData>>();
                    retMessage = new RetMessage<LabelAndOrderData>()
                    {
                        messageType = apiRet.messageType,
                        message = apiRet.message,
                        data = new LabelAndOrderData()
                        {
                            CodeContent = apiRet.data.codeContent,
                        }
                    };
                }
                else       //离线时暂定返回一个雪花ID，后续再细化
                {
                    retMessage = new RetMessage<LabelAndOrderData>()
                    {
                        messageType = RetCode.success,
                        message = "离线",
                        data = new LabelAndOrderData()
                        {
                            CodeContent = SnowFlakeSingle.instance.NextId().ToString()
                        }
                    };
                }
                //插入下线记录    根据绑定的记录去选择对应数据库
                if (retMessage.messageType == RetCode.success)
                {
                    recordOrderLogic.InsertOutLine(new RecordOrderPack()
                    {
                        TaskOrderNumber = processBind.TaskOrderNumber,
                        ProductCode = param.productCode,
                        ReessNo = retMessage.data.CodeContent
                    }, processBind.ConfigId); 
                    //删除过程绑定
                    processBindLogic.Delete(param.productCode, configId);
                }
                return retMessage;
            }
        }
        //错误信息
        private static RetMessage<T> NewErrorMessage<T>(string message)
        {

            return new RetMessage<T>()
            {
                messageType = RetCode.error,
                message = message,
                data = default
            };
        }
        private static RetMessage<T> NewSuccessMessage<T>(string message)
        {
            return new RetMessage<T>()
            {
                messageType = RetCode.success,
                message = message,
                data = default
            };
        }

        //获取当前工单参数  



        //根据内控码获取线体及工单


        //上线绑定AGV工装与箱体     M300工位使用    绑定信息上传
        [OperationContract]
        public RetMessage<object> BindPallet(BindProcessParam param,  string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<object>("没有给configId参数赋值");
            }
            int result = processBindLogic.Insert(new ProcessBind()
                {
                    PalletNo = param.palletNo,
                    ProductCode = param.productCode,
                    TaskOrderNumber = param.TaskOrderNumber,
                    ProductPartNo = param.ProductPartNo,
                    Status = param.Status,
                    ConfigId = param.ConfigId,
                    RepairFlag = param.RepairFlag,
                    RepairStations = param.RepairStations,
                    CreateTime = DateTime.Now,
                }, configId);
            
            if(result == 0)
            {
                return NewErrorMessage<object>("绑定载盘和内控码失败");
            }
            FactoryStatus factoryStatus = GetStatus(configId);
            BindPalletParam bindPalletParam = new();
            bindPalletParam.CopyField(param);
            //在线则上传工厂
            if (factoryStatus.isOnline)
            {
                string ret = APIMethod.Call(Url.BindPalletUrl, param, configId);
                return ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<object>>();
            }
            //不在线，新建未传内容的表，等后续再人工恢复上传。  
            offlineApiLogic.Insert(new RecordOfflineApi() {
                        Url= Url.BindPalletUrl,
                        RequestBody=param.ToJson(),
                        ReUpload = 0
            }, configId);
            return NewSuccessMessage<object>("工厂离线中，已离线绑定完成");
        }

        //获取Pack信息             自动工位使用       通过pallet码获取内控码    非340工位
        [OperationContract]
        public RetMessage<GetPackInfoData> GetPackInfo(GetPackInfoParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<GetPackInfoData>("没有给configId参数赋值");
            }
            if(param.palletNo.IsNullOrEmpty())
            {
                return NewErrorMessage<GetPackInfoData>("没有给palletNo参数赋值");
            }
            ProcessBind processBind = processBindLogic.GetByPalletNo(param.palletNo, configId);
            if (processBind == null) { 
                return NewErrorMessage<GetPackInfoData>("未查询到绑定信息");
            }
            return new RetMessage<GetPackInfoData>()
            {
                messageType = RetCode.success,
                message = "",
                data = new GetPackInfoData()
                {
                    productCode = processBind.ProductCode
                }
            };
        }
        //获取Pack信息             自动工位使用    通过pallet码获取内控码     340工位使用
        //完全无法离线
        [OperationContract]
        public RetMessage<GetPackInfoData> GetPackInfoFactory(GetPackInfoParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<GetPackInfoData>("没有给configId参数赋值");
            }
            if (param.palletNo.IsNullOrEmpty())
            {
                return NewErrorMessage<GetPackInfoData>("没有给palletNo参数赋值");
            }
            //API接口上传
            string ret = APIMethod.Call(Url.GetPackInfUrl, param, configId);
            RetMessage<GetPackInfoData> retMessage =  ret.ToObject<RetMessage<GetPackInfoData>>();
            //判断是否查询到内控码
            //查询到内控码需要更新processBind表
            if (!retMessage.data.productCode.IsNullOrEmpty())
            {
                return retMessage;
            }
            ProcessBind processBind = processBindLogic.GetByProductCode(retMessage.data.productCode, configId);
            if(processBind == null)
            {
                retMessage.messageType = RetCode.error;
                retMessage.message = "已从工厂查询到内控码，但过程绑定信息中没有此内控码信息";
                return retMessage;
            }
            processBind.PalletNo = param.palletNo;
            //insert里面会删除  内控码和pallet码重复项
            int v = processBindLogic.Insert(processBind, configId);
            if(v == 0)
            {
                retMessage.messageType = RetCode.error;
                retMessage.message = "已从工厂查询到内控码，但重新绑定数据失败";
            }
            return retMessage;
        }

        //下线解绑AGV工装与箱体               M460工位使用
        [OperationContract]
        public RetMessage<object> UnBindPallet( BindPalletParam param,  string configId)
        {
            //先内部解绑工装，，但是过程绑定数据需要保留。    
            //内部数据解绑 
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<object>("没有给configId参数赋值");
            }
            if (param.palletNo.IsNullOrEmpty())
            {
                return NewErrorMessage<object>("没有给palletNo参数赋值");
            }
            ProcessBind processBind = processBindLogic.GetByPalletNo(param.palletNo, configId);
            if (processBind == null)
            {
                return NewSuccessMessage<object>("查无此绑定信息，不用解绑");
            }
            processBind.PalletNo = "";
            //insert里面会删除  内控码和pallet码重复项
            int v = processBindLogic.Insert(processBind, configId);
            if (v == 0)
            {
                return NewErrorMessage<object>("本地解绑失败");
            }
            //API接口上传
            FactoryStatus factoryStatus = GetStatus(configId);
            if (factoryStatus.isOnline)
            {
                return APIMethod.Call(Url.UnBindPalletUrl, param, configId).ToObject<RetMessage<object>>();
            }
            offlineApiLogic.Insert(new RecordOfflineApi() {
                Url = Url.UnBindPalletUrl,
                RequestBody = param.ToJson(),
                ReUpload = 0 },configId);
            return  NewSuccessMessage<object>("工厂离线中，已离线解绑完成");
        }




        //进站        过站查询  本地过站和api过站         通用
        [OperationContract]
        public RetMessage<InStationData> InStation( InStationParam param,  string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<InStationData>("没有给configId参数赋值");
            }
            if (param.productCode.IsNullOrEmpty())
            {
                return NewErrorMessage<InStationData>("没有给productCode参数赋值");
            }
            //内部过站、、、、、、
            ProcessBind processBind = processBindLogic.GetByProductCode(param.productCode, configId);
            if (processBind == null)
            {
                return NewErrorMessage<InStationData>("未查询到绑定纪录");
            }
            //绑定表状态NG则直接NG
            if(processBind.Status == "0")
            {
                return new RetMessage<InStationData>()
                {
                    messageType = RetCode.success,
                    message = "过站结果NG",
                    data = new InStationData()
                    {
                        result = "NG",
                        errorReason = "产品状态为NG",
                        qualityParams = null
                    }
                };
            }
            ParamProductStep paramProductStep = productStepLogic.Query(processBind.ProductPartNo, param.stationCode, configId);
            List<RecordOutStation> recordOutStations = outStationLogic.GetList(param.productCode, configId);
            if(paramProductStep == null)
            {
                return new RetMessage<InStationData>()
                {
                    messageType = RetCode.success,
                    message = "过站结果NG",
                    data = new InStationData()
                    {
                        result = "NG",
                        errorReason = "产品工序未包含当前工位",
                        qualityParams = null
                    }
                };
            }
            RetMessage<InStationData> retMessage;
            //返修处理
            if (processBind.RepairFlag=="1") {
                param.productStatus = "REWORK";
                //包含返修工位
                if (processBind.RepairStations.Contains(param.stationCode))
                {
                    //有没有返修纪录，工位能不能重复做
                    if (paramProductStep.IsAllowRepeat)
                    {
                        //返修且允许重做则不校验
                        retMessage = new RetMessage<InStationData>()
                        {
                            messageType = RetCode.success,
                            message = "返修过站成功",
                            data = new InStationData()
                            {
                                result = "OK",
                                errorReason = "",
                                qualityParams = null
                            }
                        };
                    }
                    else
                    {
                        int v = recordOutStations.Where(it => it.StationCode == param.stationCode && it.ProductStatus == "REWORK").Count();
                        if (v > 0)
                        {
                            retMessage = new RetMessage<InStationData>()
                            {
                                messageType = RetCode.success,
                                message = "返修过站成功",
                                data = new InStationData()
                                {
                                    result = "NG",
                                    errorReason = "该工位已有返修纪录，且不允许重复作业",
                                    qualityParams = null
                                }
                            };
                        }
                        else
                        {
                            retMessage = new RetMessage<InStationData>()
                            {
                                messageType = RetCode.success,
                                message = "返修过站成功",
                                data = new InStationData()
                                {
                                    result = "OK",
                                    errorReason = "返修过站成功",
                                    qualityParams = null
                                }
                            };  
                        }
                    }
                }
                //返修不包含此工位
                else
                {
                    retMessage = new RetMessage<InStationData>()
                    {
                        messageType = RetCode.success,
                        message = "返修过站结果NG",
                        data = new InStationData()
                        {
                            result = "NG",
                            errorReason = "返修未包含当前工位",
                            qualityParams = null
                        }
                    }; 
                }
            }
            //非返修
            else
            {
                bool checkRepeat = false;
                bool checkInstation = false;
                string errorMsg = "";
                //校验重复作业
                if (paramProductStep.IsAllowRepeat)
                {
                    checkRepeat = true;
                }
                else
                {
                    checkRepeat = recordOutStations.Where(it => it.StationCode == param.stationCode).Count()==0;
                }
                //校验过站
                if (paramProductStep.IsCheckInStation && paramProductStep.CheckLists.Count() > 0)
                {
                    List<bool> checkStations = new();
                    List<string> errorStation = new();
                    foreach (var item in paramProductStep.CheckLists)
                    {
                        RecordOutStation recordOutStation = recordOutStations.FirstOrDefault(it => it.StationCode == item);
                        if (recordOutStation != null)
                        {
                            bool v = recordOutStation.ProductStatus == "OK";
                            checkStations.Add(v);
                            if (!v)
                            {
                                errorStation.Add(item);
                            }
                        }
                        else
                        {
                            checkStations.Add(false);
                            errorStation.Add(item);
                        }
                        
                    }
                    checkInstation = checkStations.All(it => it);
                    if (!checkInstation)
                    {
                        errorMsg = String.Join(", ", errorStation) + "工序校验不通过";
                    }
                }
                else
                {
                    checkInstation = true;
                }

                if(checkRepeat && checkInstation)
                {
                    retMessage = new RetMessage<InStationData>()
                    {
                        messageType = RetCode.success,
                        message = "过站结果OK",
                        data = new InStationData()
                        {
                            result = "OK",
                            errorReason = "",
                            qualityParams = null
                        }
                    };
                }
                else
                {
                    retMessage = new RetMessage<InStationData>()
                    {
                        messageType = RetCode.success,
                        message = "过站结果NG",
                        data = new InStationData()
                        {
                            result = "NG",
                            errorReason = "",
                            qualityParams = null
                        }
                    };
                    if (!checkRepeat) {
                        retMessage.data.errorReason = "本工位已作业，不允许重复作业";
                    }
                    if (!checkInstation)
                    {
                        retMessage.data.errorReason = errorMsg;
                    }

                }
            }
            //工厂过站
            
                FactoryStatus factoryStatus = GetStatus(configId);
                if (factoryStatus.isOnline)
                {
                    return APIMethod.Call(Url.InStationUrl, param, configId).ToObject<RetMessage<InStationData>>();
                }
                offlineApiLogic.Insert(new RecordOfflineApi()
                {
                    Url = Url.InStationUrl,
                    RequestBody = param.ToJson(),
                    ReUpload = 0
                }, configId);
                 return retMessage;
        }

        //出站        出站结果绑定       本地和API 
        [OperationContract]
        public RetMessage<OutStationData> OutStation( OutStationParam param,  string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<OutStationData>("没有给configId参数赋值");
            }
            if (param.productCode.IsNullOrEmpty())
            {
                return NewErrorMessage<OutStationData>("没有给productCode参数赋值");
            }

            ProcessBind processBind = processBindLogic.GetByProductCode(param.productCode, configId);
            if (processBind.RepairFlag == "1")
            {
                param.productStatus = "REWORK";
            }
            RecordOutStation recordOutStation = new RecordOutStation();
            recordOutStation.CopyField(param);
            int v = outStationLogic.Insert(recordOutStation, configId);
            if(v == 0)
            {
                return NewErrorMessage<OutStationData>("插入本地数据记录出错");

            }
            FactoryStatus factoryStatus = GetStatus(configId);
            if (factoryStatus.isOnline)
            {
                return APIMethod.Call(Url.OutStationUrl, param, configId).ToObject<RetMessage<OutStationData>>();
            }
            offlineApiLogic.Insert(new RecordOfflineApi()
            {
                Url = Url.OutStationUrl,
                RequestBody = param.ToJson(),
                ReUpload = 0
            }, configId);
            return NewSuccessMessage<OutStationData>("工厂离线中，已离线上传完成");
        }

        //物料绑定接口         通用
        [OperationContract]
        public RetMessage<object> PartUpload( PartUploadParam param,  string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<object>("没有给configId参数赋值");
            }
            if (param.productCode.IsNullOrEmpty())
            {
                return NewErrorMessage<object>("没有给productCode参数赋值");
            }
            int v = partUploadLogic.Insert(param, configId);
            if (v == 0)
            {
                return NewErrorMessage<object>("插入本地数据记录出错");

            }
            FactoryStatus factoryStatus = GetStatus(configId);
            if (factoryStatus.isOnline)
            {
                return APIMethod.Call(Url.PartUploadUrl, param, configId).ToObject<RetMessage<object>>();
            }
            offlineApiLogic.Insert(new RecordOfflineApi()
            {
                Url = Url.PartUploadUrl,
                RequestBody = param.ToJson(),
                ReUpload = 0
            }, configId);
            return NewSuccessMessage<object>("工厂离线中，已离线上传完成");
        }

        //过程数据接口     
        [OperationContract]
        public RetMessage<object> ProcessUpload( ProcessUploadParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<object>("没有给configId参数赋值");
            }
            if (param.productCode.IsNullOrEmpty())
            {
                return NewErrorMessage<object>("没有给productCode参数赋值");
            }
            int v = processUploadLogic.Insert(param, configId);
            if (v == 0)
            {
                return NewErrorMessage<object>("插入本地数据记录出错");

            }
            FactoryStatus factoryStatus = GetStatus(configId);
            if (factoryStatus.isOnline)
            {
                return APIMethod.Call(Url.ProcessUploadUrl, param, configId).ToObject<RetMessage<object>>();
            }
            offlineApiLogic.Insert(new RecordOfflineApi()
            {
                Url = Url.ProcessUploadUrl,
                RequestBody = param.ToJson(),
                ReUpload = 0
            }, configId);
            return NewSuccessMessage<object>("工厂离线中，已离线上传完成");
        }

        //Andon     
        [OperationContract]
        public RetMessage<object> Andon( AndonParam param, string configId)
        {
             return APIMethod.Call(Url.AndonUrl, param, configId).ToObject<RetMessage<object>>();
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

        //设备状态    检测到设备状态恢复时需要上传一条停机统计信息
        [OperationContract]
        public RetMessage<object> EquipmentState(EquipmentState param,  string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<object>("没有给configId参数赋值");
            }
            if (param.bigStationCode.IsNullOrEmpty())
            {
                return NewErrorMessage<object>("没有给bigStationCode参数赋值");
            }
            RecordEquipmentStatus status = new();
            status.CopyField(param);
            RecordEquipmentStop stopParam = null;
            int v = recordEquipmentLogic.InsertStatus(status, configId,ref stopParam);
            FactoryStatus factoryStatus = GetStatus(configId);
            if (v == 0)
            {
                return NewErrorMessage<object>("插入本地数据记录出错");
            }
            EquipmentStateParam apiStatusParam = new();
            apiStatusParam.CopyField(param);
            EquipmentStopParam apiStopParam = new();
            if (stopParam != null)
            {
                apiStopParam.CopyField(stopParam);
            }

            if (factoryStatus.isOnline)
            {
                if (stopParam != null)
                {
                    APIMethod.Call(Url.EquipmentStopUrl, apiStopParam, configId).ToObject<RetMessage<object>>();
                }
                return APIMethod.Call(Url.EquipmentStateUrl, apiStatusParam, configId).ToObject<RetMessage<object>>();
            }
            _ = offlineApiLogic.Insert(new RecordOfflineApi()
                {
                Url = Url.EquipmentStateUrl,
                RequestBody = apiStatusParam.ToJson(),
                ReUpload = 0
                 }, configId);
            if (stopParam != null)
            {
                _ = offlineApiLogic.Insert(new RecordOfflineApi()
                    {
                    Url = Url.EquipmentStopUrl,
                    RequestBody = apiStopParam.ToJson(),
                    ReUpload = 0
                      }, configId);
            }
            return NewSuccessMessage<object>("工厂离线中，已离线上传完成");
        }

        //设备报警
        [OperationContract]
        public RetMessage<object> EquipmentError(EquipmentErrorParam param,  string configId)
        {
            //内部数据绑定、、、、、、
            //API数据上传
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<object>("没有给configId参数赋值");
            }
            if (param.bigStationCode.IsNullOrEmpty())
            {
                return NewErrorMessage<object>("没有给productCode参数赋值");
            }
            int v = recordEquipmentLogic.InsertError(param, configId);
            if (v == 0)
            {
                return NewErrorMessage<object>("插入本地数据记录出错");

            }
            FactoryStatus factoryStatus = GetStatus(configId);
            if (factoryStatus.isOnline)
            {
                return APIMethod.Call(Url.EquipmentErrorUrl, param, configId).ToObject<RetMessage<object>>();
            }
            offlineApiLogic.Insert(new RecordOfflineApi()
            {
                Url = Url.EquipmentErrorUrl,
                RequestBody = param.ToJson(),
                ReUpload = 0
            }, configId);
            return NewSuccessMessage<object>("工厂离线中，已离线上传完成");
        }

        //返修   TODO 返修暂时未清除具体流程。
        [OperationContract]
        public RetMessage<object> Rework( ReworkParam param,  string configId)
        {
            //内部数据绑定、、、、、、

            string ret = APIMethod.Call(Url.ReworkUrl, param, configId);
            RetMessage<object> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<object>>();

            return retMessage;

            //API数据上传

        }

        //夹治具寿命
         [OperationContract]
         public RetMessage<object> ToolRemain(ToolRemainParam param, string configId)
         {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<object>("没有给configId参数赋值");
            }
            if (param.stationCode.IsNullOrEmpty())
            {
                return NewErrorMessage<object>("没有给productCode参数赋值");
            }
            int v = toolRemainLogic.Insert(param, configId);
            if (v == 0)
            {
                return NewErrorMessage<object>("插入本地数据记录出错");

            }
            FactoryStatus factoryStatus = GetStatus(configId);
            if (factoryStatus.isOnline)
            {
                return APIMethod.Call(Url.ToolRemainUrl, param, configId).ToObject<RetMessage<object>>();
            }
            offlineApiLogic.Insert(new RecordOfflineApi()
            {
                Url = Url.ToolRemainUrl,
                RequestBody = param.ToJson(),
                ReUpload = 0
            }, configId);
            return NewSuccessMessage<object>("工厂离线中，已离线上传完成");
         }


        //心跳
        [OperationContract]
        public RetMessage<object> Heartbeat(HeartbeatParam param, string configId)
        {
            FactoryStatus status = GetStatus(configId);
            string ret = APIMethod.Call(Url.HeartbeatUrl, param, configId);
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
                        Status = 1,
                        ConfigId = configId,
                        CreateTime = DateTime.Now,
                        Retry = 0
                    });
                    //todo此处需要异步处理离线接口程序。
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