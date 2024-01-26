using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using FNMES.Utility.Core;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;
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
                return db.MasterQueryable<ParamEquipmentError>().ToList();

            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }
        public List<ParamEquipmentStatus> GetAllStatus(string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.MasterQueryable<ParamEquipmentStatus>().ToList();

            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }

        public List<ParamEquipmentStopCode> GetAllStopCode(string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.MasterQueryable<ParamEquipmentStopCode>().ToList();

            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }


        public List<ParamEquipmentError> GetErrorList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamEquipmentError> queryable = db.MasterQueryable<ParamEquipmentError>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.StationCode.Contains(keyWord) || it.EquipmentID.Contains(keyWord));
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }
        public List<ParamEquipmentStatus> GetStatusList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamEquipmentStatus> queryable = db.MasterQueryable<ParamEquipmentStatus>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.StationCode.Contains(keyWord) || it.EquipmentID.Contains(keyWord));
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }

        public List<ParamEquipmentStopCode> GetCodeList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamEquipmentStopCode> queryable = db.MasterQueryable<ParamEquipmentStopCode>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.StopCode.Contains(keyWord) || it.StopCodeDesc.Contains(keyWord));
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
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


        public int InsertStopCode(List<ParamEquipmentStopCode> models, string configId)
        {
            //批量导入时初始化表
            int res = 0;
            try
            {
                var db = GetInstance(configId);
                models.ForEach(model => { model.Id = SnowFlakeSingle.instance.NextId(); });
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





        public PlcParam GetPlcParam(string configId,int plcNo) 
        {
            //查询参数必须走主库
            try
            {
                List<ErrorParam> errorParams = new();
                List<StatusParam> statusParams = new();
                var db = GetInstance(configId);
                List<ParamEquipmentError> errors = db.MasterQueryable<ParamEquipmentError>().Where(it => it.PlcNo == plcNo).ToList();
                List<ParamEquipmentStatus> statuses = db.MasterQueryable<ParamEquipmentStatus>().Where(it => it.PlcNo == plcNo).ToList();
                List<ParamEquipmentStopCode> stopCodes = db.MasterQueryable<ParamEquipmentStopCode>().ToList();
                List<string> stopCode = stopCodes.Select(it => it.StopCode).ToList();
                List<string> stopCodeDesc = stopCodes.Select(it => it.StopCodeDesc).ToList();
                //获取error配置
                if (errors.Count != 0)
                {
                    var errorGroups = errors.GroupBy(it => it.StationCode);
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
                        string bigStation = paramEquipmentError.StationCode;
                        string smallStation = paramEquipmentError.SmallStationCode;
                        string equipmentId = paramEquipmentError.EquipmentID;
                        errorParams.Add(new ErrorParam()
                        {
                            StationCode = bigStation,
                            SmallStationCode = smallStation,
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
                            StationCode = item.StationCode,
                            SmallStationCode = item.SmallStationCode,
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
