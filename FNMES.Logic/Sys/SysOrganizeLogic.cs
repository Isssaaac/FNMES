using FNMES.Entity.Sys;
using FNMES.Logic.Base;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Utility.Operator;
using FNMES.Utility.Extension;
using FNMES.Utility.Core;
using FNMES.Utility.Other;

namespace FNMES.Logic.Sys
{
    public class SysOrganizeLogic : BaseLogic
    {
        public List<SysOrganize> GetList()
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysOrganize>().Where(it => it.DeleteFlag == "N").ToList();
            }
        }

        public List<SysOrganize> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            using (var db = GetInstance())
            {
                ISugarQueryable<SysOrganize> queryable = db.Queryable<SysOrganize>().Where(it => it.ParentId != "0" && it.DeleteFlag == "N");

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => (it.FullName.Contains(keyWord) || it.EnCode.Contains(keyWord)));
                }
                return queryable
                    .OrderBy(it => it.SortCode)
                    .ToPageList(pageIndex, pageSize, ref totalCount);
            }
        }



        public int GetChildCount(string parentId)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysOrganize>().Where(it => it.ParentId == parentId && it.DeleteFlag == "N").Count();
            }
        }


        public int AppUpdate(SysOrganize model, string opreaterUser)
        {
            using (var db = GetInstance())
            {
                model.ModifyUserId = opreaterUser;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysOrganize>(model).UpdateColumns(it => new
                {
                    it.EnCode,
                    it.FullName,
                    it.Type,
                    it.ManagerId,
                    it.TelePhone,
                    it.WeChat,
                    it.Fax,
                    it.Email,
                    it.Address,
                    it.SortCode,
                    it.Remark,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }

        public int AppInsert(SysOrganize model, string opreaterUser)
        {
            using (var db = GetInstance())
            {
                SysOrganize s = db.Queryable<SysOrganize>().Where(it => it.ParentId == "0").First();
                model.Id = UUID.StrSnowId;
                model.Layer = s.Layer + 1;
                model.ParentId = s.Id;

                model.EnableFlag = "Y";
                model.DeleteFlag = "N";
                model.CreateUserId = opreaterUser;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                return db.Insertable<SysOrganize>(model).ExecuteCommand();
            }
        }
        public int Insert(SysOrganize model, string account)
        {
            using (var db = GetInstance())
            {
                model.Id = UUID.StrSnowId;
                model.Layer = Get(model.ParentId).Layer += 1;
                model.EnableFlag = "Y";
                model.DeleteFlag = "N";
                model.CreateUserId = account;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                return db.Insertable<SysOrganize>(model).ExecuteCommand();
            }
        }

        public int Delete(string primaryKey)
        {
            using (var db = GetInstance())
            {
                SysOrganize organize = db.Queryable<SysOrganize>().Where(it => it.Id == primaryKey).First();
                if (organize == null)
                    return 0;
                organize.DeleteFlag = "Y";
                return db.Updateable<SysOrganize>(organize).ExecuteCommand();
            }
        }
        public SysOrganize Get(string primaryKey)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysOrganize>().Where(it => it.Id == primaryKey).Includes(it => it.CreateUser).Includes(it => it.ModifyUser).First();
            }
        }
        public int Update(SysOrganize model, string account)
        {
            using (var db = GetInstance())
            {
                model.ModifyUserId = account;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysOrganize>(model).UpdateColumns(it => new
                {
                    it.ParentId,
                    it.Layer,
                    it.EnCode,
                    it.FullName,
                    it.Type,
                    it.ManagerId,
                    it.TelePhone,
                    it.WeChat,
                    it.Fax,
                    it.Email,
                    it.Address,
                    it.SortCode,
                    it.EnableFlag,
                    it.Remark,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }
    }
}
