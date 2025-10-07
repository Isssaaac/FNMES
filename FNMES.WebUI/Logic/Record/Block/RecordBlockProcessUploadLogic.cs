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

namespace FNMES.WebUI.Logic.Record
{
    public class RecordBlockProcessUploadLogic:BaseLogic
    {
        public int Insert(ProcessUploadParam model, string configId)
        {
            try
            {
                RecordProcessUpload process = new RecordProcessUpload();
                process.CopyField(model);
                process.Id = SnowFlakeSingle.instance.NextId();
                process.CreateTime = DateTime.Now;
                List<RecordBlockProcessData> processList = new();
                foreach (var item in model.processData)
                {
                    RecordBlockProcessData buf = new();
                    buf.CopyField(item);
                    buf.Id = SnowFlakeSingle.instance.NextId();
                    buf.ProcessUploadId = process.Id;
                    buf.CreateTime = DateTime.Now;
                    processList.Add(buf);
                }
                Db.BeginTran();
                var db = GetInstance(configId);
                db.Insertable<RecordBlockProcessUpload>(process).SplitTable().ExecuteCommand();
                db.Insertable<RecordBlockProcessData>(processList).SplitTable().ExecuteCommand();
                Db.CommitTran();
                return 1;
            }
            catch (Exception e)
            {
                Db.RollbackTran();
                Logger.ErrorInfo($"BlockProcessUpload本地插入出错", e);
                return 0;
            }
        }
    }
}
