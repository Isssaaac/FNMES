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
using FNMES.Entity.Param;

namespace FNMES.WebUI.Logic.Param
{
    public class UnitProcedureLogic : BaseLogic
    {
      

        public List<ParamUnitProcedure> GetList(int pageIndex, int pageSize, string keyWord,string configId, ref int totalCount)
        {
            try
            {
                using var db = GetInstance(configId);
                if (keyWord.IsNullOrEmpty())
                {
                    return db.Queryable<ParamUnitProcedure>().ToPageList(pageIndex, pageSize, ref totalCount);
                }

                return db.Queryable<ParamUnitProcedure>().Where(it => it.Name.Contains(keyWord) || it.Encode.Contains(keyWord)).ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception)
            {

                return null;
            }
        }

        public int Delete(long primaryKey,string configId)
        {
            using var db = GetInstance(configId);
            try
            {
                Db.BeginTran();
                //删除权限与角色的对应关系。
                db.Deleteable<ParamUnitProcedure>().Where((it) => primaryKey == it.Id).ExecuteCommand();
                db.Deleteable<ParamUnitProcedure>().Where((it) => primaryKey == it.Pid).ExecuteCommand();
                Db.CommitTran();
                return 1;
            }
            catch (Exception)
            {
                Db.RollbackTran();
                return 0;
            }

        }

        public ParamUnitProcedure Get(long primaryKey ,string configId)
        {
            try
            {
                using var db = GetInstance(configId);
                ParamUnitProcedure entity = db.Queryable<ParamUnitProcedure>().Where(it => it.Id == primaryKey).First();
                var sysDb = GetInstance();
                entity.CreateUser = sysDb.Queryable<SysUser>().Where(it => it.Id == entity.CreateUserId).First();
                entity.ModifyUser = sysDb.Queryable<SysUser>().Where(it => it.Id == entity.ModifyUserId).First();
                return entity;
            }
            catch (Exception)
            {
                return new ParamUnitProcedure { Id = 0,Name = "查无此项"};
            }
        }


        public int Insert(ParamUnitProcedure model, long  operateId)
        {
            try
            {
                using var db = GetInstance(model.ConfigId);
                model.Id = SnowFlakeSingle.instance.NextId();
                model.CreateUserId = operateId;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                return db.Insertable<ParamUnitProcedure>(model).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }

        public int Update(ParamUnitProcedure model, long operateId)
        {
            using var db = GetInstance(model.ConfigId);
            model.ModifyUserId = operateId;
            model.ModifyTime = DateTime.Now;
            return db.Updateable<ParamUnitProcedure>(model).IgnoreColumns(it => new
            {
                it.CreateTime, it.CreateUserId
            }).ExecuteCommand();
        }

        public List<ParamUnitProcedure> GetParentList(string configId)
        {

            try
            {
                using var db = GetInstance(configId);
                return db.Queryable<ParamUnitProcedure>().Where(it => it.IsParent == "1").ToList();
            }
            catch (Exception)
            {

                return new List<ParamUnitProcedure>();
            }
        }
        public List<ParamUnitProcedure> GetSonList(string configId)
        {

            try
            {
                using var db = GetInstance(configId);
                return db.Queryable<ParamUnitProcedure>().Where(it => it.IsParent == "0").ToList();
            }
            catch (Exception)
            {

                return new List<ParamUnitProcedure>();
            }
        }






        public List<ParamUnitProcedure> GetProcedureList(string configId, string parent)
        {
            try
            {
                using var db = GetInstance(configId);
                
                if (parent == null || parent.Length ==0)
                {
                    return db.Queryable<ParamUnitProcedure>().Where(it => it.IsParent == "0").ToList(); 
                }
                ParamUnitProcedure p =  db.Queryable<ParamUnitProcedure>().Where(it => it.Encode == parent).First();
                if (p != null)
                {
                    return db.Queryable<ParamUnitProcedure>().Where(it => (it.IsParent == "0" && it.Pid == p.Id)).ToList();
                }

                return new List<ParamUnitProcedure>();

            }
            catch (Exception)
            {

                return new List<ParamUnitProcedure>();
            }
        }
    }
}
