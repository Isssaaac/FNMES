using FNMES.Entity.Sys;
using FNMES.Logic.Base;
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

namespace FNMES.Logic.Sys
{
    public class SysItemLogic : BaseLogic
    {

        public List<SysItem> GetList()
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysItem>().Where(it => it.DeleteFlag == "N")
                    .Includes(it => it.CreateUser)
                    .Includes(it => it.ModifyUser)
                    .OrderBy(it => it.SortCode)
                    .ToList();
            }
        }

        public List<SysItem> GetAppList(int pageIndex, int pageSize, ref int totalCount)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysItem>().Where(it => it.ParentId != "0" && it.DeleteFlag == "N")
                    .Includes(it => it.CreateUser)
                    .Includes(it => it.ModifyUser)
                    .OrderBy(it => it.SortCode)
                    .ToPageList(pageIndex, pageSize, ref totalCount);
            }
        }
        public List<SysItem> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            using (var db = GetInstance())
            {
                ISugarQueryable<SysItem> queryable = db.Queryable<SysItem>().Where(it => it.DeleteFlag == "N");

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
        }


        public int GetChildCount(string parentId)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysItem>()
                    .Where(it => it.ParentId == parentId && it.DeleteFlag == "N")
                    .ToList().Count();
            }
        }


        public SysItem Get(string primaryKey)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysItem>().Where(it => it.DeleteFlag == "N")
                    .Where(it => it.Id == primaryKey)
                    .Includes(it => it.CreateUser)
                    .Includes(it => it.ModifyUser)
                    .First();
            }
        }


        public int AppInsert(SysItem model, string operateUser)
        {
            using (var db = GetInstance())
            {
                SysItem s = db.Queryable<SysItem>().Where(it => it.ParentId == "0").First();
                model.Id = UUID.StrSnowId;
                model.Layer = s.Layer + 1;
                model.ParentId = s.Id; 
                model.DeleteFlag = "N";
                model.CreateUserId = operateUser;
                model.CreateTime = DateTime.Now;
                model.ModifyUser = model.CreateUser;
                model.ModifyTime = model.CreateTime;
                return db.Insertable<SysItem>(model).ExecuteCommand();
            }
        }

        public int Insert(SysItem model, string account)
        {
            using (var db = GetInstance())
            {
                model.Id = UUID.StrSnowId;
                model.Layer = Get(model.ParentId).Layer += 1;
                model.DeleteFlag = "N";
                model.CreateUserId = account;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                return db.Insertable<SysItem>(model).ExecuteCommand();
            }
        }

        public int Delete(string primaryKey)
        {
            using (var db = GetInstance())
            {
                SysItem item = db.Queryable<SysItem>().Where(it => it.Id == primaryKey).First();
                if (item == null)
                    return 0;
                item.DeleteFlag = "Y";
                return db.Updateable<SysItem>(item).ExecuteCommand();
            }
        }
        public int Update(SysItem model, string account)
        {
            using (var db = GetInstance())
            {
                model.Layer = Get(model.ParentId).Layer += 1;
                model.ModifyUserId = account;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysItem>(model).UpdateColumns(it => new
                {
                    it.ParentId,
                    it.Layer,
                    it.EnCode,
                    it.Name,
                    it.SortCode,
                    it.EnableFlag,
                    it.Remark,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }


        public int AppUpdate(SysItem model, string operateUser)
        {
            using (var db = GetInstance())
            {
                model.ModifyUserId = operateUser;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysItem>(model).UpdateColumns(it => new
                {
                    it.EnCode,
                    it.Name,
                    it.SortCode,
                    it.Remark,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }
    }
}
