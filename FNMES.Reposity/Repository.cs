using FNMES.Entity;
using FNMES.Utility.Extension.SqlSugar;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FNMES.Reposity
{
    /// <summary>
    /// 仓储模式
    /// </summary>
    /// <typeparam name="T"></typeparam> 
    [AppService]
    public class Repository<T> : SimpleClient<T>, IRepository<T> where T : BaseModelEntity, new()
    {

        public ISqlSugarClient _Db { get { return base.Context; } set { } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        public Repository(ISqlSugarClient context) : base(context)//注意这里要有默认值等于null
        {
            //开始Saas分库！
            //单个公共基础库+多个租户业务库
            //1:先判断操作对应实体上是否存在租户特性，如果存在说明是公共库
            //如果是公共库，直接使用默认库即可
            //如果不是公共库
            //2:根据上下文对象获取用户的租户id
            //3:根据租户id获取到对应上下文对象
            //4：替换仓储中的上下文对象即可
            //强化：如果租户要做到动态配置不写死，租户信息连接字符串等存入数据库，带入token中，还需要在sqlsugarAop中动态获取进行切换
            //base.Context =  context.AsTenant().GetConnectionScopeWithAttr<T>();

        }

        /// <summary>
        /// 异步事务
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public virtual async Task<bool> UseTranAsync(Func<Task> func)
        {
            var con = Context;
            var res = await _Db.AsTenant().UseTranAsync(func);
            return res.IsSuccess;
        }

        public ISugarQueryable<T> _DbQueryable
        {
            //判断一下当前用户的角色DataScope
            get
            {
                return base.Context.Queryable<T>();
            }
            set
            {

            }
        }

        public ISugarQueryable<T> _DbQueryableAll { get { return base.Context.Queryable<T>(); } set { } }


        public virtual bool Exists(Expression<Func<T, bool>> predicate)
        {
            return _DbQueryable.Where(predicate).Any();
        }


        #region  删除方法
        public override bool Delete(T deleteObj)
        {
            return _Db.Deleteable(deleteObj).ExecuteCommand() >= 0;
        }

        public override bool Delete(List<T> deleteObjs)
        {
            return _Db.Deleteable(deleteObjs).ExecuteCommand() > 0;
        }


        public override bool DeleteById(dynamic id)
        {
            T t = _Db.Queryable<T>().In(id).First();
            if (t == null)
                return false;
            return Delete(t);
        }

        public override bool DeleteByIds(dynamic[] ids)
        {
            List<T> list = _Db.Queryable<T>().In(ids).ToList();
            return Delete(list);
        }

        public override bool Delete(Expression<Func<T, bool>> whereExpression)
        {
            List<T> list = _Db.Queryable<T>().Where(whereExpression).ToList();
            return Delete(list);
        }

        public override async Task<bool> DeleteAsync(T deleteObj)
        {
            int row = await _Db.Deleteable(deleteObj).ExecuteCommandAsync();
            return row >= 0;
        }

        public override async Task<bool> DeleteAsync(List<T> deleteObjs)
        {
            
            int row = await _Db.Deleteable(deleteObjs).ExecuteCommandAsync();
            return row >= 0;
        }

        public override async Task<bool> DeleteByIdAsync(dynamic id)
        {
            T t = await _Db.Queryable<T>().In(id).FirstAsync();
            if (t == null)
                return false;
            return await DeleteAsync(t);
        }

        public override async Task<bool> DeleteByIdsAsync(dynamic[] ids)
        {
            List<T> list = await _Db.Queryable<T>().In(ids).ToListAsync();
            return await DeleteAsync(list);
        }

        public override async Task<bool> DeleteAsync(Expression<Func<T, bool>> whereExpression)
        {
            List<T> list = await _Db.Queryable<T>().Where(whereExpression).ToListAsync();
            return await DeleteAsync(list);
        }
        #endregion
    }
}