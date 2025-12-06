using FNMES.WebUI.Logic.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
using FNMES.Entity.Record;

using SqlSugar;
using System;
using System.Linq;

using FNMES.Utility.Core;
using ServiceStack;

namespace FNMES.WebUI.Logic.Record
{
    public class RecordCellBindBlockLogic:BaseLogic
    {
        public async Task<int> InsertAsync(List<RecordCellBindBlock> models, string configId = "default")
        {
            try
            {
                var db = GetInstance(configId);
                //是否能生效
                foreach (var model in models)
                {
                    model.Id = SnowFlakeSingle.Instance.NextId();
                    model.CreateTime = DateTime.Now;
                }
                var ret = await db.Insertable(models).SplitTable().ExecuteCommandAsync();
                return ret;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo($"上传批量数据失败", e);
                return -1;
            }
        }

        public async Task<RecordCellBindBlock> GetBlockProductCodeAsync(string cellProductCode, string configId = "default")
        {
            try
            {
                var db = GetInstance(configId);
                var cellBindBlocks = await db.Queryable<RecordCellBindBlock>().Where(e => e.CellBarcode == cellProductCode).SplitTable(e=>e.Take(3)).ToListAsync();
                var cellBindBlock = cellBindBlocks.OrderByDescending(it => it.CreateTime).First();
                return cellBindBlock;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo($"通过电芯码{cellProductCode}获取block码失败", e);
                return null;
            }
        }

        public List<RecordCellBindBlock> GetSplitPageList(int pageIndex, int pageSize, string configId, string startDate, string endDate, string keyword, ref int totalCount)
        {
            try
            {

                var db = GetInstance(configId);
                ISugarQueryable<RecordCellBindBlock> queryable = db.Queryable<RecordCellBindBlock>();

                if (startDate.IsNullOrEmpty())
                {
                    DateTime nowTime = DateTime.Now;
                    startDate = nowTime.AddDays(-30).ToString();
                }

                if (endDate.IsNullOrEmpty())
                {
                    endDate = DateTime.Now.ToString();
                }

                DateTime start = Convert.ToDateTime(startDate);
                DateTime end = Convert.ToDateTime(endDate);
                TimeSpan daysSpan = new TimeSpan(end.Ticks - start.Ticks);

                if (daysSpan.TotalDays > 90)
                    end = start.AddDays(-90);

                queryable = queryable.SplitTable(start, end);
                if (!keyword.IsNullOrEmpty())
                {
                    queryable.Where(e => e.BlockBarcode == keyword || e.CellBarcode == keyword);
                }
                var ret = queryable.ToPageList(pageIndex, pageSize, ref totalCount);
                return ret;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<RecordCellBindBlock>();
            }
        }
    }
}
