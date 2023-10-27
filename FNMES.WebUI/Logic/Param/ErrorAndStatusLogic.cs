using FNMES.Entity.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Utility.Operator;
using FNMES.Utility.Security;
using FNMES.Utility.Extension;
using FNMES.Utility.Core;
using FNMES.Utility.Other;
using System.Drawing.Printing;
using Microsoft.VisualBasic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using FNMES.Entity.DTO.AppData;

namespace FNMES.WebUI.Logic.Param
{
    public class ErrorAndStatusLogic : BaseLogic
    {
        public int InsertError(List<ParamEquipmentError> paramErrors,string configId)
        {
            //只支持批量导入初始化，不支持单个操作
            int res = 0;
            try
            {
                var db = GetInstance(configId);
                paramErrors.ForEach(err => { err.Id = SnowFlakeSingle.instance.NextId(); });
                Db.BeginTran();
                db.DbMaintenance.TruncateTable<ParamEquipmentError>();
                res = db.Insertable<ParamEquipmentError>(paramErrors).ExecuteCommand();
                Db.CommitTran();
            }
            catch (Exception ex)
            {
                Logger.ErrorInfo(ex.Message);
                Db.RollbackTran();
            }
            return res;
        }


        public List<ParamEquipmentError> GetAllError(string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.Queryable<ParamEquipmentError>().ToList();

            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message.ToString());
                return null;
            }
        }
        public List<ParamEquipmentStatus> GetAllStatus(string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.Queryable<ParamEquipmentStatus>().ToList();

            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message.ToString());
                return null;
            }
        }

        public List<ParamEquipmentError> GetErrorList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamEquipmentError> queryable = db.Queryable<ParamEquipmentError>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.BigStationCode.Contains(keyWord) || it.EquipmentID.Contains(keyWord));
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message.ToString());
                return null;
            }
        }
        public List<ParamEquipmentStatus> GetStatusList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamEquipmentStatus> queryable = db.Queryable<ParamEquipmentStatus>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.BigStationCode.Contains(keyWord) || it.EquipmentID.Contains(keyWord));
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message.ToString());
                return null;
            }
        }

        public List<ParamEquipmentStopCode> GetCodeList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamEquipmentStopCode> queryable = db.Queryable<ParamEquipmentStopCode>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.StopCode.Contains(keyWord) || it.StopCodeDesc.Contains(keyWord));
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message.ToString());
                return null;
            }
        }

        public int InsertStatus(List<ParamEquipmentStatus> paramStatus,string configId)
        {
            //批量导入时初始化表
            int res = 0;
            try
            {
                var db = GetInstance(configId);
                paramStatus.ForEach(err => { err.Id = SnowFlakeSingle.instance.NextId(); });
                Db.BeginTran();
                db.DbMaintenance.TruncateTable<ParamEquipmentStatus>();
                res = db.Insertable<ParamEquipmentStatus>(paramStatus).ExecuteCommand();
                Db.CommitTran();
            }
            catch (Exception ex)
            {
                Logger.ErrorInfo(ex.Message);
                Db.RollbackTran();
            }
            return res;
        }


        public int InsertStop(List<ParamEquipmentStopCode> models, string configId)
        {
            //批量导入时初始化表
            int res = 0;
            try
            {
                var db = GetInstance(configId);
                Db.BeginTran();
                db.DbMaintenance.TruncateTable<ParamEquipmentStopCode>();
                res = db.Insertable<ParamEquipmentStopCode>(models).ExecuteCommand();
                Db.CommitTran();
            }
            catch (Exception ex)
            {
                Logger.ErrorInfo(ex.Message);
                Db.RollbackTran();
            }
            return res;
        }





        public PlcParam getPlcParam(string configId,int plcNo) 
        {
            try
            {
                List<ErrorParam> errorParams = new();
                List<StatusParam> statusParams = new();
                var db = GetInstance(configId);
                List<ParamEquipmentError> errors = db.Queryable<ParamEquipmentError>().Where(it => it.PlcNo == plcNo).ToList();
                List<ParamEquipmentStatus> statuses = db.Queryable<ParamEquipmentStatus>().Where(it => it.PlcNo == plcNo).ToList();
                List<ParamEquipmentStopCode> stopCodes = db.Queryable<ParamEquipmentStopCode>().ToList();
                List<string> stopCode = stopCodes.Select(it => it.StopCode).ToList();
                List<string> stopCodeDesc = stopCodes.Select(it => it.StopCodeDesc).ToList();
                //获取error配置
                if (errors.Count != 0)
                {
                    var errorGroups = errors.GroupBy(it => it.BigStationCode);
                    foreach (var errorGroup in errorGroups)
                    {
                        var errorAddresss = new List<ErrorAddress>();

                        foreach (var item in errorGroup)
                        {
                            errorAddresss.Add(new ErrorAddress
                            {
                                Offset = item.Offset,
                                AlarmCode = item.AlarmCode,
                                AlarmDesc = item.AlarmDesc,
                            });
                        }
                        ParamEquipmentError paramEquipmentError = errorGroup.First();
                        string bigStation = paramEquipmentError.BigStationCode;
                        string equipmentId = paramEquipmentError.EquipmentID;
                        errorParams.Add(new ErrorParam()
                        {
                            BigStationCode = bigStation,
                            EquipmentID = equipmentId,
                            ErrorAddresss = errorAddresss,
                        });
                    }
                }
                //获取status配置
                if (statuses.Count != 0)
                {
                    foreach (var item in statuses)
                    {
                        statusParams.Add(new StatusParam()
                        {
                            BigStationCode = item.BigStationCode,
                            EquipmentID = item.EquipmentID,
                            Offset = item.Offset,
                            StopCodeOffset = item.StopCodeOffset
                        });
                    }
                }
                return new PlcParam()
                {
                    ErrorParams = errorParams,
                    StatusParams = statusParams,
                    StopCode = stopCode,
                    StopCodeDesc = stopCodeDesc,
                };
            }
            catch (Exception ex)
            {
                Logger.ErrorInfo(ex.Message);
                return null;
            }
        }

    }
}
