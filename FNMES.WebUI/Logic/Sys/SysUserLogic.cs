using FNMES.Entity.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Utility.Operator;
using FNMES.Utility.Security;
using FNMES.Utility.Extension;
using FNMES.Utility.Core;
using FNMES.Utility.Other;
using System.Drawing.Printing;
using Microsoft.VisualBasic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using FNMES.WebUI.Logic.Base;

namespace FNMES.WebUI.Logic.Sys
{
    public class SysUserLogic : BaseLogic
    {
        /// <summary>
        /// 根据账号得到用户信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public SysUser GetByUserName(string account)
        {
            using var db = GetInstance();
            return db.Queryable<SysUser>().Where(it => it.UserNo == account)
                   //  .Includes(it => it.Organize) && it.DeleteFlag == "0"
                   .Includes(it => it.CreateUser)
                   .Includes(it => it.ModifyUser)
                 .First();
        }

        /// <summary>
        /// 修改用户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateBasicInfo(SysUser model, long account)
        {
            using var db = GetInstance();
            model.ModifyUserId = account;
            model.ModifyTime = DateTime.Now;
            return db.Updateable<SysUser>(model).UpdateColumns(it => new
            {
                it.UserNo,
                it.CardNo,
                it.Name,
                it.EnableFlag,
                it.Description,
                it.SortCode,
                it.ModifyUserId,
                it.ModifyTime
            }).ExecuteCommand();
        }

        public int AppUpdateBasicInfo(SysUser model)
        {
            using var db = GetInstance();
            model.ModifyUserId = model.Id;
            model.ModifyTime = DateTime.Now;
            return db.Updateable<SysUser>(model).UpdateColumns(it => new
            {
                it.UserNo,
                it.CardNo,
                it.Name,
                it.EnableFlag,
                it.Description,
                it.SortCode,
                it.ModifyUserId,
                it.ModifyTime
            }).ExecuteCommand();
        }

        public int Insert(SysUser model, string password, long account, string[] roleIds)
        {
            using var db = GetInstance();
            try
            {
                Db.BeginTran();
                ////新增用户基本信息。
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.EnableFlag = "1";
                model.CreateUserId = account;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                int row = db.Insertable<SysUser>(model).ExecuteCommand();
                if (row == 0)
                {
                    Db.RollbackTran();
                    return row;
                }

                //新增新的角色
                List<SysUserRoleRelation> list = new();
                foreach (string roleId in roleIds)
                {
                    SysUserRoleRelation roleRelation = new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        UserId = model.Id,
                        RoleId = long.Parse(roleId),
                        CreateUser = account,
                        CreateTime = DateTime.Now,
                    };
                    list.Add(roleRelation);
                }
                row = db.Insertable<SysUserRoleRelation>(list).ExecuteCommand();
                if (row == 0)
                {
                    Db.RollbackTran();
                    return row;
                }
                //新增用户登陆信息。
                SysUserLogOn userLogOnEntity = new()
                {
                    Id = SnowFlakeSingle.instance.NextId(),
                    UserId = model.Id
                };
                userLogOnEntity.SecretKey = userLogOnEntity.Id.ToString().DESEncrypt()[..8];
                userLogOnEntity.Password = password.MD5Encrypt().DESEncrypt(userLogOnEntity.SecretKey).MD5Encrypt();
                userLogOnEntity.LoginCount = 0;
                userLogOnEntity.IsOnLine = "0";
                userLogOnEntity.ModifyUserId = account;
                userLogOnEntity.ModifyTime = DateTime.Now;
                row = db.Insertable<SysUserLogOn>(userLogOnEntity).ExecuteCommand();
                if (row == 0)
                {
                    Db.RollbackTran();
                    return row;
                }
                Db.CommitTran();
                return row;
            }
            catch (Exception)
            {
                Db.RollbackTran();
                return 0;
            }
        }

