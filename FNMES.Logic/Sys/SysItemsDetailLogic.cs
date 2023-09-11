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
    public class SysItemsDetailLogic : BaseLogic
    {
        public List<SysItemDetail> GetItemDetailList(string strItemCode)
        {
            using (var db = GetInstance())
            {

                SysItem item = db.Queryable<SysItem>().Where(it => it.EnCode == strItemCode && it.DeleteFlag == "N").First();
                if (null == item)
                    return null;
                return db.Queryable<SysItemDetail>().Where(it => it.ItemId == item.Id && it.DeleteFlag == "N")
                    .Includes(it => it.CreateUser)
                    .Includes(it => it.ModifyUser)
                    .OrderBy(it => it.SortCode)
                    .ToList();
            }
        }

        public List<SysItemDetail> GetList(int pageIndex, int pageSize, string itemId, string keyWord, ref int totalCount)
        {
            using (var db = GetInstance())
            {
                ISugarQueryable<SysItemDetail> queryable = db.Queryable<SysItemDetail>().Where(it => it.DeleteFlag == "N" && it.ItemId == itemId);
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

        public List<SysItemDetail> GetListByItemId(string itemId)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysItemDetail>()
                    .Where(it => it.ItemId == itemId && it.DeleteFlag == "N")
                    .Includes(it => it.CreateUser)
                    .Includes(it => it.ModifyUser)
                    .ToList();
            }
        }

        public void InsertItemDetail(string itemId, List<SysItemDetail> list)
        {
            using (var db = GetInstance())
            {
                try
                {
                    db.BeginTran();
                    List<SysItemDetail> list2 = db.Queryable<SysItemDetail>().Where(it => it.ItemId == itemId && it.DeleteFlag == "N").ToList();
                    list2.ForEach(it => { it.DeleteFlag = "Y"; });
                    db.Updateable<SysItemDetail>(list2).ExecuteCommand();
                    db.Insertable<SysItemDetail>(list).ExecuteCommand();
                    db.CommitTran();
                }
                catch
                {
                    db.RollbackTran();
                }
            }
        }

        public SysItemDetail Get(string primaryKey)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysItemDetail>()
                    .Where(it => it.DeleteFlag == "N" && it.Id == primaryKey)
                    .Includes(it => it.CreateUser)
                    .Includes(it => it.ModifyUser)
                    .First();
            }
        }

        public int Insert(SysItemDetail model, string account)
        {
            using (var db = GetInstance())
            {
                model.Id = UUID.StrSnowId;
                model.IsDefault = model.IsDefault == null ? "0" : "1";
                model.DeleteFlag = "N";
                model.CreateUserId = account;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                return db.Insertable<SysItemDetail>(model).ExecuteCommand();
            }
        }

        public int AppInsert(SysItemDetail model, string operateUser)
        {
            using (var db = GetInstance())
            {
                model.Id = UUID.StrSnowId;
                model.IsDefault = "0";
                model.DeleteFlag = "N";
                model.CreateUserId = operateUser;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                return db.Insertable<SysItemDetail>(model).ExecuteCommand();
            }
        }


        public int AppUpdate(SysItemDetail model, string operateUser)
        {
            using (var db = GetInstance())
            {
                model.ModifyUserId = operateUser;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysItemDetail>(model).UpdateColumns(it => new
                {
                    it.EnCode,
                    it.Name,
                    it.SortCode,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }

        public int Delete(string itemId)
        {
            using (var db = GetInstance())
            {
                SysItemDetail itemDetail = db.Queryable<SysItemDetail>().Where(it => it.Id == itemId).First();
                if (itemDetail == null)
                    return 0;
                itemDetail.DeleteFlag = "Y";
                return db.Updateable<SysItemDetail>(itemDetail).ExecuteCommand();
            }
        }

        public int Update(SysItemDetail model, string account)
        {
            using (var db = GetInstance())
            {
                model.IsDefault = model.IsDefault == null ? "0" : "1";
                model.ModifyUserId = account;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysItemDetail>(model).UpdateColumns(it => new
                {
                    it.ItemId,
                    it.EnCode,
                    it.Name,
                    it.IsDefault,
                    it.SortCode,
                    it.EnableFlag,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }

        public SysItemDetail GetSoftwareName()
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysItemDetail>().Where(it => it.EnCode == "SoftwareName").First();
            }
        }
    }
}
