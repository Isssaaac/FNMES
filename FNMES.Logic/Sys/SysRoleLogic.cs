using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Entity.Sys;
using FNMES.Logic.Base;
using SqlSugar;
using FNMES.Utility.Operator;
using FNMES.Utility.Extension;
using FNMES.Utility.Core;
using FNMES.Utility.Other;

namespace FNMES.Logic.Sys
{
    public class SysRoleLogic : BaseLogic
    {
        /// <summary>
        /// 得到角色列表
        /// </summary>
        /// <returns></returns>
        public List<SysRole> GetList()
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysRole>().Where(it => it.DeleteFlag == "N")
                    .Includes(it => it.Organize)
                    .Includes(it => it.CreateUser)
                    .Includes(it => it.ModifyUser)
                    .ToList();
            }
        }

        /// <summary>
        /// 获得角色列表分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<SysRole> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            using (var db = GetInstance())
            {

                ISugarQueryable<SysRole> queryable = db.Queryable<SysRole>().Where(it => it.DeleteFlag == "N");

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => (it.Name.Contains(keyWord) || it.EnCode.Contains(keyWord)));
                }

                return queryable.Includes(it => it.Organize)
                    .Includes(it => it.CreateUser)
                    .Includes(it => it.ModifyUser)
                    .OrderBy(it => it.SortCode)
                    .ToPageList(pageIndex, pageSize, ref totalCount);
            }
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SysRole model, string account)
        {
            using (var db = GetInstance())
            {
                model.Id = UUID.StrSnowId;
                model.AllowEdit = model.AllowEdit == null ? "0" : "1";
                model.DeleteFlag = "N";
                model.CreateUserId = account;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                return db.Insertable<SysRole>(model).ExecuteCommand();
            }
        }

        public int AppInsert(SysRole model, string operateUser)
        {
            using (var db = GetInstance())
            {
                model.Id = UUID.StrSnowId;
                model.AllowEdit = "1";
                model.DeleteFlag = "N";
                model.CreateUserId = operateUser;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                return db.Insertable<SysRole>(model).ExecuteCommand();
            }
        }

        public int AppUpdate(SysRole model, string operateUser)
        {
            using (var db = GetInstance())
            {
                model.AllowEdit = model.AllowEdit == null ? "0" : "1";
                model.ModifyUserId = operateUser;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysRole>(model).UpdateColumns(it => new
                {
                    it.OrganizeId,
                    it.EnCode,
                    it.Type,
                    it.Name,
                    it.Remark,
                    it.SortCode,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }


        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(SysRole model, string account)
        {
            using (var db = GetInstance())
            {
                model.AllowEdit = model.AllowEdit == null ? "0" : "1";
                model.ModifyUserId = account;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysRole>(model).UpdateColumns(it => new
                {
                    it.OrganizeId,
                    it.EnCode,
                    it.Type,
                    it.Name,
                    it.AllowEdit,
                    it.EnableFlag,
                    it.Remark,
                    it.SortCode,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }

        /// <summary>
        /// 根据主键得到角色信息
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public SysRole Get(string primaryKey)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysRole>().Where(it => it.Id == primaryKey)
                   .Includes(it => it.Organize)
                   .Includes(it => it.CreateUser)
                   .Includes(it => it.ModifyUser)
                   .First();
            }
        }
        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        public int Delete(List<string> primaryKeys)
        {
            using (var db = GetInstance())
            {
                List<SysRole> list = db.Queryable<SysRole>().Where(it => primaryKeys.Contains(it.Id)).ToList();
                list.ForEach(it => { it.DeleteFlag = "Y"; });
                return db.Updateable<SysRole>(list).ExecuteCommand();
            }
        }
    }
}
