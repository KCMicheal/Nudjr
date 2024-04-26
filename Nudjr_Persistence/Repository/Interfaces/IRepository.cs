using Nudjr_Domain.Models.UtilityModels;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Persistence.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T Add(T obj);

        Task<T> AddAsync(T obj);

        void AddRange(IEnumerable<T> records);

        Task AddRangeAsync(IEnumerable<T> records);       
        bool Delete(Expression<Func<T, bool>> predicate);

        bool Delete(T obj);

        Task DeleteAsync(Expression<Func<T, bool>> predicate);

        Task DeleteAsync(T obj);

        bool DeleteById(object id);

        Task DeleteByIdAsync(object id);
        void Dispose();
        Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        Task<T> GetSingleByAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int? skip = null, int? take = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool tracking = false);
        T GetById(object id);
        Task<T> GetByIdAsync(object id);
        Task<PaginationResult<T>> GetPagedItems(RequestParameters parameters, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy, Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = true);
        T GetSingleBy(Expression<Func<T, bool>> predicate);

        Task<T> GetSingleByAsync(Expression<Func<T, bool>> predicate);

        int Save();

        Task<int> SaveAsync();

        T Update(T obj);

        Task<T> UpdateAsync(T obj);
        Task UpdateRangeAsync(IEnumerable<T> records);
    }
}
