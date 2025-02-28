using FNMES.Entity.Sys;
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
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.DTO.AppData;
using ServiceStack;

namespace FNMES.WebUI.Logic.Sys
{
    public class SysPermissionLogic : BaseLogic
    {

        //权限均走主库
        public bool ActionValidate(long userId, string action)
        {
            List<SysPermission> authorizeModules;
            if (new SysUserLogic().ContainsUser("admin", userId.ToString()))
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


        public List<SysPermission> GetList(long userId)
        {
            try
            {
                var db = GetInstance();
                Db.BeginTran();
                List<long> permissionIdList = db.Queryable<SysUserRoleRelation, SysRoleAuthorize, SysPermission>((A, B, C) => new object[] {
                          JoinType.Left,A.RoleId == B.RoleId,
                          JoinType.Left,C.Id == B.ModuleId,
                        })
                    .Where((A, B, C) => A.UserId == userId && C.EnableFlag == "1")
                    .Select((A, B, C) => C.Id).ToList();
                Db.CommitTran();
                return db.MasterQueryable<SysPermission>().Where(it => permissionIdList.Contains(it.Id)).OrderBy(it => it.SortCode).ToList();
            }
            catch (Exception ex) {
                Logger.ErrorInfo("获取权限异常", ex);
                return new List<SysPermission>();
            }
        }


        public List<SysPermission> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            //为了修改后实时显示，直接走主库
            var db = GetInstance();
            if (keyWord.IsNullOrEmpty())
            {
                return db.MasterQueryable<SysPermission>().OrderBy(it => it.SortCode).ToPageList(pageIndex, pageSize, ref totalCount);
            }
            
            return db.MasterQueryable<SysPermission>().Where(it => it.Name.Contains(keyWord) || it.EnCode.Contains(keyWord)).OrderBy(it => it.SortCode).ToPageList(pageIndex, pageSize, ref totalCount);
        }

        public int Delete(params string[] primaryKeys)
        {
            var db = GetInstance();
            try
            {
                Logger.RunningInfo(primaryKeys.ToJson());
                Db.BeginTran();
                //删除权限与角色的对应关系。
                db.Deleteable<SysPermission>().Where((it) => primaryKeys.Contains(it.Id.ToString())).ExecuteCommand();
                db.Deleteable<SysRoleAuthorize>().Where((it) => primaryKeys.Contains(it.ModuleId.ToString())).ExecuteCommand();
                Db.CommitTran();
                return 1;
            }
            catch (Exception)
            {
                Db.RollbackTran();
                return 0;
            }

        }
        public int GetMaxChildMenuOrderCode(string parentId)
        {
            var db = GetInstance();
            //得到当前节点
            SysPermission permission = db.MasterQueryable<SysPermission>().Where(it => it.Id.ToString() == parentId).First();
            if (permission == null)
                return 0;
            //得到子的
            SysPermission child = db.MasterQueryable<SysPermission>().Where(it => it.ParentId.ToString() == parentId).OrderBy(it => it.SortCode, OrderByType.Desc).First();
            if (child == null)
                return permission.SortCode.Value + 100;
            else
                return child.SortCode.Value + 100;
        }
        public int GetChildCount(long parentId)
        {
            var db = GetInstance();
            return db.MasterQueryable<SysPermission>().Where(it => it.ParentId == parentId).ToList().Count;
        }

        public List<SysPermission> GetList()
        {
            try
            {
                var db = GetInstance();
                return db.MasterQueryable<SysPermission>().OrderBy(it => it.SortCode).ToList();
            }
            catch (Exception ex)
            {
                Logger.ErrorInfo("获取权限异常", ex);
                return new List<SysPermission>();
            }
        }

        public SysPermission Get(long primaryKey = 0)
        {
            var db = GetInstance();
            return db.MasterQueryable<SysPermission>().Where(it => it.Id == primaryKey).Includes(it => it.CreateUser).Includes(it => it.ModifyUser).First();
        }


        public int Insert(SysPermission model, long  operateId)
        {
            var db = GetInstance();
            model.Id = SnowFlakeSingle.instance.NextId();
            model.Layer = model.Type == 2 ? 0 : model.Type == 0 ? 1 : 2;
            model.IsEdit = model.IsEdit == null ? "0" : "1";
            model.IsEnabled = model.IsEnabled;
            model.CreateUserId = operateId;
            model.CreateTime = DateTime.Now;
            model.ModifyUserId = model.CreateUserId;
            model.ModifyTime = model.CreateTime;
            return db.Insertable<SysPermission>(model).ExecuteCommand();
        }
        public int AppInsert(SysPermission model, long operateId)
        {
            var db = GetInstance();
            model.Id = SnowFlakeSingle.instance.NextId();
            model.Layer = model.Type == 2 ? 0 : model.Type == 0 ? 1 : 2;
            model.IsEdit = "1";
            model.EnableFlag = "Y";
            model.CreateUserId = operateId;
            model.CreateTime = DateTime.Now;
            model.ModifyUserId = model.CreateUserId;
            model.ModifyTime = model.CreateTime;
            return db.Insertable<SysPermission>(model).ExecuteCommand();
        }

        public int AppUpdate(SysPermission model, long operateId)
        {
            var db = GetInstance();
            model.Layer = model.Type == 2 ? 0 : model.Type == 0 ? 1 : 2;
            model.ModifyUserId = operateId;
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
                it.Description,
                it.EnableFlag,
                it.IsEdit,
                it.Type,
                it.SortCode,
                it.ModifyUserId,
                it.ModifyTime,
            }).ExecuteCommand();
        }


        public int Update(SysPermission model, long operateId)
        {
            var db = GetInstance();
            model.Layer = model.Type == 2 ? 0 : model.Type == 0 ? 1 : 2;
            model.IsEdit = model.IsEdit == null ? "0" : "1";
            model.ModifyUserId = operateId;
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
                it.Description,
                it.EnableFlag,
                it.IsEdit,
                it.Type,
                it.SortCode,
                it.ModifyUserId,
                it.ModifyTime,
            }).ExecuteCommand();
        }
        public int InsertList(List<SysPermission> permissionList)
        {
            var db = GetInstance();
            permissionList.ForEach(it => it.Id = SnowFlakeSingle.instance.NextId());
            return db.Insertable<SysPermission>(permissionList).ExecuteCommand();
        }

        public List<SysPermission> GetPermissions(List<string> roleEncodes)
        {
            //权限查询只能通过主库查询
            var db = GetInstance();
            List<long> roleIds = db.MasterQueryable<SysRole>().Where(it => roleEncodes.Contains(it.EnCode)).Select(it=> it.Id).ToList();
            List<SysPermission> sysPermissions = new List<SysPermission>();
            if (roleIds.Count > 0)
            {
                List<long> permissionIds = db.MasterQueryable< SysRoleAuthorize>().Where(it => roleIds.Contains((long)it.RoleId)).Select(it => (long)it.ModuleId).ToList();
                sysPermissions = db.MasterQueryable<SysPermission>().Where(it => permissionIds.Contains(it.Id) && it.Type>=3).ToList();
            }
            return sysPermissions;
        }
    }
}
