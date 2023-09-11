using FNMES.Entity.Sys;
using FNMES.Logic.Base;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Utility.Operator;

namespace FNMES.Logic.Sys
{
    public class SysUserRoleRelationLogic : BaseLogic
    {
        /// <summary>
        /// 删除用户角色关系
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public int Delete(List<string> userIds)
        {
            using (var db = GetInstance())
            {
                List<SysUserRoleRelation> list = db.Queryable<SysUserRoleRelation>().Where(it => userIds.Contains(it.UserId)).ToList();
                list.ForEach(it => { it.DeleteFlag = "Y"; });
                return db.Updateable<SysUserRoleRelation>(list).ExecuteCommand();
            }
        }

        /// <summary>
        /// 根据ID得到用户角色关系
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public List<SysUserRoleRelation> GetList(string userId)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysUserRoleRelation>().Where(it => it.UserId == userId && it.DeleteFlag=="N").ToList();
            }
        }

        /// <summary>
        /// 从用户角色关系表中得到所有角色绑定信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<SysUserRoleRelation> GetByRoles(List<string> ids)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysUserRoleRelation>().Where(it => ids.Contains(it.RoleId) && it.DeleteFlag == "N").ToList();
            }
        }
         
    }
}
