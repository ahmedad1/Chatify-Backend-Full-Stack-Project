using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.Core.Interfaces
{
    public interface IBaseRepository<T>where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> expression, int? pageNum = null, string[]? includes=null,bool getNewestAdded=false,int pageSize=8);
        Task<T?> GetOneByAsync(Expression<Func<T, bool>> expression,bool track=true, string[]? includes=null);
        Task<int>ExecuteDeleteAsync(Expression<Func<T, bool>> expression);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        void Remove(T entity);
        void Attach(T entity);






    }
}
