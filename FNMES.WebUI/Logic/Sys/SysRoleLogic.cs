using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Entity.Sys;
using SqlSugar;
using FNMES.Utility.Operator;
using FNMES.Utility.Extension;
using FNMES.Utility.Core;
using FNMES.Utility.Other;
using FNMES.WebUI.Logic.Base;

namespace FNMES.WebUI.Logic.Sys
{
    public class SysRoleLogic : BaseLogic
    {
        /// <summary>
        /// 得到角色列表
        /// </summary>
        /// <returns></returns>
        public List<SysRole> GetList()
        {
            var db = GetInstance();
            return db.MasterQueryable<SysRole>()
                .Includes(it => it.CreateUser)
                .Includes(it => it.ModifyUser)
                .ToList();
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
             var db = GetInstance();

            ISugarQueryable<SysRole> queryable = db.MasterQueryable<SysRole>();

            if (!keyWord.IsNullOrEmpty())
            {
                queryable = queryable.Where(it => (it.Name.Contains(keyWord) || it.EnCode.Contains(keyWord)));
            }

            return queryable.Includes(it => it.CreateUser)
                .Includes(it => it.ModifyUser)
                .OrderBy(it => it.SortCode)
                .ToPageList(pageIndex, pageSize, ref totalCount);
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SysRole model, long account)
        {
             var db = GetInstance();
            model.Id = SnowFlakeSingle.instance.NextId();
            model.AllowEdit = model.AllowEdit == null ? "0" : "1";
            model.CreateUserId = account;
            model.CreateTime = DateTime.Now;
            model.ModifyUserId = model.CreateUserId;
            model.ModifyTime = model.CreateTime;
            return db.Insertable<SysRole>(model).ExecuteCommand();
        }

        public int AppInsert(SysRole model, long operateUser)
        {
             var db = GetInstance();
            model.Id = SnowFlakeSingle.instance.NextId();
            model.AllowEdit = "1";
            model.CreateUserId = operateUser;
            model.CreateTime = DateTime.Now;
            model.ModifyUserId = model.CreateUserId;
            model.ModifyTime = model.CreateTime;
            return db.Insertable<SysRole>(model).ExecuteCommand();
        }

        public int AppUpdate(SysRole model, long operateUser)
        {
             var db = GetInstance();
            model.AllowEdit = model.AllowEdit == null ? "0" : "1";
            model.ModifyUserId = operateUser;
            model.ModifyTime = DateTime.Now;
            return db.Updateable<SysRole>(model).UpdateColumns(it => new
            {
                it.EnCode,
                it.Type,
                it.Name,
                it.AllowEdit,
                it.Description,
                it.SortCode,
                it.ModifyUserId,
                it.ModifyTime
            }).ExecuteCommand();
        }


        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(SysRole model, long operateUser)
        {
             var db = GetInstance();
            model.AllowEdit = model.AllowEdit == null ? "0" : "1";
            model.ModifyUserId = operateUser;
            model.ModifyTime = DateTime.Now;
            return db.Updateable<SysRole>(model).UpdateColumns(it => new
            {
                it.EnCode,
                it.Type,
                it.Name,
                it.AllowEdit,
                it.Description,
                it.SortCode,
                it.EnableFlag,
                it.ModifyUserId,
                it.ModifyTime
            }).ExecuteCommand();
        }

        /// <summary>
        /// 根据主键得到角色信息
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public SysRole Get(long primaryKey)
        {
             var db = GetInstance();
            return db.MasterQueryable<SysRole>().Where(it => it.Id == primaryKey)
               .Includes(it => it.CreateUser)
               .Includes(it => it.ModifyUser)
               .First();
        }
        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        public int Delete(List<long> primaryKeys)
        {
             var db = GetInstance();

            return db.Deleteable<SysRole>().Where(it => primaryKeys.Contains(it.Id)).ExecuteCommand();
        }
    }
}
