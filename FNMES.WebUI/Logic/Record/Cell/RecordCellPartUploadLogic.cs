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
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Drawing.Printing;
using FNMES.Utility.Network;

namespace FNMES.WebUI.Logic.Record
{
    public class RecordCellPartUploadLogic:BaseLogic
    {

        public int Insert(PartUploadParam model, string configId)
        {
            try
            {
                RecordCellPartUpload recordPartUpload = new RecordCellPartUpload();
                recordPartUpload.CopyField(model);
                recordPartUpload.Id = SnowFlakeSingle.instance.NextId();
                recordPartUpload.CreateTime = DateTime.Now;
                List<RecordCellPartData> partList = new();
                foreach (Part item in model.partList)
                {
                    RecordCellPartData buf = new RecordCellPartData();
                    buf.CopyField(item);
                    buf.Id = SnowFlakeSingle.instance.NextId();
                    buf.PartUploadId = recordPartUpload.Id;
                    buf.CreateTime = DateTime.Now;
                    partList.Add(buf);
                }
                Db.BeginTran();
                var db = GetInstance(configId);
                db.Insertable(recordPartUpload).SplitTable().ExecuteCommand();
                db.Insertable(partList).SplitTable().ExecuteCommand();
                Db.CommitTran();
                return 1;
            }
            catch (Exception e)
            {
                Db.RollbackTran();
                Logger.ErrorInfo($"物料绑定异常,线体:{configId},内控码:{model.productCode}", e);
                return 0;
            }
        }
    }
}
