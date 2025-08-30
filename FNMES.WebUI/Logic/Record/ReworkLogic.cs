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
    public class ReworkLogic : BaseLogic
    {
        //注意，分表数据需要加SplitTable()

        public int SelectOldData(ReworkParam model, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                foreach (var item in model.productList)
                {
                    var unbindPack = db.Queryable<RecordUnbindPack>().SplitTable(tabs => tabs.Take(3)).Where(it => it.ProductCode == item.productCode && it.StationCode == model.stationCode).First();
                    if (!unbindPack.IsNullOrEmpty())
                    {
                        foreach (var e in item.partList)
                        {
                            var moduleData = db.Queryable<RecordModuleData>().SplitTable(tabs => tabs.Take(3)).Where(it => it.UnbindPackId == unbindPack.Id && it.PartNumber == e.batchOrSN).First();
                            if (!moduleData.IsNullOrEmpty())
                            {
                                e.newPartNumber = moduleData.PartNumber;
                                //e.batchOrSN = moduleData
                            }
                        }
                    }
                }
                return 1;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("查询返修数据失败", e);
                return -1;
            }
        }
    }
}
