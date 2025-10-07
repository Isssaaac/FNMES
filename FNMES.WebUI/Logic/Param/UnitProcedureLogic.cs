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
using System.Drawing.Printing;
using FNMES.Entity.DTO;

namespace FNMES.WebUI.Logic.Param
{
    public class UnitProcedureLogic : BaseLogic
    {
        public ParamUnitProcedure GetByStation(string stationCode, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamUnitProcedure> queryable = db.MasterQueryable<ParamUnitProcedure>().Where(it =>  it.Encode == stationCode);
                var unitProcedure = queryable.First();
                return unitProcedure;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }
        public List<ParamUnitProcedure> GetList(int pageIndex, int pageSize, string keyWord,string configId, ref int totalCount)
        {
            try
            {
                var db = GetInstance(configId);
                if (keyWord.IsNullOrEmpty())
                {
                    return db.MasterQueryable<ParamUnitProcedure>().ToPageList(pageIndex, pageSize, ref totalCount);
                }

                return db.MasterQueryable<ParamUnitProcedure>().Where(it => it.Name.Contains(keyWord) || it.Encode.Contains(keyWord)).ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }

        public List<ParamUnitProcedure> GetList(string configId)
        {
            try
            {
                var db = GetInstance(configId);
                var entities = db.MasterQueryable<ParamUnitProcedure>().ToList();
                return entities;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }

        public int Delete(long primaryKey,string configId)
        {
            var db = GetInstance(configId);
            try
            {
                Db.BeginTran();
                //这里删除大工站会把对应的小工站也给删除掉
                db.Deleteable<ParamUnitProcedure>().Where((it) => primaryKey == it.Id).ExecuteCommand();
                db.Deleteable<ParamUnitProcedure>().Where((it) => primaryKey == it.Pid).ExecuteCommand();
                Db.CommitTran();
                return 1;
            }
            catch (Exception E)
            {
                Db.RollbackTran();
                Logger.ErrorInfo(E.Message);
                return 0;
            }

        }

        public ParamUnitProcedure Get(long primaryKey ,string configId)
        {
            try
            {
                var db = GetInstance(configId);
                ParamUnitProcedure entity = db.MasterQueryable<ParamUnitProcedure>().Where(it => it.Id == primaryKey).First();
                var sysDb = GetInstance();
                entity.CreateUser = sysDb.MasterQueryable<SysUser>().Where(it => it.Id == entity.CreateUserId).First();
                entity.ModifyUser = sysDb.MasterQueryable<SysUser>().Where(it => it.Id == entity.ModifyUserId).First();
                return entity;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new ParamUnitProcedure { Id = 0,Name = "查无此项"};
            }
        }


        public int Insert(ParamUnitProcedure model, long  operateId)
        {
            try
            {
                var db = GetInstance(model.ConfigId);
                model.Id = SnowFlakeSingle.instance.NextId();
                model.CreateUserId = operateId;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                return db.Insertable(model).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }

        public int Update(ParamUnitProcedure model, long operateId)
        {
            try
            {
                var db = GetInstance(model.ConfigId);
                model.ModifyUserId = operateId;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<ParamUnitProcedure>(model).IgnoreColumns(it => new
                {
                    it.CreateTime,
                    it.CreateUserId
                }).ExecuteCommand();
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return 0;
            }
        }

        public List<ParamUnitProcedure> GetParentList(string configId)
        {

            try
            {
                var db = GetInstance(configId);
                return db.MasterQueryable<ParamUnitProcedure>().Where(it => it.IsParent == "1").ToList();
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<ParamUnitProcedure>();
            }
        }

        public List<ParamUnitProcedure> GetSonList(string configId)
        {

            try
            {
                var db = GetInstance(configId);
                return db.MasterQueryable<ParamUnitProcedure>().Where(it => it.IsParent == "0").ToList();
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<ParamUnitProcedure>();
            }
        }






        public List<ParamUnitProcedure> GetProcedureList(string configId, string parent)
        {
            try
            {
                var db = GetInstance(configId);
                
                if (parent == null || parent.Length ==0)
                {
                    return db.MasterQueryable<ParamUnitProcedure>().Where(it => it.IsParent == "0").ToList(); 
                }
                ParamUnitProcedure p =  db.MasterQueryable<ParamUnitProcedure>().Where(it => it.Encode == parent).First();
                if (p != null)
                {
                    return db.MasterQueryable<ParamUnitProcedure>().Where(it => (it.IsParent == "0" && it.Pid == p.Id)).ToList();
                }

                return new List<ParamUnitProcedure>();

            }
            catch (Exception)
            {

                return new List<ParamUnitProcedure>();
            }
        }

        public bool import(List<UnitProcedure> list, string configId)
        {
            try
            {
                var db = GetInstance(configId);

                List<ParamUnitProcedure> paramUnitProcedures = new List<ParamUnitProcedure>();

                foreach (var e in list.Where(it => it.Parent==""))
                {
                    ParamUnitProcedure item = new ParamUnitProcedure();
                    item.IsParent = "1";
                    item.Pid = 0;
                    item.Id = SnowFlakeSingle.Instance.NextId();
                    item.Encode = e.Encode;
                    item.Name = e.Name;
                    item.Description = e.Description;
                    item.CreateTime = DateTime.Now;

                    item.InStationProductType = e.InStationProductType;
                    item.OutStationProductType = e.OutStationProductType;
                    paramUnitProcedures.Add(item);
                }
                //先添加大工站
                foreach (var e in list.Where(it => it.Parent != ""))
                {
                    ParamUnitProcedure item = new ParamUnitProcedure();
                    item.IsParent = "0";
                    item.Pid = paramUnitProcedures.Where(it => it.Encode == e.Parent).Select(it => it.Id).First();
                    item.Id = SnowFlakeSingle.Instance.NextId();
                    item.Encode = e.Encode;
                    item.Name = e.Name;
                    item.Description = e.Description;
                    item.CreateTime = DateTime.Now;
                    item.InStationProductType = e.InStationProductType;
                    item.OutStationProductType = e.OutStationProductType;
                    paramUnitProcedures.Add(item);
                }

                var result = db.Storageable(paramUnitProcedures)
                        .WhereColumns(w => w.Encode) // 根据 Id 判断是否存在
                            .ExecuteCommand(); // 不存在则插入，存在则更新
                return true;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return false;
            }
        }
    }
}
