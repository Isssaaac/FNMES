using FNMES.Entity.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using FNMES.Utility.Core;
using FNMES.WebUI.Logic.Base;

namespace FNMES.WebUI.Logic.Sys
{
    public class SysRoleAuthorizeLogic : BaseLogic
    {
        /// <summary>
        /// 获得角色权限关系
        /// </summary>
        /// <returns></returns>
        public List<SysRoleAuthorize> GetList()
        {
            using (var db = GetInstance())
            {
                return db.MasterQueryable<SysRoleAuthorize>().ToList();
            }
        }

        /// <summary>
        /// 根据角色ID获得角色权限关系
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<SysRoleAuthorize> GetList(long roleId)
        {
            using (var db = GetInstance())
            {
                return db.MasterQueryable<SysRoleAuthorize>().Where(it => it.RoleId == roleId).ToList();
            }
        }


        /// <summary>
        /// 给某个角色授权
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="perIds"></param>
        public void AppAuthorize(long operateId, long roleId, params long[] perIds)
        {
            using (var sysdb = GetInstance())
            {
                try
                {
                    Db.BeginTran();
                    //获得所有权限
                    List<SysPermission> permissionList = sysdb.MasterQueryable<SysPermission>().ToList();
                    List<long> perList = new();
                    foreach (long perId in perIds)
                    {
                        long id = perId;
                        while (!id.IsNullOrEmpty() && id != 0)
                        {
                            if (!perList.Contains(id))
                            {
                                perList.Add(id);
                            }
                            //选取一个权限则必须添加其父权限
                            id = (long)permissionList.Where(it => it.Id == id).Select(it => it.ParentId).FirstOrDefault();
                        }
                    }
                    //删除旧的 
                    sysdb.Deleteable<SysRoleAuthorize>().Where(it => it.RoleId == roleId).ExecuteCommand();


                    List <SysRoleAuthorize> newRoleAuthorizeList = perList.Select(it => new SysRoleAuthorize
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        RoleId = roleId,
                        PermissionId = it,
                        CreateUser = operateId,
                        CreateTime = DateTime.Now,

                    }).ToList();
                    sysdb.Insertable<SysRoleAuthorize>(newRoleAuthorizeList).ExecuteCommand();
                    Db.CommitTran();
                }
                catch
                {
                    Db.RollbackTran();
                }
            }
        }


        /// <summary>
        /// 给某个角色授权
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="perIds"></param>
        public void Authorize(long roleId, long operateId, params long[] perIds)
        {
            using (var sysdb = GetInstance())
            {
                try
                {
                    Db.BeginTran();
                    //获得所有权限
                    List<SysPermission> permissionList = sysdb.MasterQueryable<SysPermission>().ToList();
                    List<long> perList = new List<long>();
                    foreach (long perId in perIds)
                    {
                        long id = perId;
                        while (!id.IsNullOrEmpty() && id != 0)
                        {
                            if (!perList.Contains(id))
                            {
                                perList.Add(id);
                            }
                            id = (long)permissionList.Where(it => it.Id == id).Select(it => it.ParentId).FirstOrDefault();
                        }
                    }
                    //删除旧的 
                    sysdb.Deleteable<SysRoleAuthorize>().Where(it => it.RoleId == roleId).ExecuteCommand();


                    List<SysRoleAuthorize> newRoleAuthorizeList = perList.Select(it => new SysRoleAuthorize
                    {   
                        Id = SnowFlakeSingle.instance.NextId(),
                        RoleId = roleId,
                        PermissionId = it,
                        CreateUser = operateId,
                        CreateTime = DateTime.Now,
                    }).ToList();
                    sysdb.Insertable<SysRoleAuthorize>(newRoleAuthorizeList).ExecuteCommand();
                    Db.CommitTran();
                }
                catch
                {
                    Db.RollbackTran();
                }
            }
        }

        /// <summary>
        /// 从角色权限关系中删除某个模块
        /// </summary>
        /// <param name="moduleIds"></param>
        /// <returns></returns>
        public int Delete(params string[] moduleIds)
        {
            using (var db = GetInstance())
            {
                return db.Deleteable<SysRoleAuthorize>().Where(it => moduleIds.Contains(it.PermissionId.ToString())).ExecuteCommand();
            }
        }

    }
}
