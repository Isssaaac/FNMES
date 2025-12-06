using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Record;
using System;
using System.Threading.Tasks;
using FNMES.Entity.DTO.ApiParam;
using SqlSugar;
using System.Linq;

namespace FNMES.WebUI.Logic.Record
{
    public class RecordCellStartLogic : BaseLogic
    {
        public async Task<RecordCellStart> GetCellInfoAsync(string productCode, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                //业务逻辑强制走主库
                var cellStart = await db.Queryable<RecordCellStart>().Where(it=>it.ProductCode == productCode).SplitTable(it=>it.Take(1)).FirstAsync();
                return cellStart;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("查询出错", e);
                return null;
            }
        }

        public async Task<int> InsertCellInfoAsync(string productCode, GetSfcInfoData cellInfo, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                var cellStart = new RecordCellStart();
                cellStart.Id = SnowFlakeSingle.Instance.NextId();
                cellStart.ProductCode = productCode;
                cellStart.LastOCVDate = cellInfo.LastOCVDate;
                cellStart.CreateTime = DateTime.Now;
                cellStart.Grade = cellInfo.grade;
                cellStart.O2Voltage = cellInfo.Voltage;
                int ret = await db.Insertable(cellStart).ExecuteCommandAsync();
                return ret;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("插入出错", e);
                return -1;
            }
        }
    }
}
