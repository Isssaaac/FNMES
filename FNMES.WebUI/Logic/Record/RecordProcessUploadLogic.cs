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
    public class RecordProcessUploadLogic : BaseLogic
    {
        //注意，分表数据需要加SplitTable()

        public int Insert(ProcessUploadParam model, string configId)
        {
            try
            {
                RecordProcessUpload process = new RecordProcessUpload();
                process.CopyField(model);
                process.Id = SnowFlakeSingle.instance.NextId();
                process.CreateTime = DateTime.Now;
                List<RecordProcessData> processList = new();
                foreach (var item in model.processData)
                {
                    RecordProcessData buf = new();
                    buf.CopyField(item);
                    buf.Id = SnowFlakeSingle.instance.NextId();
                    buf.ProcessUploadId = process.Id;
                    buf.CreateTime = DateTime.Now;
                    processList.Add(buf);
                }
                Db.BeginTran();
                var db = GetInstance(configId);
                db.Insertable<RecordProcessUpload>(process).SplitTable().ExecuteCommand();
                db.Insertable<RecordProcessData>(processList).SplitTable().ExecuteCommand();
                Db.CommitTran();
                return 1;
            }
            catch (Exception e)
            {
                Db.RollbackTran();
                Logger.ErrorInfo($"ProcessUpload本地插入出错",e);
                return 0;
            }
        }
        public List<RecordProcessUpload> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount,string configId)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordProcessUpload> queryable = db.Queryable<RecordProcessUpload>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.ProductCode.Contains(keyWord));
                }
                return queryable.SplitTable(tabs => tabs.Take(2)).ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordProcessUpload>();
            }
        }
    }
}
