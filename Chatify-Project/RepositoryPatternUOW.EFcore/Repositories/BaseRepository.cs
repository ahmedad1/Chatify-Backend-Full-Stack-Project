using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Core.Interfaces;
using RepositoryPatternUOW.EFcore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.EFcore.Repositories
{
    public class BaseRepository<T>(AppDbContext context) : IBaseRepository<T> where T : class
    {
     

        public async Task AddAsync(T entity)
        {
           await context.AddAsync(entity);
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetOneByAsync(Expression<Func<T, bool>> expression, bool track = true, string[]? includes =null)
        {
            context.ChangeTracker.LazyLoadingEnabled = track;
            IQueryable<T> dbset = context.Set<T>() ;
            if (!track)
            {
                if(includes is not null)
                foreach (var i in includes)
                {
                    dbset=dbset.Include(i);
                }
                return await dbset.AsNoTracking().FirstOrDefaultAsync(expression);
            }
            else
            {
                return await dbset.FirstOrDefaultAsync(expression);
            }
        }

        public void Attach(T entity)
        {
            context.Attach(entity);
        }
      

        public void Remove(T entity)
        {
            context.Set<T>().Remove(entity);
        }
        public async Task<bool>ExistsAsync(Expression<Func<T, bool>> expression)
        {
            return await context.Set<T>().AnyAsync(expression);
        }
        public async Task<int> ExecuteDeleteAsync(Expression<Func<T, bool>> expression)
        {
            return await context.Set<T>().Where(expression).ExecuteDeleteAsync();
        }
        public IEnumerable<T> GetWhere(Expression<Func<T, bool>> expression, int? pageNum = null, string[]? includes = null)
        {
            context.ChangeTracker.LazyLoadingEnabled = false;
            IQueryable<T> result=context.Set<T>();
            
            if(includes is not null)
                foreach ( var i in includes)
                {
                    result = result.Include(i);
                }
           
            if(pageNum is not null and > 0)
            {
                int pageSize = 8;
                int startPoint = pageSize * ((int)pageNum - 1);
                return result.Where(expression).Skip(startPoint).Take(pageSize).AsNoTracking();
                
            }
            else
            {
                return result.Where(expression).AsNoTracking();
            }
        }

    }
}
