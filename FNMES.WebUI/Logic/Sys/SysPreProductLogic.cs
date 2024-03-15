using FNMES.Entity.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Utility.Operator;
using FNMES.Utility.Extension;
using FNMES.Utility.Core;
using FNMES.Utility.Other;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.DTO.AppData;
using FNMES.Entity.Record;

namespace FNMES.WebUI.Logic.Sys
{
    public class SysPreProductLogic : BaseLogic
    {
        //用于页面显示   显示当前各工序对产品的选择情况
        public List<SysPreSelectProduct> GetList(int pageIndex, int pageSize, long lineId, string keyWord, ref int totalCount,string index)
        {
            try
            {
                //为修改后实时查询，走主库
                var db = GetInstance();
                ISugarQueryable<SysPreSelectProduct> queryable = db.MasterQueryable<SysPreSelectProduct>().Where(it => it.LineId == lineId);
                if(index == "1")
                {
                    queryable = queryable.Where(it => it.EnableFlag == "1");
                }
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => (it.ProductPartNo.Contains(keyWord) || it.ProductDescription.Contains(keyWord)));
                }
                return queryable
                    .Includes(it => it.Line)
                    .ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<SysPreSelectProduct> ();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">要查询的对象模型</param>
        /// <param name="configId">当前选择机台的产品所属线体</param>
        /// <returns></returns>
        public int Insert(SysPreSelectProduct model, string configId)
        {
            try
            {
                var db = GetInstance();
                SysLine sysLine = db.MasterQueryable<SysLine>().Where(it => it.ConfigId == configId).First();
                if (sysLine == null)
                {
                    return 0;
                }
                Db.BeginTran();
                //将旧的记录标识未非启用状态，用做历史数据
                db.Updateable<SysPreSelectProduct>().SetColumns(it => it.EnableFlag == "0").
                    Where(it => (it.LineId == sysLine.Id) && (it.Station == model.Station)).ExecuteCommand();
                //再新增一条数据
                model.Id = SnowFlakeSingle.instance.NextId();
                model.LineId = sysLine.Id;
                model.CreateTime = DateTime.Now;
                model.IsEnabled = true;
                int v = db.Insertable<SysPreSelectProduct>(model).ExecuteCommand();
                Db.CommitTran();
                return v;
                
            }
            catch (Exception e)
            {
                Db.RollbackTran();
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }

        public SysPreSelectProduct Query(string station,string configId)
        {
            try
            {
                var db = GetInstance();
                SysLine sysLine = db.MasterQueryable<SysLine>().Where(it => it.ConfigId == configId).First();
                if (sysLine == null)
                {
                    return null;
                }
                return db.MasterQueryable<SysPreSelectProduct>().Where(it => it.LineId == sysLine.Id
                    && it.Station == station && it.EnableFlag == "1").Includes(it => it.Line).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return null;
            }
        }
        
        //将自动铆接数据临时存储
        public int InsertRivet(RecordHotRivetData model, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.Insertable<RecordHotRivetData>(model).SplitTable().ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }

        public RecordHotRivetData QueryHotRivet(string productCode,string configId)
        {
            try
            {
                var db = GetInstance(configId);
                return db.MasterQueryable<RecordHotRivetData>().Where(it => it.ProductCode == productCode).
                    SplitTable(tabs => tabs.Take(2)).OrderByDescending(it => it.Id).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return null;
            }
        }
    }
}
