using FNMES.Entity.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using FNMES.Utility.Core;
using FNMES.WebUI.Logic.Base;

namespace FNMES.WebUI.Logic.Sys
{
    /// <summary>
    /// 系统日志表数据访问
    /// </summary>
    public class SysLogLogic : BaseLogic
    {

        /// <summary>
        /// 根据日志类型获得日志
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="keyWord"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<SysLog> GetList(int pageIndex, int pageSize, string type, string index, string keyWord, ref int totalCount)
        {
            using (var db = GetInstance())
            {
                ISugarQueryable<SysLog> query = db.Queryable<SysLog>().Where(it => it.Type == type);
                if (!keyWord.IsNullOrEmpty())
                {
                    query = query.Where(it => it.Message.Contains(keyWord));
                }
                //查询当日
                if (index == "1")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today;
                    DateTime endTime = today.AddDays(1);
                    query = query.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                //近7天
                else if (index == "2")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-6);
                    DateTime endTime = today.AddDays(1);
                    query = query.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                //近1月
                else if (index == "3")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-29);
                    DateTime endTime = today.AddDays(1);
                    query = query.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                //近3月
                else if (index == "4")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-91);
                    DateTime endTime = today.AddDays(1);
                    query = query.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                return query.OrderBy(it => it.Id, OrderByType.Desc).ToPageList(pageIndex, pageSize, ref totalCount);
            }
        }



        /// <summary>
        /// 删除日志
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public int Delete(string type, string index)
        {
            using (var db = GetInstance())
            {
                IDeleteable<SysLog> query = db.Deleteable<SysLog>().Where(it => it.Type == type);
                //保留一个月
                if (index == "1")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today;
                    DateTime endTime = today.AddDays(-29);
                    query = query.Where(it => it.CreateTime < endTime);
                    return query.ExecuteCommand();
                }
                //保留7天
                else if (index == "2")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-6);
                    query = query.Where(it => it.CreateTime < startTime);
                    return query.ExecuteCommand();
                }
                //保留近3个月
                else if (index == "3")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-92);
                    query = query.Where(it => it.CreateTime < startTime);
                    return query.ExecuteCommand();
                }
                //全部
                else if (index == "4")
                {
                    return query.ExecuteCommand();
                }
                else
                {
                    return 0;
                }

            }
        }
    }
}
