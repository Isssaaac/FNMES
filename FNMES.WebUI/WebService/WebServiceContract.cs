using FNMES.Entity.DTO;
using FNMES.Entity.DTO.ApiData;
using FNMES.Entity.DTO.ApiParam;
using FNMES.Entity.DTO.AppData;
using FNMES.Entity.Param;
using FNMES.Entity.Record;
using FNMES.Entity.Sys;
using FNMES.Utility.Core;
using FNMES.Utility.Network;
using FNMES.Utility.Security;
using FNMES.WebUI.API;
using FNMES.WebUI.Logic;
using FNMES.WebUI.Logic.Param;
using FNMES.WebUI.Logic.Record;
using FNMES.WebUI.Logic.Sys;
using Org.BouncyCastle.Asn1.X509.Qualified;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
        private readonly RecipeLogic recipeLogic;
        private readonly ParamOrderLogic paramOrderLogic;
        private readonly RecordOrderLogic recordOrderLogic;
        private readonly RecordOfflineApiLogic offlineApiLogic;
        private readonly RecordOutStationLogic outStationLogic;
        private readonly RecordPartUploadLogic partUploadLogic;
        private readonly RecordProcessUploadLogic processUploadLogic;
        private readonly RecordEquipmentLogic recordEquipmentLogic;
        private readonly RecordToolRemainLogic toolRemainLogic;
        private readonly ParamAndonLogic andonLogic;
        private readonly TestDataLogic testDataLogic;



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
            recipeLogic = new RecipeLogic();
            outStationLogic = new RecordOutStationLogic();
            partUploadLogic = new RecordPartUploadLogic();
            processUploadLogic = new RecordProcessUploadLogic();
            recordEquipmentLogic = new RecordEquipmentLogic();
            toolRemainLogic = new RecordToolRemainLogic();
            andonLogic = new ParamAndonLogic();
            testDataLogic = new TestDataLogic();
        }

       
        [OperationContract]
        public string CheckLink()
        {
            return "success";
        }

        //通过IP查询设备代码，大工站、小工站、configID         通用工位     Done
        [OperationContract]
        public RetMessage<EquipmentInfo> GetEquipmentInfo(IpParam param)
        {
            SysEquipment equipment = equipmentLogic.GetByIP(param.Ip);
            RetMessage<EquipmentInfo> retMessage = new(new EquipmentInfo());
            if (equipment == null)
            {
                retMessage.messageType = RetCode.error;
                retMessage.message = "通过IP查无此设备";
            }
            else
            {
                EquipmentInfo info = new EquipmentInfo()
                {
                    StationCode = equipment.BigProcedure,
                    Name = equipment.Name,
                    SmallStationCode = equipment.UnitProcedure,
                    ConfigId = equipment.Line.ConfigId,
                    EquipmentCode = equipment.EnCode
                };
                retMessage.message = "";
                retMessage.messageType = RetCode.success;
                retMessage.data = info;
            }
            return retMessage;
        }



        #region 用户及登录
        //登录接口，返回角色、权限         通用             Done
        [OperationContract]
        public RetMessage<UserInfo> GetUserInfo(LoginParam param, string configId)
        {
            if (!param.operatorNo.IsNullOrEmpty())
            {
                param.password = AesHelper.Encrypt(param.password); 
            }
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<UserInfo>("没有给configId参数赋值");
            }
            FactoryStatus factoryStatus = GetStatus(configId);
            RetMessage<LoginData> retMessage;
            if (factoryStatus.isOnline)
            {
                string ret = APIMethod.Call(Url.LoginUrl, param, configId);
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
            RetMessage<UserInfo> message = new(new UserInfo());
            if (retMessage != null && retMessage.messageType == RetCode.success)
            {
                message.data.Name = retMessage.data.operatorName;
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
            else{
                message.messageType = retMessage.messageType;
                message.message = retMessage.message;
            }
            return message;
        }

        //登录接口，仅返回角色,给PLC登录使用              PLC使用       Done
        [OperationContract]
        public RetMessage<UserInfo> GetUserRoles(LoginParam param, string configId)
        {
            if (!param.operatorNo.IsNullOrEmpty())
            {
                //员工号不为空时需要加密密码      员工为空时密码项目不处理
                param.password = AesHelper.Encrypt(param.password); 
            }
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<UserInfo>("没有给configId参数赋值");
            }
            FactoryStatus factoryStatus = GetStatus(configId);
            RetMessage<LoginData> retMessage;
            if (factoryStatus.isOnline)
            {
                string ret = APIMethod.Call(Url.LoginUrl, param, configId);
                retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<LoginData>>();
            }
            else   //若不在线，则给默认1级别权限，不校验
            {
                retMessage = new(new LoginData()
                {
                    operatorRoleCode = new List<string> { "1" }
                }, RetCode.success, "离线");

            }
            RetMessage<UserInfo> message = new(new UserInfo());
            if (retMessage != null && retMessage.messageType == RetCode.success)
            {
                message.data.Name = retMessage.data.operatorName;
                message.messageType = retMessage.messageType;
                message.message = retMessage.message;
                message.data.Roles = retMessage.data.operatorRoleCode;
            }
            return message;
        } 
        #endregion
        //配方参数获取    单工位按照stationcode查询
        [OperationContract]
        public RetMessage<RecipeData> GetRecipe(GetRecipeParam param,string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<RecipeData>("没有给configId参数赋值");
            }
            if (param.productPartNo.IsNullOrEmpty()||param.stationCode.IsNullOrEmpty())
            {
                return NewErrorMessage<RecipeData>("没有给产品或工站赋值");
            }
            //配方从数据库查询
            RetMessage<RecipeData> res = new RetMessage<RecipeData>();
            ParamRecipeItem paramRecipeItem = recipeLogic.Query(param.productPartNo, param.stationCode,param.smallStationCode,configId);
            if (paramRecipeItem == null) { 
                return NewErrorMessage<RecipeData>("未查询到配方");
            }
            RecipeData recipeData = new RecipeData(paramRecipeItem);
            RetMessage< RecipeData> result = new RetMessage<RecipeData>() { 
                data = recipeData,
                messageType = "S",
                message = "配方查询成功"
            };
            Logger.RunningInfo(result.ToJson());
            return result;
        }
        //根据内控码获取线体及工单
        [OperationContract]
        public RetMessage<LabelAndOrderData> GetLabel(GetLabelParam param, string configId)
        {
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<LabelAndOrderData>("没有给configId参数赋值");
            }
            //校验请求类型
            if (param.requestCodeType != "packNo" && param.requestCodeType != "reessNo")
            {
                return  new RetMessage<LabelAndOrderData>( new LabelAndOrderData() )
                {
                    messageType = RetCode.error,
                    message = "requestCodeType请求类型只能为packNo或reessNo",
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
                param.taskOrderNumber = selectedOrder.TaskOrderNumber;
                if (factoryStatus.Status == 1)
                {
                    string ret = APIMethod.Call(Url.GetLabelUrl, param, configId);
                    RetMessage<GetLabelData> apiRet = ret.ToObject<RetMessage<GetLabelData>>();
                    retMessage = new RetMessage<LabelAndOrderData>(new LabelAndOrderData()
                    {
                        CodeContent = apiRet.data.codeContent,
                        ConfigId = configId,
                        ProductPartNo = selectedOrder.ProductPartNo,
                        TaskOrderNumber = selectedOrder.TaskOrderNumber
                    }, apiRet.messageType, apiRet.message);

                }
                else       //离线时暂定返回一个雪花ID，后续再细化
                {
                    retMessage = new RetMessage<LabelAndOrderData>(new LabelAndOrderData()
                    {
                        CodeContent = SnowFlakeSingle.instance.NextId().ToString(),
                        ConfigId = configId,
                        ProductPartNo = selectedOrder.ProductPartNo,
                        TaskOrderNumber = selectedOrder.TaskOrderNumber
                    }, RetCode.success, "离线");
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
                    return new RetMessage<LabelAndOrderData>(new LabelAndOrderData())
                    {
                        messageType = RetCode.error,
                        message = "未查询到内控码绑定的记录",
                    };
                }
                param.taskOrderNumber = processBind.TaskOrderNumber;
                if (factoryStatus.isOnline)
                {
                    //访问工厂获取条码
                    string ret = APIMethod.Call(Url.GetLabelUrl, param, configId);
                    RetMessage<GetLabelData> apiRet = ret.ToObject<RetMessage<GetLabelData>>();
                    retMessage = new RetMessage<LabelAndOrderData>(new LabelAndOrderData(){ CodeContent = apiRet.data.codeContent,})
                    {
                        messageType = apiRet.messageType,
                        message = apiRet.message,
                    };
                }
                else       //离线时暂定返回一个雪花ID，后续再细化
                {
                    retMessage = new RetMessage<LabelAndOrderData>(new LabelAndOrderData(){CodeContent = SnowFlakeSingle.instance.NextId().ToString()})
                    {
                        messageType = RetCode.success,
                        message = "离线",
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

        //获取当前工单参数  
        [OperationContract]
        public RetMessage<LabelAndOrderData> GetInfo(string productCode, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<LabelAndOrderData>("没有给configId参数赋值");
            }
            //从当前线体查询上线记录，获取产品工单及线体信息
            //todo
            //查询内控码绑定的工单
            ProcessBind processBind = processBindLogic.GetByProductCode(productCode, configId);
            if (processBind == null)
            {
                return new RetMessage<LabelAndOrderData>(new LabelAndOrderData())
                {
                    messageType = RetCode.error,
                    message = "未查询到内控码绑定的记录",
                };
            }
            return new RetMessage<LabelAndOrderData>(new LabelAndOrderData())
            {
                messageType = RetCode.success,
                message = "查询成功",
                data = new LabelAndOrderData()
                {
                    CodeContent = productCode,
                    ConfigId = configId,
                    ProductPartNo = processBind.ProductPartNo,
                    TaskOrderNumber = processBind.TaskOrderNumber
                },
            };
        }

        //获取Pack信息             自动工位使用       通过pallet码获取内控码    非340工位
        [OperationContract]
        public RetMessage<GetPackInfoData> GetPackInfo(GetPackInfoParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<GetPackInfoData>("没有给configId参数赋值");
            }
            if (param.palletNo.IsNullOrEmpty())
            {
                return NewErrorMessage<GetPackInfoData>("没有给palletNo参数赋值");
            }
            ProcessBind processBind = processBindLogic.GetByPalletNo(param.palletNo, configId);
            if (processBind == null)
            {
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

        //获取Pack信息             自动工位使用    通过pallet码获取内控码     340工位使用 完全无法离线
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
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            //API接口上传
            string ret = APIMethod.Call(Url.GetPackInfoUrl, param, configId);
            RetMessage<GetPackInfoData> retMessage = ret.ToObject<RetMessage<GetPackInfoData>>();
            //判断是否查询到内控码
            //查询到内控码需要更新processBind表
            if (retMessage.data.IsNullOrEmpty() || retMessage.data.productCode.IsNullOrEmpty())
            {
                return NewErrorMessage<GetPackInfoData>("未从工厂中查询到内控码");
            }
            ProcessBind processBind = processBindLogic.GetByProductCode(retMessage.data.productCode, configId);
            if (processBind == null)
            {
                retMessage.messageType = RetCode.error;
                retMessage.message = "已从工厂查询到内控码，但过程绑定信息中没有此内控码信息";
                return retMessage;
            }
            processBind.PalletNo = param.palletNo;
            //insert里面会删除  内控码和pallet码重复项
            int v = processBindLogic.Insert(processBind, configId);
            if (v == 0)
            {
                retMessage.messageType = RetCode.error;
                retMessage.message = "已从工厂查询到内控码，但重新绑定数据失败";
            }
            return retMessage;
        }
        //获取返修产品的信息，待完成
        [OperationContract]
        public RetMessage<LabelAndOrderData> GetReworkInfo(string productCode, string configId) {
            //从各个线体查询上线记录，获取产品工单及线体信息
            //todo
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<LabelAndOrderData>("没有给configId参数赋值");
            }

            return NewSuccessMessage<LabelAndOrderData>("待完成");

        }

        //上线绑定AGV工装与箱体     M300工位使用    绑定信息上传
        [OperationContract]
        public RetMessage<nullObject> BindPallet(BindProcessParam param,  string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给configId参数赋值");
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
                return NewErrorMessage<nullObject>("绑定载盘和内控码失败");
            }
            FactoryStatus factoryStatus = GetStatus(configId);
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            BindPalletParam bindPalletParam = new();
            bindPalletParam.CopyField(param);
            //在线则上传工厂
            if (factoryStatus.isOnline)
            {
                string ret = APIMethod.Call(Url.BindPalletUrl, bindPalletParam, configId);
                return ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<nullObject>>();
            }
            //不在线，新建未传内容的表，等后续再人工恢复上传。  
            offlineApiLogic.Insert(new RecordOfflineApi() {
                        Url= Url.BindPalletUrl,
                        RequestBody=param.ToJson(),
                        ReUpload = 0
            }, configId);
            return NewSuccessMessage<nullObject>("工厂离线中，已离线绑定完成");
        }

        //下线解绑AGV工装与箱体               M460工位使用
        [OperationContract]
        public RetMessage<nullObject> UnBindPallet( BindPalletParam param,  string configId)
        {
            //先内部解绑工装，，但是过程绑定数据需要保留。    
            //内部数据解绑 
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给configId参数赋值");
            }
            if (param.palletNo.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给palletNo参数赋值");
            }
            ProcessBind processBind = processBindLogic.GetByPalletNo(param.palletNo, configId);
            if (processBind == null)
            {
                return NewSuccessMessage<nullObject>("查无此绑定信息，不用解绑");
            }
            processBind.PalletNo = "";
            //insert里面会删除  内控码和pallet码重复项
            int v = processBindLogic.Insert(processBind, configId);
            if (v == 0)
            {
                return NewErrorMessage<nullObject>("本地解绑失败");
            }
            //API接口上传
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            FactoryStatus factoryStatus = GetStatus(configId);
            if (factoryStatus.isOnline)
            {
                return APIMethod.Call(Url.UnBindPalletUrl, param, configId).ToObject<RetMessage<nullObject>>();
            }
            offlineApiLogic.Insert(new RecordOfflineApi() {
                Url = Url.UnBindPalletUrl,
                RequestBody = param.ToJson(),
                ReUpload = 0 },configId);
            return  NewSuccessMessage<nullObject>("工厂离线中，已离线解绑完成");
        }

        #region 进出站及物料数据、过程数据
        //进站        过站查询  本地过站和api过站         通用
        //TODO 过站逻辑需要变更
        [OperationContract]
        public RetMessage<InStationData> InStation(InStationParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<InStationData>("没有给configId参数赋值");
            }
            if (param.productCode.IsNullOrEmpty())
            {
                return NewErrorMessage<InStationData>("没有给productCode参数赋值");
            }
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            //内部过站、、、、、、
            ProcessBind processBind = processBindLogic.GetByProductCode(param.productCode, configId);
            if (processBind == null)
            {
                return NewErrorMessage<InStationData>("未查询到绑定纪录");
            }
            //绑定表状态NG则直接NG
            if (processBind.Status == "0")
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
            ParamRecipeItem recipeItem = recipeLogic.QueryRoute(processBind.ProductPartNo, param.stationCode, configId);
            List<RecordOutStation> recordOutStations = outStationLogic.GetList(param.productCode, configId);
            if (recipeItem == null)
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
            if (processBind.RepairFlag == "1")
            {
                param.productStatus = "REWORK";
                //包含返修工位
                if (processBind.RepairStations.Contains(param.stationCode))
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
                bool checkInstation = false;
                string errorMsg = "";

                if (checkInstation)
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
                //写入内部过站结果进行工厂过站
                param.productStatus = retMessage.data.result;
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
        public RetMessage<OutStationData> OutStation(OutStationParam param, string configId)
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
            if (processBind == null)
            {
                return NewErrorMessage<OutStationData>("未查到绑定数据");
            }
            //出站时更新绑定表的当前工站
            processBind.CurrentStation = param.stationCode;
            processBindLogic.Update(processBind, configId);
            if (processBind.RepairFlag == "1")
            {
                param.productStatus = "REWORK";
            }
            RecordOutStation recordOutStation = new RecordOutStation();
            recordOutStation.CopyField(param);
            recordOutStation.TaskOrderNumber = processBind.TaskOrderNumber;
            int v = outStationLogic.Insert(recordOutStation, configId);
            if (v == 0)
            {
                return NewErrorMessage<OutStationData>("插入本地数据记录出错");

            }
            FactoryStatus factoryStatus = GetStatus(configId);
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
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
        public RetMessage<nullObject> PartUpload(PartUploadParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给configId参数赋值");
            }
            if (param.productCode.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给productCode参数赋值");
            }
            int v = partUploadLogic.Insert(param, configId);
            if (v == 0)
            {
                return NewErrorMessage<nullObject>("插入本地数据记录出错");

            }
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            FactoryStatus factoryStatus = GetStatus(configId);
            if (factoryStatus.isOnline)
            {
                return APIMethod.Call(Url.PartUploadUrl, param, configId).ToObject<RetMessage<nullObject>>();
            }
            offlineApiLogic.Insert(new RecordOfflineApi()
            {
                Url = Url.PartUploadUrl,
                RequestBody = param.ToJson(),
                ReUpload = 0
            }, configId);
            return NewSuccessMessage<nullObject>("工厂离线中，已离线上传完成");
        }

        //过程数据接口     
        [OperationContract]
        public RetMessage<nullObject> ProcessUpload(ProcessUploadParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给configId参数赋值");
            }
            if (param.productCode.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给productCode参数赋值");
            }
            int v = processUploadLogic.Insert(param, configId);
            if (v == 0)
            {
                return NewErrorMessage<nullObject>("插入本地数据记录出错");

            }
            FactoryStatus factoryStatus = GetStatus(configId);
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            if (factoryStatus.isOnline)
            {

                return APIMethod.Call(Url.ProcessUploadUrl, param, configId).ToObject<RetMessage<nullObject>>();
            }
            offlineApiLogic.Insert(new RecordOfflineApi()
            {
                Url = Url.ProcessUploadUrl,
                RequestBody = param.ToJson(),
                ReUpload = 0
            }, configId);
            return NewSuccessMessage<nullObject>("工厂离线中，已离线上传完成");
        } 
        #endregion

        #region Andon相关接口，获取andon配置及触发andon
        [OperationContract]
        public RetMessage<AndonTypeData> GetAndonParam(string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<AndonTypeData>("没有给configId参数赋值");
            }
            List<ParamAndon> paramAndons = andonLogic.GetList(configId);
            if (paramAndons == null)
            {
                return NewErrorMessage<AndonTypeData>("未查询到数据");
            }
            List<AndonType> andonTypes = new List<AndonType>();
            foreach (ParamAndon param in paramAndons)
            {
                andonTypes.Add(ConvertHelper.Mapper<AndonType, ParamAndon>(param));
            }
            return new RetMessage<AndonTypeData>()
            {
                messageType = "S",
                message = "获取成功",
                data = new AndonTypeData()
                {
                    dataList = andonTypes
                }
            };
        }

        [OperationContract]
        public RetMessage<AndonData> Andon(AndonParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<AndonData>("没有给configId参数赋值");
            }
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            return APIMethod.Call(Url.AndonUrl, param, configId).ToObject<RetMessage<AndonData>>();
        } 
        #endregion
        #region     报警及状态信息
        //状态报警信息   根据PLC号获取配置信息
        [OperationContract]
        public RetMessage<PlcParam> GetPlcParam(int plcNo, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<PlcParam>("没有给configId参数赋值");
            }
            PlcParam plcParam = errorLogic.GetPlcParam(configId, plcNo);
            if (plcParam == null)
            {
                return new RetMessage<PlcParam>()
                {
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
        public RetMessage<nullObject> EquipmentState(EquipmentState param, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给configId参数赋值");
            }
            if (param.stationCode.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给stationCode参数赋值");
            }
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            RecordEquipmentStatus status = new();
            status.CopyField(param);
            RecordEquipmentStop stopParam = null;
            int v = recordEquipmentLogic.InsertStatus(status, configId, ref stopParam);
            FactoryStatus factoryStatus = GetStatus(configId);
            if (v == 0)
            {
                return NewErrorMessage<nullObject>("插入本地数据记录出错");
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
                    APIMethod.Call(Url.EquipmentStopUrl, apiStopParam, configId).ToObject<RetMessage<nullObject>>();
                }
                return APIMethod.Call(Url.EquipmentStateUrl, apiStatusParam, configId).ToObject<RetMessage<nullObject>>();
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
            return NewSuccessMessage<nullObject>("工厂离线中，已离线上传完成");
        }

        //设备报警
        [OperationContract]
        public RetMessage<nullObject> EquipmentError(EquipmentErrorParam param, string configId)
        {
            //内部数据绑定、、、、、、
            //API数据上传
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给configId参数赋值");
            }
            if (param.stationCode.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给productCode参数赋值");
            }
            int v = recordEquipmentLogic.InsertError(param, configId);
            if (v == 0)
            {
                return NewErrorMessage<nullObject>("插入本地数据记录出错");

            }
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            FactoryStatus factoryStatus = GetStatus(configId);
            if (factoryStatus.isOnline)
            {
                return APIMethod.Call(Url.EquipmentErrorUrl, param, configId).ToObject<RetMessage<nullObject>>();
            }
            offlineApiLogic.Insert(new RecordOfflineApi()
            {
                Url = Url.EquipmentErrorUrl,
                RequestBody = param.ToJson(),
                ReUpload = 0
            }, configId);
            return NewSuccessMessage<nullObject>("工厂离线中，已离线上传完成");
        } 
        #endregion

        //返修   TODO 返修暂时未清除具体流程。
        [OperationContract]
        public RetMessage<nullObject> Rework( ReworkParam param,  string configId)
        {
            //内部数据绑定、、、、、、
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给configId参数赋值");
            }
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            string ret = APIMethod.Call(Url.ReworkUrl, param, configId);
            RetMessage<nullObject> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<nullObject>>();

            return retMessage;

            //API数据上传

        }

        //夹治具寿命
         [OperationContract]
         public RetMessage<nullObject> ToolRemain(ToolRemainParam param, string configId)
         {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给configId参数赋值");
            }
            if (param.smallStationCode.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给productCode参数赋值");
            }
            int v = toolRemainLogic.Insert(param, configId);
            if (v == 0)
            {
                return NewErrorMessage<nullObject>("插入本地数据记录出错");

            }
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            FactoryStatus factoryStatus = GetStatus(configId);
            if (factoryStatus.isOnline)
            {
                return APIMethod.Call(Url.ToolRemainUrl, param, configId).ToObject<RetMessage<nullObject>>();
            }
            offlineApiLogic.Insert(new RecordOfflineApi()
            {
                Url = Url.ToolRemainUrl,
                RequestBody = param.ToJson(),
                ReUpload = 0
            }, configId);
            return NewSuccessMessage<nullObject>("工厂离线中，已离线上传完成");
         }


        ////心跳   目前使用线体MES后台访问，暂时不需要线体上位机调用
        //[OperationContract]
        //public RetMessage<nullObject> Heartbeat(HeartbeatParam param, string configId)
        //{
        //    if (configId.IsNullOrEmpty())
        //    {
        //        return NewErrorMessage<nullObject>("没有给configId参数赋值");
        //    }
        //    FactoryStatus status = GetStatus(configId);
        //    string ret = APIMethod.Call(Url.HeartbeatUrl, param, configId);
        //    RetMessage<nullObject> retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<nullObject>>();
        //    //没有响应，根据状态当前状态处理，
        //    if (retMessage == null || retMessage.messageType != "S")
        //    {
        //        if (status.Status == 1 && status.Retry < 3)
        //        {
        //            status.Retry++;
        //            factoryLogic.Update(status);
        //        }
        //        else if (status.Status == 1 && status.Retry == 3)
        //        {
        //            factoryLogic.Insert(new FactoryStatus()
        //            {
        //                Id = SnowFlakeSingle.instance.NextId(),
        //                Status = 0,
        //                ConfigId = configId,
        //                CreateTime = DateTime.Now,
        //                Retry = 0
        //            });
        //        }
        //        else if (status.Status == 0)
        //        {
        //            //之前没连上，什么都不用做
        //        }
        //    }
        //    else if (retMessage.messageType == "S")
        //    {
        //        if (status.Status == 0 && status.Retry < 3)
        //        {
        //            status.Retry++;
        //            factoryLogic.Update(status);
        //        }
        //        else if (status.Status == 0 && status.Retry == 3)
        //        {
        //            factoryLogic.Insert(new FactoryStatus()
        //            {
        //                Id = SnowFlakeSingle.instance.NextId(),
        //                Status = 1,
        //                ConfigId = configId,
        //                CreateTime = DateTime.Now,
        //                Retry = 0
        //            });
        //            //todo此处需要异步处理离线接口程序。
        //        }
        //        else if (status.Status == 1)
        //        {
        //            //之前已连上，什么都不用做
        //        }
        //    }
        //    return retMessage;
        //    //API数据上传
        //}

        #region 共用方法
        // 获取工厂状态
        private FactoryStatus GetStatus(string configId)
        {
            FactoryStatus status = factoryLogic.Get(configId);
            if (status == null)
            {
                status = new FactoryStatus()
                {
                    Id = SnowFlakeSingle.instance.NextId(),
                    Status = 0,
                    CreateTime = DateTime.Now,
                    Retry = 0,
                    ConfigId = configId

                };
                factoryLogic.Insert(status);
            }
            status.ConfigId = configId;

            return status;
        }
        //错误信息
        private static RetMessage<T> NewErrorMessage<T>(string message) where T : new()
        {

            return new RetMessage<T>()
            {
                messageType = RetCode.error,
                message = message,
                data = new T()
            };
        }
        private static RetMessage<T> NewSuccessMessage<T>(string message) where T : new()
        {
            return new RetMessage<T>(default)
            {
                messageType = RetCode.success,
                message = message,
                data = new T()
            };
        }
        //根据类型数字转化类型
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
        #endregion

        #region    //测试仪器相关接口     测试仪通过接口上传测试数据

        //ACR测试仪
        [OperationContract]
        public RetMessage<TestUploadRes> UploadACRData(TestData data,string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<TestUploadRes>("没有给configId参数赋值");
            }
            RecordTestACR model = new()
            {
                Id = SnowFlakeSingle.instance.NextId(),
                ProductCode = data.productCode,
                Data = data.data,
                Result = data.result,
                CreateTime= DateTime.Now
            };
            long v = testDataLogic.InsertACR(model,configId);
            if (v.IsNullOrEmpty())
            {
                return NewErrorMessage<TestUploadRes>("上传数据失败");
            }
            return new RetMessage<TestUploadRes>()
            {
                messageType = RetCode.error,
                message = "上传成功",
                data = new TestUploadRes()
                {
                    primaryKey = v
                }
            };
        }
        //EOL测试仪
        [OperationContract]
        public RetMessage<TestUploadRes> UploadEOLData(TestData data, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<TestUploadRes>("没有给configId参数赋值");
            }
            RecordTestEOL model = new()
            {
                Id = SnowFlakeSingle.instance.NextId(),
                ProductCode = data.productCode,
                Data = data.data,
                Result = data.result,
                CreateTime = DateTime.Now
            };
            long v = testDataLogic.InsertEOL(model, configId);
            if (v.IsNullOrEmpty())
            {
                return NewErrorMessage<TestUploadRes>("上传数据失败");
            }
            return new RetMessage<TestUploadRes>()
            {
                messageType = RetCode.error,
                message = "上传成功",
                data = new TestUploadRes()
                {
                    primaryKey = v
                }
            };
        }
        //OCV测试仪
        [OperationContract]
        public RetMessage<TestUploadRes> UploadOCVData(TestData data, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<TestUploadRes>("没有给configId参数赋值");
            }
            RecordTestOCV model = new()
            {
                Id = SnowFlakeSingle.instance.NextId(),
                ProductCode = data.productCode,
                Data = data.data,
                Result = data.result,
                CreateTime = DateTime.Now
            };
            long v = testDataLogic.InsertOCV(model, configId);
            if (v.IsNullOrEmpty())
            {
                return NewErrorMessage<TestUploadRes>("上传数据失败");
            }
            return new RetMessage<TestUploadRes>()
            {
                messageType = RetCode.error,
                message = "上传成功",
                data = new TestUploadRes()
                {
                    primaryKey = v
                }
            };
        }
        //液冷管气密测试
        [OperationContract]
        public RetMessage<TestUploadRes> UploadGas1Data(TestData data, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<TestUploadRes>("没有给configId参数赋值");
            }
            RecordGasTightness1 model = new()
            {
                Id = SnowFlakeSingle.instance.NextId(),
                ProductCode = data.productCode,
                Data = data.data,
                Result = data.result,
                CreateTime = DateTime.Now
            };
            long v = testDataLogic.InsertGasTightness1(model, configId);
            if (v.IsNullOrEmpty())
            {
                return NewErrorMessage<TestUploadRes>("上传数据失败");
            }
            return new RetMessage<TestUploadRes>()
            {
                messageType = RetCode.error,
                message = "上传成功",
                data = new TestUploadRes()
                {
                    primaryKey = v
                }
            };
        }
        //Pack气密测试
        [OperationContract]
        public RetMessage<TestUploadRes> UploadGas2Data(TestData data, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<TestUploadRes>("没有给configId参数赋值");
            }
            RecordGasTightness2 model = new()
            {
                Id = SnowFlakeSingle.instance.NextId(),
                ProductCode = data.productCode,
                Data = data.data,
                Result = data.result,
                CreateTime = DateTime.Now
            };
            long v = testDataLogic.InsertGasTightness2(model, configId);
            if (v.IsNullOrEmpty())
            {
                return NewErrorMessage<TestUploadRes>("上传数据失败");
            }
            return new RetMessage<TestUploadRes>()
            {
                messageType = RetCode.error,
                message = "上传成功",
                data = new TestUploadRes()
                {
                    primaryKey = v
                }
            };
        }

        #endregion
        #region    //测试仪器相关接口     上位机再通过测试仪返回的主键获取数据json。
        [OperationContract]
        public RetMessage<TestData> QueryACR(long primaryKey ,string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<TestData>("没有给configId参数赋值");
            }
            RecordTestACR record = testDataLogic.GetACRByKey(primaryKey, configId);
            if (record == null)
            {
                return NewErrorMessage<TestData>("未查询到数据");
            }
            return new RetMessage<TestData>()
            {
                messageType = RetCode.error,
                message = "查询成功",
                data = new TestData()
                {
                   productCode = record.ProductCode,
                   data = record.Data,
                   result = record.Result
                }
            };
        }
        [OperationContract]
        public RetMessage<TestData> QueryEOL(long primaryKey, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<TestData>("没有给configId参数赋值");
            }
            var record = testDataLogic.GetEOLByKey(primaryKey, configId);
            if (record == null)
            {
                return NewErrorMessage<TestData>("未查询到数据");
            }
            return new RetMessage<TestData>()
            {
                messageType = RetCode.error,
                message = "查询成功",
                data = new TestData()
                {
                    productCode = record.ProductCode,
                    data = record.Data,
                    result = record.Result
                }
            };
        }
        [OperationContract]
        public RetMessage<TestData> QueryOCV(long primaryKey, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<TestData>("没有给configId参数赋值");
            }
            var record = testDataLogic.GetOCVByKey(primaryKey, configId);
            if (record == null)
            {
                return NewErrorMessage<TestData>("未查询到数据");
            }
            return new RetMessage<TestData>()
            {
                messageType = RetCode.error,
                message = "查询成功",
                data = new TestData()
                {
                    productCode = record.ProductCode,
                    data = record.Data,
                    result = record.Result
                }
            };
        }
        [OperationContract]
        public RetMessage<TestData> QueryGas1(long primaryKey, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<TestData>("没有给configId参数赋值");
            }
            var record = testDataLogic.GetGas1ByKey(primaryKey, configId);
            if (record == null)
            {
                return NewErrorMessage<TestData>("未查询到数据");
            }
            return new RetMessage<TestData>()
            {
                messageType = RetCode.error,
                message = "查询成功",
                data = new TestData()
                {
                    productCode = record.ProductCode,
                    data = record.Data,
                    result = record.Result
                }
            };
        }
        [OperationContract]
        public RetMessage<TestData> QueryGas2(long primaryKey, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<TestData>("没有给configId参数赋值");
            }
            var record = testDataLogic.GetGas2ByKey(primaryKey, configId);
            if (record == null)
            {
                return NewErrorMessage<TestData>("未查询到数据");
            }
            return new RetMessage<TestData>()
            {
                messageType = RetCode.error,
                message = "查询成功",
                data = new TestData()
                {
                    productCode = record.ProductCode,
                    data = record.Data,
                    result = record.Result
                }
            };
        }
        #endregion
       


        //物料追溯码查重
        [OperationContract]
        public RetMessage<bool> CheckPartBarcode(string partBarcode) {
            if (partBarcode.IsNullOrEmpty()) {
                return NewErrorMessage<bool>("条码不允许为空");
            }
            if(partBarcode.Length != 31)
            {
                return NewErrorMessage<bool>("条码长度不为31");
            }
            try
            {
                bool v = partUploadLogic.CheckPartBarcode(partBarcode);
                return new RetMessage<bool>
                {
                    data = v,
                    message = v ? "查重通过":"查重不通过",
                    messageType = RetCode.success,
                };
            }
            catch (Exception E )
            {
                Logger.ErrorInfo(E.Message);
                return new RetMessage<bool>
                {
                    data = false,
                    message = "查重出错"+ E.Message,
                    messageType = RetCode.error,
                };
            }
        }


    }
}