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
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected AppDbContext context { get; }

        public BaseRepository(AppDbContext context)
        {
            this.context = context;
        }

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
        public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> expression, int? pageNum = null, string[]? includes = null,bool getNewestAdded=false,int pageSize=8)
        {
            context.ChangeTracker.LazyLoadingEnabled = false;
            IQueryable<T> result=context.Set<T>().Where(expression);
            
            if(includes is not null)
                foreach ( var i in includes)
                {
                    result = result.Include(i);
                }
           
            if(pageNum is not null and > 0)
            {
                
                int startPoint = pageSize * ((int)pageNum - 1);
                if (!getNewestAdded)
                    return result.Skip(startPoint).Take(pageSize).AsNoTracking();
                else
                {
                    var count = await result.CountAsync();
                    int skipAmount = count - (int)pageNum * pageSize;
                    if (count != 0)
                    {
                        var finalPagedResult= await result.Skip(skipAmount<0?0:skipAmount).Take(skipAmount<0?(skipAmount+pageSize<0?0: skipAmount + pageSize) :pageSize).AsNoTracking().ToListAsync();
                        return finalPagedResult;
                    }
                    else
                    {
                        return Enumerable.Empty<T>();
                    }
                }
            }
            else
            {
                
                return result.AsNoTracking();
            }
        }

        public IQueryable<T> GetListWithTracking(Expression<Func<T, bool>> exp)
        {
            return context.Set<T>().Where(exp);
        }
    }
}
