using CCS.WebUI;
using FNMES.Entity.Param;
using FNMES.Entity.Record;
using FNMES.WebUI.Areas.Param.Controllers;
using FNMES.WebUI.Logic.Base;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

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
                        //根据内控码分表查询过站记录表前3个表，然后分页查询
                        var lstRecordOutStation = db.Queryable<RecordOutStation>().Where(e => e.ProductCode == processBind.ProductCode)
                        .SplitTable(e => e.Take(3)).ToPageList(pageIndex, pageSize, ref totalCount);

                        //根据ID进行排序，然后分组，对每个分组中的元素进行投影，选择每个分组中的第一个元素
                        var onlyRecords = lstRecordOutStation.OrderBy(e => e.Id).GroupBy(e => new { e.ProductCode, e.StationCode })
                        .Select(e => e.First()).ToList();

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

                            repairItem.RepairFlag = (processBind.RepairStations == recordOutStation.StationCode
                                    && processBind.RepairFlag == "1") ? "1" : "0";

                            lstRepairItem.Add(repairItem);
                        }

                        return lstRepairItem;
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
        
        
    }
}
