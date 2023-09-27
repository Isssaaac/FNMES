using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Utility.Operator;
using FNMES.Utility.Extension;
using FNMES.Utility.Core;
using FNMES.Entity;
using FNMES.Utility.Other;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Sys;

namespace FNMES.WebUI.Logic.Sys
{
    public class SysLineLogic : BaseLogic
    {

        public List<SysLine> GetList()
        {
            using var db = GetInstance();
            return db.Queryable<SysLine>()
                .Includes(it => it.CreateUser)
                .Includes(it => it.ModifyUser)
                .OrderBy(it => it.SortCode)
                .ToList();
        }

      
        public List<SysLine> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            using var db = GetInstance();
            ISugarQueryable<SysLine> queryable = db.Queryable<SysLine>();

            if (!keyWord.IsNullOrEmpty())
            {
                queryable = queryable.Where(it => (it.Name.Contains(keyWord) || it.EnCode.Contains(keyWord)));
            }
            return queryable
                .Includes(it => it.CreateUser)
                .Includes(it => it.ModifyUser)
                .OrderBy(it => it.SortCode)
                .ToPageList(pageIndex, pageSize, ref totalCount);
        }




        public SysLine Get(long primaryKey)
        {
            using var db = GetInstance();
            return db.Queryable<SysLine>()
                .Where(it => it.Id == primaryKey)
                .Includes(it => it.CreateUser)
                .Includes(it => it.ModifyUser)
                .First();
        }


        public int Insert(SysLine model, long account)
        {
            using var db = GetInstance();
            model.Id = SnowFlakeSingle.instance.NextId();
            model.CreateUserId = account;
            model.CreateTime = DateTime.Now;
            model.ModifyUserId = model.CreateUserId;
            model.ModifyTime = model.CreateTime;
            return db.Insertable<SysLine>(model).ExecuteCommand();
        }

        public int Delete(long primaryKey)
        {
            using var db = GetInstance();
            return db.Deleteable<SysLine>().Where(it => it.Id == primaryKey).ExecuteCommand();
        }
        public int Update(SysLine model, long account)
        {
            using var db = GetInstance();
            model.ModifyUserId = account;
            model.ModifyTime = DateTime.Now;
            return db.Updateable<SysLine>(model).UpdateColumns(it => new
            {
                it.EnCode,
                it.Name,
                it.ConfigId,
                it.SortCode,
                it.EnableFlag,
                it.Description,
                it.ModifyUserId,
                it.ModifyTime
            }).ExecuteCommand();
        }
    }
}
