using FNMES.Entity.Sys;
using FNMES.Logic.Base;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Utility.Web;
using FNMES.Utility.Operator;
using FNMES.Utility.Extension;
using FNMES.Utility.Core;
using FNMES.Utility.Other;

namespace FNMES.Logic.Sys
{
    public class SysPermissionLogic : BaseLogic
    {
        public bool ActionValidate(string userId, string action)
        {
            List<SysPermission> authorizeModules;
            if (new SysUserLogic().ContainsUser("admin", userId))
            {
                authorizeModules = GetList();
            }
            else
            {
                authorizeModules = GetList(userId);
            }
            foreach (var item in authorizeModules)
            {
                if (!string.IsNullOrEmpty(item.Url))
                {
                    string[] url = item.Url.Split('?');
                    if (url[0].ToLower() == action.ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public List<SysPermission> GetList(string userId)
        {
            using (var db = GetInstance())
            {
                List<string> permissionIdList = db.Queryable<SysUserRoleRelation, SysRoleAuthorize, SysPermission>((A, B, C) => new object[] {
                      JoinType.Left,A.RoleId == B.RoleId,
                      JoinType.Left,C.Id == B.ModuleId,
                    })
                    .Where((A, B, C) => A.UserId == userId && C.EnableFlag == "Y" && C.DeleteFlag == "N")
                    .Select((A, B, C) => C.Id).ToList();
                return db.Queryable<SysPermission>().Where(it => permissionIdList.Contains(it.Id)).OrderBy(it => it.SortCode).ToList();
            }
        }


        public List<SysPermission> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            using (var db = GetInstance())
            {
                if (keyWord.IsNullOrEmpty())
                {
                    return db.Queryable<SysPermission>().Where(it => it.DeleteFlag == "N").OrderBy(it => it.SortCode).ToPageList(pageIndex, pageSize, ref totalCount);
                }
                return db.Queryable<SysPermission>().Where(it => it.DeleteFlag == "N" && (it.Name.Contains(keyWord) || it.EnCode.Contains(keyWord))).OrderBy(it => it.SortCode).ToPageList(pageIndex, pageSize, ref totalCount);
            }
        }

        public int Delete(params string[] primaryKeys)
        {
            using (var db = GetInstance())
            {
                try
                {
                    db.BeginTran();
                    //删除权限与角色的对应关系。
                    List<SysPermission> list = db.Queryable<SysPermission>().Where(it => primaryKeys.Contains(it.Id) && it.DeleteFlag == "N").ToList();
                    List<string> ids = list.Select(it => it.Id).ToList();
                    list.ForEach(it => { it.DeleteFlag = "Y"; });
                    db.Updateable<SysPermission>(list).ExecuteCommand();
                    List<SysRoleAuthorize> list2 = db.Queryable<SysRoleAuthorize>().Where(it => ids.Contains(it.ModuleId) && it.DeleteFlag == "N").ToList();
                    list2.ForEach(it => { it.DeleteFlag = "Y"; });
                    db.Updateable<SysRoleAuthorize>(list2).ExecuteCommand();
                    db.CommitTran();
                    return 1;
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return 0;
                }
            }

        }
        public int GetMaxChildMenuOrderCode(string parentId)
        {
            using (var db = GetInstance())
            {
                //得到当前节点
                SysPermission permission = db.Queryable<SysPermission>().Where(it => it.Id == parentId && it.DeleteFlag == "N").First();
                if (permission == null)
                    return 0;
                //得到子的
                SysPermission child = db.Queryable<SysPermission>().Where(it => it.ParentId == parentId && it.DeleteFlag == "N").OrderBy(it => it.SortCode, OrderByType.Desc).First();
                if (child == null)
                    return permission.SortCode.Value + 100;
                else
                    return child.SortCode.Value + 100;
            }
        }
        public int GetChildCount(string parentId)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysPermission>().Where(it => it.ParentId == parentId && it.DeleteFlag == "N").ToList().Count();
            }
        }

        public List<SysPermission> GetList()
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysPermission>().Where(it => it.DeleteFlag == "N").OrderBy(it => it.SortCode).ToList();
            }
        }

        public SysPermission Get(string primaryKey)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysPermission>().Where(it => it.Id == primaryKey).Includes(it => it.CreateUser).Includes(it => it.ModifyUser).First();
            }
        }


        public int Insert(SysPermission model, string account)
        {
            using (var db = GetInstance())
            {
                model.Id = UUID.StrSnowId;
                model.Layer = model.Type == 2 ? 0 : model.Type == 0 ? 1 : 2;
                model.IsEdit = model.IsEdit == null ? "0" : "1";
                model.IsPublic = model.IsPublic == null ? "0" : "1";

                model.DeleteFlag = "N";
                model.EnableFlag = "Y";
                model.CreateUserId = account;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                return db.Insertable<SysPermission>(model).ExecuteCommand();
            }
        }
        public int AppInsert(SysPermission model, string operateUser)
        {
            using (var db = GetInstance())
            {
                model.Id = UUID.StrSnowId;
                model.Layer = model.Type == 2 ? 0 : model.Type == 0 ? 1 : 2;
                model.IsEdit = "1";
                model.IsPublic = "0";

                model.DeleteFlag = "N";
                model.EnableFlag = "Y";
                model.CreateUserId = operateUser;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                return db.Insertable<SysPermission>(model).ExecuteCommand();
            }
        }

        public int AppUpdate(SysPermission model, string operateUser)
        {
            using (var db = GetInstance())
            {
                model.Layer = model.Type == 2 ? 0 : model.Type == 0 ? 1 : 2;
                model.ModifyUserId = operateUser;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysPermission>(model).UpdateColumns(it => new
                {
                    it.ParentId,
                    it.Layer,
                    it.EnCode,
                    it.Name,
                    it.JsEvent,
                    it.Icon,
                    it.SymbolIndex,
                    it.Url,
                    it.Remark,
                    it.Type,
                    it.SortCode,
                    it.ModifyUserId,
                    it.ModifyTime,
                }).ExecuteCommand();
            }
        }


        public int Update(SysPermission model, string account)
        {
            using (var db = GetInstance())
            {
                model.Layer = model.Type == 2 ? 0 : model.Type == 0 ? 1 : 2;
                model.IsEdit = model.IsEdit == null ? "0" : "1";
                model.IsPublic = model.IsPublic == null ? "0" : "1";
                model.ModifyUserId = account;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysPermission>(model).UpdateColumns(it => new
                {
                    it.ParentId,
                    it.Layer,
                    it.EnCode,
                    it.Name,
                    it.JsEvent,
                    it.Icon,
                    it.SymbolIndex,
                    it.Url,
                    it.Remark,
                    it.Type,
                    it.SortCode,
                    it.IsPublic,
                    it.EnableFlag,
                    it.IsEdit,
                    it.ModifyUserId,
                    it.ModifyTime,
                }).ExecuteCommand();
            }
        }
        public int InsertList(List<SysPermission> permissionList)
        {
            using (var db = GetInstance())
            {
                return db.Insertable<SysPermission>(permissionList).ExecuteCommand();
            }
        }
    }
}
