using FNMES.Entity.Sys;
using FNMES.Logic.Base;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Utility.Operator;
using FNMES.Utility.Other;
using FNMES.Utility.Core;

namespace FNMES.Logic.Sys
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
                return db.Queryable<SysRoleAuthorize>().ToList();
            }
        }

        /// <summary>
        /// 根据角色ID获得角色权限关系
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<SysRoleAuthorize> GetList(string roleId)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysRoleAuthorize>().Where(it => it.RoleId == roleId && it.DeleteFlag == "N").ToList();
            }
        }


        /// <summary>
        /// 给某个角色授权
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="perIds"></param>
        public void AppAuthorize(string operate, string roleId, params string[] perIds)
        {
            using (var db = GetInstance())
            {
                try
                {
                    db.BeginTran();
                    //获得所有权限
                    List<SysPermission> permissionList = db.Queryable<SysPermission>().Where(it => it.DeleteFlag == "N").ToList();
                    List<string> perList = new List<string>();
                    foreach (string perId in perIds)
                    {
                        string id = perId;
                        while (!id.IsNullOrEmpty() && id != "0")
                        {
                            if (!perList.Contains(id))
                            {
                                perList.Add(id);
                            }
                            id = permissionList.Where(it => it.Id == id).Select(it => it.ParentId).FirstOrDefault();
                        }
                    }
                    //删除旧的 
                    List<SysRoleAuthorize> list2 = db.Queryable<SysRoleAuthorize>().Where(it => it.RoleId == roleId && it.DeleteFlag == "N").ToList();
                    list2.ForEach(it => { it.DeleteFlag = "Y"; });
                    db.Updateable<SysRoleAuthorize>(list2).ExecuteCommand();


                    List<SysRoleAuthorize> newRoleAuthorizeList = perList.Select(it => new SysRoleAuthorize
                    {
                        Id = UUID.StrSnowId,
                        RoleId = roleId,
                        ModuleId = it,
                        DeleteFlag = "N",
                        EnableFlag = "Y",
                        CreateUserId = operate,
                        CreateTime = DateTime.Now,
                        ModifyUserId = operate,
                        ModifyTime = DateTime.Now

                    }).ToList();
                    db.Insertable<SysRoleAuthorize>(newRoleAuthorizeList).ExecuteCommand();
                    db.CommitTran();
                }
                catch
                {
                    db.RollbackTran();
                }
            }
        }


        /// <summary>
        /// 给某个角色授权
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="perIds"></param>
        public void Authorize(string roleId, string account, params string[] perIds)
        {
            using (var db = GetInstance())
            {
                try
                {
                    db.BeginTran();
                    //获得所有权限
                    List<SysPermission> permissionList = db.Queryable<SysPermission>().ToList();
                    List<string> perList = new List<string>();
                    foreach (string perId in perIds)
                    {
                        string id = perId;
                        while (!id.IsNullOrEmpty() && id != "0")
                        {
                            if (!perList.Contains(id))
                            {
                                perList.Add(id);
                            }
                            id = permissionList.Where(it => it.Id == id).Select(it => it.ParentId).FirstOrDefault();
                        }
                    }
                    //删除旧的
                    List<SysRoleAuthorize> list2 = db.Queryable<SysRoleAuthorize>().Where(it => it.RoleId == roleId && it.DeleteFlag == "N").ToList();
                    list2.ForEach(it => { it.DeleteFlag = "Y"; });
                    db.Updateable<SysRoleAuthorize>(list2).ExecuteCommand();


                    List<SysRoleAuthorize> newRoleAuthorizeList = perList.Select(it => new SysRoleAuthorize
                    {
                        Id = UUID.StrSnowId,
                        RoleId = roleId,
                        ModuleId = it,
                        DeleteFlag = "N",
                        EnableFlag = "Y",
                        CreateUserId = account,
                        CreateTime = DateTime.Now,
                        ModifyUserId = account,
                        ModifyTime = DateTime.Now
                    }).ToList();
                    db.Insertable<SysRoleAuthorize>(newRoleAuthorizeList).ExecuteCommand();
                    db.CommitTran();
                }
                catch
                {
                    db.RollbackTran();
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
                List<SysRoleAuthorize> list = db.Queryable<SysRoleAuthorize>().Where(it => moduleIds.Contains(it.ModuleId)).ToList();
                list.ForEach(it => { it.DeleteFlag = "Y"; });
                return db.Updateable<SysRoleAuthorize>(list).ExecuteCommand();
            }
        }

    }
}
