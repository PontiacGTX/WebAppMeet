using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data.Entities;
using WebAppMeet.DataAcess.DataContext;

namespace WebAppMeet.DataAcess.Repository
{
    public class EntityRepository<T>
        where T: class, IEntity, new()
    {
        AppDbContext _ctx { get; }
        public EntityRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<long> Count()
           => _ctx.Set<T>().LongCount();
        public async Task<long> Count<TId>(Expression<Func<T,bool>> selector)
            =>  _ctx.Set<T>().LongCount(selector);

        public async Task<T> Get<TId>(TId id)
            => await _ctx.Set<T>().FindAsync(id);

        public async Task<T> FirstOrDefault(Expression<Func<T, bool>> selector)
           => await _ctx.Set<T>().FirstOrDefaultAsync(selector);

        public async Task<long> Count<TEntity>(Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
           Expression<Func<T, bool>> whereClause = null, Expression<Func<T, TEntity>> selector = null)
          where TEntity : class, new()
        {
            var query = _ctx.Set<T>().AsQueryable();

            query = include(query);

            var res = (whereClause is null ? query.Select(selector) : query.Where(whereClause).Select(selector));

            return  res.LongCount();

        }

        public async Task<TEntity> FirstOrDefault<TEntity>(Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            Expression<Func<T, bool>> whereClause = null, Expression<Func<T, TEntity>> selector = null, Expression<Func<TEntity, bool>> selectorFirst = null)
        {
            var query = _ctx.Set<T>().AsQueryable();

            query = include(query);

            query = (whereClause is null ? query : query.Where(whereClause));

            
            return selectorFirst is null ? await query.Select(selector).FirstOrDefaultAsync() : await query.Select(selector).FirstOrDefaultAsync(selectorFirst); ;

        }


        public async Task<IList<TEntity>>  GetAll<TEntity>(Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, 
            Expression<Func<T, bool>> whereClause = null, Expression<Func<T, TEntity>> selector=null, Expression<Func<TEntity, bool>> selectorFirst = null)
        {
            var result = _ctx.Set<T>().AsQueryable();

           if (include != null)
            result = include(result);

            var res =  (whereClause is null ? result.Select(selector) : result.Where(whereClause).Select(selector));

            return selectorFirst is null? await res.ToListAsync(): new List<TEntity>() { await res.FirstOrDefaultAsync(selectorFirst) };
        }


        public async Task<IList<TGroupSelect>> GetAll<TEntity, TGroup, TGroupSelect>(Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
          Expression<Func<T, bool>> whereClause = null, Expression<Func<T, TEntity>> selector = null,
          Func<IList<TEntity>, IEnumerable<IGrouping<TGroup, TEntity>>> groupBy = null,
          Func<IGrouping<TGroup, TEntity>, TGroupSelect> groupSelector = null)
        {
            var result = _ctx.Set<T>().AsQueryable();

            result = include(result);

            var res = await (whereClause is null
                       ? result.Select(selector)
                       : result.Where(whereClause).Select(selector)
                     ).ToListAsync();

            var group = groupBy(res);

            return group.Select(groupSelector).ToList();
        }

        public async Task<T> AddAndSave(T entity)
        {
            try
            {
                EntityEntry<T> entry = _ctx.Set<T>().Add(entity);
                await _ctx.SaveChangesAsync() ;
                return entry.Entity;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<EntityEntry<T>> Add(T entity)
        {
            try
            {
                EntityEntry<T> entry = _ctx.Set<T>().Add(entity);
                return entry;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> Any(Expression<Func<T, bool>> selector) 
        =>  await _ctx.Set<T>().AnyAsync(selector);

        public async Task<bool> SaveChanges()
        {
            return await _ctx.SaveChangesAsync()>0;
        }

        public async Task<T> UpdateAndSave<TId>(T entity, TId id)
        {
            try
            {
                var entityStored = await  this.Get<TId>(id);
                _ctx.Entry(entityStored).CurrentValues.SetValues(entity);
                await _ctx.SaveChangesAsync();
                return await this.Get<TId>(id);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<IList<T>> GetAll()
            => await _ctx.Set<T>().ToListAsync();
        public async Task<IList<T>> GetAll<TId, TInclude>(Expression<Func<T, TInclude>> includeclause, Expression<Func<T, bool>> selector)
         where TInclude : class, new()
                    => await _ctx.Set<T>()
                                 .Include(includeclause)
                                 .Where(selector)
                                 .ToListAsync();
        public async Task<IList<T>> GetAll<TId,TInclude,TIncludeSecond>(Expression<Func<T,TInclude>> includeclause, Expression<Func<T, TIncludeSecond>> secondIncludeclause, Expression<Func<T, bool>> selector)
            where TInclude:class,new()
            where TIncludeSecond: class ,new()
            =>await _ctx.Set<T>()
                        .Include(includeclause)
                        .Include(secondIncludeclause)
                        .Where(selector)
                        .ToListAsync();
    


        public async Task<bool> Delete<TId>(TId id)
           
        {

            var item = await _ctx.Set<T>().FindAsync(id);

            if (item is null)
                return true;


            _ctx.Set<T>().Remove(item);

            await _ctx.SaveChangesAsync();

            return _ctx.Set<T>().FindAsync(id) == null;
        }

        public async Task DeleteRange(IEnumerable<T> collection)
        {
            _ctx.Set<T>().RemoveRange(collection);

            await _ctx.SaveChangesAsync();

            await  Task.CompletedTask;
        }


        public async Task<IList<TEntity>> GetAll<TEntity>(
            Expression<Func<T, bool>> whereClause = null, Expression<Func<T, TEntity>> selector = null, 
            Expression<Func<TEntity, bool>> selectorFirst = null, params string[] includeProperties)
            where TEntity : class,new()
        {
            IQueryable<T> query = _ctx.Set<T>().AsQueryable();
            foreach(var includeProperty in includeProperties)
            {
               
                query = query.Include(includeProperty);
                
            }

            var res = (whereClause is null ? query.Select(selector) : query.Where(whereClause).Select(selector));

            return selectorFirst is null ? await res.ToListAsync() : new List<TEntity>() { await res.FirstOrDefaultAsync(selectorFirst) };

        }

        public Task GetAll<T1, T2, T3>(Func<IQueryable<T1>, IIncludableQueryable<T1, object>> include, Expression<Func<T1, bool>> whereClause, Expression<Func<T1, T1>> selector, IQueryable<T2> groupBy, Func<T2, T3> value)
        {
            throw new NotImplementedException();
        }
    }
}
