using SqlSugar;
using System;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;
using FNMES.Entity.Record;
using System.Collections.Generic;
using FNMES.Utility.Core;
using System.Linq;
using FNMES.Entity.DTO.ApiParam;
using Org.BouncyCastle.Asn1.Ess;
using FNMES.Entity.DTO.ApiData;

namespace FNMES.WebUI.Logic.Record
{
    public class RecordSelfDischargeLogic:BaseLogic
    {
        public bool Insert(selfDischargeParamIn model, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                List<RecordSelfDischarge> param_list = new List<RecordSelfDischarge>();
                foreach (var module in model.moduleSelfDischarges)
                {
                    foreach (var cell in module.cellSelfDischarges)
                    {
                        RecordSelfDischarge param = new RecordSelfDischarge();
                        param.Id = SnowFlakeSingle.instance.NextId();
                        param.productCode = model.productCode;
                        param.maxVoltageDrop = module.maxVoltageDrop;
                        param.minVoltageDrop = module.minVoltageDrop;
                        param.averageVoltageDrop = module.averageVoltageDrop;
                        param.stdDeviationVoltageDrop = module.stdDeviationVoltageDrop;
                        param.judgment1Lo = module.judgment1Lo;
                        param.judgment1Up = module.judgment1Up;
                        param.judgment1LoResult = module.judgment1LoResult;
                        param.judgment1UpResult = module.judgment1UpResult;
                        param.judgment1Result = module.judgment1Result;
                        param.judgment2Result = module.judgment2Result;
                        param.result = module.result;
                        param.createTime = DateTime.Now;
                        
                        param.cellCode = cell.cellCode;
                        param.voltageDrop = cell.voltageDrop;
                        param.a020TestTime = cell.a020TestTime;
                        param.a020TestVoltage = cell.a020TestVoltage;
                        param.m350TestTime = cell.m350TestTime;
                        param.m350TestVoltage = cell.m350TestVoltage;
                        param.timeInterval = cell.timeInterval;
                        param.intervalVoltageDrop = cell.intervalVoltageDrop;

                        param_list.Add(param);
                    }
                }
                var ret = db.Insertable<RecordSelfDischarge>(param_list).SplitTable().ExecuteCommand();
                return ret > 0;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("插入自放电数据失败",e);
                return false;
            }
        }
    }
}
