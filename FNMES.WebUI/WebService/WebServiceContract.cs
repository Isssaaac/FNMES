using CCS.WebUI;
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
using Newtonsoft.Json;
using ServiceStack;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using FNMES.Utility;
using JinianNet.JNTemplate.Resources;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;
using System.Reflection.Emit;



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
        private readonly DetectDataLogic testDataLogic;
        private readonly ParamProductLogic productLogic;
        private readonly SysPreProductLogic preProductLogic;
        private readonly RouteLogic routeLogic;
        private readonly PlcRecipeLogic plcRecipeLogic;
        private readonly ParamBarcodeRuleLogic barcodeRuleLogic;
        private readonly UnitProcedureLogic unitProcedureLogic;
        private readonly RecordCellStartLogic cellStartLogic;

        
        private readonly RecordCellBindBlockLogic cellBindBlockLogic;
        private readonly RecordCellOutStationLogic cellOutStationLogic;
        private readonly RecordCellPartUploadLogic cellPartUploadLogic;
        private readonly RecordCellProcessUploadLogic cellProcessUploadLogic;

        private readonly RecordBlockBindPackLogic blockBindPackLogic;
        private readonly RecordBlockOutStationLogic blockOutStationLogic;
        private readonly RecordBlockPartUploadLogic blockPartUploadLogic;
        private readonly RecordBlockProcessUploadLogic blockProcessUploadLogic;

        private readonly ParamItemLogic paramItemLogic;


        public WebServiceContract()
        {
            
            cellStartLogic = new RecordCellStartLogic();
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
            testDataLogic = new DetectDataLogic();
            productLogic = new ParamProductLogic();
            preProductLogic = new SysPreProductLogic();
            routeLogic = new RouteLogic();
            plcRecipeLogic = new PlcRecipeLogic();
            barcodeRuleLogic = new ParamBarcodeRuleLogic();
            unitProcedureLogic = new UnitProcedureLogic();
            blockBindPackLogic = new RecordBlockBindPackLogic();
            paramItemLogic = new ParamItemLogic();
        }
        //获取已完成数量
        [OperationContract]
        public RetMessage<int> GetFinishCount(string taskOrderNumber,string configId)
        {
            RetMessage<int> retMessage = new(new int());
            int count = recordOrderLogic.GetFinishedCount(taskOrderNumber, configId);
            retMessage.data = count;
            if (count >= 0)
            {
                retMessage.message = "查询成功";
                retMessage.messageType = RetCode.Success;
            }
            else
            {
                retMessage.message = "查询失败";
                retMessage.messageType = RetCode.Error;
            }
            return retMessage;
        }

        [OperationContract]
        public string CheckLink()
        {
            TestData D = new TestData()
            {
                productCode = "t",
                result = "ok",
                data = "nono"
            };
            string j = JsonConvert.SerializeObject(D);
            return "success";
        }

        //通过IP查询设备代码，大工站、小工站、configID         通用工位     Done
        //有些工位是单个IP对应多个工序的，考虑把返回变成列表
        [OperationContract]
        public RetMessage<EquipmentInfo> GetEquipmentInfo(IpParam param)
        {
            SysEquipment equipment = equipmentLogic.GetByIP(param.Ip);
            RetMessage<EquipmentInfo> retMessage = new(new EquipmentInfo());
            if (equipment == null)
            {
                retMessage.messageType = RetCode.Error;
                retMessage.message = $"通过IP:<{param.Ip}>查无此设备";
            }
            else
            {
                //如果equipment.Line为空，会报Object reference not set to an instance of an object.
                EquipmentInfo info = new EquipmentInfo()
                {
                    StationCode = equipment.BigProcedure,
                    Name = equipment.Name,
                    SmallStationCode = equipment.UnitProcedure,
                    ConfigId = equipment.Line.ConfigId,
                    EquipmentCode = equipment.EnCode
                };
                retMessage.message = "";
                retMessage.messageType = RetCode.Success;
                retMessage.data = info;
            }
            return retMessage;
        }

        /// <summary>
        /// 通过工站、线别查询设备信息
        /// </summary>
        /// <param name="station">大工站</param>
        /// <param name="configId">线别</param>
        /// <returns></returns>
        [OperationContract]
        public RetMessage<EquipmentInfo> GetEquipmentInfo_Station(string station,string configId)
        {
            SysEquipment equipment = equipmentLogic.GetByLineStation(station,configId);
            RetMessage<EquipmentInfo> retMessage = new(new EquipmentInfo());
            if (equipment == null)
            {
                retMessage.messageType = RetCode.Error;
                retMessage.message = $"通过工站:<{station}>查无此设备";
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
                retMessage.messageType = RetCode.Success;
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
            if (factoryStatus.IsOnline)
            {
                string ret = APIMethod.Call(Url.LoginUrl, param, configId);
                retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<LoginData>>();
            }
            else   //若不在线，则给默认1级别权限，不校验
            {
                retMessage = new()
                {
                    messageType = RetCode.Success,
                    message = "离线",
                    data = new LoginData()
                    {
                        operatorRoleCode = new List<string> { "1" }
                    }
                };
            }
            RetMessage<UserInfo> message = new(new UserInfo());
            if (retMessage != null && retMessage.messageType == RetCode.Success)
            {
                message.data.Name = retMessage.data.operatorName;
                message.messageType = retMessage.messageType;
                message.message = retMessage.message;
                message.data.Roles = retMessage.data.operatorRoleCode;
                message.data.OperatorNo = retMessage.data.operatorNo;
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

        //登录接口，仅返回角色,给PLC登录使用 PLC使用 Done
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
            if (factoryStatus.IsOnline)
            {
                string ret = APIMethod.Call(Url.LoginUrl, param, configId);
                retMessage = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<LoginData>>();
            }
            else   //若不在线，则给默认1级别权限，不校验
            {
                retMessage = new(new LoginData()
                {
                    operatorRoleCode = new List<string> { "1" }
                }, RetCode.Success, "离线");

            }
            RetMessage<UserInfo> message = new(new UserInfo());
            if (retMessage != null && retMessage.messageType == RetCode.Success)
            {
                message.data.Name = retMessage.data.operatorName;
                message.messageType = retMessage.messageType;
                message.message = retMessage.message;
                message.data.Roles = retMessage.data.operatorRoleCode;
                message.data.OperatorNo = retMessage.data.operatorNo;
            }
            return message;
        } 
        #endregion
        //配方参数获取，单工位按照stationcode查询
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
            //配方从数据库查询，实际没有用子工站
            ParamRecipeItem paramRecipeItem = recipeLogic.Query(param.productPartNo, param.stationCode,param.smallStationCode,configId);
            if (paramRecipeItem == null) { 
                return NewErrorMessage<RecipeData>("未查询到配方");
            }
            RecipeData recipeData = new RecipeData(paramRecipeItem);
            RetMessage<RecipeData> result = new RetMessage<RecipeData>() { 
                data = recipeData,
                messageType = "S",
                message = "配方查询成功"
            };
            return result;
        }

        //可以调用
        //根据箱体码或内控码申请条码，M300工位生成内控码，M490工位生成RESS码给M500工位使用。
        //Getlabel这个接口来改变工单状态，如果满足数量要求了，就会将工单状态设置为4(完成)
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
            if (param.requestCodeType != "packNo" && param.requestCodeType != "reessNo" )
            {
                return  new RetMessage<LabelAndOrderData>( new LabelAndOrderData() )
                {
                    messageType = RetCode.Error,
                    message = $"当前请求类型:<{param.requestCodeType}>,requestCodeType请求类型只能为packNo或reessNo",
                };
            }
            FactoryStatus factoryStatus = GetStatus(configId);
            //访问接口
            RetMessage<LabelAndOrderData> retMessage = new RetMessage<LabelAndOrderData>();
            //上线时候使用当前激活订单的产品
            if (param.requestCodeType == "packNo" || true)
            {
                //查询当前工单
                ParamOrder selectedOrder = paramOrderLogic.GetSelected(configId);
                if (selectedOrder == null)
                {
                    return NewErrorMessage<LabelAndOrderData>("无激活的工单！");
                }
                param.taskOrderNumber = selectedOrder.TaskOrderNumber;
                //目前默认使用01工厂，后续使用配置
                param.plantCode = AppSetting.PlantCode;  //param.plantCode = "Z08"


                //241216记录：向工厂申请内控码，如果工厂数据库已存在对应的箱体和内控码绑定记录，则改为发旧的内控码给线体mes，则orderstart表格可能会存在箱体码和内控码一对多的现象
                //但不影响当前统计逻辑，因为统计的是不重复内控码的个数
                //string ret = APIMethod.Call(Url.GetLabelUrl, param, configId);

                string barcode = "";
                var genResult = barcodeRuleLogic.GenBarcode(configId,out barcode);

                //判断一下生成内控码的长度，长度不为26则修改接口访问结果
                if (barcode.Length != 24)//内控码必须为26位长度
                {
                    retMessage.messageType = RetCode.Error;
                    retMessage.message += $"内控码:<{barcode}>长度不为24位";
                    return retMessage;
                }

                retMessage = new RetMessage<LabelAndOrderData>(new LabelAndOrderData()
                {
                    CodeContent = barcode,
                    ConfigId = configId,
                    ProductPartNo = selectedOrder.ProductPartNo,
                    TaskOrderNumber = selectedOrder.TaskOrderNumber
                }, RetCode.Success,"获取条码成功");

                //插入上线记录
                if (retMessage.messageType == RetCode.Success)
                {
                    int finishedQty = recordOrderLogic.InsertInLine(new RecordOrderStart()
                    {
                        TaskOrderNumber = selectedOrder.TaskOrderNumber,
                        PackNo = param.productCode,
                        ProductCode = retMessage.data.CodeContent
                    }, configId);
                    
                    //判断订单是否已完成，1126，计划10个，做了11个才报无激活工单
                    //if (selectedOrder.PlanQty <= v)
                    bool orderIsDone = finishedQty >= selectedOrder.PlanQty;

                    //看日志实际产量是从0开始的
                    Logger.RunningInfo($"当前工单:<{selectedOrder.TaskOrderNumber}>,计划产量:<{selectedOrder.PlanQty}>,实际产量:<{finishedQty}>,工单完成:<{orderIsDone}>");

                    if (orderIsDone)
                    {
                        Logger.RunningInfo($"当前工单已完成:<{selectedOrder.TaskOrderNumber}>,计划产量:<{selectedOrder.PlanQty}>,实际产量:<{finishedQty}>");
                        selectedOrder.OperatorNo = param.operatorNo;
                        selectedOrder.Flag = "4";
                        paramOrderLogic.Update(selectedOrder,configId);
                    }
                }
                return retMessage;
            }
            /*******************************以下应该没用了*********************************/
            //下线时候使用绑定的记录   用不上了，国标码不通过访问接口生成，直接在指定工位进站的时候触发生成

            {
                //查询内控码绑定的工单
                ProcessBind processBind = processBindLogic.GetByProductCode(param.productCode, configId);
                if (processBind == null)
                {
                    return new RetMessage<LabelAndOrderData>(new LabelAndOrderData())
                    {
                        messageType = RetCode.Error,
                        message = "未查询到内控码绑定的记录",
                    };
                }
                param.taskOrderNumber = processBind.TaskOrderNumber;
               
                //默认使用01
                param.plantCode = AppSetting.PlantCode;// param.plantCode = "Z08"
                //访问工厂获取条码
                string ret = APIMethod.Call(Url.GetLabelUrl, param, configId);

                RetMessage<GetLabelData> apiRet = ret.ToObject<RetMessage<GetLabelData>>();
                retMessage = new RetMessage<LabelAndOrderData>(new LabelAndOrderData(){ CodeContent = apiRet.data.codeContent,})
                {
                    messageType = apiRet.messageType,
                    message = apiRet.message,
                };
                
                //插入下线记录    根据绑定的记录去选择对应数据库
                if (retMessage.messageType == RetCode.Success)
                {
                    recordOrderLogic.InsertOutLine(new RecordOrderPack()
                    {
                        TaskOrderNumber = processBind.TaskOrderNumber,
                        ProductCode = param.productCode,
                        ReessNo = retMessage.data.CodeContent
                    }, processBind.ConfigId); 
                    //删除过程绑定    
                    //processBindLogic.Delete(param.productCode, configId);
                }
                return retMessage;
            }
        }

        //用对应条码获取ProcessBind数据
        [OperationContract]
        public RetMessage<ProcessBind> GetProcessBind(string productCode, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<ProcessBind>("没有给configId参数赋值");
            }

            //当内控码非空时，查询内控码绑定的工单纪录
            //当内控码非空时，从绑定记录表中获取内控码绑定的记录
            ProcessBind processBind = processBindLogic.GetByProductCode(productCode, configId);
            if (processBind == null)
            {
                return new RetMessage<ProcessBind>(new ProcessBind())
                {
                    messageType = RetCode.Error,
                    message = $"未查询到内控码:<{productCode}>绑定的记录",
                };
            }
            return new RetMessage<ProcessBind>(new ProcessBind())
            {
                messageType = RetCode.Success,
                message = "查询成功",
                data = processBind,
            };
        }

        //获取当前工单参数，是从process_bind这份表获取的信息，包括分流器条码，物料编码等信息
        [OperationContract]
        public RetMessage<LabelAndOrderData> GetInfo(string productCode, string configId)
        {   
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<LabelAndOrderData>("没有给configId参数赋值");
            }

            //当内控码为空时，查询当前激活的工单信息
            if(productCode.IsNullOrEmpty())
            {
                //查询当前工单
                ParamOrder selectedOrder = paramOrderLogic.GetSelected(configId);
                if (selectedOrder == null)
                {
                    return NewErrorMessage<LabelAndOrderData>($"线体:<{configId}>无激活的工单！");
                }
                return new RetMessage<LabelAndOrderData>(new LabelAndOrderData())
                {
                    messageType = RetCode.Success,
                    message = "查询成功",
                    data = new LabelAndOrderData()
                    {
                        CodeContent = "",
                        ConfigId = configId,
                        ProductPartNo = selectedOrder.ProductPartNo,
                        TaskOrderNumber = selectedOrder.TaskOrderNumber
                    },
                };
            }

            //当内控码非空时，查询内控码绑定的工单纪录
            //当内控码非空时，从绑定记录表中获取内控码绑定的记录
            ProcessBind processBind = processBindLogic.GetByProductCode(productCode, configId);
            if (processBind == null)
            {
                return new RetMessage<LabelAndOrderData>(new LabelAndOrderData())
                {
                    messageType = RetCode.Error,
                    message = $"未查询到内控码:<{productCode}>绑定的记录",
                };
            }

            return new RetMessage<LabelAndOrderData>(new LabelAndOrderData())
            {
                messageType = RetCode.Success,
                message = "查询成功",
                data = new LabelAndOrderData()
                {
                    CodeContent = productCode,
                    ConfigId = configId,
                    ProductPartNo = processBind.ProductPartNo,
                    TaskOrderNumber = processBind.TaskOrderNumber,
                    ReessNo = processBind.ReessNo,
                    Diverter = processBind.Diverter,
                    GlueTime = processBind.GlueTime
                },
            };
        }

        //入口调的,
        [OperationContract]
        public RetMessage<LabelAndOrderData> CheckOrder(string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<LabelAndOrderData>("没有给configId参数赋值");
            }
            //查询当前工单
            ParamOrder selectedOrder = paramOrderLogic.GetSelected(configId);

            if (selectedOrder == null)
            {
                return NewErrorMessage<LabelAndOrderData>($"线体:<{configId}>无激活的工单！");
            }
            return new RetMessage<LabelAndOrderData>(new LabelAndOrderData())
            {
                messageType = RetCode.Success,
                message = "查询成功",
                data = new LabelAndOrderData()
                {
                    CodeContent = "",
                    ConfigId = configId,
                    ProductPartNo = selectedOrder.ProductPartNo,
                    TaskOrderNumber = selectedOrder.TaskOrderNumber
                },
            };
        }


        //获取Pack信息  自动工位使用  通过pallet码获取内控码  非340工位
        //如果是中转或入口工位，则返回OK结果，让人工判定有没有箱子
        //如果不是中转或入口工位，则根据AGV码查询，如无记录则返回N
        //出现如果没有激活工单，线体仍然在做的产品会报没有激活工单，应改为上线时候检查，其余时候不检查

        [OperationContract]
        public RetMessage<GetPackInfoList> GetPackInfo(GetPackInfoParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<GetPackInfoList>("没有给configId参数赋值");
            }
            if (param.palletNo.IsNullOrEmpty())
            {
                return NewErrorMessage<GetPackInfoList>("没有给palletNo参数赋值");
            }
            //这个是实时绑定,正常来说应该有数据，如果没数据，也查不出物料编码，从而查不出工艺路线，应该当成异常
            List<ProcessBind> processBinds = processBindLogic.GetByPalletNo(param.palletNo, configId);
            if (processBinds == null)
            {
                return NewNgMessage<GetPackInfoList>($"查询agv:<{param.palletNo}>上没有绑定纪录！");
            }
            //用第一个能查到的作为型号
            ParamLocalRoute route = routeLogic.Get(param.stationCode, processBinds[0].ProductPartNo, configId);
            if (route == null)
            {
                return NewErrorMessage<GetPackInfoList>($"托盘:<{param.palletNo}>绑定条码<{processBinds[0].ProductCode}>,该物料编码<{processBinds[0].ProductPartNo}>,工艺路线中不包含{param.stationCode}工序");
            }

            if (route.IsEntrance || route.IsTranshipStation)
            {
                return NewSuccessMessage<GetPackInfoList>($"该工站:<{param.stationCode}>为中转或重新上线工位，不通过AGV:<{param.palletNo}>查询箱体信息");
            }

            GetPackInfoList packList = new GetPackInfoList();
            foreach (var processBind in processBinds)
            {
                GetPackInfoData packInfo = new GetPackInfoData();
                packInfo.productCode = processBind.ProductCode;
                packInfo.position = processBind.Position;
                packList.packInfoList.Add(packInfo);
            }

            return new RetMessage<GetPackInfoList>()
            {
                messageType = RetCode.Success,
                message = "",
                data = packList
            };
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

        //重新AGV工装与箱体  中转工位使用  绑定信息上传

        [OperationContract]
        public RetMessage<nullObject> TranshipStationBindPallet(TranshipStationBindProcessParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给configId参数赋值");
            }

            List<ProcessBind> processBinds = new List<ProcessBind>();
            processBinds.Add(new ProcessBind()
            {
                PalletNo = param.palletNo,
                ProductCode = param.productCode,
                TaskOrderNumber = param.TaskOrderNumber,
                ProductPartNo = param.ProductPartNo,
                Status = param.Status == "OK" || param.Status == "1" ? "OK" : "NG",
                ConfigId = param.ConfigId,
                RepairFlag = param.RepairFlag,
                RepairStations = param.RepairStations,
                CreateTime = DateTime.Now,
                //241203 添加，M330出现没有当前工站的问题
                CurrentStation = param.CurrentStation,

                Diverter = param.Diverter,
                GlueTime = param.GlueTime,
                ReessNo = param.ReessNo,
            });
            long result = processBindLogic.Insert(processBinds, configId);
            Logger.RunningInfo($"方法TranshipStationBindPallet,本地绑定载盘<{param.palletNo}>和内控码<{param.productCode}>完成");

            if (result == 0)
            {
                return NewErrorMessage<nullObject>($"绑定载盘<{param.palletNo}>和内控码<{param.productCode}>失败");
            }
            FactoryStatus factoryStatus = GetStatus(configId);
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;

            BindPalletParam bindPalletParam = new();
            bindPalletParam.CopyField(param);
            
            //在线则上传工厂，agv和内控码要上传到厂级mes的
            if (factoryStatus.IsOnline)
            {
                string ret = APIMethod.Call(Url.BindPalletUrl, bindPalletParam, configId);
                Logger.RunningInfo($"方法TranshipStationBindPallet,绑定载盘<{param.palletNo}>和内控码<{param.productCode}>绑定厂级mes返回:<{ret}>");
                return ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<nullObject>>();
            }
            //不在线，新建未传内容的表，等后续再人工恢复上传。  
            offlineApiLogic.Insert(new RecordOfflineApi()
            {
                Url = Url.BindPalletUrl,
                RequestBody = param.ToJson(),
                ReUpload = 0
            }, configId);
            return NewSuccessMessage<nullObject>("工厂离线中，已离线绑定完成");
        }

        //上线绑定AGV工装与箱体     M300工位使用    绑定信息上传
        //瑞普项目要绑定多个箱体码,因此productCode是列表
        [OperationContract]
        public RetMessage<nullObject> BindPallet(List<BindProcessParam> param,  string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给configId参数赋值");
            }
            if (param == null || param.Count==0)
            {
                return NewErrorMessage<nullObject>("没有给param参数赋值");
            }
            List<ProcessBind> processBinds = new();
            foreach(var item in param)
            {
                processBinds.Add(new ProcessBind()
                {
                    Id = SnowFlakeSingle.Instance.NextId(),
                    PalletNo = item.palletNo,
                    ProductCode = item.productCode,
                    Position = item.position,
                    TaskOrderNumber = item.TaskOrderNumber,
                    ProductPartNo = item.ProductPartNo,
                    Status = item.Status == "OK" || item.Status == "1" ? "OK" : "NG",
                    ConfigId = item.ConfigId,
                    RepairFlag = item.RepairFlag,
                    RepairStations = item.RepairStations,
                    CreateTime = DateTime.Now
                });
            }
            var result = processBindLogic.Insert(processBinds, configId);

            if(result == 0)
            {
                string productCodes = param.Select(it => it.productCode).ToList().Join(",");
                return NewErrorMessage<nullObject>($"绑定载盘<{param[0].palletNo}>和内控码<{productCodes}>失败");
            }
            
            if (GlobalContext.SystemConfig.EnableFactoryMes)
            {
                FactoryStatus factoryStatus = GetStatus(configId);
                SysLine sysLine = lineLogic.GetByConfigId(configId);
                param[0].productionLine = sysLine.EnCode;
                BindPalletParam bindPalletParam = new();
                bindPalletParam.CopyField(param);

                //在线则上传工厂，agv和内控码要上传到厂级mes的
                if (factoryStatus.IsOnline)
                {
                    string ret = APIMethod.Call(Url.BindPalletUrl, bindPalletParam, configId);
                    return ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<nullObject>>();
                }
                //不在线，新建未传内容的表，等后续再人工恢复上传。  
                offlineApiLogic.Insert(new RecordOfflineApi()
                {
                    Url = Url.BindPalletUrl,
                    RequestBody = param.ToJson(),
                    ReUpload = 0
                }, configId);
            }
            return NewSuccessMessage<nullObject>("工厂离线中，已离线绑定完成");
        }

        //下线解绑AGV工装与箱体               M460工位使用
        [OperationContract]
        public RetMessage<nullObject> UnBindPallet(BindPalletParam param,  string configId)
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
            List<ProcessBind> processBinds = processBindLogic.GetByPalletNo(param.palletNo, configId);
            if (processBinds == null)
            {
                return NewSuccessMessage<nullObject>("查无此绑定信息，不用解绑");
            }
            foreach (var processBind in processBinds)
            {
                processBind.PalletNo = "";
                //insert里面会删除  内控码和pallet码重复项
                long v = processBindLogic.Update(processBind, configId);
                if (v == 0)
                {
                    return NewErrorMessage<nullObject>("本地解绑失败");
                }
            }
            
            
            
            if (GlobalContext.SystemConfig.EnableFactoryMes)
            {
                //API接口上传
                SysLine sysLine = lineLogic.GetByConfigId(configId);
                param.productionLine = sysLine.EnCode;
                FactoryStatus factoryStatus = GetStatus(configId);
                if (factoryStatus.IsOnline)
                {
                    return APIMethod.Call(Url.UnBindPalletUrl, param, configId).ToObject<RetMessage<nullObject>>();
                }
                offlineApiLogic.Insert(new RecordOfflineApi()
                {
                    Url = Url.UnBindPalletUrl,
                    RequestBody = param.ToJson(),
                    ReUpload = 0
                }, configId);
            }
            return  NewSuccessMessage<nullObject>("工厂离线中，已离线解绑完成");
        }

        //物料绑定接口         通用
        //会报错
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
            param.GUID = Guid.NewGuid().ToString();
            FactoryStatus factoryStatus = GetStatus(configId);
            if (factoryStatus.IsOnline)
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

        //过程数据接口         有空参数先用“0”“NG”填充
        [OperationContract]
        public RetMessage<nullObject> ProcessUpload(ProcessUploadParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
                return NewErrorMessage<nullObject>("没有给configId参数赋值");
            if (param.productCode.IsNullOrEmpty())
                return NewErrorMessage<nullObject>("没有给productCode参数赋值");
            int v = processUploadLogic.Insert(param, configId);

            if (v == 0)
                return NewErrorMessage<nullObject>("插入本地数据记录出错");

            FactoryStatus factoryStatus = GetStatus(configId);
            //对上传参数中的空项进行填充
            param.processData.FindAll(it => it.paramValue.IsNullOrEmpty()).ForEach(it => { it.paramValue = "0"; it.itemFlag = "NG"; });
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            ProcessUploadParamA processUploadParamA = new ProcessUploadParamA();
            processUploadParamA.CopyField(param);
            processUploadParamA.processData = new List<ProcessA>();
            processUploadParamA.GUID = Guid.NewGuid().ToString();
            foreach (var item in param.processData)
            {
                ProcessA buf = new ProcessA() {
                    paramCode = item.paramCode,
                    paramName = item.paramName,
                    paramValue = item.paramValue,
                    itemFlag = item.itemFlag,
                    maxValue = item.MaxValue,
                    minValue = item.MinValue,
                    decisionType = item.DecisionType,
                    standardValue = item.StandValue
                };
                processUploadParamA.processData.Add(buf);
            }

            if (factoryStatus.IsOnline && GlobalContext.SystemConfig.EnableFactoryMes)
            {
                return APIMethod.Call(Url.ProcessUploadUrl, processUploadParamA, configId).ToObject<RetMessage<nullObject>>();
            }
            offlineApiLogic.Insert(new RecordOfflineApi()
            {
                Url = Url.ProcessUploadUrl,
                RequestBody = param.ToJson(),
                ReUpload = 0
            }, configId);
            return NewSuccessMessage<nullObject>("工厂离线中，已离线上传完成");
        } 
       

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
            //转了一手线体编码
            param.productionLine = sysLine.EnCode;
            //这里调用了一个接口，接口返回
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
                    messageType = RetCode.Error,
                    message = "无数据或查询出错",
                    data = null
                };
            }
            else
            {
                return new RetMessage<PlcParam>()
                {
                    messageType = RetCode.Success,
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

            if (factoryStatus.IsOnline)
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
            if (factoryStatus.IsOnline)
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
            if (factoryStatus.IsOnline)
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
                messageType = RetCode.Error,
                message = message,
                data = new T()
            };
        }
        private static RetMessage<T> NewNgMessage<T>(string message) where T : new()
        {

            return new RetMessage<T>()
            {
                messageType = RetCode.Ng,
                message = message,
                data = new T()
            };
        }
        private static RetMessage<T> NewSuccessMessage<T>(string message) where T : new()
        {
            return new RetMessage<T>(default)
            {
                messageType = RetCode.Success,
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
        //既没有失败也没返回成功
        //ACR测试仪
        [OperationContract]
        public RetMessage<TestUploadRes> UploadACRData(TestData data,string configId)
        {
            Logger.RunningInfo($"接收到线体<{configId}>条码为{data.productCode}的ACR测试仪数据上传请求");
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
            if (v == 0L)
            {
                return NewErrorMessage<TestUploadRes>("上传数据失败");
            }
            return new RetMessage<TestUploadRes>()
            {
                messageType = RetCode.Success,
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
            Logger.RunningInfo($"接收到线体<{configId}>条码为{data.productCode}的EOL测试仪数据上传请求");
            
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<TestUploadRes>("没有给configId参数赋值");
            }
            //1125新增，EOL是別的厂家，防止错码
            if (data.productCode.Length != 26)
            {
                return NewErrorMessage<TestUploadRes>($"条码:<{data.productCode}>长度不符合26长度");
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
            if (v == 0L)
            {
                return NewErrorMessage<TestUploadRes>("上传数据失败");
            }
            return new RetMessage<TestUploadRes>()
            {
                messageType = RetCode.Success,
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
            Logger.RunningInfo($"接收到线体<{configId}>条码为{data.productCode}的OCV测试仪数据上传请求");
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
            if (v == 0L)
            {
                return NewErrorMessage<TestUploadRes>("上传数据失败");
            }
            
            return new RetMessage<TestUploadRes>()
            {
                messageType = RetCode.Success,
                message = "上传成功",
                data = new TestUploadRes()
                {
                    primaryKey = v
                }
            };
            
        }

        //ELEC测试仪
        [OperationContract]
        public RetMessage<TestUploadRes> UploadElectricData(TestData data, string configId)
        {
            Logger.RunningInfo($"接收到线体<{configId}>条码为{data.productCode}的ELEC测试仪数据上传请求");
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<TestUploadRes>("没有给configId参数赋值");
            }
            if (data.productCode.IsNullOrEmpty())
            {
                return NewErrorMessage<TestUploadRes>("没有给productCode参数赋值");
            }

            if (data.productCode.Length != 26)
            {
                return NewErrorMessage<TestUploadRes>($"条码:{data.productCode},长度:{data.productCode.Length}不符合26长度要求");
            }

            RecordTestElectric model = new()
            {
                Id = SnowFlakeSingle.instance.NextId(),
                ProductCode = data.productCode,
                Data = data.data,
                Result = data.result,
                CreateTime = DateTime.Now
            };
            long v = testDataLogic.InsertELEC(model, configId);
            if (v == 0L)
            {
                return NewErrorMessage<TestUploadRes>("上传数据失败");
            }

            return new RetMessage<TestUploadRes>()
            {
                messageType = RetCode.Success,
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
        public RetMessage<TestData> QueryACR(string primaryKey ,string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<TestData>("没有给configId参数赋值");
            }
            RecordTestACR record = testDataLogic.GetACRByKey(long.Parse(primaryKey), configId);
            if (record == null)
            {
                return NewErrorMessage<TestData>("未查询到数据");
            }
            return new RetMessage<TestData>()
            {
                messageType = RetCode.Success,
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
        public RetMessage<TestData> QueryEOL(string primaryKey, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<TestData>("没有给configId参数赋值");
            }
            var record = testDataLogic.GetEOLByKey(long.Parse(primaryKey), configId);
            if (record == null)
            {
                return NewErrorMessage<TestData>("未查询到数据");
            }
            return new RetMessage<TestData>()
            {
                messageType = RetCode.Success,
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
        public RetMessage<TestData> QueryOCV(string primaryKey, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<TestData>("没有给configId参数赋值");
            }
            var record = testDataLogic.GetOCVByKey(long.Parse(primaryKey), configId);
            if (record == null)
            {
                return NewErrorMessage<TestData>("未查询到数据");
            }
            
            return new RetMessage<TestData>()
            {
                messageType = RetCode.Success,
                message = "查询成功",
                data = new TestData()
                {
                    productCode = record.ProductCode,
                    data = record.Data,
                    result = record.Result
                }
            };
        }

        /// <summary>
        /// 没有与仪器通讯，只能查询内控码，上位机需要再本地存储ID，并检查是否重复
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        [OperationContract]
        public RetMessage<TestELECData> QueryElectric(string productCode, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<TestELECData>("没有给configId参数赋值");
            }
            var record = testDataLogic.GetELECByKey(productCode, configId);
            if (record == null)
            {
                return NewErrorMessage<TestELECData>("未查询到数据");
            }

            return new RetMessage<TestELECData>()
            {
                messageType = RetCode.Success,
                message = "查询成功",
                data = new TestELECData()
                {
                    id = record.Id,
                    productCode = record.ProductCode,
                    data = record.Data,
                    result = record.Result
                }
            };
        }

        #endregion

        #region     条码物料相关，这个是查重
        //物料追溯码查重
        [OperationContract]
        public RetMessage<bool> CheckPartBarcode(string partBarcode)
        {
            if (partBarcode.IsNullOrEmpty())
            {
                return NewErrorMessage<bool>("条码不允许为空");
            }
            try
            {  
                //校验分流器
                bool v = partUploadLogic.CheckPartBarcode(partBarcode);
                return new RetMessage<bool>
                {
                    data = v,
                    message = v ? "查重通过" : "查重不通过",
                    messageType = RetCode.Success,
                };
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new RetMessage<bool>
                {
                    data = false,
                    message = "查重出错" + E.Message,
                    messageType = RetCode.Error,
                };
            }
        }
        //通过物料条码，查产品条码
        [OperationContract]
        public RetMessage<RecordPartUpload> GetProductCode(string partBarcode, string configId)
        {
            if (partBarcode.IsNullOrEmpty())
            {
                return NewErrorMessage<RecordPartUpload>("条码不允许为空");
            }
            if (partBarcode.Length != 31)
            {
                return NewErrorMessage<RecordPartUpload>("条码长度不为31");
            }
            return partUploadLogic.GetProductCode(partBarcode,configId);

        }
        //查物料条码绑定的是否是对应的产品条码
        [OperationContract]
        public RetMessage<bool> CheckProductCode(string productCode, string partBarcode, string configId)
        {
            if (partBarcode.IsNullOrEmpty())
            {
                return NewErrorMessage<bool>("物料条码不允许为空");
            }
            if (partBarcode.Length != 31)
            {
                return NewErrorMessage<bool>("物料条码长度不为31");
            }
            if (productCode.IsNullOrEmpty())
            {
                return NewErrorMessage<bool>("内控条码不允许为空");
            }
            return partUploadLogic.CheckProductCode(productCode, partBarcode, configId);
        }

        #endregion







        #region    //预处理相关接口
        //通过线体查询产品列表
        [OperationContract]
        public RetMessage<List<Product>> Pre_GetProductList(string configId) {
            //内部数据绑定、、、、、、
            if (configId.IsNullOrEmpty())
            {
                configId = "1";
            }
            List<ParamRecipe> paramRecipes = productLogic.GetList(configId);
            if (paramRecipes != null && paramRecipes.Count > 0)
            {
                List<Product> products = new List<Product>();
                foreach (var item in paramRecipes)
                {
                    products.Add(new Product()
                    {
                        lineId = configId,
                        productPartNo = item.ProductPartNo,
                        productDescription = item.ProductDescription,
                        sapCustomerProjNo = item.SapCustomerProjNo,
                    });
                }
                return new RetMessage<List<Product>>
                {
                    data = products,
                    message = "查询成功",
                    messageType = RetCode.Success,
                };
            }
            else
            {
                return NewSuccessMessage<List<Product>>("当前线体没有产品");
            }

        }

        //为指定线体的单个工序选择指定的产品信息
        //这里使SysPreSelectProduct的isenable置为真，只在M305热铆机那里调用
        [OperationContract]
        public RetMessage<nullObject> Pre_SelectProduct(Product product,string station,string configId, string user)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给configId参数赋值");
            }
            SysPreSelectProduct model = new() { 
                Station = station,
                ProductPartNo = product.productPartNo,
                ProductDescription = product.productDescription,
                ProductLineId = product.lineId,
                SapCustomerProjNo= product.sapCustomerProjNo,
                CreateUser = user,
            };
            int v = preProductLogic.Insert(model,configId);
            if (v > 0)
            {
                return NewSuccessMessage<nullObject>("选中产品成功");
            }
            else
            {
                return NewErrorMessage<nullObject>("选中产品失败");
            }
        }

        //查询单工序选中的产品
        [OperationContract]
        public RetMessage<Product> Pre_GetSelected(string station, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<Product>("没有给configId参数赋值");
            }
            SysPreSelectProduct sysPreSelectProduct = preProductLogic.Query(station,configId);
            if (sysPreSelectProduct == null)
            {
                return NewErrorMessage<Product>("没有查询到选中记录");
            }
            else
            {
                return new RetMessage<Product>()
                {
                    data = new Product()
                    {
                        productPartNo = sysPreSelectProduct.ProductPartNo,
                        productDescription = sysPreSelectProduct.ProductDescription,
                        lineId = sysPreSelectProduct.Line.ConfigId,
                        sapCustomerProjNo = sysPreSelectProduct.SapCustomerProjNo
                    },
                    message = "查询成功",
                    messageType = RetCode.Success
                };
            }
        }

        //根据选中的产品查询对应的工序参数配方
        [OperationContract]
        public RetMessage<RecipeData> Pre_GetRecipe(Product product,string stationCode) {
            if(product == null || stationCode.IsNullOrEmpty())
            {
                return NewErrorMessage<RecipeData>("传参不正确");
            }

            if (product.lineId.IsNullOrEmpty())
            {
                return NewErrorMessage<RecipeData>("产品信息不完整");
            }
            if (product.productPartNo.IsNullOrEmpty())
            {
                return NewErrorMessage<RecipeData>("产品代码不能为空");
            }
            //配方从数据库查询
            RetMessage<RecipeData> res = new RetMessage<RecipeData>();
            ParamRecipeItem paramRecipeItem = recipeLogic.Query(product.productPartNo, stationCode, "", product.lineId);
            if (paramRecipeItem == null)
            {
                return NewErrorMessage<RecipeData>("未查询到配方");
            }
            RecipeData recipeData = new(paramRecipeItem);
            RetMessage<RecipeData> result = new RetMessage<RecipeData>()
            {
                data = recipeData,
                messageType = "S",
                message = "配方查询成功"
            };
            //Logger.RunningInfo(result.ToJson());
            return result;
        }

        //热铆机数据上传      自动工位上传后，人工工位再根据条码进行查询拿出数据
        [OperationContract]
        public RetMessage<nullObject> Pre_UploadRivetData(HotRivetRecord param, string configId)
        {   try
            {
                Logger.RunningInfo($"接收到线体<{configId}>条码为{param.productCode}的自动热铆临时上传数据");
                if (configId.IsNullOrEmpty())
                {
                    return NewErrorMessage<nullObject>("没有给configId参数赋值");
                }
                string jsonData = param.data.ToJson();
                RecordHotRivetData model = new()
                {
                    Id = SnowFlakeSingle.instance.NextId(),
                    ProductCode = param.productCode,
                    BatchCode = param.batchCode,
                    Station = param.station,
                    Data = jsonData,
                    Result = param.result,
                    CreateTime = DateTime.Now
                };
                long v = preProductLogic.InsertRivet(model, configId);
                if (v == 0L)
                {
                    Logger.ErrorInfo($"接收到线体<{configId}>条码为{param.productCode}的自动热铆临时上传数据,上传失败");
                    return NewErrorMessage<nullObject>("上传数据失败");
                }
                return NewSuccessMessage<nullObject>("上传成功");
            }
            catch (Exception e)
            {
                Logger.ErrorInfo($"接收到线体<{configId}>条码为{param.productCode}的自动热铆临时上传数据,上传异常",e);
                return NewErrorMessage<nullObject>("上传数据失败");
            }
        }

        [OperationContract]
        public RetMessage<HotRivetRecord> Pre_QueryRivetData(string productCode,string configId) {
            if (configId.IsNullOrEmpty()||productCode.IsNullOrEmpty())
            {
                return NewErrorMessage<HotRivetRecord>("没有给configId或条码参数赋值");
            }
            RecordHotRivetData record = preProductLogic.QueryHotRivet(productCode, configId);
            if (record == null)
            {
                return NewSuccessMessage<HotRivetRecord>("无相关数据");
            }
            List<HotRivetData> hotRivetDatas = record.Data.ToObject<List<HotRivetData>>();
            return new RetMessage<HotRivetRecord>()
            {
                message = "查询成功",
                messageType = RetCode.Success,
                data = new HotRivetRecord()
                {
                    productCode = record.ProductCode,
                    batchCode = record.BatchCode,
                    station = record.Station,
                    result = record.Result,
                    data = hotRivetDatas
                }
            };
        }

        #endregion

        #region   PLC配方上传下载
        [OperationContract]
        public RetMessage<nullObject> PLC_Upload(PlcRecipeData recipe,string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<nullObject>("没有给configId参数赋值");
            }
            Logger.RunningInfo($"接收到线体<{configId}>PLC{recipe.plc}的{recipe.product}的配方上传请求");
            ParamPlcRecipe model = new() { 
                  Product = recipe.product,
                  Content = recipe.content,
                  Plc = recipe.plc,
            };
            long v = plcRecipeLogic.Insert(model, configId);
            if (v == 0L)
            {
                return NewErrorMessage<nullObject>("上传数据失败");
            }
            return NewSuccessMessage<nullObject>("上传数据成功");
        }

        [OperationContract]
        public RetMessage<PlcRecipeData> PLC_Download(string product,string plc,string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<PlcRecipeData>("没有给configId参数赋值");
            }
            var record = plcRecipeLogic.Get(product,plc,configId);
            if (record == null)
            {
                return NewErrorMessage<PlcRecipeData>("未查询到数据");
            }

            return new RetMessage<PlcRecipeData>()
            {
                messageType = RetCode.Success,
                message = "查询成功",
                data = new PlcRecipeData()
                {
                    product = product,
                    plc = plc,
                    content = record.Content
                }
            };
        }
        #endregion

        #region 2024-04-11 增加查询过程数据不良信息
        [OperationContract]
        public RetMessage<ProcessUploadParam> RepairGetProcessNGData(string productCode,string currentStation, string configId)
        {
            if (configId.IsNullOrEmpty() || productCode.IsNullOrEmpty())
            {
                return NewErrorMessage<ProcessUploadParam>("没有给configId或条码参数赋值");
            }
            //内控码与当前站
            //确实是从processbind中读取的多个返修工站
            ProcessNGLogic processNGLogic = new ProcessNGLogic();
            var result = processNGLogic.GetProcessNGData(productCode, currentStation, configId);

            if (result == null)
            {
                return NewSuccessMessage<ProcessUploadParam>("无相关数据");
            }
            if (result.totalFlag == "NG")
            {
                if (result.processData == null)
                    return new RetMessage<ProcessUploadParam>()
                    {
                        message = "未查询到NG数据",
                        messageType = RetCode.Error,
                        data = result
                    };
                return new RetMessage<ProcessUploadParam>()
                {
                    message = "查询成功",
                    messageType = RetCode.Success,
                    data = result
                };
            }
            else
            {
                return NewSuccessMessage<ProcessUploadParam>("上一站过程数据OK");
            }
        }

        /// <summary>
        /// 返修时，查询已组装的过程数据
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="currentStation"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        [OperationContract]
        public RetMessage<List<Process>> RepairGetProcessData(string productCode, string currentStation, string configId)
        {
            if (configId.IsNullOrEmpty() || productCode.IsNullOrEmpty())
            {
                return NewErrorMessage<List<Process>>("没有给configId或条码参数赋值");
            }
            ProcessNGLogic processNGLogic = new ProcessNGLogic();
            var lst_process = processNGLogic.GetProcessData(productCode, currentStation, configId);
            List<Process> result = new List<Process>();
            foreach (var item in lst_process)
            {
                Process process = new Process
                {
                    DecisionType = item.DecisionType,
                    itemFlag = item.ItemFlag,
                    MaxValue = item.MaxValue,
                    MinValue = item.MinValue,
                    paramCode = item.ParamCode,
                    paramName = item.ParamName,
                    ParamType = item.ParamType,
                    paramValue = item.ParamValue,
                    SetValue = item.SetValue,
                    StandValue = item.StandValue,
                    UnitOfMeasure = item.UnitOfMeasure,
                };
                result.Add(process);
            }
            return new RetMessage<List<Process>> { data = result, message = "查询成功", messageType = RetCode.Success };
        }

        /// <summary>
        /// 返修时，查询已组装的物料数据
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="currentStation"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        [OperationContract]
        public RetMessage<List<Entity.DTO.ApiParam.Part>> RepairGetPartData(string productCode, string currentStation, string configId)
        {
            if (configId.IsNullOrEmpty() || productCode.IsNullOrEmpty())
            {
                return NewErrorMessage<List<Entity.DTO.ApiParam.Part>>("没有给configId或条码参数赋值");
            }
            ProcessNGLogic processNGLogic = new ProcessNGLogic();
            var lst_part = processNGLogic.GetPartData(productCode, currentStation, configId);
            List<Entity.DTO.ApiParam.Part> result = new List<Entity.DTO.ApiParam.Part>();
            foreach (var item in lst_part)
            {
                Entity.DTO.ApiParam.Part part = new Entity.DTO.ApiParam.Part
                {
                    partNumber = item.PartNumber,
                    partBarcode = item.PartBarcode,
                    partDescription = item.PartDescription,
                    traceType = item.TraceType,
                    uom = item.Uom,
                    usageQty = item.UsageQty,
                    
                };
                result.Add(part);
            }
            return new RetMessage<List<Entity.DTO.ApiParam.Part>> { data = result, message = "查询成功", messageType = RetCode.Success };
        }

        #endregion


        #region 2024-06-15增加PACK拆解与PACK重组接口
        /// <summary>
        /// Pack拆解
        /// </summary>
        /// <param name="param">解绑数据</param>
        /// <param name="configId">线别</param>
        /// <returns></returns>
        [OperationContract]
        public RetMessage<nullObject> UnbindPack(DisAssembleParam param , string configId)
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
            param.GUID = Guid.NewGuid().ToString();
            ParamRepairLogic logic = new ParamRepairLogic();
            int v = logic.Insert(param, configId);
            if (v == 0)
            {
                return NewErrorMessage<nullObject>("插入本地数据记录出错");
            }
            FactoryStatus factoryStatus = GetStatus(configId);

            //要删除物料绑定记录，没删除process_bind的数据
            RecordPartUploadLogic l = new RecordPartUploadLogic();
            foreach (var e in param.partList)
            {
                bool ret = l.UnBindPartBarcode(e.partBarcode);
                Logger.RunningInfo($"物料条码:<{e.partBarcode}>解绑,结果:<{ret}>");
            }

            if (factoryStatus.IsOnline)
            {
                return APIMethod.Call(Url.UnbindMaterial, param, configId).ToObject<RetMessage<nullObject>>();
                //return APIMethod.Call(Url.UnbindPackUrl, param, configId).ToObject<RetMessage<nullObject>>();
            }
            offlineApiLogic.Insert(new RecordOfflineApi()
            {
                Url = Url.UnbindMaterial,
                RequestBody = param.ToJson(),
                ReUpload = 0
            }, configId);
            return NewSuccessMessage<nullObject>("工厂离线中，已离线上传完成");
        }

        /// <summary>
        /// Pack重组，拆解后重新建立绑定关系
        /// </summary>
        /// <param name="param">请求数据</param>
        /// <param name="configId">线别</param>
        /// <returns></returns>
        [OperationContract]
        public RetMessage<nullObject> BindPack(AssembleParam param, string configId)
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
            FactoryStatus factoryStatus = GetStatus(configId);

            if (factoryStatus.IsOnline)
            {
                return APIMethod.Call(Url.BindPackUrl, param, configId).ToObject<RetMessage<nullObject>>();
            }
            offlineApiLogic.Insert(new RecordOfflineApi()
            {
                Url = Url.BindPackUrl,
                RequestBody = param.ToJson(),
                ReUpload = 0
            }, configId);
            return NewSuccessMessage<nullObject>("工厂离线中，已离线上传完成");

        }
        #endregion

        #region 2024-06-25增加返修房查询返修信息，记录返修信息
        //查应该返修的工站
        //241129更新了该接口，改为搜过站表
        [OperationContract]
        public RetMessage<FinishedStation> GetFinishedStation(string productCode)
        {
            ProcessNGLogic processNGLogic = new ProcessNGLogic();
            var result = processNGLogic.GetFinishedStation(productCode);
            if (result != null)
            {
                return new RetMessage<FinishedStation>()
                {
                    message = "查询成功！",
                    messageType = RetCode.Success,
                    data = result
                };
            }
            return NewErrorMessage<FinishedStation>($"GetFinishStation查询失败,内控码:<{productCode}>!");
        }

        //这个接口就只允许一次登记返修多个站
        [OperationContract]
        public RetMessage<nullObject> RegisterRepair(string configId,string productCode,List<string> stations)
        {
            ProcessBindLogic bindLogic = new ProcessBindLogic();
            ProcessBind processBind = bindLogic.GetByProductCode(productCode, configId);
            if (processBind == null)
            {
                return NewErrorMessage<nullObject>("返修登记失败！该内控码不在绑定信息表中！");
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < stations.Count; i++)
            {
                sb.Append(stations[i]);
                if (i < stations.Count - 1)
                {
                    sb.Append(",");
                }
            }
            processBind.RepairFlag = "1";
            processBind.RepairStations = sb.ToString();
            int v = bindLogic.Update(processBind, configId);
            if (v != 0)
            {
                return new RetMessage<nullObject>()
                {
                    message = "返修登记成功！",
                    messageType = RetCode.Success,
                };
            }
            else
            {
                return NewErrorMessage<nullObject>("返修登记绑定信息表更新失败！");
            }
        }

        //这个接口允许叠加上传，不覆盖
        [OperationContract]
        public RetMessage<nullObject> RegisterRepairAppend(string configId, string productCode, string station)
        {
            ProcessBindLogic bindLogic = new ProcessBindLogic();
            ProcessBind processBind = bindLogic.GetByProductCode(productCode, configId);
            if (processBind == null)
            {
                return NewErrorMessage<nullObject>("返修登记失败！该内控码不在绑定信息表中！");
            }
            StringBuilder sb = new StringBuilder();

            List<string> newRepairStations = processBind.RepairStations.Split(",").Distinct().ToList();
            newRepairStations.Add(station);
            string newRepairStationString = string.Join(",", newRepairStations.ToArray());

            processBind.RepairFlag = "1";
            processBind.RepairStations = newRepairStationString;
            int v = bindLogic.Update(processBind, configId);
            if (v != 0)
            {
                return new RetMessage<nullObject>()
                {
                    message = $"工站<{newRepairStationString}>返修登记成功！",
                    messageType = RetCode.Success,
                };
            }
            else
            {
                return NewErrorMessage<nullObject>($"工站<{newRepairStationString}>返修登记绑定信息表更新失败！");
            }
        }
        //查询返修信息
        [OperationContract]
        public RetMessage<RepairInfoData> QueryRepairInfo(string productCode,string stationCode, string configId)
        {
            ProcessNGLogic processNGLogic = new ProcessNGLogic();
            var data =  processNGLogic.QueryRepairInfo(productCode, stationCode, configId);
            if (data != null)
            {
                return new RetMessage<RepairInfoData>
                {
                    message = "查询返修登记信息成功！",
                    messageType = RetCode.Success,
                    data = data
                };
            }
            return NewErrorMessage<RepairInfoData>("查询返修登记信息失败！");
        }

        //上传返修登记信息
        [OperationContract]
        public RetMessage<nullObject> UploadRepairInfo(RepairInfoData param, string configId)
        {
            ProcessNGLogic processNGLogic = new ProcessNGLogic();
            var msg = processNGLogic.UploadRepairInfo(param, configId);
            if (msg == "OK")
            {
                return new RetMessage<nullObject>
                {
                    message = "上传返修登记信息成功！",
                    messageType = RetCode.Success,
                };
            }
            return new RetMessage<nullObject>
            {
                message = msg,
                messageType = RetCode.Error
            };
        }

        /// <summary>
        /// GetMarking
        /// </summary>
        /// <param name="param">请求数据</param>
        /// <param name="configId">线别</param>
        /// <returns></returns>
        [OperationContract]
        public RetMessage<GetMarkingResponse> GetMarking(GetMarkingParamIn param, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<GetMarkingResponse>("没有给configId参数赋值");
            }
            if (param.stationCode.IsNullOrEmpty())
            {
                return NewErrorMessage<GetMarkingResponse>("没有给stationCode参数赋值");
            }
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;

            FactoryStatus factoryStatus = GetStatus(configId);
            if (factoryStatus.IsOnline)
            {
                return APIMethod.Call(Url.GetMarking, param, configId).ToObject<RetMessage<GetMarkingResponse>>();
            }
            return NewErrorMessage<GetMarkingResponse>("查询mark信息失败");
        }

        /// <summary>
        /// GetCellVoltage
        /// </summary>
        /// <param name="param">请求数据</param>
        /// <param name="configId">线别</param>
        /// <returns></returns>
        [OperationContract]
        public RetMessage<GetCellVoltageData> GetCellVoltage(GetCellVoltageParamIn param, string configId)
        {
            if (configId.IsNullOrEmpty())
            {
                return NewErrorMessage<GetCellVoltageData>("没有给configId参数赋值");
            }
            if (param.stationCode.IsNullOrEmpty())
            {
                return NewErrorMessage<GetCellVoltageData>("没有给stationCode参数赋值");
            }
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            param.callTime = ExtDateTime.GetTimeStamp(DateTime.Now);
            FactoryStatus factoryStatus = GetStatus(configId);
            if (factoryStatus.IsOnline)
            {
                return APIMethod.Call(Url.GetCellVoltage, param, configId).ToObject<RetMessage<GetCellVoltageData>>();
            }
            return NewErrorMessage<GetCellVoltageData>("查询ocv信息失败");
        }

        /// <summary>
        /// 上传自放电数据
        /// </summary>
        /// <param name="param">请求数据</param>
        /// <param name="configId">线别</param>
        /// <returns></returns>
        /// 
        [OperationContract]
        public RetMessage<nullObject> UploadCellVoltage(selfDischargeParamIn param, string configId)
        {
            try
            {
                if (configId.IsNullOrEmpty())
                {
                    return NewErrorMessage<nullObject>("没有给configId参数赋值");
                }

                RecordSelfDischargeLogic logic = new RecordSelfDischargeLogic();
                var ret = logic.Insert(param, configId);
                if(ret)
                    return NewSuccessMessage<nullObject>("插入自放电成功");
                else
                    return  NewErrorMessage<nullObject>("插入自放电失败");
            }
            catch (Exception e)
            {
                return NewErrorMessage<nullObject>("插入自放电失败");
            }
        }
        #endregion

        /**********************************************************************/
        
        

        public RetMessage<OutStationData> CellOutStation(OutStationParam param, PartUploadParam part, ProcessUploadParam process, string configId)
        {
            if (configId.IsNullOrEmpty())
                return NewErrorMessage<OutStationData>("没有给configId参数赋值");
            if (param.productCode.IsNullOrEmpty())
                return NewErrorMessage<OutStationData>("没有给productCode参数赋值");

            RecordOutStation recordOutStation = new RecordOutStation();
            recordOutStation.CopyField(param);
            recordOutStation.TaskOrderNumber = param.taskOrderNumber;

            int v = outStationLogic.Insert(recordOutStation, configId); //这里是插入outstation表
            if (v == 0)
                return NewErrorMessage<OutStationData>("插入本地出站数据记录出错");
            v = cellPartUploadLogic.Insert(part, configId);
            if (v == 0)
                return NewErrorMessage<OutStationData>("插入本地物料数据记录出错");
            v = cellProcessUploadLogic.Insert(process, configId);
            if (v == 0)
                return NewErrorMessage<OutStationData>("插入本地过程数据记录出错");
            
            paramItemLogic.GetNgCodes(param.smallStationCode, process.processData, configId,out List<string> ngCodes);

            RetMessage<OutStationData> partRet = new RetMessage<OutStationData>();
            RetMessage<OutStationData> outRet = new RetMessage<OutStationData>();

            if (GlobalContext.SystemConfig.EnableFactoryMes)
            {
                if (part != null)
                {
                    foreach (var e in part.partList)
                    {
                        UpAssembleDataParam mesParam = new UpAssembleDataParam(param, e);
                        var partMesRet = APIMethod.Call(Url.UpAssembleData, mesParam, configId).ToObject<MesRet>();
                    }
                }
                UploadData_FParam uploadData_FParam = new UploadData_FParam(param, process.processData, ngCodes);
                var mesRet = APIMethod.Call(Url.UploadData_F, uploadData_FParam, configId).ToObject<MesRet>();

                outRet = new RetMessage<OutStationData>(mesRet);
                return outRet;
            }
            else
            {
                offlineApiLogic.Insert(new RecordOfflineApi()
                {
                    Url = Url.OutStationUrl,
                    RequestBody = param.ToJson(),
                    ReUpload = 0
                }, configId);
                return NewSuccessMessage<OutStationData>("工厂离线中，已离线上传完成");
            }
        }
        
        public RetMessage<OutStationData> BlockOutStation(OutStationParam param, PartUploadParam part, ProcessUploadParam process, List<BindProduct> bindProducts, string configId)
        {
            if (configId.IsNullOrEmpty())
                return NewErrorMessage<OutStationData>("没有给configId参数赋值");
            if (param.productCode.IsNullOrEmpty())
                return NewErrorMessage<OutStationData>("没有给productCode参数赋值");
            RecordOutStation recordOutStation = new RecordOutStation();
            recordOutStation.CopyField(param);
            recordOutStation.TaskOrderNumber = param.taskOrderNumber;

            FactoryStatus factoryStatus = GetStatus(configId);
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            int v = outStationLogic.Insert(recordOutStation, configId); //这里是插入outstation表
            if (v == 0)
                return NewErrorMessage<OutStationData>("插入本地数据记录出错");

            v = blockPartUploadLogic.Insert(part, configId);
            if (v == 0)
                return NewErrorMessage<OutStationData>("插入本地物料数据记录出错");
            v = blockProcessUploadLogic.Insert(process, configId);
            if (v == 0)
                return NewErrorMessage<OutStationData>("插入本地过程数据记录出错");

            paramItemLogic.GetNgCodes(param.smallStationCode, process.processData, configId, out List<string> ngCodes);

            var unitProcedure = unitProcedureLogic.GetByStation(param.stationCode, configId);
            if (unitProcedure.OutStationProductType == null)
                return NewErrorMessage<OutStationData>($"{unitProcedure.Encode}未设置出站产品类型");

            RetMessage<OutStationData> partRet = new RetMessage<OutStationData>();
            RetMessage<OutStationData> outRet = new RetMessage<OutStationData>();

            if (GlobalContext.SystemConfig.EnableFactoryMes)
            {
                if (part != null)
                {
                    foreach (var e in part.partList)
                    {
                        UpAssembleDataParam mesParam = new UpAssembleDataParam(param,e);
                        var partMesRet = APIMethod.Call(Url.UpAssembleData, mesParam, configId).ToObject<MesRet>();
                    }
                }
                if (unitProcedure.InStationProductType == "Cell")
                {
                    List<RecordCellBindBlock> items = new List<RecordCellBindBlock>();
                    foreach (var e in bindProducts)
                    {
                        RecordCellBindBlock item = new RecordCellBindBlock();
                        item.BlockBarcode = param.productCode;
                        item.CellBarcode = e.productCode;
                        item.Position = e.position;
                        items.Add(item);
                    }
                    cellBindBlockLogic.InsertSplitTableList(items, configId);

                    UploadData_MZParam uploadData_MZParam = new UploadData_MZParam(param, process.processData, ngCodes, bindProducts);
                    var mesRet = APIMethod.Call(Url.UploadData_MZ, uploadData_MZParam, configId).ToObject<MesRet>();
                    outRet = new RetMessage<OutStationData>(mesRet);
                    return outRet;
                }
                else
                {
                    UploadData_FParam uploadData_FParam = new UploadData_FParam(param,process.processData, ngCodes);
                    var mesRet = APIMethod.Call(Url.UploadData_F, uploadData_FParam, configId).ToObject<MesRet>();
                    outRet = new RetMessage<OutStationData>(mesRet);
                    return outRet;
                }
            }
            else
            {
                offlineApiLogic.Insert(new RecordOfflineApi()
                {
                    Url = Url.OutStationUrl,
                    RequestBody = param.ToJson(),
                    ReUpload = 0
                }, configId);
                return NewSuccessMessage<OutStationData>("工厂离线中，已离线上传完成");
            }
        }

        /// <summary>
        /// 出站  出站结果绑定  本地和API
        /// </summary>出站时携带了分流器则对对应参数进行更新
        /// <param name="diverter"> 用于更新绑定信息的分流器条码</param>
        /// <returns></returns>
        [OperationContract]
        public RetMessage<OutStationData> PackOutStation(OutStationParam param, PartUploadParam part,ProcessUploadParam process, List<BindProduct> bindProducts, string configId)
        {
            if (configId.IsNullOrEmpty())
                return NewErrorMessage<OutStationData>("没有给configId参数赋值");
            if (param.productCode.IsNullOrEmpty())
                return NewErrorMessage<OutStationData>("没有给productCode参数赋值");

            //查绑定表
            ProcessBind processBind = processBindLogic.GetByProductCode(param.productCode, configId);
            if (processBind == null)
                return NewErrorMessage<OutStationData>($"条码:<{param.productCode}>未查到绑定数据");

            //出站时更新绑定表的当前工站      //此处出站没有更新绑定表的结果
            processBind.CurrentStation = param.stationCode;

            //这里有出站的返修逻辑
            if (processBind.RepairFlag == "1")
            {
                //出站时，如果为返修工站，则修改绑定表，清除返修标识
                //param.productStatus = "REWORK"; //出站时，不需要REWORK了吧
                List<string> strArray = new List<string>(processBind.RepairStations.Split(","));
                strArray.Remove(param.stationCode);
                if (strArray.Count == 0)
                {
                    processBind.RepairFlag = "0";
                    processBind.RepairStations = "";
                    processBind.Status = param.productStatus;
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < strArray.Count; i++)
                    {
                        sb.Append(strArray[i]);
                        if (i < strArray.Count - 1)
                        {
                            sb.Append(",");
                        }
                    }
                    processBind.RepairFlag = "1";
                    processBind.Status = "NG";//仍然还有待返修的工站，依旧是NG产品
                    processBind.RepairStations = sb.ToString();
                }
            }
            else
            {
                processBind.Status = param.productStatus;
            }

            RecordOutStation recordOutStation = new RecordOutStation();
            recordOutStation.CopyField(param);
            recordOutStation.TaskOrderNumber = processBind.TaskOrderNumber;

            //出站的时候调用了processBind的删除过期数据，可能会导致返修数据丢失
            processBindLogic.RemoveOldData(configId);

            FactoryStatus factoryStatus = GetStatus(configId);
            SysLine sysLine = lineLogic.GetByConfigId(configId);
            param.productionLine = sysLine.EnCode;
            int v = outStationLogic.Insert(recordOutStation, configId); //这里是插入outstation表
            if (v == 0)
                return NewErrorMessage<OutStationData>("插入本地数据记录出错");

            var unitProcedure = unitProcedureLogic.GetByStation(param.stationCode,configId);
            if(unitProcedure.OutStationProductType == null)
                return NewErrorMessage<OutStationData>($"{unitProcedure.Encode}未设置出站产品类型");

            List<RecordBlockBindPack> items = new List<RecordBlockBindPack>();
            foreach (var e in bindProducts)
            {
                RecordBlockBindPack item = new RecordBlockBindPack();
                item.PackBarcode = param.productCode;
                item.BlockBarcode = e.productCode;
                item.Position = e.position;
                items.Add(item);
            }
            if (part != null && part.partList != null && part.partList.Count > 0)
            {
                v = partUploadLogic.Insert(part, configId);
                if (v == 0)
                    return NewErrorMessage<OutStationData>("插入本地物料数据记录出错");
            }
            if (process != null && process.processData != null && process.processData.Count > 0)
            {
                v = processUploadLogic.Insert(process, configId);
                if (v == 0)
                    return NewErrorMessage<OutStationData>("插入本地过程数据记录出错");
            }

            paramItemLogic.GetNgCodes(param.smallStationCode, process.processData, configId, out List<string> ngCodes);
            //要看是否屏蔽了厂级mes
            if (factoryStatus.IsOnline && GlobalContext.SystemConfig.EnableFactoryMes)
            {
                RetMessage<OutStationData> partRet = new RetMessage<OutStationData>();
                RetMessage<OutStationData> outRet = new RetMessage<OutStationData>();
                if (part != null)
                {
                    foreach (var e in part.partList)
                    {
                        UpAssembleDataParam mesParam = new UpAssembleDataParam(param, e);
                        var partMesRet = APIMethod.Call(Url.UpAssembleData, mesParam, configId).ToObject<MesRet>();
                    }
                }
                if (bindProducts != null && bindProducts.Count>0)
                {
                    //
                    UploadData_MZParam uploadData_MZParam = new UploadData_MZParam(param,process.processData,ngCodes,bindProducts);
                    var outMesRet = APIMethod.Call(Url.UploadData_MZ, uploadData_MZParam, configId).ToObject<MesRet>();
                    outRet = new RetMessage<OutStationData>(outMesRet);
                }
                else
                {
                    UploadData_FParam uploadData_FParam = new UploadData_FParam(param, process.processData, ngCodes);
                    var outMesRet = APIMethod.Call(Url.UploadData_F, uploadData_FParam, configId).ToObject<MesRet>();
                    outRet = new RetMessage<OutStationData>(outMesRet);
                }

                if (outRet != null && outRet.messageType == "S")
                {
                    //processBind.Status = param.productStatus;
                    //出站会重新绑定一下到信息绑定表ProcessBind
                    processBindLogic.Update(processBind, configId);
                }
                return outRet;
            }
            else
            {
                processBindLogic.Update(processBind, configId);
                offlineApiLogic.Insert(new RecordOfflineApi()
                {
                    Url = Url.OutStationUrl,
                    RequestBody = param.ToJson(),
                    ReUpload = 0
                }, configId);
                return NewSuccessMessage<OutStationData>("工厂离线中，已离线上传完成");
            }
        }

        
        //进站 
        [OperationContract]
        public RetMessage<InStationData> PackInStation(InStationParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
                return NewErrorMessage<InStationData>("没有给configId参数赋值");
            if (param.productCode.IsNullOrEmpty())
                return NewErrorMessage<InStationData>("没有给productCode参数赋值");
            RetMessage<InStationData> retMessage = new RetMessage<InStationData>();
            try
            {
                SysLine sysLine = lineLogic.GetByConfigId(configId);
                param.productionLine = sysLine.EnCode;
                #region 内部过站
                //先查询路线，如果路线中无此工位，则直接放行，绑定表中查的工艺路线
                List<ParamLocalRoute> paramLocalRoutes = routeLogic.Get(param.ProductPartNo, configId);
                //查询当前工位的工位路线
                int routStep = paramLocalRoutes.FindIndex(it => it.StationCode == param.stationCode);
                if (routStep == -1)
                {
                    Logger.RunningInfo($"{param.productCode}在{param.stationCode}工位过站失败，工艺路线中不包含{param.stationCode}工序");
                    return NewNgMessage<InStationData>($"{param.productCode}在{param.stationCode}工位过站失败，工艺路线中不包含{param.stationCode}工序");
                }
                //如果是入口工站，直接厂级进站
                if (paramLocalRoutes[routStep].IsEntrance)
                {
                    //工厂过站
                    FactoryStatus factoryStatusEntrance = GetStatus(configId);
                    if (factoryStatusEntrance.IsOnline && GlobalContext.SystemConfig.EnableFactoryMes)
                    {
                        GetItemDataParam getItemDataParam = new GetItemDataParam(param);
                        var callresult = APIMethod.Call(Url.GetItemData, getItemDataParam, configId).ToObject<RetMessage<InStationData>>();
                        return callresult;
                    }
                    offlineApiLogic.Insert(new RecordOfflineApi()
                    {
                        Url = Url.InStationUrl,
                        RequestBody = param.ToJson(),
                        ReUpload = 0
                    }, configId);
                    return NewSuccessMessage<InStationData>("工厂离线中，已离线上传完成"); ;
                }
                //首站过站怎么办
                ProcessBind processBind = processBindLogic.GetByProductCode(param.productCode, configId);
                if (processBind == null)
                {
                    //未查询到绑定记录，返回错误，需要人工进行判定\
                    Logger.RunningInfo($"{param.productCode}在{param.stationCode}工位过站失败，未查询到绑定纪录");
                    return NewErrorMessage<InStationData>("未查询到绑定纪录");
                }

                //非返修状态
                if (processBind.RepairFlag != "1")
                {
                    if (processBind.Status == "NG" && param.productStatus != "REWORK")
                    {
                        Logger.RunningInfo($"{param.productCode}在{param.stationCode}工位过站失败，产品状态为NG");
                        return new RetMessage<InStationData>()
                        {
                            messageType = RetCode.Ng,
                            message = "产品状态为NG",
                            data = new InStationData()
                            {
                                result = "NG",
                                errorReason = "产品状态为NG",
                                qualityParams = null
                            }
                        };
                    }
                }


                //读取上一站
                ParamLocalRoute lastRoute = null;
                if (routStep >= 1)
                {
                    //上一步路线
                    lastRoute = paramLocalRoutes.ElementAt(routStep - 1);
                }

                //查了recipeitem路线
                ParamRecipeItem recipeItem = recipeLogic.QueryRoute(processBind.ProductPartNo, param.stationCode, configId);

                //校验配方是否存在
                if (recipeItem == null)
                {
                    //产品配方中无此工序
                    Logger.RunningInfo($"{param.productCode}在{param.stationCode}工位进站失败，产品配方中无此工序");
                    return new RetMessage<InStationData>()
                    {
                        messageType = RetCode.Error,
                        message = "产品配方中无此工序",
                        data = new InStationData()
                        {
                            result = "NG",
                            errorReason = "产品配方中无此工序",
                            qualityParams = null
                        }
                    };
                }

                
                //返修处理      TODO  返修的逻辑需要根据实际修改
                if (processBind.RepairFlag == "1")
                {
                    param.productStatus = "REWORK";

                    List<string> strArray = new List<string>(processBind.RepairStations.Split(","));
                    int repairStep = paramLocalRoutes.FindIndex(it => it.StationCode == strArray[0]);
                    if (repairStep < 0)
                    {
                        return NewNgMessage<InStationData>("返修工站信息登记错误！");
                    }
                    //返修工站序号大于当前工站则放行
                    if (repairStep > routStep)
                    {
                        retMessage = new RetMessage<InStationData>()
                        {
                            messageType = RetCode.Repair,
                            message = "返修进站成功，当前工位已完成",
                            data = new InStationData()
                            {
                                result = "OK",
                                errorReason = "该产品为返修品，此工位已过站，且不允许重复作业",
                                qualityParams = null
                            }
                        };
                        return retMessage;
                    }
                    //等于则开始在该工位正常上线
                    else if (repairStep == routStep)
                    {
                        retMessage = new RetMessage<InStationData>()
                        {
                            messageType = RetCode.Success,
                            message = "返修进站成功，当前工位为返修工位",
                            data = new InStationData()
                            {
                                result = "REWORK",
                                errorReason = "该产品为返修品，在此工位进行返修",
                                qualityParams = null
                            }
                        };
                        return retMessage;
                        //过站后应该改变绑定表的返修状态
                    }
                    //小于则报警，前面还有工位没做，不能在该工位上线
                    else
                    {
                        retMessage = new RetMessage<InStationData>()
                        {
                            messageType = RetCode.Ng,
                            message = "返修进站失败，该产品前面工位未完成",
                            data = new InStationData()
                            {
                                result = "NG",
                                errorReason = $"该产品为返修品，返修工位为{processBind.RepairStations}",
                                qualityParams = null
                            }
                        };
                        return retMessage;
                    }
                }
                //当前工序用的是localroute
                ParamLocalRoute currentRoute = paramLocalRoutes.ElementAt(routStep);
                if (!currentRoute.Criterion.IsNullOrEmpty())
                    currentRoute.CheckStations = currentRoute.Criterion.Split(',').ToList();
                else
                    currentRoute.CheckStations = new List<string>();
                //非返修
                if (!currentRoute.IsAllowRepeat)  //不允许重复判定最新工位是否当前工位
                {
                    if (processBind.CurrentStation == param.stationCode && param.productStatus != "REWORK")
                    {
                        Logger.RunningInfo($"{param.productCode}在{param.stationCode}工位过站失败，当前工位不允许重复作业");
                        return NewNgMessage<InStationData>("过站NG，当前工位不允许重复作业");
                    }
                }
                if (!currentRoute.IsAllowJump)   //不允许跳站
                {
                    //如果不允许跳站，且存在上一个站，则必须要保证最新记录为上一站或为本站
                    if (lastRoute != null && processBind.CurrentStation != lastRoute.StationCode && processBind.CurrentStation != param.stationCode)
                    {
                        Logger.RunningInfo($"{param.productCode}在{param.stationCode}工位过站失败，上一作业工序为<{processBind.CurrentStation}>与预期上一工位<{lastRoute.StationCode}>不符合");
                        return NewNgMessage<InStationData>($"过站NG，不允许跳站，上一作业工序为<{processBind.CurrentStation}>与预期上一工位<{lastRoute.StationCode}>不符合");
                    }
                }
                //校验工位不为空，则需要校验所有需要校验的工序
                if (currentRoute.CheckStations != null && currentRoute.CheckStations.Count() > 0)
                {
                    List<RecordOutStation> recordOutStations = outStationLogic.GetList(param.productCode, configId);
                    foreach (var checkStation in currentRoute.CheckStations)
                    {
                        RecordOutStation record = recordOutStations.Find(it => it.StationCode == checkStation);
                        //没有找到对应的记录或记录不为OK   则校验失败
                        if (record == null || record.ProductStatus != "OK")
                        {
                            Logger.RunningInfo($"内控码<{param.productCode}>在<{param.stationCode}>工位过站失败，校验工序<{checkStation}>无作业信息或作业不为OK");
                            return NewNgMessage<InStationData>($"校验工序<{checkStation}>无作业信息或作业不为OK");
                        }
                    }
                }

                #endregion
                //工厂过站
                FactoryStatus factoryStatus = GetStatus(configId);
                if (factoryStatus.IsOnline && GlobalContext.SystemConfig.EnableFactoryMes)
                {
                    GetItemDataParam getItemDataParam = new GetItemDataParam(param);
                    var callresult = APIMethod.Call(Url.GetItemData, getItemDataParam, configId).ToObject<RetMessage<InStationData>>();

                    if (processBind.RepairFlag == "1")
                    {
                        retMessage.message += "," + callresult.message;
                        retMessage.messageType = RetCode.Success;
                        retMessage.data = new InStationData()
                        {
                            result = "OK",
                            errorReason = "",
                            qualityParams = null
                        };
                        return retMessage;//在此处增加判断，是否为返修，然后返回
                    }
                    return callresult;
                }
                offlineApiLogic.Insert(new RecordOfflineApi()
                {
                    Url = Url.InStationUrl,
                    RequestBody = param.ToJson(),
                    ReUpload = 0
                }, configId);
                return NewSuccessMessage<InStationData>("工厂离线中，已离线上传完成");
            }
            catch (Exception e)
            {
                Logger.ErrorInfo($"内控码:<{param.productCode}>,工站:<{param.stationCode}>错误", e);
                return NewErrorMessage<InStationData>($"内控码:<{param.productCode}>,工站:<{param.stationCode}>,错误信息:<{e.Message}>");
            }
        }

        //电芯进站要记住档位
        [OperationContract]
        public RetMessage<nullObject> UploadCellInfo(string productCode,string gear, string configId)
        {
            if (configId.IsNullOrEmpty())
                return NewErrorMessage<nullObject>("没有给configId参数赋值");
            if (productCode.IsNullOrEmpty())
                return NewErrorMessage<nullObject>("没有给productCode参数赋值");
            try
            {
                RecordCellStart cellStart = new RecordCellStart();
                cellStart.ProductCode = productCode;
                cellStart.Gear = gear;

                var ret = cellStartLogic.InsertSplitTableRow(cellStart);
                RetMessage<nullObject> retMessage = new RetMessage<nullObject>()
                {
                    messageType = ret != -1 ? RetCode.Success : RetCode.Error,
                    message = "上传电芯数据失败",
                    data = new nullObject()
                };
                return retMessage;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo($"电芯码:<{productCode}>,档位:<{gear}>,报错误", e);
                return NewErrorMessage<nullObject>($"电芯码:<{productCode}>,档位:<{gear}>,错误信息:<{e.Message}>");
            }
        }

        [OperationContract]
        public RetMessage<RecordCellStart> GetCellInfo(string productCode, string configId)
        {
            if (configId.IsNullOrEmpty())
                return NewErrorMessage<RecordCellStart>("没有给configId参数赋值");
            if (productCode.IsNullOrEmpty())
                return NewErrorMessage<RecordCellStart>("没有给productCode参数赋值");
            try
            {
                RecordCellStart cellStart = cellStartLogic.GetCellInfo(productCode, configId);
                RetMessage<RecordCellStart> retMessage = new RetMessage<RecordCellStart>()
                {
                    messageType = cellStart != null ? RetCode.Success : RetCode.Error,
                    message = "上传电芯数据失败",
                    data = new RecordCellStart()
                };
                return retMessage;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo($"电芯码:<{productCode}>报错误", e);
                return NewErrorMessage<RecordCellStart>($"电芯码:<{productCode}>错误信息:<{e.Message}>");
            }
        }

        //进站  电芯进站要记住档位
        [OperationContract]
        public RetMessage<InStationData> CellInStation(InStationParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
                return NewErrorMessage<InStationData>("没有给configId参数赋值");
            if (param.productCode.IsNullOrEmpty())
                return NewErrorMessage<InStationData>("没有给productCode参数赋值");
            try
            {
                RetMessage<InStationData> retMessage;
                //工厂过站
                FactoryStatus factoryStatus = GetStatus(configId);
                if (factoryStatus.IsOnline && GlobalContext.SystemConfig.EnableFactoryMes)
                {
                    GetItemDataParam getItemDataParam = new GetItemDataParam(param);
                    var callresult = APIMethod.Call(Url.GetItemData, getItemDataParam, configId).ToObject<MesRet>();
                    retMessage = new RetMessage<InStationData>(callresult);
                    return retMessage;
                }
                else
                {
                    offlineApiLogic.Insert(new RecordOfflineApi()
                    {
                        Url = Url.OutStationUrl,
                        RequestBody = param.ToJson(),
                        ReUpload = 0
                    }, configId);
                    return NewSuccessMessage<InStationData>("工厂离线中，已离线上传完成");
                }
            }
            catch (Exception e)
            {
                Logger.ErrorInfo($"内控码:<{param.productCode}>,工站:<{param.stationCode}>错误", e);
                return NewErrorMessage<InStationData>($"内控码:<{param.productCode}>,工站:<{param.stationCode}>,错误信息:<{e.Message}>");
            }
        }

        [OperationContract]
        public RetMessage<InStationData> BlockInStation(InStationParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
                return NewErrorMessage<InStationData>("没有给configId参数赋值");
            if (param.productCode.IsNullOrEmpty())
                return NewErrorMessage<InStationData>("没有给productCode参数赋值");
            try
            {
                RetMessage<InStationData> retMessage;
                //工厂过站
                FactoryStatus factoryStatus = GetStatus(configId);
                if (factoryStatus.IsOnline && GlobalContext.SystemConfig.EnableFactoryMes)
                {
                    GetItemDataParam getItemDataParam = new GetItemDataParam(param);
                    var callresult = APIMethod.Call(Url.GetItemData, getItemDataParam, configId).ToObject<MesRet>();
                    retMessage = new RetMessage<InStationData>(callresult);
                    return retMessage;
                }
                else
                {
                    offlineApiLogic.Insert(new RecordOfflineApi()
                    {
                        Url = Url.OutStationUrl,
                        RequestBody = param.ToJson(),
                        ReUpload = 0
                    }, configId);
                    return NewSuccessMessage<InStationData>("工厂离线中，已离线上传完成");
                }
            }
            catch (Exception e)
            {
                Logger.ErrorInfo($"内控码:<{param.productCode}>,工站:<{param.stationCode}>错误", e);
                return NewErrorMessage<InStationData>($"内控码:<{param.productCode}>,工站:<{param.stationCode}>,错误信息:<{e.Message}>");
            }
        }

        [OperationContract]
        public RetMessage<InStationData> InStation(InStationParam param, string configId)
        {
            if (configId.IsNullOrEmpty())
                return NewErrorMessage<InStationData>("没有给configId参数赋值");
            if (param.productCode.IsNullOrEmpty())
                return NewErrorMessage<InStationData>("没有给productCode参数赋值");
            try
            {
                var unitProcedure = unitProcedureLogic.GetByStation(param.stationCode, configId);
                switch (unitProcedure.InStationProductType)
                {
                    case "Cell":
                        return CellInStation(param, configId);
                    case "Block":
                        return BlockInStation(param, configId);
                    default:
                        return PackInStation(param, configId);
                }
            }
            catch (Exception e)
            {
                Logger.ErrorInfo($"内控码:<{param.productCode}>,工站:<{param.stationCode}>错误", e);
                return NewErrorMessage<InStationData>($"内控码:<{param.productCode}>,工站:<{param.stationCode}>,错误信息:<{e.Message}>");
            }
        }

        [OperationContract]
        public RetMessage<OutStationData> OutStation(OutStationParam param, PartUploadParam part, ProcessUploadParam process, List<BindProduct> bindProducts, string configId)
        {
            if (configId.IsNullOrEmpty())
                return NewErrorMessage<OutStationData>("没有给configId参数赋值");
            if (param.productCode.IsNullOrEmpty())
                return NewErrorMessage<OutStationData>("没有给productCode参数赋值");
            try
            {
                var unitProcedure = unitProcedureLogic.GetByStation(param.stationCode, configId);
                switch (unitProcedure.OutStationProductType)
                {
                    case "Cell":
                        return CellOutStation(param, part, process, configId);
                    case "Block":
                        return BlockOutStation(param, part, process, bindProducts, configId);
                    default:
                        return PackOutStation(param, part, process, bindProducts, configId);
                }
            }
            catch (Exception e)
            {
                Logger.ErrorInfo($"内控码:<{param.productCode}>,工站:<{param.stationCode}>出站错误", e);
                return NewErrorMessage<OutStationData>($"内控码:<{param.productCode}>,工站:<{param.stationCode}>,出站错误信息:<{e.Message}>");
            }
        }
    }
}