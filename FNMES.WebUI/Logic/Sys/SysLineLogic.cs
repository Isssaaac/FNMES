using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Utility.Operator;
using FNMES.Utility.Extension;
using FNMES.Utility.Core;
using FNMES.Entity;
using FNMES.Utility.Other;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Sys;

namespace FNMES.WebUI.Logic.Sys
{
    public class SysLineLogic : BaseLogic
    {

        public List<SysLine> GetList()
        {
            try
            {
                //为了修改后实时显示，直接走主库
                var db = GetInstance();
                return db.MasterQueryable<SysLine>()
                    //.Includes(it => it.CreateUser)
                    //.Includes(it => it.ModifyUser)
                    .OrderBy(it => it.SortCode)
                    .ToList();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<SysLine>();
            }
        }

      
        public List<SysLine> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount)
        {
            try
            {
                //为了修改后实时显示，直接走主库
                var db = GetInstance();
                ISugarQueryable<SysLine> queryable = db.MasterQueryable<SysLine>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => (it.Name.Contains(keyWord) || it.EnCode.Contains(keyWord)));
                }
                return queryable
                    .Includes(it => it.CreateUser)
                    .Includes(it => it.ModifyUser)
                    .OrderBy(it => it.SortCode)
                    .ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new List<SysLine>();
            }
        }




        public SysLine Get(long primaryKey)
        {
            try
            {
                //为了修改后实时显示，直接走主库
                var db = GetInstance();
                return db.MasterQueryable<SysLine>()
                    .Where(it => it.Id == primaryKey)
                    .Includes(it => it.CreateUser)
                    .Includes(it => it.ModifyUser)
                    .First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new SysLine();
            }
        }

        public SysLine GetByConfigId(string configId)
        {
            try
            {
                var db = GetInstance();
                return db.Queryable<SysLine>()
                    .Where(it => it.ConfigId == configId)
                    .First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return new SysLine();
            }
        }

        public int Insert(SysLine model, long account)
        {
            try
            {
                var db = GetInstance();
                model.Id = SnowFlakeSingle.instance.NextId();
                model.CreateUserId = account;
                model.CreateTime = DateTime.Now;
                model.ModifyUserId = model.CreateUserId;
                model.ModifyTime = model.CreateTime;
                return db.Insertable<SysLine>(model).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }

        public int Delete(long primaryKey)
        {
            try
            {
                var db = GetInstance();
                return db.Deleteable<SysLine>().Where(it => it.Id == primaryKey).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }
        public int Update(SysLine model, long account)
        {
            try
            {
                var db = GetInstance();
                model.ModifyUserId = account;
                model.ModifyTime = DateTime.Now;
                return db.Updateable<SysLine>(model).UpdateColumns(it => new
                {
                    it.EnCode,
                    it.Name,
                    it.ConfigId,
                    it.SortCode,
                    it.EnableFlag,
                    it.Description,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }
    }
}
