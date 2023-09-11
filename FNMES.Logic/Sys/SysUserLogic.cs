using FNMES.Entity.Sys;
using FNMES.Logic.Base;
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

namespace FNMES.Logic.Sys
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
            using (var db = GetInstance())
            {
                return db.Queryable<SysUser>().Where(it => it.Account == account && it.DeleteFlag == "0")
                     .Includes(it => it.Organize)
                     .Includes(it => it.CreateUser)
                     .Includes(it => it.ModifyUser)
                     .First();
            }
        }

        /// <summary>
        /// 修改用户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateBasicInfo(SysUser model, string account)
        {
            using (var db = GetInstance())
            {
                model.ModifyUserId = account;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysUser>(model).UpdateColumns(it => new
                {
                    it.RealName,
                    it.NickName,
                    it.Gender,
                    it.Birthday,
                    it.MobilePhone,
                    it.Avatar,
                    it.Email,
                    it.Signature,
                    it.Address,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }

        public int AppUpdateBasicInfo(SysUser model)
        {
            using (var db = GetInstance())
            {
                model.ModifyUserId = model.Id;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysUser>(model).UpdateColumns(it => new
                {
                    it.RealName,
                    it.NickName,
                    it.Gender,
                    it.Birthday,
                    it.MobilePhone,
                    it.Avatar,
                    it.Email,
                    it.Signature,
                    it.Address,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }

        public int Insert(SysUser model, string password, string account, string[] roleIds)
        {
            using (var db = GetInstance())
            {
                try
                {
                    db.BeginTran();
                    ////新增用户基本信息。
                    model.Id = UUID.StrSnowId;
                    model.EnableFlag = "Y";
                    model.DeleteFlag = "N";
                    model.CreateUserId = account;
                    model.CreateTime = DateTime.Now;
                    model.ModifyUserId = model.CreateUserId;
                    model.ModifyTime = model.CreateTime;
                    model.Avatar = "/Content/framework/images/avatar.png";
                    int row = db.Insertable<SysUser>(model).ExecuteCommand();
                    if (row == 0)
                    {
                        db.RollbackTran();
                        return row;
                    }
                   
                    //新增新的角色
                    List<SysUserRoleRelation> list = new List<SysUserRoleRelation>();
                    foreach (string roleId in roleIds)
                    {
                        SysUserRoleRelation roleRelation = new SysUserRoleRelation
                        {
                            Id = UUID.StrSnowId,
                            UserId = model.Id,
                            RoleId = roleId,
                            EnableFlag = "Y",
                            DeleteFlag = "N",
                            CreateUserId = account,
                            CreateTime = DateTime.Now,
                            ModifyUserId = account,
                            ModifyTime = DateTime.Now
                        };
                        list.Add(roleRelation);
                    }
                    row = db.Insertable<SysUserRoleRelation>(list).ExecuteCommand();
                    if (row == 0)
                    {
                        db.RollbackTran();
                        return row;
                    }
                    //新增用户登陆信息。
                    SysUserLogOn userLogOnEntity = new SysUserLogOn();
                    userLogOnEntity.Id = UUID.StrSnowId;
                    userLogOnEntity.UserId = model.Id;
                    userLogOnEntity.SecretKey = userLogOnEntity.Id.DESEncrypt().Substring(0, 8);
                    userLogOnEntity.Password = password.MD5Encrypt().DESEncrypt(userLogOnEntity.SecretKey).MD5Encrypt();
                    userLogOnEntity.LoginCount = 0;
                    userLogOnEntity.IsOnLine = "0";
                    userLogOnEntity.EnableFlag = "Y";
                    userLogOnEntity.DeleteFlag = "N";
                    userLogOnEntity.CreateUserId = account;
                    userLogOnEntity.CreateTime = DateTime.Now;
                    userLogOnEntity.ModifyUserId = account;
                    userLogOnEntity.ModifyTime = DateTime.Now;
                    row = db.Insertable<SysUserLogOn>(userLogOnEntity).ExecuteCommand();
                    if (row == 0)
                    {
                        db.RollbackTran();
                        return row;
                    }
                    db.CommitTran();
                    return row;
                }
                catch
                {
                    db.RollbackTran();
                    return 0;
                }
            }
        }

        public bool ContainsUser(string userAccount, params string[] userIdList)
        {
            using (var db = GetInstance())
            {
                List<string> accountList = db.Queryable<SysUser>().Where(it => userIdList.Contains(it.Id)).Select(it => it.Account).ToList();
                if (accountList.IsNullOrEmpty())
                    return false;
                if (accountList.Contains(userAccount))
                    return true;
                return false;
            }
        }
        public int AppInsert(SysUser model, string password, string[] roleIds, string opearateUser)
        {
            using (var db = GetInstance())
            {
                try
                {
                    db.BeginTran();
                    ////新增用户基本信息。
                    model.Id = UUID.StrSnowId;
                    model.EnableFlag = "Y";
                    model.DeleteFlag = "N";
                    model.CreateUserId = opearateUser;
                    model.CreateTime = DateTime.Now;
                    model.ModifyUserId = model.CreateUserId;
                    model.ModifyTime = model.CreateTime;
                    model.Avatar = "/Content/framework/images/avatar.png";
                    int row = db.Insertable<SysUser>(model).ExecuteCommand();
                    if (row == 0)
                    {
                        db.RollbackTran();
                        return row;
                    }
                  
                    //新增新的角色
                    List<SysUserRoleRelation> list = new List<SysUserRoleRelation>();
                    foreach (string roleId in roleIds)
                    {
                        SysUserRoleRelation roleRelation = new SysUserRoleRelation
                        {
                            Id = UUID.StrSnowId,
                            UserId = model.Id,
                            RoleId = roleId,
                            EnableFlag = "Y",
                            DeleteFlag = "N",
                            CreateUserId = opearateUser,
                            CreateTime = DateTime.Now,
                            ModifyUserId = opearateUser,
                            ModifyTime = DateTime.Now
                        };
                        list.Add(roleRelation);
                    }
                    row = db.Insertable<SysUserRoleRelation>(list).ExecuteCommand();
                    if (row == 0)
                    {
                        db.RollbackTran();
                        return row;
                    }
                    //新增用户登陆信息。
                    SysUserLogOn userLogOnEntity = new SysUserLogOn();
                    userLogOnEntity.Id = UUID.StrSnowId;
                    userLogOnEntity.UserId = model.Id;
                    userLogOnEntity.SecretKey = userLogOnEntity.Id.DESEncrypt().Substring(0, 8);
                    userLogOnEntity.Password = password.DESEncrypt(userLogOnEntity.SecretKey).MD5Encrypt();
                    userLogOnEntity.LoginCount = 0;
                    userLogOnEntity.IsOnLine = "0";
                    userLogOnEntity.EnableFlag = "Y";
                    userLogOnEntity.DeleteFlag = "N";
                    userLogOnEntity.CreateUserId = opearateUser;
                    userLogOnEntity.CreateTime = DateTime.Now;
                    userLogOnEntity.ModifyUserId = userLogOnEntity.CreateUserId;
                    userLogOnEntity.ModifyTime = userLogOnEntity.CreateTime;
                    row = db.Insertable<SysUserLogOn>(userLogOnEntity).ExecuteCommand();
                    if (row == 0)
                    {
                        db.RollbackTran();
                        return row;
                    }
                    db.CommitTran();
                    return row;
                }
                catch
                {
                    db.RollbackTran();
                    return 0;
                }
            }
        }


        /// <summary>
        /// 根据主键得到用户信息
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public SysUser Get(string primaryKey)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysUser>().Where(it => it.Id == primaryKey)
                     .Includes(it => it.Organize)
                     .Includes(it => it.CreateUser)
                     .Includes(it => it.ModifyUser).First();
            }
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
            using (var db = GetInstance())
            {
                ISugarQueryable<SysUser> queryable = db.Queryable<SysUser>().Where(it => it.DeleteFlag == "N");
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.Account.Contains(keyWord) || it.RealName.Contains(keyWord));
                }
                return queryable.OrderBy(it => it.SortCode)
                     .Includes(it => it.Organize)
                     .Includes(it => it.CreateUser)
                     .Includes(it => it.ModifyUser).ToPageList(pageIndex, pageSize, ref totalCount);
            }
        }

     

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        public int Delete(List<string> primaryKeys)
        {
            using (var db = GetInstance())
            {
                List<SysUser> list = db.Queryable<SysUser>().Where(it => primaryKeys.Contains(it.Id)).ToList();
                list.ForEach(it => { it.DeleteFlag = "Y"; });
                return db.Updateable<SysUser>(list).ExecuteCommand();
            }
        }

        /// <summary>
        /// 新增用户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SysUser model, string account)
        {
            using (var db = GetInstance())
            {
                model.Id = UUID.StrSnowId;
                model.DeleteFlag = "N";
                model.EnableFlag = "Y";


                model.CreateUserId = account;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                model.Avatar = "/Content/framework/images/avatar.png";
                return db.Insertable<SysUser>(model).ExecuteCommand();
            }
        }
        /// <summary>
        /// 更新用户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(SysUser model, string account)
        {
            using (var db = GetInstance())
            {
                model.ModifyUserId = account;
                model.ModifyTime = DateTime.Now;

                return db.Updateable<SysUser>(model).UpdateColumns(it => new
                {
                    it.NickName,
                    it.RealName,
                    it.Birthday,
                    it.Gender,
                    it.Email,
                    it.DepartmentId,
                    it.RoleId,
                    it.MobilePhone,
                    it.Address,
                    it.Signature,
                    it.SortCode,
                    it.IsEnabled,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }



        public int AppUpdateAndSetRole(SysUser model, string[] roleIds, string opereateUser)
        {
            using (var db = GetInstance())
            {
                try
                {
                    db.BeginTran();
                    model.ModifyUserId = opereateUser;
                    model.ModifyTime = DateTime.Now;
                    int row = db.Updateable<SysUser>(model).UpdateColumns(it => new
                    {
                        it.NickName,
                        it.RealName,
                        it.Birthday,
                        it.Gender,
                        it.Email,
                        it.DepartmentId,
                        it.RoleId,
                        it.MobilePhone,
                        it.Address,
                        it.Signature,
                        it.SortCode,
                        it.ModifyUserId,
                        it.ModifyTime
                    }).ExecuteCommand();
                    if (row == 0)
                    {
                        db.RollbackTran();
                        return row;
                    }
                    //删除原来的角色
                    List<SysUserRoleRelation> list2 = db.Queryable<SysUserRoleRelation>().Where(it => it.UserId == model.Id && it.DeleteFlag == "N").ToList();
                    list2.ForEach(it => { it.DeleteFlag = "Y"; });
                    db.Updateable<SysUserRoleRelation>(list2).ExecuteCommand();
                    //新增新的角色
                    List<SysUserRoleRelation> list = new List<SysUserRoleRelation>();
                    foreach (string roleId in roleIds)
                    {
                        SysUserRoleRelation roleRelation = new SysUserRoleRelation
                        {
                            Id = UUID.StrSnowId,
                            UserId = model.Id,
                            RoleId = roleId,
                            DeleteFlag = "N",
                            EnableFlag = "Y",
                            CreateUserId = opereateUser,
                            CreateTime = DateTime.Now,
                            ModifyUserId = opereateUser,
                            ModifyTime = DateTime.Now
                        };
                        list.Add(roleRelation);
                    }
                    row = db.Insertable<SysUserRoleRelation>(list).ExecuteCommand();
                    if (row == 0)
                    {
                        db.RollbackTran();
                        return row;
                    }
                    db.CommitTran();
                    return row;
                }
                catch
                {
                    db.RollbackTran();
                    return 0;
                }
            }
        }


        public int UpdateAndSetRole(SysUser model, string account, string[] roleIds)
        {
            using (var db = GetInstance())
            {
                try
                {
                    db.BeginTran();
                    model.ModifyUserId = account;
                    model.ModifyTime = DateTime.Now;
                    int row = db.Updateable<SysUser>(model).UpdateColumns(it => new
                    {
                        it.NickName,
                        it.RealName,
                        it.Birthday,
                        it.Gender,
                        it.Email,
                        it.DepartmentId,
                        it.RoleId,
                        it.MobilePhone,
                        it.Address,
                        it.Signature,
                        it.SortCode,
                        it.EnableFlag,
                        it.ModifyUserId,
                        it.ModifyTime
                    }).ExecuteCommand();
                    if (row == 0)
                    {
                        db.RollbackTran();
                        return row;
                    }
                    //删除原来的角色
                    List<SysUserRoleRelation> list2 = db.Queryable<SysUserRoleRelation>().Where(it => it.UserId == model.Id && it.DeleteFlag == "N").ToList();
                    list2.ForEach(it => { it.DeleteFlag = "Y"; });
                    db.Updateable<SysUserRoleRelation>(list2).ExecuteCommand();
                    //新增新的角色
                    List<SysUserRoleRelation> list = new List<SysUserRoleRelation>();
                    foreach (string roleId in roleIds)
                    {
                        SysUserRoleRelation roleRelation = new SysUserRoleRelation
                        {
                            Id = UUID.StrSnowId,
                            UserId = model.Id,
                            RoleId = roleId,
                            EnableFlag = "Y",
                            DeleteFlag = "N",
                            CreateUserId = account,
                            CreateTime = DateTime.Now,
                            ModifyTime = DateTime.Now,
                            ModifyUserId = account
                        };
                        list.Add(roleRelation);
                    }
                    row = db.Insertable<SysUserRoleRelation>(list).ExecuteCommand();
                    if (row == 0)
                    {
                        db.RollbackTran();
                        return row;
                    }
                    db.CommitTran();
                    return row;
                }
                catch
                {
                    db.RollbackTran();
                    return 0;
                }
            }
        }
    }
}
