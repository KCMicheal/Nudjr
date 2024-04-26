using Nudjr_Persistence.Repository.Interfaces;
using Nudjr_Persistence.Repository.Services;
using Nudjr_Persistence.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Nudjr_Persistence.UnitOfWork.Services
{
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        private Dictionary<Type, object> _repositories;
        private readonly TContext _context;

        public UnitOfWork(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRepositoryExtension<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_repositories == null) _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new RepositoryExtension<TEntity>(_context);
            return (IRepositoryExtension<TEntity>)_repositories[type];
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
