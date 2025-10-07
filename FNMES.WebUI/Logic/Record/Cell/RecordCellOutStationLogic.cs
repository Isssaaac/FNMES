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
    public class RecordCellOutStationLogic:BaseLogic
    {
        public bool processExist(string productCode, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.Queryable<RecordCellProcessUpload>().Where(it => it.ProductCode == productCode).SplitTable(tabs => tabs.Take(4)).Any();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return false;
            }
        }
        public bool partExist(string productCode, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.Queryable<RecordCellPartUpload>().Where(it => it.ProductCode == productCode).SplitTable(tabs => tabs.Take(4)).Any();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return false;
            }
        }

        public List<RecordCellProcessData> GetProcessData(int pageIndex, int pageSize, string keyWord, ref int totalCount, string configId, string productCode, string stationCode)
        {
            try
            {
                var db = GetInstance(configId);
                RecordProcessUpload record = db.Queryable<RecordProcessUpload>().Where(it => it.ProductCode == productCode && it.StationCode == stationCode)
                    .SplitTable(tabs => tabs.Take(4)).OrderByDescending(it => it.Id).First();

                

                if (record != null)
                {
                    DateTime start = record.CreateTime.AddMonths(-1);
                    DateTime end = record.CreateTime.AddMonths(6);
                    if (!keyWord.IsNullOrEmpty())
                    {
                        return db.Queryable<RecordCellProcessData>().Where(it => it.ProcessUploadId == record.Id && (it.ParamCode.Contains(keyWord) || it.ItemFlag.Contains(keyWord)))
                        .SplitTable(start, end).ToPageList(pageIndex, pageSize, ref totalCount);
                    }
                    return db.Queryable<RecordCellProcessData>().Where(it => it.ProcessUploadId == record.Id)
                        .SplitTable(start, end).ToPageList(pageIndex, pageSize, ref totalCount);
                }
                else
                {
                    return new List<RecordCellProcessData>();
                }
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordCellProcessData>();
            }
        }

        public List<RecordCellPartData> GetPartData(int pageIndex, int pageSize, ref int totalCount, string configId, string productCode)
        {
            try
            {
                var db = GetInstance(configId);

                RecordCellPartUpload recordPartUpload = db.Queryable<RecordCellPartUpload>().Where(it => it.ProductCode == productCode)
                    .SplitTable(tabs => tabs.Take(4)).OrderByDescending(it => it.Id).First();
                

                if (recordPartUpload != null)
                {
                    //250514修改，原本查不到4个月之前的物料数据
                    DateTime start = recordPartUpload.CreateTime.AddMonths(-1);
                    DateTime end = recordPartUpload.CreateTime.AddMonths(6);
                    return db.Queryable<RecordCellPartData>().Where(it => it.PartUploadId == recordPartUpload.Id)
                        .SplitTable(start, end).ToPageList(pageIndex, pageSize, ref totalCount);
                }
                else
                {
                    return new List<RecordCellPartData>();
                }
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordCellPartData>();
            }
        }
    }
}
