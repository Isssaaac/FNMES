using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using FNMES.Utility.Operator;
using FNMES.Utility.Extension;
using FNMES.Utility.Core;
using FNMES.Utility.Other;
using FNMES.WebUI.Logic.Base;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Drawing.Printing;
using FNMES.Entity.Param;

namespace FNMES.WebUI.Logic.Param
{
    public class ParamAndonLogic : BaseLogic
    {
        /// <summary>
        /// 得到角色列表
        /// </summary>
        /// <returns></returns>
        public List<ParamAndon> GetList(string configId)
        {
            try
            {
                //业务逻辑，必须走主库
                var db = GetInstance(configId);
                return db.MasterQueryable<ParamAndon>()
                    .ToList();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return null;

            }
        }

        /// <summary>
        /// 获得角色列表分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<ParamAndon> GetList(int pageIndex, int pageSize, string keyWord, ref int totalCount, string configId)
        {
            var db = GetInstance(configId);
            db.CodeFirst.InitTables(typeof(ParamAndon));
            ISugarQueryable<ParamAndon> queryable = db.Queryable<ParamAndon>();

            if (!keyWord.IsNullOrEmpty())
            {
                queryable = queryable.Where(it => it.AndonCode.Contains(keyWord) || it.AndonName.Contains(keyWord));
            }

            return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
        }

        public int Update(List<ParamAndon> list, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                Db.BeginTran();
                db.DbMaintenance.TruncateTable<ParamAndon>();
                int v = db.Insertable(list).ExecuteCommand();
                Db.CommitTran();
                return v;
            }
            catch (Exception)
            {
                Db.RollbackTran();
                return 0;
            }
        }
    }
}
