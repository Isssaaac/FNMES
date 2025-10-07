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

namespace FNMES.WebUI.Logic.Sys
{
    public class SysEquipmentLogic : BaseLogic
    {
        public List<SysEquipment> GetEquipmentList(string strItemCode)
        {
            try
            {
                var db = GetInstance();
                SysEquipment equipment = db.Queryable<SysEquipment>().Where(it => it.EnCode == strItemCode).First();
                if (null == equipment)
                    return null;
                return db.Queryable<SysEquipment>().Where(it => it.LineId == equipment.Id)
                    //.Includes(it => it.CreateUser)
                    //.Includes(it => it.ModifyUser)
                    .OrderBy(it => it.SortCode)
                    .ToList();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<SysEquipment>();
            }
        }

        public List<SysEquipment> GetList(int pageIndex, int pageSize, long lineId, string keyWord, ref int totalCount)
        {
            try
            {
                //为修改后实时查询，走主库
                var db = GetInstance();
                ISugarQueryable<SysEquipment> queryable = db.MasterQueryable<SysEquipment>().Where(it => it.LineId == lineId);
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => (it.Name.Contains(keyWord) || it.EnCode.Contains(keyWord)));
                }
                return queryable
                    //.Includes(it => it.CreateUser)
                    //.Includes(it => it.ModifyUser)
                    .Includes(it => it.Line)
                    .OrderBy(it => it.SortCode)
                    .ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<SysEquipment> ();
            }
        }

        public List<SysEquipment> GetListByLineId(long lineId)
        {
            try
            {
                var db = GetInstance();
                return db.Queryable<SysEquipment>()
                    .Where(it => it.LineId == lineId)
                    //.Includes(it => it.CreateUser)
                    //.Includes(it => it.ModifyUser)
                    .ToList();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<SysEquipment> ();
            }
        }

        
        public SysEquipment GetByIP(string IP)
        {
            try
            {
                var db = GetInstance();
                //业务逻辑强制走主库
                return db.MasterQueryable<SysEquipment>()
                    .Where(it => it.IP == IP)
                    .Includes(it => it.Line)
                    //.Includes(it => it.CreateUser)
                    //.Includes(it => it.ModifyUser)
                    .First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new SysEquipment ();
            }
        }

        public SysEquipment GetByLineStation(string station,string configId)
        {
            try
            {
                var db = GetInstance();
                //业务逻辑强制走主库
                return db.MasterQueryable<SysEquipment>()
                    .Where(it => it.BigProcedure == station)
                    .Includes(it => it.Line)
                    .Where(it=>it.Line.ConfigId == configId)
                    .First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new SysEquipment();
            }
        }




        public SysEquipment Get(long primaryKey)
        {
            try
            {
                //为了修改后实时显示，直接走主库
                var db = GetInstance();
                return db.MasterQueryable<SysEquipment>()
                    .Where(it => it.Id == primaryKey)
                    //.Includes(it => it.CreateUser)
                    //.Includes(it => it.ModifyUser)
                    .First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new SysEquipment();
            }
        }

        public int Insert(SysEquipment model, long account)
        {
            try
            {
                var db = GetInstance();
                model.Id = SnowFlakeSingle.instance.NextId();
                model.CreateUserId = account;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                return db.Insertable(model).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }




        public int Delete(long pk)
        {
            try
            {
                var db = GetInstance();
                return db.Deleteable<SysEquipment>().Where(it => it.Id == pk).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }

        public int Update(SysEquipment model, long account)
        {
            try
            {
                var db = GetInstance();
                model.ModifyUserId = account;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysEquipment>(model).UpdateColumns(it => new
                {
                    it.LineId,
                    it.IP,
                    it.EnCode,
                    it.Name,
                    it.UnitProcedure,
                    it.BigProcedure,
                    it.SortCode,
                    it.EnableFlag,
                    it.Description,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }

        public SysEquipment GetSoftwareName()
        {
            try
            {
                var db = GetInstance();
                return db.Queryable<SysEquipment>().Where(it => it.EnCode == "SoftwareName").First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new SysEquipment();
            }
        }

        public bool Align(List<SysEquipment> equipments)
        {
            try
            {
                var db = GetInstance();
                foreach (var equipment in equipments)
                {
                    var existingItem = db.Queryable<SysEquipment>().First(x => x.EnCode == equipment.EnCode);

                    if (existingItem != null)
                    {
                        // 更新现有记录，忽略某些字段
                        db.Updateable(equipment)
                          .WhereColumns(w => w.EnCode)
                          .UpdateColumns(e => new { e.Name, e.BigProcedure})
                          .ExecuteCommand();
                    }
                    else
                    {
                        db.Insertable(equipment).ExecuteCommand();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return false;
            }
        }
    }
}