        public bool ContainsUser(string userAccount, params string[] userIdList)
        {
            using var db = GetInstance();
            List<string> accountList = db.Queryable<SysUser>().Where(it => userIdList.Contains(it.Id.ToString())).Select(it => it.UserNo).ToList();
            if (accountList.IsNullOrEmpty())
                return false;
            if (accountList.Contains(userAccount))
                return true;
            return false;
        }
        public int AppInsert(SysUser model, string password, string[] roleIds, long opearateUser)
        {
            using var db = GetInstance();
            try
            {
                Db.BeginTran();
                ////新增用户基本信息。
                model.Id = SnowFlakeSingle.instance.NextId();
                model.EnableFlag = "Y";
                model.CreateUserId = opearateUser;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                int row = db.Insertable<SysUser>(model).ExecuteCommand();
                if (row == 0)
                {
                    Db.RollbackTran();
                    return row;
                }

                //新增新的角色
                List<SysUserRoleRelation> list = new();
                foreach (string roleId in roleIds)
                {
                    SysUserRoleRelation roleRelation = new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        UserId = model.Id,
                        RoleId = long.Parse(roleId),
                        CreateUser = opearateUser,
                        CreateTime = DateTime.Now,
                    };
                    list.Add(roleRelation);
                }
                row = db.Insertable<SysUserRoleRelation>(list).ExecuteCommand();
                if (row == 0)
                {
                    Db.RollbackTran();
                    return row;
                }
                //新增用户登陆信息。
                SysUserLogOn userLogOnEntity = new()
                {
                    Id = SnowFlakeSingle.instance.NextId(),
                    UserId = model.Id
                };
                userLogOnEntity.SecretKey = userLogOnEntity.Id.ToString().DESEncrypt()[..8];
                userLogOnEntity.Password = password.DESEncrypt(userLogOnEntity.SecretKey).MD5Encrypt();
                userLogOnEntity.LoginCount = 0;
                userLogOnEntity.IsOnLine = "0";
                userLogOnEntity.ModifyUserId = opearateUser;
                userLogOnEntity.ModifyTime = DateTime.Now;
                row = db.Insertable<SysUserLogOn>(userLogOnEntity).ExecuteCommand();
                if (row == 0)
                {
                    Db.RollbackTran();
                    return row;
                }
                Db.CommitTran();
                return row;
            }
            catch
            {
                Db.RollbackTran();
                return 0;
            }
        }


        /// <summary>
        /// 根据主键得到用户信息
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public SysUser Get(long primaryKey)
        {
            using var db = GetInstance();
            return db.Queryable<SysUser>().Where(it => it.Id == primaryKey)
                 .Includes(it => it.CreateUser)
                 .Includes(it => it.ModifyUser).First();
        }

        /// <summary>
        /// 获得用户列表分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<SysUser> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            using var db = GetInstance();
            ISugarQueryable<SysUser> queryable = db.Queryable<SysUser>();
            if (!keyWord.IsNullOrEmpty())
            {
                queryable = queryable.Where(it => it.UserNo.Contains(keyWord) || it.Name.Contains(keyWord));
            }
            return queryable.OrderBy(it => it.SortCode)
                 .Includes(it => it.CreateUser)
                 .Includes(it => it.ModifyUser).ToPageList(pageIndex, pageSize, ref totalCount);
        }

     

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        public int Delete(List<long> primaryKeys)
        {
            using var db = GetInstance();

            return db.Deleteable<SysUser>().Where(it => primaryKeys.Contains(it.Id)).ExecuteCommand();
        }

        /// <summary>
        /// 新增用户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SysUser model, long userId)
        {
            using var db = GetInstance();
            model.Id = SnowFlakeSingle.instance.NextId();
            model.EnableFlag = "Y";
            model.CreateUserId = userId;
            model.CreateTime = DateTime.Now;
            model.ModifyUserId = model.CreateUserId;
            model.ModifyTime = model.CreateTime;
            return db.Insertable<SysUser>(model).ExecuteCommand();
        }
        /// <summary>
        /// 更新用户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(SysUser model, long userId)
        {
            using var db = GetInstance();
            model.ModifyUserId = userId;
            model.ModifyTime = DateTime.Now;

            return db.Updateable<SysUser>(model).UpdateColumns(it => new
            {
                it.CardNo,
                it.UserNo,
                it.Name,
                it.EnableFlag,
                it.SortCode,
                it.IsEnabled,
                it.ModifyUserId,
                it.ModifyTime
            }).ExecuteCommand();
        }



        public int AppUpdateAndSetRole(SysUser model, long[] roleIds, long opereateUser)
        {
            using var db = GetInstance();
            try
            {
                Db.BeginTran();
                model.ModifyUserId = opereateUser;
                model.ModifyTime = DateTime.Now;
                int row = db.Updateable<SysUser>(model).UpdateColumns(it => new
                {
                    it.CardNo,
                    it.UserNo,
                    it.Name,
                    it.EnableFlag,
                    it.SortCode,
                    it.IsEnabled,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
                if (row == 0)
                {
                    Db.RollbackTran();
                    return row;
                }
                //删除原来的角色
                db.Deleteable<SysUserRoleRelation>().Where(it => it.UserId == model.Id);

                //新增新的角色
                List<SysUserRoleRelation> list = new();
                foreach (long roleId in roleIds)
                {
                    SysUserRoleRelation roleRelation = new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        UserId = model.Id,
                        RoleId = roleId,
                        CreateUser = opereateUser,
                        CreateTime = DateTime.Now,
                    };
                    list.Add(roleRelation);
                }
                row = db.Insertable<SysUserRoleRelation>(list).ExecuteCommand();
                if (row == 0)
                {
                    Db.RollbackTran();
                    return row;
                }
                Db.CommitTran();
                return row;
            }
            catch
            {
                Db.RollbackTran();
                return 0;
            }
        }


        public int UpdateAndSetRole(SysUser model, long opereateUser, long[] roleIds)
        {
            using var db = GetInstance();
            try
            {
                Db.BeginTran();
                model.ModifyUserId = opereateUser;
                model.ModifyTime = DateTime.Now;
                int row = db.Updateable<SysUser>(model).UpdateColumns(it => new
                {
                    it.CardNo,
                    it.UserNo,
                    it.Name,
                    it.EnableFlag,
                    it.SortCode,
                    it.IsEnabled,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
                Logger.RunningInfo($"更新用户{row}");
                if (row == 0)
                {
                    Db.RollbackTran();
                    return row;
                }
                //删除原来的角色
                row = db.Deleteable<SysUserRoleRelation>().Where(it => it.UserId == model.Id).ExecuteCommand();
                Logger.RunningInfo($"删除角色{row}");
                //新增新的角色
                List<SysUserRoleRelation> list = new();
                foreach (long roleId in roleIds)
                {
                    SysUserRoleRelation roleRelation = new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        UserId = model.Id,
                        RoleId = roleId,
                        CreateUser = opereateUser,
                        CreateTime = DateTime.Now,
                    };
                    list.Add(roleRelation);
                }
                row = db.Insertable<SysUserRoleRelation>(list).ExecuteCommand();
                Logger.RunningInfo($"插入角色{row}");
                if (row == 0)
                {
                    Db.RollbackTran();
                    return row;
                }
                Db.CommitTran();
                return row;
            }
            catch
            {
                Db.RollbackTran();
                return 0;
            }
        }
    }
}
