using FNMES.Entity.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Utility.Operator;
using FNMES.WebUI.Logic.Base;

namespace FNMES.WebUI.Logic.Sys
{
    public class SysUserRoleRelationLogic : BaseLogic
    {
        /// <summary>
        /// 删除用户角色关系
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public int Delete(List<long> userIds)
        {
            using var db = GetInstance();
            return db.Deleteable<SysUserRoleRelation>().Where(it => userIds.Contains(it.UserId)).ExecuteCommand();
        }

        /// <summary>
        /// 根据ID得到用户角色关系
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public List<SysUserRoleRelation> GetList(long userId)
        {
            using var db = GetInstance();
            return db.Queryable<SysUserRoleRelation>().Where(it => it.UserId == userId).ToList();
        }

        /// <summary>
        /// 从用户角色关系表中得到所有角色绑定信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<SysUserRoleRelation> GetByRoles(List<long> ids)
        {
            using var db = GetInstance();
            return db.Queryable<SysUserRoleRelation>().Where(it => ids.Contains(it.RoleId)).ToList();
        }
         
    }
}
