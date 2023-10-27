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
    public class RecordPartUploadLogic : BaseLogic
    {
        //注意，分表数据需要加SplitTable()

        public int Insert(PartUploadParam model, string configId)
        {
            try
            {
                RecordPartUpload recordPartUpload = new RecordPartUpload();
                recordPartUpload.CopyField(model);
                recordPartUpload.Id = SnowFlakeSingle.instance.NextId();
                recordPartUpload.CreateTime = DateTime.Now;
                List<RecordPartData> partList = new();
                foreach (var item in model.partList)
                {
                    RecordPartData buf = new RecordPartData();
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
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }
        public List<RecordPartUpload> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount,string configId)
        {
            var db = GetInstance(configId);
            ISugarQueryable<RecordPartUpload> queryable = db.Queryable<RecordPartUpload>();

            if (!keyWord.IsNullOrEmpty())
            {
                queryable = queryable.Where(it => it.ProductCode.Contains(keyWord));
            }
            return queryable.SplitTable(tabs => tabs.Take(2)).ToPageList(pageIndex, pageSize, ref totalCount);
        }






    }
}
