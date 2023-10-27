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
    public class RecordToolRemainLogic : BaseLogic
    {
        //注意，分表数据需要加SplitTable()

        public int Insert(ToolRemainParam model, string configId)
        {
            try
            {
                RecordToolRemain toolRemain = new();
                toolRemain.CopyField(model);
                toolRemain.Id = SnowFlakeSingle.instance.NextId();
                toolRemain.CreateTime = DateTime.Now;
                List<RecordToolData> partList = new();
                foreach (var item in model.toolList)
                {
                    RecordToolData buf = new RecordToolData();
                    buf.CopyField(item);
                    buf.Id = SnowFlakeSingle.instance.NextId();
                    buf.ToolRemainId = toolRemain.Id;
                    buf.CreateTime = DateTime.Now;
                    partList.Add(buf);
                }
                Db.BeginTran();
                var db = GetInstance(configId);
                db.Insertable(toolRemain).SplitTable().ExecuteCommand();
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
        public List<RecordToolRemain> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount,string configId)
        {
            var db = GetInstance(configId);
            ISugarQueryable<RecordToolRemain> queryable = db.Queryable<RecordToolRemain>();

            if (!keyWord.IsNullOrEmpty())
            {
                queryable = queryable.Where(it => it.StationCode.Contains(keyWord));
            }
            return queryable.SplitTable(tabs => tabs.Take(2)).ToPageList(pageIndex, pageSize, ref totalCount);
        }






    }
}
