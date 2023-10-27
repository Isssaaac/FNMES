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
            var db = GetInstance();

            SysEquipment equipment = db.Queryable<SysEquipment>().Where(it => it.EnCode == strItemCode).First();
            if (null == equipment)
                return null;
            return db.Queryable<SysEquipment>().Where(it => it.LineId == equipment.Id)
                .Includes(it => it.CreateUser)
                .Includes(it => it.ModifyUser)
                .OrderBy(it => it.SortCode)
                .ToList();
        }

        public List<SysEquipment> GetList(int pageIndex, int pageSize, long lineId, string keyWord, ref int totalCount)
        {
            var db = GetInstance();
            ISugarQueryable<SysEquipment> queryable = db.Queryable<SysEquipment>().Where(it => it.LineId == lineId);
            if (!keyWord.IsNullOrEmpty())
            {
                queryable = queryable.Where(it => (it.Name.Contains(keyWord) || it.EnCode.Contains(keyWord)));
            }
            return queryable
                .Includes(it => it.CreateUser)
                .Includes(it => it.ModifyUser)
                .Includes(it => it.Line)
                .OrderBy(it => it.SortCode)
                .ToPageList(pageIndex, pageSize, ref totalCount);
        }

        public List<SysEquipment> GetListByLineId(long lineId)
        {
            var db = GetInstance();
            return db.Queryable<SysEquipment>()
                .Where(it => it.LineId == lineId)
                .Includes(it => it.CreateUser)
                .Includes(it => it.ModifyUser)
                .ToList();
        }
        public SysEquipment GetByIP(string IP)
        {
            var db = GetInstance();
            return db.Queryable<SysEquipment>()
                .Where(it => it.IP == IP)
                .Includes(it => it.Line)
                .Includes(it => it.CreateUser)
                .Includes(it => it.ModifyUser)
                .First();
        }






        public SysEquipment Get(long primaryKey)
        {
            var db = GetInstance();
            return db.Queryable<SysEquipment>()
                .Where(it => it.Id == primaryKey)
                .Includes(it => it.CreateUser)
                .Includes(it => it.ModifyUser)
                .First();
        }

        public int Insert(SysEquipment model, long account)
        {
            var db = GetInstance();
            model.Id = SnowFlakeSingle.instance.NextId();
            model.CreateUserId = account;
            model.CreateTime = DateTime.Now;
            model.ModifyUserId = model.CreateUserId;
            model.ModifyTime = model.CreateTime;
            return db.Insertable<SysEquipment>(model).ExecuteCommand();
        }




        public int Delete(long pk)
        {
            var db = GetInstance();
            return db.Deleteable<SysEquipment>().Where(it => it.Id == pk).ExecuteCommand();
        }

        public int Update(SysEquipment model, long account)
        {
            using (var db = GetInstance())
            {
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
        }

        public SysEquipment GetSoftwareName()
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysEquipment>().Where(it => it.EnCode == "SoftwareName").First();
            }
        }
    }
}
