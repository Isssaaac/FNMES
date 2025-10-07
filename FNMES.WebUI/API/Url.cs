namespace FNMES.WebUI.API
{
    public class Url
    {
        public const string UserLogin_info = "/api/equipment/UserLogin_info";              //P1员工登录
        public const string GetItemData = "/api/equipment/GetItemData";                    //P7进站,这里获取的是电芯档位
        public const string UploadData_S = "/api/equipment/UploadData_S";                  //P13设备状态
        public const string UploadData_W = "/api/equipment/UploadData_W";                  //P14设备报警
        public const string UploadData_F = "/api/equipment/UploadData_F";                  //P16产品工序批次完成,出站
        public const string UploadData_MZ = "/api/equipment/UploadData_MZ";                //P16-4产品完成-堆叠首站,绑定工站，入箱也是
        public const string UpAssembleData = "/api/equipment/UpAssembleData";              //P32产品物料批次装配接口


        public const string HeartbeatUrl = "/api/pa/healthCheck";                          //心跳接口
        public const string LoginUrl = "/api/pa/doLogin";                                  //人员登录接口
        public const string GetOrderUrl = "/api/pa/syncTaskOrders";                        //派工单同步接口
        public const string SelectOrderUrl = "/api/pa/syncTaskOrderStatus";                //派工单状态同步接口
        public const string GetRecipeUrl = "/api/pa/dispatchRecipe";                       //工艺参数和配方下发接口
        public const string GetLabelUrl = "/api/pa/applySNCode";                           //内控码&REESS码申请 
        public const string InStationUrl = "/api/pa/doInbound";                            //进站接口
        public const string PartUploadUrl = "/api/pa/uploadMaterialConsumption";           //追溯件信息上传接口（物料绑定）
        public const string ProcessUploadUrl = "/api/pa/uploadProcessParameters";          //过程数据上传接口
        public const string OutStationUrl = "/api/pa/doOutbound";                          //出站接口
        public const string EquipmentStateUrl = "/api/pa/syncEquipStatus";                 //工位/设备状态变更接口
        public const string EquipmentErrorUrl = "/api/pa/syncEquipAlarms";                 //工位/设备报警变更接口
        public const string EquipmentStopUrl = "/api/pa/shutdownEquip";                    //工位/设备停机接口

        public const string ReworkUrl = "/api/pa/uploadRepairInfo";                        //返修信息上传接口

        public const string ToolRemainUrl = "/api/pa/uploadWearPartLife";                  //夹治具寿命上传接口
        public const string QualityStop = "/api/pa/qualityStopTag";                        //质量停机牌下发接口
        public const string GetPackInfoUrl = "/api/pa/getPackInfo";                        //PACK信息获取接口  
        public const string BindPalletUrl = "/api/pa/bindSNAndAGV";                        //内控码与AGV工装码绑定上传接口
        public const string UnBindPalletUrl = "/api/pa/unbindSNAndAGV";                    //内控码与AGV工装码解绑上传接口
        public const string AndonUrl = "/api/pa/getAndonInfo";                             //ANDON接口
        public const string AndonParamUrl = "/api/pa/getAndonType";                        //ANDON异常类型下发接口
        public const string UnbindPackUrl = "/api/pa/unbindPack";                          //PACK拆解接口
        public const string BindPackUrl = "/api/pa/bindPack";                              //PACK重组接口
        public const string UnbindMaterial = "/api/pa/uploadUnbindMaterial";               //返修房PACK拆解接口
        public const string SynScrapInfo = "/api/pa/synScrapInfo";                         //电芯报废接口

        //202503018新增marking接口功能
        public const string GetMarking = "/api/pa/getMarking";

        //20250319新增获取前段ocv结果
        public const string GetCellVoltage = "/api/pa/getCellVoltage";
    }
}
