using CCS.WebUI;
using FNMES.Entity.DTO.ApiParam;
using FNMES.Entity.Param;
using FNMES.Entity.Record;
using FNMES.Utility.Core;
using FNMES.WebUI.Areas.Param.Controllers;
using FNMES.WebUI.Logic.Base;
using FNMES.WebUI.Logic.Record;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FNMES.WebUI.Logic.Param
{
    public class ParamRepairLogic: BaseLogic
    {
        public List<RepairItem> GetList(int pageIndex, int pageSize, string productCode, ref int totalCount)
        {
            List<RepairItem> lstRepairItem = new List<RepairItem>();

            foreach (var item in AppSetting.lineConnections)
            {
                try
                {
                    ISqlSugarClient db = GetInstance(item.ConfigId);
                    //根据内控码查询绑定信息表
                    ProcessBind processBind = db.MasterQueryable<ProcessBind>().Where(e => e.ProductCode == productCode).First();
                    if (processBind != null)
                    {
                        ////根据内控码分表查询过站记录表前3个表，然后分页查询
                        //var lstRecordOutStation = db.Queryable<RecordOutStation>().Where(e => e.ProductCode == processBind.ProductCode)
                        //.SplitTable(e => e.Take(3)).ToPageList(pageIndex, pageSize, ref totalCount);

                        ////根据ID进行排序，然后分组，对每个分组中的元素进行投影，选择每个分组中的第一个元素
                        //var onlyRecords = lstRecordOutStation.OrderBy(e => e.Id).GroupBy(e => new { e.ProductCode, e.StationCode })
                        //.Select(e => e.First()).ToList();

                        var onlyRecords = db.Queryable<RecordOutStation>().Where(e => e.ProductCode == processBind.ProductCode)
                             .SplitTable(e => e.Take(3))
                             .Select(it => new
                             {
                                 index = SqlFunc.RowNumber($"{it.Id} desc", $"{it.ProductCode}, {it.StationCode}"),
                                 it.Id,
                                 it.ProductCode,
                                 it.ProductStatus,
                                 it.StationCode,
                                 it.SmallStationCode,
                                 it.TaskOrderNumber,
                                 it.OperatorNo,
                                 it.EquipmentID,
                                 it.DefectCode,
                                 it.DefectDesc,
                                 it.CreateTime
                             })
                             .MergeTable()
                             .Where(it => it.index == 1)
                             .Select(it => new RecordOutStation()
                             {
                                 Id = it.Id,
                                 ProductCode = it.ProductCode,
                                 ProductStatus = it.ProductStatus,
                                 StationCode = it.StationCode,
                                 SmallStationCode = it.SmallStationCode,
                                 TaskOrderNumber = it.TaskOrderNumber,
                                 OperatorNo = it.OperatorNo,
                                 EquipmentID = it.EquipmentID,
                                 DefectCode = it.DefectCode,
                                 DefectDesc = it.DefectDesc,
                                 CreateTime = it.CreateTime
                             })
                             .OrderByDescending(it => it.Id)
                             .ToPageList(pageIndex, pageSize, ref totalCount);

                        foreach (var recordOutStation in onlyRecords)
                        {
                            RepairItem repairItem = new RepairItem();
                            repairItem.ProduceCode = recordOutStation.ProductCode;
                            repairItem.LineId = item.ConfigId;
                            repairItem.StationCode = recordOutStation.StationCode;
                            repairItem.DefectCode = recordOutStation.DefectCode;
                            repairItem.DefectDesc = recordOutStation.DefectDesc;
                            ////出站的id没有用
                            //repairItem.Id = recordOutStation.Id;

                            repairItem.RepairFlag = (processBind.RepairStations.Contains(recordOutStation.StationCode)
                                    && processBind.RepairFlag == "1") ? "1" : "0";

                            lstRepairItem.Add(repairItem);
                        }

                        return lstRepairItem;
                    }
                    else
                    {
                        RecordBindHistory oldProcessBind = db.Queryable<RecordBindHistory>().Where(e => e.ProductCode == productCode).SplitTable(tabs => tabs.Take(4)).First();
                        if (oldProcessBind != null)
                        {
                            var onlyRecords = db.Queryable<RecordOutStation>().Where(e => e.ProductCode == oldProcessBind.ProductCode)
                                .SplitTable(e => e.Take(3))
                                 .Select(it => new
                                 {
                                     index = SqlFunc.RowNumber($"{it.Id} desc", $"{it.ProductCode}, {it.StationCode}"),
                                     it.Id,
                                     it.ProductCode,
                                     it.ProductStatus,
                                     it.StationCode,
                                     it.SmallStationCode,
                                     it.TaskOrderNumber,
                                     it.OperatorNo,
                                     it.EquipmentID,
                                     it.DefectCode,
                                     it.DefectDesc,
                                     it.CreateTime
                                 })
                                 .MergeTable()
                                 .Where(it => it.index == 1)
                                 .Select(it => new RecordOutStation()
                                 {
                                     Id = it.Id,
                                     ProductCode = it.ProductCode,
                                     ProductStatus = it.ProductStatus,
                                     StationCode = it.StationCode,
                                     SmallStationCode = it.SmallStationCode,
                                     TaskOrderNumber = it.TaskOrderNumber,
                                     OperatorNo = it.OperatorNo,
                                     EquipmentID = it.EquipmentID,
                                     DefectCode = it.DefectCode,
                                     DefectDesc = it.DefectDesc,
                                     CreateTime = it.CreateTime
                                 })
                                 .OrderByDescending(it => it.Id)
                                 .ToPageList(pageIndex, pageSize, ref totalCount);

                            foreach (var recordOutStation in onlyRecords)
                            {
                                RepairItem repairItem = new RepairItem();
                                repairItem.ProduceCode = recordOutStation.ProductCode;
                                repairItem.LineId = item.ConfigId;
                                repairItem.StationCode = recordOutStation.StationCode;
                                repairItem.DefectCode = recordOutStation.DefectCode;
                                repairItem.DefectDesc = recordOutStation.DefectDesc;
                                ////出站的id没有用
                                //repairItem.Id = recordOutStation.Id;

                                repairItem.RepairFlag = (oldProcessBind.RepairStations.Contains(recordOutStation.StationCode)
                                        && oldProcessBind.RepairFlag == "1") ? "1" : "0";

                                lstRepairItem.Add(repairItem);
                            }
                            return lstRepairItem;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorInfo(ex.Message);
                    return lstRepairItem;
                }
            }

            return lstRepairItem;
        }

        public int Insert(DisAssembleParam model, string configId)
        {
            try
            {
                RecordUnbindPack unbindPack = new();
                unbindPack.CopyField(model);
                unbindPack.Id = SnowFlakeSingle.instance.NextId();
                unbindPack.CreateTime = DateTime.Now;
                List<RecordModuleData> partList = new();
                foreach (var item in model.partList)
                {
                    RecordModuleData buf = new RecordModuleData();
                    buf.CopyField(item);
                    buf.Id = SnowFlakeSingle.instance.NextId();
                    buf.UnbindPackId = unbindPack.Id;
                    buf.CreateTime = DateTime.Now;
                    partList.Add(buf);
                }
                Db.BeginTran();
                var db = GetInstance(configId);
                db.Insertable(unbindPack).SplitTable().ExecuteCommand();
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

       public List<UnbingPackData> GetUnbindPack(int pageIndex, int pageSize, string keyWord, ref int totalCount, string configId, string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordUnbindPack> queryable = db.Queryable<RecordUnbindPack>();
                ISugarQueryable<RecordModuleData> queryable1 = db.Queryable<RecordModuleData>();
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.StationCode.Contains(keyWord) || it.ProductCode.Contains(keyWord));
                }
                //查询当日
                if (index == "1")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today;
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                //近7天
                else if (index == "2")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-6);
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                //近1月
                else if (index == "3")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-29);
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                //近3月
                else if (index == "4")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-91);
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }


                var unbindpack = queryable.SplitTable(tabs => tabs.Take(2));

                var moduledata = queryable1.SplitTable(tabs => tabs.Take(2));

                var lst = unbindpack.LeftJoin(moduledata, (o, c) => o.Id == c.UnbindPackId)
                    .Select((o, c) => new UnbingPackData
                    {
                        Id = c.Id,
                        ProductCode = o.ProductCode,
                        CreateTime = o.CreateTime,
                        EquipmentID = o.EquipmentID,
                        OperatorNo = o.OperatorNo,
                        StationCode = o.StationCode,
                        SmallStationCode = o.SmallStationCode,
                        PartNumber = c.PartNumber,
                        PartDescription = c.PartDescription,
                        PartBarcode = c.PartBarcode,
                        Reason = c.Reason,
                    })
                    .MergeTable()
                    .OrderByDescending(e => e.Id)
                     .ToPageList(pageIndex, pageSize, ref totalCount);

                return lst;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<UnbingPackData>();
            }

        }

        public List<UnbingPackData> GetUnbindPack2(string keyWord, string configId ,string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordUnbindPack> queryable = db.Queryable<RecordUnbindPack>();
                ISugarQueryable<RecordModuleData> queryable1 = db.Queryable<RecordModuleData>();
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.StationCode.Contains(keyWord) || it.ProductCode.Contains(keyWord));
                }
                //查询当日
                if (index == "1")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today;
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                //近7天
                else if (index == "2")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-6);
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                //近1月
                else if (index == "3")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-29);
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                //近3月
                else if (index == "4")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-91);
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }


                var unbindpack = queryable.SplitTable(tabs => tabs.Take(2));

                var moduledata = queryable1.SplitTable(tabs => tabs.Take(2));

                var lst = unbindpack.LeftJoin(moduledata, (o, c) => o.Id == c.UnbindPackId)
                    .Select((o, c) => new UnbingPackData
                    {
                        Id = c.Id,
                        ProductCode = o.ProductCode,
                        CreateTime = o.CreateTime,
                        EquipmentID = o.EquipmentID,
                        OperatorNo = o.OperatorNo,
                        StationCode = o.StationCode,
                        SmallStationCode = o.SmallStationCode,
                        PartNumber = c.PartNumber,
                        PartDescription = c.PartDescription,
                        PartBarcode = c.PartBarcode,
                        Reason = c.Reason,
                    })
                    .MergeTable()
                    .OrderByDescending(e => e.Id)
                     .ToList();

                return lst;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<UnbingPackData>();
            }

        }

    }

    public class UnbingPackData
    {
        public long Id { get; set; }
        public string ProductCode { get; set; }
        public string StationCode { get; set; }
        public string SmallStationCode { get; set; }
        public string EquipmentID { get; set; }
        public string OperatorNo { get; set; }
        public string PartNumber { get; set; } //物料号

        public string PartDescription { get; set; } //物料描述

        public string PartBarcode { get; set; } //物料条码

        [DataMember]
        public string Reason { get; set; } //解绑原因
        public DateTime CreateTime { get; set; }
    }
}
