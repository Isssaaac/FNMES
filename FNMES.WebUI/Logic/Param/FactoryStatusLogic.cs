using SqlSugar;
using System;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;

namespace FNMES.WebUI.Logic.Param
{
    public class FactoryStatusLogic : BaseLogic
    {


        public int Insert(FactoryStatus model)
        {
            try
            {
                var db = GetInstance(model.ConfigId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                model.CreateTime = DateTime.Now;
                return db.Insertable<FactoryStatus>(model).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
        }
        /// <summary>
        /// 获取最新状态
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public FactoryStatus Get(string configId)
        {
            try
            {
                var db = GetInstance(configId);
                FactoryStatus factoryStatus = db.Queryable<FactoryStatus>().OrderBy(it => it.Id ,OrderByType.Desc).First();
                return factoryStatus;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return null;
            }

        }
    
       
        /// <summary>
        /// 更新用户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(FactoryStatus model)
        {
            try
            {
                var db = GetInstance(model.ConfigId);

                return db.Updateable<FactoryStatus>(model).IgnoreColumns(it => new
                {
                    it.CreateTime
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
