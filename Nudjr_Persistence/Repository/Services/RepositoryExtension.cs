using Nudjr_Persistence.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Persistence.Repository.Services
{
    public class RepositoryExtension<T> : Repository<T>, IRepositoryExtension<T> where T : class
    {
        public RepositoryExtension(DbContext context) : base(context)
        {
        }

        public virtual IQueryable<T> Query(string sql, params object[] parameters) => _dbSet.FromSqlRaw(sql, parameters);

        public T Find(params object[] keyValues) => _dbSet.Find(keyValues);

        public async Task<T> FindAsync(params object[] keyValues) => await _dbSet.FindAsync(keyValues);

        public T SingleOrDefault(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracking = true)
        {

            IQueryable<T> query = _dbSet;
            if (disableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            var result = query.SingleOrDefault(predicate);
            return result;

        }

        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate,
          Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
          bool disableTracking = true)
        {

            IQueryable<T> query = _dbSet;
            if (disableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            var result = await query.SingleOrDefaultAsync(predicate);
            return result;

        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
           bool disableTracking = true)
        {
            IQueryable<T> query = _dbSet;
            if (disableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return orderBy(query).FirstOrDefault();
            return query.FirstOrDefault();
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
          Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
          bool disableTracking = true)
        {
            IQueryable<T> query = _dbSet;
            if (disableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).FirstOrDefaultAsync();
            return await query.FirstOrDefaultAsync();
        }


        public T LastOrDefault(Expression<Func<T, bool>> predicate = null,
          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
          Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
          bool disableTracking = true)
        {
            IQueryable<T> query = _dbSet;
            if (disableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return orderBy(query).LastOrDefault();
            return query.LastOrDefault();
        }

        public async Task<T> LastOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
         Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
         Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
         bool disableTracking = true)
        {
            IQueryable<T> query = _dbSet;
            if (disableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).LastOrDefaultAsync();
            return await query.LastOrDefaultAsync();
        }
        public async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracking = true)
        {
            IQueryable<T> query = _dbSet;
            if (disableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            return orderBy != null
                ? await orderBy(query).ToListAsync()
                : await query.ToListAsync();
        }

        public IQueryable<T> GetQueryableList(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracking = true)
        {
            IQueryable<T> query = _dbSet;
            if (disableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            return orderBy != null
                ? orderBy(query)
                : query;
        }


    }
}
