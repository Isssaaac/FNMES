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
                foreach (Part item in model.partList)
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
        public List<RecordPartUpload> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordPartUpload> queryable = db.Queryable<RecordPartUpload>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.ProductCode.Contains(keyWord));
                }
                return queryable.SplitTable(tabs => tabs.Take(2)).ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<RecordPartUpload>();
            }
        }

        public bool CheckPartBarcode(string partBarCode){
            //需要查询每条线的数据
            try
            {
                for (int i = 1; i <= 5; i++)
                {
                    var db = GetInstance(i.ToString());
                    bool v = db.Queryable<RecordPartData>().Where(it => it.PartBarcode == partBarCode).SplitTable(tables => tables.Take(2)).Any();
                    if (v)
                    {
                        //如果存在，直接跳出循环，查重结束
                        return false;
                    }
                }
                return true;
            }
            catch 
            {
                throw;
            }
        }

        public RetMessage<RecordPartUpload> GetProductCode(string partBarCode, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                var part = db.Queryable<RecordPartData>().Where(it => it.PartBarcode == partBarCode).SplitTable(tables => tables.Take(3)).First();
                if (part == null)
                {
                    return new RetMessage<RecordPartUpload>
                    {
                        data = null,
                        message = $"未查询到物料条码{partBarCode}",
                        messageType = RetCode.Error,
                    };

                }
                else
                {
                    var partupload = db.Queryable<RecordPartUpload>().Where(t => t.Id == part.PartUploadId).SplitTable(table => table.Take(3)).First();
                    if (partupload == null)
                    {
                        return new RetMessage<RecordPartUpload>
                        {
                            data = null,
                            message = $"未查询到内控码",
                            messageType = RetCode.Error,
                        };
                    }

                    return new RetMessage<RecordPartUpload>
                    {
                        data = partupload,
                        message = $"内控码查询成功",
                        messageType = RetCode.Success,
                    };
                }
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new RetMessage<RecordPartUpload>
                {
                    data = null,
                    message = $"查询出错,{E.Message}",
                    messageType = RetCode.Error,
                };
            }
        }

        public RetMessage<bool> CheckProductCode(string productCode, string partBarCode, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                var part = db.Queryable<RecordPartData>().Where(it => it.PartBarcode == partBarCode).SplitTable(tables => tables.Take(3)).First();
                if (part == null)
                {
                    return new RetMessage<bool>
                    {
                        data = false,
                        message = $"未查询到物料条码{partBarCode}",
                        messageType = RetCode.Error,
                    };

                }
                else
                {
                    var partupload = db.Queryable<RecordPartUpload>().Where(t => t.Id == part.PartUploadId).SplitTable(table => table.Take(3)).First();
                    if (partupload == null)
                    {
                        return new RetMessage<bool>
                        {
                            data = false,
                            message = $"未查询到内控码",
                            messageType = RetCode.Error,
                        };
                    }
                    if (partupload.ProductCode == productCode)
                    {
                        return new RetMessage<bool>
                        {
                            data = true,
                            message = $"内控码与物料条码绑定关系正确",
                            messageType = RetCode.Success,
                        };
                    }
                    else
                    {
                        return new RetMessage<bool>
                        {
                            data = false,
                            message = $"内控码与物料条码绑定关系错误",
                            messageType = RetCode.Error,
                        };
                    }

                }
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new RetMessage<bool>
                {
                    data = false,
                    message = $"查询出错,{E.Message}",
                    messageType = RetCode.Error,
                };
            }
        }

        //20241125添加，解绑后应删除对应物料记录
        public bool UnBindPartBarcode(string partBarcode)
        {
            //需要查询每条线的数据
            try
            {
                for (int i = 1; i <= 5; i++)
                {
                    var db = GetInstance(i.ToString());
                    db.Deleteable<RecordPartData>().Where(it => it.PartBarcode == partBarcode).SplitTable(tables => tables.Take(2));
                }
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}
