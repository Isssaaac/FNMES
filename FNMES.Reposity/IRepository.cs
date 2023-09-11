using FNMES.Entity;
using SqlSugar;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FNMES.Reposity
{ 
    public interface IRepository<T> : ISimpleClient<T> where T : BaseModelEntity, new()
    {
        ISugarQueryable<T> _DbQueryable { get; set; }

        ISqlSugarClient _Db { get; set; }
        /// <summary>
        /// 查询所有数据包括已删除的
        /// </summary>
        ISugarQueryable<T> _DbQueryableAll { get; set; }

        Task<bool> UseTranAsync(Func<Task> func);
        bool Exists(Expression<Func<T, bool>> expression);
    }
}
