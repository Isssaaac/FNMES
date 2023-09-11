using FNMES.Entity;
using FNMES.Reposity;
using FNMES.Utility.Extension.SqlSugar;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FNMES.Service
{

    [AppService]
    public class BaseService<T, TRepository> : IBaseService<T, TRepository> where T : BaseModelEntity, new() where TRepository : IRepository<T>
    {
        public IRepository<T> _repository { get; set; }
        public BaseService(TRepository iRepository)
        {
            _repository = iRepository;
        }


        public bool Delete(List<string> primaryKey)
        {
            return _repository.DeleteByIds(primaryKey.ToArray());
        }
        public T Get(string primaryKey)
        {
            return _repository._DbQueryable.In(primaryKey).First();
        }
        public List<T> GetList(int pageIndex, int pageSize, Expression<Func<T, bool>> expression, ref int totalCount)
        {
            ISugarQueryable<T> queryable = _repository._DbQueryable;
            if (expression != null)
                queryable = queryable.Where(expression);
            return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
        }
    }
}
