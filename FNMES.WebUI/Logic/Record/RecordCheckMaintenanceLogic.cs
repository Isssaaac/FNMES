using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Record;
using FNMES.Entity.Param;
using SqlSugar;
using System;
using System.Collections.Generic;
using FNMES.Utility.Core;
using System.Linq;
using FNMES.Entity.DTO.ApiParam;
using Org.BouncyCastle.Asn1.Ess;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Drawing.Printing;
using FNMES.Utility.Network;
using System.Threading.Tasks;

namespace FNMES.WebUI.Logic.Record
{
    public class RecordCheckMaintenanceLogic : BaseLogic
    {

        public async Task<int> InsertAsync(GetCheckMaitenanceParam param, string configId)
        {
            try
            {
                RecordCheckMaintenance maintenance = new RecordCheckMaintenance();
                maintenance.StationCode = param.operation_no;
                maintenance.SmallStationCode = param.resource_no;
                maintenance.OperatorNo = param.cz_user;
                maintenance.CheckType = param.cz_class;
                maintenance.MaintenanceList = new List<RecordCheckMaintenanceData>();
                foreach (var e in param.json_data)
                {
                    RecordCheckMaintenanceData item = new RecordCheckMaintenanceData();
                    item.CopyMatchingProperties(e);
                    maintenance.MaintenanceList.Add(item);
                }
                await InsertAsync(maintenance, configId);
                return 1;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo($"点检数据上传数据库失败,工站:{param.operation_no}", e);
                return 0;
            }
        }
        public async Task<int> InsertAsync(RecordCheckMaintenance model, string configId)
        {
            try
            {
                RecordCheckMaintenance maintenance = new RecordCheckMaintenance();
                maintenance.CopyMatchingProperties(model);
                maintenance.Id = SnowFlakeSingle.instance.NextId();
                maintenance.CreateTime = DateTime.Now;
                List<RecordCheckMaintenanceData> dataList = new();
                foreach (RecordCheckMaintenanceData item in model.MaintenanceList)
                {
                    RecordCheckMaintenanceData buf = new RecordCheckMaintenanceData();
                    buf.CopyField(item);
                    buf.Id = SnowFlakeSingle.instance.NextId();
                    buf.Pid = maintenance.Id;
                    buf.CreateTime = DateTime.Now;
                    dataList.Add(buf);
                }
                await Db.BeginTranAsync();
                var db = GetInstance(configId);
                await db.Insertable(maintenance).SplitTable().ExecuteCommandAsync();
                await db.Insertable(dataList).SplitTable().ExecuteCommandAsync();
                await Db.CommitTranAsync();
                return 1;
            }
            catch (Exception e)
            {
                await Db.RollbackTranAsync();
                Logger.ErrorInfo($"点检数据上传数据库失败,工站:{model.StationCode}", e);
                return 0;
            }
        }
    }
}
